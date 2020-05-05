﻿using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateJourneyCommand(1, "TitleA", "AAAAAAAAABA=");
            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}
