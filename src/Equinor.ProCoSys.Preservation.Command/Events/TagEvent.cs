﻿using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class TagEvent : ITagEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public string Plant { get; init; }
    public string ProjectName { get; init; }

    public Guid StepGuid { get; init; }

    public string Description { get; init; }
    public string Remark { get; init; }
    public DateTime? NextDueTimeUtc { get; init; }
    public string DisciplineCode { get; init; }
    public string AreaCode { get; init; }
    public string TagFunctionCode { get; init; }
    public string PurchaseOrderNo { get; init; }
    public string TagType { get; init; }
    public string StorageArea { get; init; }
    public string AreaDescription { get; init; }
    public string DisciplineDescription { get; init; }
    public string Status { get; init; }
    public Guid? CommPkgGuid { get; init; }
    public Guid? McPkgGuid { get; init; }
    public bool IsVoided { get; init; }
    public bool IsVoidedInSource { get; init; }
    public bool IsDeletedInSource { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}