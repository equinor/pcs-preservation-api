﻿using System;

namespace Equinor.Procosys.Preservation.Query.GetActionDetails
{
    public class ActionDetailsDto
    {
        public ActionDetailsDto(
            int id,
            PersonDto createdBy,
            DateTime createdAt,
            string title,
            string description,
            DateTime? dueTimeUtc,
            bool isClosed,
            PersonDto closedBy,
            DateTime? closedAtUtc,
            string rowVersion,
            PersonDto modifiedBy,
            DateTime? modifiedAt)
        {
            Id = id;
            CreatedBy = createdBy;
            CreatedAtUtc = createdAt;
            Title = title;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            ClosedBy = closedBy;
            ClosedAtUtc = closedAtUtc;
            RowVersion = rowVersion;
            ModifiedBy = modifiedBy;
            ModifiedAtUtc = modifiedAt;
        }

        public int Id { get; }
        public PersonDto CreatedBy { get; }
        public DateTime CreatedAtUtc { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public PersonDto ClosedBy { get; }
        public DateTime? ClosedAtUtc { get; }
        public string RowVersion { get; }
        public PersonDto ModifiedBy { get; }
        public DateTime? ModifiedAtUtc { get; }
    }
}
