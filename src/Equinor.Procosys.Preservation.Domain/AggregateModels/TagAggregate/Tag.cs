﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class Tag : SchemaEntityBase, IAggregateRoot
    {
        private readonly List<Requirement> _requirements = new List<Requirement>();

        public const int TagNoLengthMax = 255;
        public const int ProjectNameLengthMax = 255;

        protected Tag()
            : base(null)
        {
        }

        public Tag(
            string schema,
            string tagNo,
            string projectName,
            string areaCode,
            string calloff,
            string disciplineCode,
            string mcPkgNo,
            string commPkgNo,
            string purchaseOrderNo,
            string tagFunctionCode,
            Step step, 
            IEnumerable<Requirement> requirements)
            : base(schema)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }
            var reqList = requirements.ToList();
            if (reqList.Count < 1)
            {
                throw new Exception("Must have at least one requirement");
            }

            Status = PreservationStatus.NotStarted;
            TagNo = tagNo;
            ProjectName = projectName;
            AreaCode = areaCode;
            Calloff = calloff;
            DisciplineCode = disciplineCode;
            McPkgNo = mcPkgNo;
            CommPkgNo = commPkgNo;
            PurchaseOrderNo = purchaseOrderNo;
            TagFunctionCode = tagFunctionCode;
            StepId = step.Id;
            _requirements.AddRange(reqList);
        }

        public PreservationStatus Status { get; private set; }
        public string AreaCode { get; private set; }
        public string Calloff { get; private set; }
        public string CommPkgNo { get; private set; }
        public string DisciplineCode { get; private set; }
        public bool IsAreaTag { get; private set; }
        public string McPkgNo { get; private set; }
        public string ProjectName { get; private set; }
        public string PurchaseOrderNo { get; private set; }
        public int StepId { get; private set; }
        public string TagFunctionCode { get; private set; }
        public string TagNo { get; private set; }
        public IReadOnlyCollection<Requirement> Requirements => _requirements.AsReadOnly();
        public bool IsVoided { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;

        public void SetStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            StepId = step.Id;
        }

        public void AddRequirement(Requirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            _requirements.Add(requirement);
        }
    }
}
