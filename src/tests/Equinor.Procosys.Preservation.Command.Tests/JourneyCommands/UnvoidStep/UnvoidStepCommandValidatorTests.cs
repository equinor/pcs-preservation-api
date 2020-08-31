﻿using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidStep;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UnvoidStep
{
    [TestClass]
    public class UnvoidStepCommandValidatorTests
    {
        private UnvoidStepCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UnvoidStepCommand _command;

        private int _journeyId = 2;
        private int _stepId = 1;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.HasStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new UnvoidStepCommand(_journeyId, _stepId, _rowVersion);
            _dut = new UnvoidStepCommandValidator(_journeyValidatorMock.Object, _stepValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyDoesNotHaveStep()
        {
            _journeyValidatorMock.Setup(r => r.HasStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exist within given journey!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsNotVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UnvoidStepCommand(_journeyId, _stepId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
