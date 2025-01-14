using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Project : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable, IHaveGuid
    {
        private readonly List<Tag> _tags = new();

        public const int NameLengthMax = 30;
        public const int DescriptionLengthMax = 1000;

        protected Project()
            : base(null)
        {
        }

        public Project(string plant, string name, string description, Guid guid)
            : base(plant)
        {
            Name = name;
            Description = description;
            Guid = guid;
        }

        // private setters needed for Entity Framework
        public string Name { get; private set; }
        public string Description { get; set; }
        public bool IsClosed { get; set; }
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }
        public Guid Guid { get; private set; }
        public List<Journey> Journeys { get; set; } = [];
        public void AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.Plant != Plant)
            {
                throw new ArgumentException($"Can't relate item in {tag.Plant} to item in {Plant}");
            }

            if (_tags.Any(t => t.TagNo == tag.TagNo))
            {
                throw new ArgumentException($"Tag {tag.TagNo} already exists in project");
            }

            _tags.Add(tag);

            AddDomainEvent(new ChildAddedEvent<Project, Tag>(this, tag));
        }

        public void Close() => IsClosed = true;

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }

        public void RemoveTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (!tag.IsVoided)
            {
                throw new Exception($"{nameof(tag)} must be voided before delete");
            }

            if (tag.Plant != Plant)
            {
                throw new ArgumentException($"Can't remove item in {tag.Plant} from item in {Plant}");
            }

            _tags.Remove(tag);
        }

        public void MoveMcPkg(string mcPkgNo, string fromCommPkg, string toCommPkg)
        {
            if (string.IsNullOrWhiteSpace(mcPkgNo) || string.IsNullOrWhiteSpace(fromCommPkg) || string.IsNullOrWhiteSpace(toCommPkg))
            {
                throw new ArgumentNullException($"Unable to move pkg {mcPkgNo} from {fromCommPkg} to {toCommPkg}.");
            }

            var affectedTags = _tags.Where(t => t.CommPkgNo == fromCommPkg && t.McPkgNo == mcPkgNo).ToList();

            affectedTags.ForEach(t => t.CommPkgNo = toCommPkg);
        }

        public void RenameMcPkg(string commPkgNo, string fromMcPkgNo, string toMcPkgNo)
        {
            if (string.IsNullOrWhiteSpace(fromMcPkgNo) || string.IsNullOrWhiteSpace(toMcPkgNo) || string.IsNullOrWhiteSpace(commPkgNo))
            {
                throw new ArgumentNullException($"Unable to rename mc pkg from {fromMcPkgNo} to {toMcPkgNo} on comm pkg {commPkgNo}.");
            }

            var affectedTags = _tags.Where(t => t.CommPkgNo == commPkgNo && t.McPkgNo == fromMcPkgNo).ToList();

            affectedTags.ForEach(t => t.McPkgNo = toMcPkgNo);
        }

        public void MoveToProject(Tag tag, Project toProject)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (toProject == null)
            {
                throw new ArgumentNullException(nameof(toProject));
            }

            _tags.Remove(tag);
            toProject.AddTag(tag);
        }
    }
}
