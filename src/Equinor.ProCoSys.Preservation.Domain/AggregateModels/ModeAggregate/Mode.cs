﻿using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using HeboTech.TimeService;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public class Mode : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IVoidable
    {
        public const int TitleLengthMin = 3;
        public const int TitleLengthMax = 255;

        protected Mode()
            : base(null)
        {
        }

        public Mode(string plant, string title, bool forSupplier) : base(plant)
        {
            Title = title;
            ForSupplier = forSupplier;
        }

        public string Title { get; set; }
        public bool IsVoided { get; set; }
        public bool ForSupplier { get; set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.Now;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.Now;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }
    }
}
