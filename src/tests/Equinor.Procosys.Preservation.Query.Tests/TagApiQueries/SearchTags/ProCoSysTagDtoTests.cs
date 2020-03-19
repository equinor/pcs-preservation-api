﻿using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagApiQueries.SearchTags
{
    [TestClass]
    public class ProCoSysTagDtoTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new ProcosysTagDto("TagNo", "Desc", "PoNo", "CommPkgNo", "McPkgNo", "TFC", "RC", true);

            Assert.AreEqual("TagNo", dut.TagNo);
            Assert.AreEqual("Desc", dut.Description);
            Assert.AreEqual("PoNo", dut.PurchaseOrderNumber);
            Assert.AreEqual("CommPkgNo", dut.CommPkgNo);
            Assert.AreEqual("McPkgNo", dut.McPkgNo);
            Assert.AreEqual("McPkgNo", dut.McPkgNo);
            Assert.AreEqual("TFC", dut.TagFunctionCode);
            Assert.AreEqual("RC", dut.RegisterCode);
            Assert.IsTrue(dut.IsPreserved);
        }
    }
}
