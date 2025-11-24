using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IJournalRepository
    {
        Task<List<Journal>> GetAllByTouristId(long touristId);
        Task<Journal?> GetById(long journalId);
        Task<Journal> Save(Journal journal);
        Task Delete(long journalId);
    }
}

