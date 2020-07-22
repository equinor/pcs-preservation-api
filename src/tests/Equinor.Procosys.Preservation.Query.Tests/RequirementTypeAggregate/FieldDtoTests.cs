﻿using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class FieldDtoTests
    {
        private const string _rowVersion = "AAAAAAAAABA=";

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new FieldDto(1, "LabelA", true, FieldType.CheckBox, 10, "UnitA", _rowVersion, true);

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("LabelA", dut.Label);
            Assert.AreEqual("UnitA", dut.Unit);
            Assert.AreEqual(FieldType.CheckBox, dut.FieldType);
            Assert.AreEqual(10, dut.SortKey);
            Assert.IsTrue(dut.ShowPrevious.HasValue);
            Assert.IsTrue(dut.ShowPrevious.Value);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(_rowVersion, dut.RowVersion);
        }
    }
}
