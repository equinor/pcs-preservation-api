﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly Mock<IPlantApiService> _plantApiServiceMock;
        private readonly Mock<IPermissionApiService> _permissionApiServiceMock;
        private readonly string _connectionString;
        private readonly string _configPath;
        private HttpClient _anonymousClient;
        private HttpClient _libraryAdminClient;
        private HttpClient _plannerClient;
        private HttpClient _preserverClient;
        private HttpClient _hackerClient;

        private readonly List<ProcosysPlant> _normalPlantAccess = new List<ProcosysPlant>
        {
            new ProcosysPlant {Id = PlantWithAccess, HasAccess = true}, 
            new ProcosysPlant {Id = PlantWithoutAccess}
        };
        private readonly List<ProcosysPlant> _noPlantAccess = new List<ProcosysPlant>()
        {
            new ProcosysPlant {Id = PlantWithAccess}, 
            new ProcosysPlant {Id = PlantWithoutAccess}
        };

        private readonly List<Action> _teardownList = new List<Action>();

        public static string PlantWithAccess => "PLANT1";
        public static string PlantWithoutAccess => "PLANT999";
        public string UnknownPlant => "UNKNOWN_PLANT";
        public string ProjectWithAccess => "Project1";
        public string ProjectWithoutAccess => "Project999";

        public TestFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            _connectionString = GetTestDbConnectionString(projectDir);
            _configPath = Path.Combine(projectDir, "appsettings.json");

            _plantApiServiceMock = new Mock<IPlantApiService>();

            _permissionApiServiceMock = new Mock<IPermissionApiService>();

            _anonymousClient = CreateTestClient(null);
            _libraryAdminClient = CreateTestClient(LibraryAdminProfile.Tokens);
            _plannerClient = CreateTestClient(PlannerProfile.Tokens);
            _preserverClient = CreateTestClient(PreserverProfile.Tokens);
            _hackerClient = CreateTestClient(HackerProfile.Tokens);
        }

        public HttpClient GetAnonymousClient()
        {
            ClearPlants();
            ClearPermissions();
            return _anonymousClient;
        }

        public HttpClient GetLibraryAdminClient()
        {
            SetupPlants(_normalPlantAccess);
            SetupPermissionApiService(LibraryAdminProfile.ProCoSysPermissions);
            return _libraryAdminClient;
        }
        public HttpClient GetPlannerClient()
        {
            SetupPlants(_normalPlantAccess);
            SetupPermissionApiService(PlannerProfile.ProCoSysPermissions);
            return _plannerClient;
        }

        public HttpClient GetPreserverClient()
        {
            SetupPlants(_normalPlantAccess);
            SetupPermissionApiService(PreserverProfile.ProCoSysPermissions);
            return _preserverClient;
        }

        public HttpClient GetAuthenticatedHackerClient()
        {
            SetupPlants(_noPlantAccess);
            SetupPermissionApiService(new List<string>());
            return _hackerClient;
        }

        public new void Dispose()
        {
            // Run teardown
            foreach (var action in _teardownList)
            {
                action();
            }

            if (_anonymousClient != null)
            {
                _anonymousClient.Dispose();
                _anonymousClient = null;
            }
            if (_libraryAdminClient != null)
            {
                _libraryAdminClient.Dispose();
                _libraryAdminClient = null;
            }
            if (_plannerClient != null)
            {
                _plannerClient.Dispose();
                _plannerClient = null;
            }
            if (_preserverClient != null)
            {
                _preserverClient.Dispose();
                _preserverClient = null;
            }
            if (_hackerClient != null)
            {
                _hackerClient.Dispose();
                _hackerClient = null;
            }

            base.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication()
                    .AddScheme<IntegrationTestAuthOptions, IntegrationTestAuthHandler>(
                        IntegrationTestAuthHandler.TestAuthenticationScheme, opts => { });

                services.PostConfigureAll<JwtBearerOptions>(jwtBearerOptions =>
                    jwtBearerOptions.ForwardAuthenticate = IntegrationTestAuthHandler.TestAuthenticationScheme);

                services.AddScoped(serviceProvider => _plantApiServiceMock.Object);
                services.AddScoped(serviceProvider => _permissionApiServiceMock.Object);
            });

            builder.ConfigureServices(CreateDatabaseWithMigreations);
        }

        private void CreateDatabaseWithMigreations(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault
                (d => d.ServiceType == typeof(DbContextOptions<PreservationContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PreservationContext>(context => context.UseSqlServer(_connectionString));

            using var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<PreservationContext>();

            dbContext.Database.EnsureDeleted();

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

            var migrations = dbContext.Database.GetPendingMigrations();
            if (migrations.Any())
            {
                dbContext.Database.Migrate();
            }

            // Put the teardown here, as we don't have the generic TContext in the dispose method.
            _teardownList.Add(() => { dbContext.Database.EnsureDeleted(); });
        }

        private string GetTestDbConnectionString(string projectDir)
        {
            var dbName = "IntegrationTestsDB";
            var dbPath = Path.Combine(projectDir, $"{dbName}.mdf");
            return $"Server=(LocalDB)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=true;AttachDbFileName={dbPath}";
        }

        private void SetupPlants(List<ProcosysPlant> plants)
            => _plantApiServiceMock.Setup(p => p.GetAllPlantsAsync()).Returns(Task.FromResult(plants));

        private void ClearPlants() =>
            _plantApiServiceMock.Setup(p => p.GetAllPlantsAsync()).Returns(Task.FromResult<List<ProcosysPlant>>(null));
        
        private void SetupPermissionApiService(IList<string> proCoSysPermissions)
        {
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult(proCoSysPermissions));

            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(new List<string>()));
                        
            _permissionApiServiceMock.Setup(p => p.GetAllOpenProjectsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<ProcosysProject>>(new List<ProcosysProject>
                {
                    new ProcosysProject {Name = ProjectWithAccess, HasAccess = true},
                    new ProcosysProject {Name = ProjectWithoutAccess}
                }));
        }

        private void ClearPermissions()
        {
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(null));

            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(null));
                        
            _permissionApiServiceMock.Setup(p => p.GetAllOpenProjectsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<ProcosysProject>>(null));
        }

        private HttpClient CreateTestClient(Profile profile)
        {
            var client = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Startup.IntegrationTestEnvironment);
                builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(_configPath));

            }).CreateClient();

            if (profile != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", BearerTokenUtility.WrapAuthToken(profile));
            }
            return client;
        }
    }
}