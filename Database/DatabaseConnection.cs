using System;
using Npgsql;

namespace AutoSalon.Database
{
    public static class DatabaseConnection
    {
        private static readonly string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=autosalon";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

    }
} 