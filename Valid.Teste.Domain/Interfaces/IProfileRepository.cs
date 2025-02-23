using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valid.Teste.Domain.Entities;

namespace Valid.Teste.Domain.Interfaces
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        Task<Profile> GetByProfileName(string profileName);

        Task<int> DeleteByProfileName(string profileName);
    }

}
