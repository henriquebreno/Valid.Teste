using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Valid.Teste.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly IDbConnection _connection;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void EnsureDatabaseSetup()
        {
            var query = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfileParameters')
            BEGIN
                CREATE TABLE ProfileParameters (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    ProfileName NVARCHAR(100) UNIQUE NOT NULL,
                    ParametersJson NVARCHAR(MAX) NOT NULL DEFAULT ('{}')
                );
            END
            ";

            _connection.Execute(query);
        }
    }

}
