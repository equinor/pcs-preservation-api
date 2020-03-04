﻿using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ProjectAggregate
{
    [TestClass]
    public class TagDtoTests
    {
        private TagDto _dut;

        [TestInitialize]
        public void Setup() => _dut = new TagDto(
            1,
            "AreaCode",
            "CallOffNo",
            "CommPkgNo",
            "DisciplineCode",
            true,
            true,
            "McPkgNo",
            "Mode",
            true,
            true,
            "PoNo",
            "Remark!",
            new List<RequirementDto> {new RequirementDto(0, null, default, default, false)},
            "Resp",
            PreservationStatus.Active,
            "TagFunctionCode",
            "TagDesc",
            "TagNo",
            TagType.Standard);

        [TestMethod]
        public void Constructor_SetsProperties()
        {
            Assert.AreEqual(1, _dut.Id);
            Assert.AreEqual("AreaCode", _dut.AreaCode);
            Assert.AreEqual("CallOffNo", _dut.CalloffNo);
            Assert.AreEqual("CommPkgNo", _dut.CommPkgNo);
            Assert.AreEqual("DisciplineCode", _dut.DisciplineCode);
            Assert.AreEqual(TagType.Standard, _dut.TagType);
            Assert.IsTrue(_dut.IsNew);
            Assert.IsTrue(_dut.IsVoided);
            Assert.AreEqual("McPkgNo", _dut.McPkgNo);
            Assert.AreEqual("Mode", _dut.Mode);
            Assert.IsTrue(_dut.ReadyToBePreserved);
            Assert.IsTrue(_dut.ReadyToBeTransferred);
            Assert.AreEqual("TagDesc", _dut.Description);
            Assert.AreEqual("PoNo", _dut.PurchaseOrderNo);
            Assert.AreEqual("Remark!", _dut.Remark);
            Assert.IsNotNull(_dut.Requirements);
            Assert.AreEqual(1, _dut.Requirements.Count());
            Assert.AreEqual(PreservationStatus.Active, _dut.Status);
            Assert.AreEqual("Resp", _dut.ResponsibleCode);
            Assert.AreEqual("TagFunctionCode", _dut.TagFunctionCode);
            Assert.AreEqual("TagNo", _dut.TagNo);
        }
    }
}
