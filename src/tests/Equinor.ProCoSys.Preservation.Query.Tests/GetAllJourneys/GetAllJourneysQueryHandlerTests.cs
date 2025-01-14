using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetAllJourneys;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetAllJourneys
{
    [TestClass]
    public class GetAllJourneysQueryHandlerTests : ReadOnlyTestsBase
    {
        private readonly string _journeyTitle = "J1";
        private readonly string _step1Title = "S1";
        private readonly string _step2Title = "S2";
        private readonly string _mode1Title = "M1";
        private readonly string _mode2Title = "M2";
        private readonly string _responsible1Code = "R1";
        private readonly string _responsible2Code = "R2";
        private int _supplierModeId;
        private int _otherModeId;
        private int _responsible1Id;
        private int _responsible2Id;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var supplierMode = AddMode(context, _mode1Title, true);
                var responsible1 = AddResponsible(context, _responsible1Code);
                var journey = AddJourneyWithStep(context, _journeyTitle, _step1Title, supplierMode, responsible1);

                var otherMode = AddMode(context, _mode2Title, false);
                var responsible2 = AddResponsible(context, _responsible2Code);
                journey.AddStep(new Step(TestPlant, _step2Title, otherMode, responsible2));
                context.SaveChangesAsync().Wait();

                _supplierModeId = supplierMode.Id;
                _otherModeId = otherMode.Id;
                _responsible1Id = responsible1.Id;
                _responsible2Id = responsible2.Id;
            }
        }

        [TestMethod]
        public async Task HandleGetAllJourneysQuery_ShouldReturnNonVoidedJourneysOnly_WhenNotGettingVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllJourneysQueryHandler(context);

                var result = await dut.Handle(new GetAllJourneysQuery(false, null), default);

                var journeys = result.Data.ToList();

                Assert.AreEqual(1, journeys.Count);
                var journey = journeys.First();
            
                Assert.IsFalse(journey.IsVoided);
                Assert.AreEqual(_journeyTitle, journey.Title);
            
                var steps = journey.Steps.ToList();
                Assert.AreEqual(2, steps.Count);

                AssertStep(steps.ElementAt(0), _step1Title, _supplierModeId, _responsible1Id, _mode1Title, _responsible1Code, false, true);
                AssertStep(steps.ElementAt(1), _step2Title, _otherModeId, _responsible2Id, _mode2Title, _responsible2Code, false, false);
            }
        }

        [TestMethod]
        public async Task HandleGetAllJourneysQuery_ShouldReturnVoidedJourneys_WhenGettingVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Include(j => j.Steps).First();
                journey.IsVoided = true;
                journey.Steps.ToList().ForEach(s => s.IsVoided = true);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetAllJourneysQueryHandler(context);
                var result = await dut.Handle(new GetAllJourneysQuery(true, null), default);

                var journeys = result.Data.ToList();
                Assert.AreEqual(1, journeys.Count);
                var journey = journeys.First();
                Assert.IsTrue(journey.IsVoided);
                Assert.IsTrue(journey.Steps.All(s => s.IsVoided));
            }
        }

        private void AssertStep(StepDto step, string title, int modeId, int responsibleId, string modeTitle, string responsibleCode, bool isVoided, bool forSupplier)
        {
            Assert.IsNotNull(step.Mode);
            Assert.AreEqual(forSupplier, step.Mode.ForSupplier);
            Assert.IsNotNull(step.Responsible);
            Assert.AreEqual(title, step.Title);
            Assert.AreEqual(isVoided, step.IsVoided);
            Assert.AreEqual(modeId, step.Mode.Id);
            Assert.AreEqual(modeTitle, step.Mode.Title);
            Assert.AreEqual(responsibleId, step.Responsible.Id);
            Assert.AreEqual(responsibleCode, step.Responsible.Code);
        }
    }
}
