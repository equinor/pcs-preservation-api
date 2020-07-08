﻿using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementTypeValidatorTests : ReadOnlyTestsBase
    {
        private int _reqTypeId;
        private const string Code = "Code";
        private int _newReqTypeId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var requirementType = AddRequirementTypeWith1DefWithoutField(context, Code, "D", RequirementTypeIcon.Other);
                _reqTypeId = requirementType.Id;
                _newReqTypeId = _reqTypeId + 1;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsAsync(_reqTypeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqType = context.RequirementTypes.Single(rd => rd.Id == _reqTypeId);
                reqType.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(_reqTypeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(_reqTypeId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueTitleAsync_SameTitleAsAnotherRequirementType_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueTitleAsync(_newReqTypeId, $"Title{Code}", default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueTitleAsync_SameTitleAsAnotherRequirementType_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueTitleAsync(_newReqTypeId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueTitleAsync_NewTitleOnSameRequirementType_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueTitleAsync(_reqTypeId, $"Title{Code}", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueCodeAsync_SameCodeAsAnotherRequirementType_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueCodeAsync(_newReqTypeId, Code, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueCodeAsync_SameCodeAsAnotherRequirementType_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueCodeAsync(_newReqTypeId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsNotUniqueCodeAsync_NewCodeOnSameRequirementType_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsNotUniqueCodeAsync(_reqTypeId, Code, default);
                Assert.IsFalse(result);
            }
        }
    }
}
