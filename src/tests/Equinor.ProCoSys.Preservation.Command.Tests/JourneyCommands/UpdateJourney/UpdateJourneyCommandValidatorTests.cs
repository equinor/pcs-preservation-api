using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandValidatorTests
    {
        private UpdateJourneyCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateJourneyCommand _command;

        private int _id = 1;
        private string _title = "Title";
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _command = new UpdateJourneyCommand(_id, _title, _rowVersion, null);

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _dut = new UpdateJourneyCommandValidator(_journeyValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenJourneyNotExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnotherJourneyWithSameTitleAlreadyExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsWithSameTitleInAnotherJourneyAsync(_id, _title, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another journey with this title already exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenJourneyIsVoided()
        {
            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";
            
            var command = new UpdateJourneyCommand(_id, _title, invalidRowVersion, null);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
