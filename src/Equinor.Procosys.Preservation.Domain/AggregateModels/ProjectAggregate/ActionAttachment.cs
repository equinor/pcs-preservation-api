﻿using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class ActionAttachment : Attachment
    {
        protected ActionAttachment() : base()
        {
        }

        public ActionAttachment(string plant, string fileName, Guid blobStorageId, string title)
            : base(plant, fileName, blobStorageId, title)
        {
        }

        public override string BlobPath => $"{Plant.Substring(4)}/Action/{BlobStorageId.ToString()}/{FileName}";
    }
}
