﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public class Person : EntityBase, IAggregateRoot, IModificationAuditable
    {
        public const int FirstNameLengthMax = 64;
        public const int LastNameLengthMax = 64;

        private readonly List<SavedFilter> _savedFilters = new List<SavedFilter>();

        protected Person() : base()
        {
        }

        public Person(Guid oid, string firstName, string lastName) : base()
        {
            Oid = oid;
            FirstName = firstName;
            LastName = lastName;
        }

        public IReadOnlyCollection<SavedFilter> SavedFilters => _savedFilters.AsReadOnly();
        public Guid Oid { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetModified(Person modifiedBy)
        {
            ModifiedAtUtc = TimeService.UtcNow;
            if (modifiedBy == null)
            {
                throw new ArgumentNullException(nameof(modifiedBy));
            }
            ModifiedById = modifiedBy.Id;
        }

        public void AddSavedFilter(SavedFilter savedFilter)
        {
            if (savedFilter == null)
            {
                throw new ArgumentNullException(nameof(savedFilter));
            }

            _savedFilters.Add(savedFilter);
        }

        public SavedFilter GetDefaultFilter(string plant, int projectId)
        {
            var savedFil = _savedFilters.SingleOrDefault(s => s.Plant == plant && s.ProjectId == projectId && s.DefaultFilter);
            return savedFil;
        }
    }
}
