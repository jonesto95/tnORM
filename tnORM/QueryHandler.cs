using System.Data;
using System.Data.SqlClient;

using tnORM.Shared;

namespace tnORM
{
    public static class QueryHandler
    {
        private static string? connectionString;
        private static SqlConnection SqlConnection
        {
            get
            {
                if(sqlConnection == null)
                {
                    connectionString = tnORMConfig.GetString("ConnectionString");
                    sqlConnection = new(connectionString);
                    sqlConnection.Open();
                }
                return sqlConnection;
            }
        }
        private static SqlConnection sqlConnection;


        public static DataTable GetQueryResults(string query)
        {
            DataTable table = new();
            using (SqlCommand command = new (query, SqlConnection))
            {
                using SqlDataAdapter adapter = new(command);
                adapter.Fill(table);
            }
            return table;
        }


        public static void ExecuteNonQuery(string query)
        {
            using SqlCommand command = new(query, SqlConnection);
            command.ExecuteNonQuery();
        }
    }
}
