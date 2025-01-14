﻿using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<Person> GetByOidAsync(Guid oid);
        Task<Person> GetWithSavedFiltersByOidAsync(Guid oid);
        Task<Person> GetReadOnlyByIdAsync(int id);
        void RemoveSavedFilter(SavedFilter savedFilter);
    }
}
