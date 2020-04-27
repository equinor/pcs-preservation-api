﻿using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class ActionDto
    {
        public ActionDto(
            int id,
            string title,
            DateTime? dueTimeUtc,
            bool isClosed,
            ulong rowVersion)
        {
            Id = id;
            Title = title;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public ulong RowVersion { get; }
    }
}
