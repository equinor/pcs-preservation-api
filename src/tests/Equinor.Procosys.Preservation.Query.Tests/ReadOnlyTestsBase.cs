﻿using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Query.Tests
{
    public abstract class ReadOnlyTestsBase
    {
        protected const string _schema = "PCS$TEST";
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected Mock<IEventDispatcher> _eventDispatcherMock;
        protected Mock<IPlantProvider> _plantProviderMock;

        [TestInitialize]
        public void SetupBase()
        {
            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(_schema);

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            SetupNewDatabase(_dbContextOptions);
        }

        protected abstract void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions);

        protected Responsible AddResponsible(PreservationContext context, string code)
        {
            var responsible = new Responsible(_schema, code);
            context.Responsibles.Add(responsible);
            context.SaveChanges();
            return responsible;
        }

        protected Mode AddMode(PreservationContext context, string title)
        {
            var mode = new Mode(_schema, title);
            context.Modes.Add(mode);
            context.SaveChanges();
            return mode;
        }

        protected Journey AddJourneyWithStep(PreservationContext context, string title, Mode mode, Responsible responsible)
        {
            var journey = new Journey(_schema, title);
            journey.AddStep(new Step(_schema, mode, responsible));
            context.Journeys.Add(journey);
            context.SaveChanges();
            return journey;
        }

        protected RequirementType AddRequirementTypeWith1DefWithoutField(PreservationContext context, string type, string def)
        {
            var requirementType = new RequirementType(_schema, $"Code{type}", $"Title{type}", 0);
            context.RequirementTypes.Add(requirementType);
            context.SaveChanges();

            var requirementDefinition = new RequirementDefinition(_schema, $"Title{def}", 2, 1);
            requirementType.AddRequirementDefinition(requirementDefinition);
            context.SaveChanges();

            return requirementType;
        }

        protected Person AddPerson(PreservationContext context, string firstName, string lastName)
        {
            var person = new Person(Guid.Empty, firstName, lastName);
            context.Persons.Add(person);
            context.SaveChanges();
            return person;
        }
    }
}
