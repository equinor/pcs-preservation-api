﻿using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        private readonly PreservationContext _context;

        public PersonRepository(PreservationContext context)
            : base(context.Persons) => _context = context;

        public Task<Person> GetByOidAsync(Guid oid)
            => DefaultQuery.SingleOrDefaultAsync(p => p.Oid == oid);

        public Task<Person> GetWithSavedFilterByOidAsync(Guid oid)
            => DefaultQuery
                .Include(p => p.SavedFilters)
                .SingleOrDefaultAsync(p => p.Oid == oid);

        public void RemoveSavedFilter(SavedFilter savedFilter)
            => _context.SavedFilters.Remove(savedFilter);

    }
}
