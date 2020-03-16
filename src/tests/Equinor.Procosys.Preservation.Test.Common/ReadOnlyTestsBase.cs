﻿using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Test.Common
{
    public abstract class ReadOnlyTestsBase
    {
        protected const string TestPlant = "PlantA";
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected Mock<IPlantProvider> _plantProviderMock;
        protected IPlantProvider _plantProvider;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;
        protected ManualTimeProvider _timeProvider;

        [TestInitialize]
        public void SetupBase()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(TestPlant);
            _plantProvider = _plantProviderMock.Object;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(x => x.GetCurrentUser())
                .Returns(_currentUserOid);
            _currentUserProvider = currentUserProviderMock.Object;

            var eventDispatcher = new Mock<IEventDispatcher>();
            _eventDispatcher = eventDispatcher.Object;

            _timeProvider = new ManualTimeProvider(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            SetupNewDatabase(_dbContextOptions);
        }

        protected abstract void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions);

        protected Responsible AddResponsible(PreservationContext context, string code)
        {
            var responsible = new Responsible(TestPlant, code, "Title");
            context.Responsibles.Add(responsible);
            context.SaveChangesAsync().Wait();
            return responsible;
        }

        protected Mode AddMode(PreservationContext context, string title)
        {
            var mode = new Mode(TestPlant, title);
            context.Modes.Add(mode);
            context.SaveChangesAsync().Wait();
            return mode;
        }

        protected Journey AddJourneyWithStep(PreservationContext context, string title, Mode mode, Responsible responsible)
        {
            var journey = new Journey(TestPlant, title);
            journey.AddStep(new Step(TestPlant, mode, responsible));
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();
            return journey;
        }

        protected RequirementType AddRequirementTypeWith1DefWithoutField(PreservationContext context, string type, string def, int sortKey = 0)
        {
            var requirementType = new RequirementType(TestPlant, type, $"Title{type}", sortKey);
            context.RequirementTypes.Add(requirementType);
            context.SaveChangesAsync().Wait();

            var requirementDefinition = new RequirementDefinition(TestPlant, def, 2, 1);
            requirementType.AddRequirementDefinition(requirementDefinition);
            context.SaveChangesAsync().Wait();

            return requirementType;
        }

        protected Person AddPerson(PreservationContext context, Guid oid, string firstName, string lastName)
        {
            var person = new Person(oid, firstName, lastName);
            context.Persons.Add(person);
            context.SaveChangesAsync().Wait();
            return person;
        }

        protected Project AddProject(PreservationContext context, string name, string description, bool isClosed = false)
        {
            var project = new Project(TestPlant, name, description);
            if (isClosed)
            {
                project.Close();
            }
            context.Projects.Add(project);
            context.SaveChangesAsync().Wait();
            return project;
        }

        protected Tag AddTag(PreservationContext context, Project parentProject, TagType tagType, string tagNo, string description, Step step, IEnumerable<Requirement> requirements)
        {
            var tag = new Tag(TestPlant, tagType, tagNo, description, "", "", "", "", "", "", "", "", "", step, requirements);
            parentProject.AddTag(tag);
            context.SaveChangesAsync().Wait();
            return tag;
        }

        protected Field AddInfoField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.Info, 0);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }

        protected Field AddNumberField(PreservationContext context, RequirementDefinition rd, string label, string unit, bool showPrevious)
        {
            var field = new Field(TestPlant, label, FieldType.Number, 0, unit, showPrevious);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }

        protected Field AddCheckBoxField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.CheckBox, 0);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }
    }
}
