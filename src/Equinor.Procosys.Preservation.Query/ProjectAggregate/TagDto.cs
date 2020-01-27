﻿using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class TagDto
    {
        public TagDto(
            int id,
            string areaCode,
            string calloffNo,
            string commPkgNo,
            string disciplineCode,
            bool isAreaTag,
            bool isVoided,
            string mcPkgNo, 
            bool readyToBePreserved,
            string purchaseOrderNo,
            IEnumerable<RequirementDto> requirements,
            PreservationStatus status,
            int stepId,
            string tagFunctionCode,
            string tagDescription,
            string tagNo)
        {
            Id = id;
            AreaCode = areaCode;
            CalloffNo = calloffNo;
            CommPkgNo = commPkgNo;
            Description = tagDescription;
            DisciplineCode = disciplineCode;
            IsAreaTag = isAreaTag;
            IsVoided = isVoided;
            McPkgNo = mcPkgNo;
            ReadyToBePreserved = readyToBePreserved;
            PurchaseOrderNo = purchaseOrderNo;
            TagNo = tagNo;
            Status = status;
            StepId = stepId;
            TagFunctionCode = tagFunctionCode;
            Requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
        }

        public string AreaCode { get; }
        public string CalloffNo { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public int Id { get; }
        public bool IsAreaTag { get; }
        public bool IsVoided { get; }
        public string McPkgNo { get; }
        public string PurchaseOrderNo { get; }
        public bool ReadyToBePreserved { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public PreservationStatus Status { get; }
        public int StepId { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
    }
}
