﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementDefinition
{
    [TestClass]
    public class UpdateRequirementDefinitionCommandValidatorTests
    {
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRequirementDefinitionValidator> _reqDefinitionValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;

        private UpdateRequirementDefinitionCommand _command;
        private UpdateRequirementDefinitionCommandValidator _dut;
        private readonly int _requirementTypeId = 2;
        private readonly int _requirementDefinitionId = 3;
        private readonly int _updateFieldId = 4;
        private readonly int _sortKey = 10;
        private readonly string _title = "Title test";
        private RequirementUsage _usage = RequirementUsage.ForAll;
        private int _defaultIntervalWeeks = 4;
        private string _rowVersion = "AAAAAAAAABA=";
        private IList<UpdateFieldsForCommand> _updatedFields;
        private IList<FieldsForCommand> _newFields;
        private readonly FieldType _fieldType = FieldType.CheckBox;

        [TestInitialize]
        public void Setup_OkState()
        {
            _updatedFields = new List<UpdateFieldsForCommand>
            {
                new UpdateFieldsForCommand(_updateFieldId, "L", _fieldType, 1, false, null)
            };
            _newFields = new List<FieldsForCommand>();

            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));
            _reqTypeValidatorMock
                .Setup(rd => rd.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _reqDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _reqDefinitionValidatorMock
                .Setup(r => r.AllExcludedFieldsIsVoidedAsync(_requirementDefinitionId, new List<int> {_updateFieldId}, default))
                .Returns(Task.FromResult(true));

            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(f => f.ExistsAsync(_updateFieldId, default)).Returns(Task.FromResult(true));
            _fieldValidatorMock.Setup(f => f.VerifyFieldTypeAsync(_updateFieldId, _fieldType, default)).Returns(Task.FromResult(true));

            _command = new UpdateRequirementDefinitionCommand(
                _requirementTypeId, 
                _requirementDefinitionId, 
                _sortKey, 
                _usage, 
                _title, 
                _defaultIntervalWeeks, 
                _rowVersion, 
                _updatedFields, 
                _newFields);
            _dut = new UpdateRequirementDefinitionCommandValidator(
                _reqTypeValidatorMock.Object, 
                _reqDefinitionValidatorMock.Object,
                _fieldValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeNotExists()
        {
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionNotExists()
        {
            _reqTypeValidatorMock.Setup(rd => rd.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionIsVoided()
        {
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionTitleIsNotUniqueOnType()
        {
            var fieldTypes1 = _updatedFields.Select(f => f.FieldType).ToList();
            var fieldTypes2 = _newFields.Select(f => f.FieldType).ToList();
            var fieldTypesConcatenated = fieldTypes1.Concat(fieldTypes2).ToList();

            _reqTypeValidatorMock
                .Setup(r => r.OtherRequirementDefinitionExistsWithSameTitleAsync(
                    _requirementTypeId,
                    _requirementDefinitionId,
                    _title, 
                    fieldTypesConcatenated,
                    default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A requirement definition with this title already exists on the requirement type"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldToUpdateChangeType()
        {
            _fieldValidatorMock.Setup(f => f.ExistsAsync(_updateFieldId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldToUpdateNotExists()
        {
            _fieldValidatorMock.Setup(f => f.VerifyFieldTypeAsync(_updateFieldId, _fieldType, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Cannot change field type on existing fields!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldToDeleteIsNotVoided()
        {
            _reqDefinitionValidatorMock
                .Setup(r => r.AllExcludedFieldsIsVoidedAsync(_requirementDefinitionId, new List<int> {_updateFieldId}, default))
                .Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Fields to be deleted must be voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldToDeleteIsInUse()
        {
            _reqDefinitionValidatorMock
                .Setup(r => r.AnyExcludedFieldsIsInUseAsync(_requirementDefinitionId, new List<int> {_updateFieldId}, default))
                .Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Fields to be deleted can not be in use!"));
        }
    }
}
