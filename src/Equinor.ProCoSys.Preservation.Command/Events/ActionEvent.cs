﻿using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class ActionEvent : IActionEventV1
{
    public ActionEvent(Guid guid, string plant, string projectName, string tagNr, string title, string description, DateOnly? dueDate, bool overdue, DateOnly? closed)
    {
        Guid = guid;
        Plant = plant;
        ProjectName = projectName;
        TagNr = tagNr;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Overdue = overdue;
        Closed = closed;
    }

    public Guid Guid { get; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; }
    public string ProjectName { get; }
    public string TagNr { get; }
    public string Title { get; }
    public string Description { get; }
    public DateOnly? DueDate { get; }
    public bool Overdue { get; }
    public DateOnly? Closed { get; }
}
