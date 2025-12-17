using MySqlConnector;

namespace FCAT
{
    static class Database
    {
        static MySqlConnectionStringBuilder CreateConnectionStr()
        {
            var res = new MySqlConnectionStringBuilder
            {
                Server = Environment.GetEnvironmentVariable("DATABASE_ADDR"),
                UserID = Environment.GetEnvironmentVariable("DATABASE_USER"),
                Password = Environment.GetEnvironmentVariable("DATABASE_PASSWD"),
                Database = "elite",
            };

            return res;
        }
        private static async Task<int> GetTableLenth(MySqlConnection conn, string tableName)
        {
            await using var tableLenCommand = conn.CreateCommand();
            tableLenCommand.CommandText = $"SELECT COUNT(*) FROM {tableName};";
            await using var reader = await tableLenCommand.ExecuteReaderAsync();
            reader.Read();
            int TableLength = Convert.ToInt32(reader.GetInt64(0));
            return TableLength;
        }
        public static async Task<List<string>> LoadStarSystemsAsync()
        {
            using var conn = new MySqlConnection(CreateConnectionStr().ConnectionString);
            await conn.OpenAsync();

            List<string> result = [];

            int cacheFileLen = File.ReadLines(@"systems.csv").Count();
            int tableLen = await GetTableLenth(conn, "star_systems");

            if (!(File.Exists("systems.csv") && cacheFileLen == tableLen))
            {//pull table
                Console.WriteLine("Updating systems.csv cache file");

                File.Delete("systems.csv");
                using StreamWriter writer = new("systems.csv");

                await using var command = conn.CreateCommand();
                command.CommandText = @"SELECT system_name,latitude,longitude,height FROM star_systems;";
                await using var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    string system_name = reader.GetString("system_name");
                    int latitude = Convert.ToInt32(reader.GetFloat("latitude"));
                    int longitude = Convert.ToInt32(reader.GetFloat("longitude"));
                    int height = Convert.ToInt32(reader.GetFloat("height"));

                    //Purico,87.625,113.938,-60.0
                    result.Add($"{system_name},{latitude},{longitude},{height}");
                    writer.WriteLine($"{system_name},{latitude},{longitude},{height}");
                }

                writer.Close();
            }
            else
            {
                Console.WriteLine("Using systems.csv cache file");
                using StreamReader reader = new("systems.csv");
                bool isDone = false;
                while (!isDone)
                {
                    string? line = await reader.ReadLineAsync();
                    isDone = line == string.Empty || line == null;
                    if (!isDone)
                    {
                        result.Add(line!);
                    }
                }
                reader.Close();
            }
            return result;
        }
        public static async Task<List<string>> LoadCarriersAsync()
        {
            using var conn = new MySqlConnection(CreateConnectionStr().ConnectionString);
            await conn.OpenAsync();

            List<string> result = [];

            int cacheFileLen = File.ReadLines(@"carriers.csv").Count();
            int tableLen = await GetTableLenth(conn, "FleetCarriers");

            if (!(File.Exists("carriers.csv") && cacheFileLen == tableLen))
            {//pull table
                Console.WriteLine("Updating carriers.csv cache file");

                File.Delete("carriers.csv");
                using StreamWriter writer = new("carriers.csv");

                await using var command = conn.CreateCommand();
                command.CommandText = @"SELECT * FROM FleetCarriers;";
                await using var reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    string system_name = reader.GetString("system_name");
                    int carriers = /*Convert.ToInt32(*/reader.GetInt32("carriers")/*)*/;

                    //Purico,87
                    result.Add($"{system_name},{carriers}");
                    writer.WriteLine($"{system_name},{carriers}");
                }

                writer.Close();
            }
            else
            {
                Console.WriteLine("Using carriers.csv cache file");
                using StreamReader reader = new("carriers.csv");
                bool isDone = false;
                while (!isDone)
                {
                    string? line = await reader.ReadLineAsync();
                    isDone = line == string.Empty || line == null;
                    if (!isDone)
                    {
                        result.Add(line!);
                    }
                }
                reader.Close();
            }
            return result;
        }
    }
}