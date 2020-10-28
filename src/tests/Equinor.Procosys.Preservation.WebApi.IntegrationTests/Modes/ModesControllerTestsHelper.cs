﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    public static class ModesControllerTestsHelper
    {
        private const string ModesPath = "Modes";

        public static async Task<List<ModeDto>> GetAllModesAsync(
            HttpClient client,
            string requestingPlant,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            client.UpdatePlantInHeader(requestingPlant);
            
            // Act
            var response = await client.GetAsync($"{ModesPath}");

            // Assert
            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            
            return JsonConvert.DeserializeObject<List<ModeDto>>(content);
        }
    }
}
