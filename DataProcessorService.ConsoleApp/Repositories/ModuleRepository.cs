using DataProcessorService.ConsoleApp.Interfaces;
using DataProcessorService.ConsoleApp.Models;
using Microsoft.Data.Sqlite;


namespace DataProcessorService.ConsoleApp.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly string _connectionString;

        public ModuleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Modules (
                    ModuleCategoryID TEXT PRIMARY KEY,
                    State INTEGER NOT NULL); ";
                      
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpsertAsync(Module module)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Modules (ModuleCategoryID, State)
                VALUES ($id, $state)
                ON CONFLICT(ModuleCategoryID)
                DO UPDATE SET State = excluded.State; ";
      
            command.Parameters.AddWithValue("$id", module.ModuleCategoryID);
            command.Parameters.AddWithValue("$state", module.State);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            var modules = new List<Module>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT ModuleCategoryID, State FROM Modules";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                modules.Add(new Module
                {
                    ModuleCategoryID = reader.GetString(0),
                    State = (ModuleState)reader.GetInt32(1)
                });
            }

            return modules;
        }
    }
}
