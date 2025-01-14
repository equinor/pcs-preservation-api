using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable, IHaveGuid
    {
        public const string DuplicatePrefix = " - Copy";
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        private readonly List<Step> _steps = new List<Step>();

        protected Journey()
            : base(null)
        {
        }

        public Journey(string schema, string title, Project project = null)
            : base(schema)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            Title = title;
            Guid = Guid.NewGuid();
            Project = project;
        }

        public Guid Guid { get; private set; }
        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
        public string Title { get; set; }
        public bool IsVoided { get; set; }

        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }
        public Project? Project { get; set; }
        public override string ToString() => Title;

        public void AddStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }
            
            if (step.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {step.Plant} to item in {Plant}");
            }

            _steps.Add(step);
            step.SortKey = _steps.Select(s => s.SortKey).Max() + 1;
            
            AddDomainEvent(new ChildAddedEvent<Journey, Step>(this, step));
        }

        public void RemoveStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (!step.IsVoided)
            {
                throw new Exception($"{nameof(step)} must be voided before delete");
            }
            
            if (step.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {step.Plant} from item in {Plant}");
            }

            _steps.Remove(step);
            AddDomainEvent(new DeletedEvent<Step>(step));
        }

        public void SwapSteps(int stepId1, int stepId2)
        {
            if (!AreAdjacentSteps(stepId1, stepId2))
            {
                throw new Exception($"{nameof(Step)} {stepId1} and {stepId1} in {nameof(Journey)} {Title} are not adjacent and can't be swapped");
            }

            var step1 = _steps.Single(s => s.Id == stepId1);
            var step2 = _steps.Single(s => s.Id == stepId2);
            (step1.SortKey, step2.SortKey) = (step2.SortKey, step1.SortKey);
            
            AddDomainEvent(new ChildModifiedEvent<Journey, Step>(this, step1));
            AddDomainEvent(new ChildModifiedEvent<Journey, Step>(this, step2));
        }

        public Step VoidStep(int stepId, string stepRowVersion)
        {
            var step = Steps.Single(s => s.Id == stepId);

            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (step.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {step.Plant} to item in {Plant}");
            }

            step.IsVoided = true;
            step.SetRowVersion(stepRowVersion);
            
            AddDomainEvent(new ChildModifiedEvent<Journey, Step>(this, step));
            
            return step;
        }

        public Step UnvoidStep(int stepId, string stepRowVersion)
        {
            var step = Steps.Single(s => s.Id == stepId);

            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (step.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {step.Plant} to item in {Plant}");
            }

            step.IsVoided = false;
            step.SetRowVersion(stepRowVersion);
            
            AddDomainEvent(new ChildModifiedEvent<Journey, Step>(this, step));
            
            return step;
        }

        public bool AreAdjacentSteps(int stepId1, int stepId2)
        {
            var orderedSteps = OrderedSteps().ToList();

            var stepIndex1 = orderedSteps.FindIndex(s => s.Id == stepId1);
            if (stepIndex1 == -1)
            {
                throw new Exception($"{nameof(Step)} {stepId1} not found in {nameof(Journey)} {Title}");
            }

            var stepIndex2 = orderedSteps.FindIndex(s => s.Id == stepId2);
            if (stepIndex2 == -1)
            {
                throw new Exception($"{nameof(Step)} {stepId2} not found in {nameof(Journey)} {Title}");
            }

            return Math.Abs(stepIndex1 - stepIndex2) == 1;
        }

        public Step GetNextStep(int stepId)
        {
            var orderedSteps = OrderedSteps().Where(r => !r.IsVoided).ToList();
            var stepIndex = orderedSteps.FindIndex(s => s.Id == stepId);
            if (stepIndex < orderedSteps.Count - 1)
            {
                return orderedSteps[stepIndex+1];
            }

            return null;
        }

        public bool HasNextStep(int stepId) => GetNextStep(stepId) != null;

        public IOrderedEnumerable<Step> OrderedSteps() => _steps.OrderBy(r => r.SortKey);

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;

            AddDomainEvent(new CreatedEvent<Journey>(this));
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;

            AddDomainEvent(new ModifiedEvent<Journey>(this));
        }
    }
}
