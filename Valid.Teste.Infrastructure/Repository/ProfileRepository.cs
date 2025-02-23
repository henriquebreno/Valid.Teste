using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valid.Teste.Domain.Entities;
using Valid.Teste.Domain.Interfaces;
using Valid.Teste.Infrastructure.Repository;

namespace Valid.Teste.Infrastructure.Repository
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        public ProfileRepository(DapperContext context) : base(context)
        {
        }

        public async Task<Profile> GetByProfileName(string profileName)
        {
            var query = $"SELECT * FROM profiles WHERE ProfileName = @ProfileName";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Profile>(query, new { ProfileName = profileName });
        }





        public async Task<int> DeleteByProfileName(string profileName)
        {
            var query = $"DELETE FROM profiles WHERE ProfileName = @ProfileName";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, new { ProfileName = profileName });
        }


    }
}
