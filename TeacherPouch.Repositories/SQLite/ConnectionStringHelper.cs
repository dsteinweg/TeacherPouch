using System.Collections.Generic;
using System.Configuration;

namespace TeacherPouch.Models
{
    public static class ConnectionStringHelper
    {
        private static string _connectionString = null;

        public static string GetConnectionString()
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
