﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Requirement : SchemaEntityBase, ICreationAuditable, IModificationAuditable
    {
        public const int InitialPreservationPeriodStatusMax = 64;

        // _initialPreservationPeriodStatus is made as DB property. Can't be readonly
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private PreservationPeriodStatus _initialPreservationPeriodStatus;
        private readonly List<PreservationPeriod> _preservationPeriods = new List<PreservationPeriod>();

        protected Requirement()
            : base(null)
        {
        }

        public Requirement(string schema, int intervalWeeks, RequirementDefinition requirementDefinition)
            : base(schema)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }
            
            if (requirementDefinition.Schema != schema)
            {
                throw new ArgumentException($"Can't relate item in {requirementDefinition.Schema} to item in {schema}");
            }

            IntervalWeeks = intervalWeeks;
            RequirementDefinitionId = requirementDefinition.Id;
            
            _initialPreservationPeriodStatus = requirementDefinition.NeedsUserInput
                ? PreservationPeriodStatus.NeedsUserInput
                : PreservationPeriodStatus.ReadyToBePreserved;
        }

        public int IntervalWeeks { get; private set; }
        public DateTime? NextDueTimeUtc { get; private set; }
        public bool IsVoided { get; private set; }
        public int RequirementDefinitionId { get; private set; }
        public IReadOnlyCollection<PreservationPeriod> PreservationPeriods => _preservationPeriods.AsReadOnly();
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public bool ReadyToBePreserved => PeriodReadyToBePreserved != null;

        public override string ToString() => $"Interval {IntervalWeeks}, NextDue {NextDueTimeUtc}, ReqDefId {RequirementDefinitionId}";

        public int? GetNextDueInWeeks()
        {
            if (!NextDueTimeUtc.HasValue)
            {
                return null;
            }

            return TimeService.UtcNow.GetWeeksUntil(NextDueTimeUtc.Value);
        }
        
        public bool IsReadyAndDueToBePreserved()
        {
            if (!ReadyToBePreserved)
            {
                return false;
            }

            var nextDueInWeeks = GetNextDueInWeeks();

            return nextDueInWeeks.HasValue && nextDueInWeeks.Value <= 0;
        }

        public bool HasActivePeriod => ActivePeriod != null;

        public void StartPreservation()
        {
            if (HasActivePeriod)
            {
                throw new Exception($"{nameof(Requirement)} {Id} already have an active {nameof(PreservationPeriod)}. Can't start");
            }
            PrepareNewPreservation();
        }

        public void Preserve(Person preservedBy, bool bulkPreserved)
        {
            if (!ReadyToBePreserved)
            {
                throw new Exception($"{nameof(Requirement)} {Id} is not ready to be preserved");
            }

            PeriodReadyToBePreserved.Preserve(preservedBy, bulkPreserved);
            PrepareNewPreservation();
        }

        public PreservationPeriod ActivePeriod
            => PreservationPeriods.SingleOrDefault(pp => 
                pp.Status == PreservationPeriodStatus.ReadyToBePreserved ||
                pp.Status == PreservationPeriodStatus.NeedsUserInput);

        public FieldValue GetCurrentFieldValue(Field field)
            => ActivePeriod?.GetFieldValue(field.Id);

        public string GetCurrentComment() => ActivePeriod?.Comment;

        public FieldValue GetPreviousFieldValue(Field field)
        {
            if (!field.ShowPrevious.HasValue || !field.ShowPrevious.Value)
            {
                return null;
            }
            
            var lastPreservedPeriod = PreservationPeriods
                .Where(pp => pp.Status == PreservationPeriodStatus.Preserved)
                .OrderByDescending(pp => pp.PreservationRecord.PreservedAtUtc)
                .FirstOrDefault();
        
            return lastPreservedPeriod?.GetFieldValue(field.Id);
        }

        public void RecordValues(Dictionary<int, string> fieldValues, string comment, RequirementDefinition requirementDefinition)
        {
            if (requirementDefinition == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinition));
            }

            if (requirementDefinition.Id != RequirementDefinitionId)
            {
                throw new Exception($"{nameof(Requirement)} {Id} belong to RequirementDefinition {RequirementDefinitionId}. Can't record values for RequirementDefinition {requirementDefinition.Id}");
            }

            if (!HasActivePeriod)
            {
                throw new Exception($"{nameof(Requirement)} {Id} don't have an active {nameof(PreservationPeriod)}. Can't record values");
            }

            var period = ActivePeriod;

            if (fieldValues != null)
            {
                foreach (var fieldValue in fieldValues)
                {
                    var field = requirementDefinition.Fields.Single(f => f.Id == fieldValue.Key);

                    period.RecordValueForField(field, fieldValue.Value);
                }
            }

            period.UpdateStatus(requirementDefinition);
            period.SetComment(comment);
        }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            ModifiedById = modifiedBy.Id;
        }

        private PreservationPeriod PeriodReadyToBePreserved
            => PreservationPeriods.SingleOrDefault(pp => pp.Status == PreservationPeriodStatus.ReadyToBePreserved);

        private void PrepareNewPreservation()
        {
            NextDueTimeUtc = TimeService.UtcNow.AddWeeks(IntervalWeeks);
            var preservationPeriod = new PreservationPeriod(Schema, NextDueTimeUtc.Value, _initialPreservationPeriodStatus);
            _preservationPeriods.Add(preservationPeriod);
        }
    }
}
