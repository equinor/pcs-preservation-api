﻿using System.Linq;
using Equinor.Procosys.Preservation.Query.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTags
{
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new Filter();
            Assert.IsNull(dut.CallOffStartsWith);
            Assert.IsNull(dut.CommPkgNoStartsWith);
            Assert.IsNull(dut.McPkgNoStartsWith);
            Assert.IsNull(dut.PurchaseOrderNoStartsWith);
            Assert.IsNull(dut.TagNoStartsWith);
            Assert.IsFalse(dut.PreservationStatus.HasValue);
            Assert.IsFalse(dut.ActionStatus.HasValue);
            Assert.AreEqual(0, dut.AreaCodes.Count());
            Assert.AreEqual(0, dut.DisciplineCodes.Count());
            Assert.AreEqual(0, dut.DueFilters.Count());
            Assert.AreEqual(0, dut.JourneyIds.Count());
            Assert.AreEqual(0, dut.ModeIds.Count());
            Assert.AreEqual(0, dut.RequirementTypeIds.Count());
            Assert.AreEqual(0, dut.ResponsibleIds.Count());
            Assert.AreEqual(0, dut.StepIds.Count());
            Assert.AreEqual(0, dut.TagFunctionCodes.Count());
        }
    }
}
