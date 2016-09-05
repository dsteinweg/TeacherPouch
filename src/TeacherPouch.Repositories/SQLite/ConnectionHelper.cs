using System.Configuration;
using System.Data.SQLite;

namespace TeacherPouch.Models
{
    public static class ConnectionHelper
    {
        private static string _connectionString = null;

        public static SQLiteConnection GetSQLiteConnection()
        {
            return new SQLiteConnection(GetConnectionString());
        }

        private static string GetConnectionString()
        {
            if (_connectionString == null)
            {
                var connectionStringObj = ConfigurationManager.ConnectionStrings["TeacherPouch"];

                if (connectionStringObj == null)
                    throw new ConfigurationErrorsException("\"TeacherPouch\" connection string missing.");

                _connectionString = connectionStringObj.ConnectionString;
            }

            return _connectionString;
        }
    }
}
