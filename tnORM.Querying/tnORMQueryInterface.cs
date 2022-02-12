using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using tnORM.Shared;

namespace tnORM.Querying
{
    public static class tnORMQueryInterface
    {
        private static SqlConnection SqlConnection
        {
            get
            {
                if(sqlConnection == null)
                {
                    string connectionString = tnORMConfig.GetString("ConnectionString");
                    sqlConnection = new(connectionString);
                    sqlConnection.Open();
                }
                return sqlConnection;
            }
        }
        private static SqlConnection sqlConnection;


        #region Query methods

        public static DataTable ExecuteQueryText(string query)
        {
            SqlCommand sqlCommand = new(query, SqlConnection);
            DataTable result = new();
            using (SqlDataAdapter adapter = new(sqlCommand))
            {
                adapter.Fill(result);
            }
            return result;
        }


        public static DataSet ExecuteQueryTextIntoDataSet(string query)
        {
            var sqlCommand = new SqlCommand(query, SqlConnection);
            var result = new DataSet();
            using (var sqlDataAdapter = new SqlDataAdapter(sqlCommand))
            {
                sqlDataAdapter.Fill(result);
            }
            return result;
        }


        public static int ExecuteNonQueryText(string sqlText)
        {
            SqlCommand sqlCommand = new(sqlText, SqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }

        #endregion


        #region Query result conversion methods

        public static T[] ConvertToModelArray<T>(this DataTable table) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            T[] result = new T[table.Rows.Count];
            string[] fields = tableInstance.Fields.GetFieldNames();
            for (int i = 0; i < result.Length; i++)
            {
                T entity = Activator.CreateInstance<T>();
                foreach (string field in fields)
                {
                    object cellValue = table.Rows[i].TryGetColumnValue<object>(field);
                    entity.Data.SetProperty(field, cellValue);
                }
                result[i] = entity;
            }
            return result;
        }

        #endregion


        #region SQL text generators

        public static string InsertString<T>(this T entity) where T : tnORMTableBase
        {
            bool hasIdentityColumn = entity.Fields.HasIdentityColumn();
            string[] fields = entity.Fields.GetFieldNames(!hasIdentityColumn);
            string table = $"{entity.DatabaseName}.[{entity.SchemaName}].[{entity.TableName}]";
            string result = $"INSERT INTO {table} (";
            foreach(string field in fields)
            {
                result += $"{field}, ";
            }
            result = result[..^2] + ") VALUES (";
            object property = null;
            foreach(string field in fields)
            {
                property = entity.GetProperty<object>(field);
                result += property.ToSqlString() + ", ";
            }
            result = result[..^2] + ")";
            return result;
        }


        public static string UpdateString<T>(this T entity) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string[] fields = entity.Fields.GetFieldNames(false);
            string updateString =
                $"UPDATE {tableInstance.FullyQualifiedTableName} SET ";
            foreach(string field in fields)
            {
                object value = entity.GetProperty<object>(field);
                updateString += $"{field} = {value.ToSqlString()},";
            }
            updateString = updateString[..^1];

            var primaryKeys = entity.Fields.GetPrimaryKeyFields();
            object primaryKeyValue = entity.GetProperty<object>(primaryKeys[0].Name);
            updateString += $"WHERE {primaryKeys[0].Name} = {primaryKeyValue.ToSqlString()} ";
            for(int i = 1; i < primaryKeys.Length; i++)
            {
                primaryKeyValue = entity.GetProperty<object>(primaryKeys[i].Name);
                updateString += $"AND {primaryKeys[i].Name} = {primaryKeyValue.ToSqlString()} ";
            }
            return updateString;
        }


        public static string DeleteString<T>(this T entity) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string deleteString = $"DELETE FROM {tableInstance.FullyQualifiedTableName} ";
            var primaryKeys = entity.Fields.GetPrimaryKeyFields();
            object primaryKeyValue = entity.GetProperty<object>(primaryKeys[0].Name);
            deleteString += $"WHERE {primaryKeys[0].Name} = {primaryKeyValue.ToSqlString()} ";
            for (int i = 1; i < primaryKeys.Length; i++)
            {
                primaryKeyValue = entity.GetProperty<object>(primaryKeys[i].Name);
                deleteString += $"AND {primaryKeys[i].Name} = {primaryKeyValue.ToSqlString()} ";
            }
            return deleteString;
        }

        #endregion


        public static T TryGetColumnValue<T>(this DataTable table, string columnName)
        {
            return table.TryGetColumnValue<T>(columnName, 0);
        }


        public static T TryGetColumnValue<T>(this DataTable table, string columnName, int rowIndex = 0)
        {
            if (rowIndex < table.Rows.Count)
            {
                DataRow row = table.Rows[rowIndex];
                return row.TryGetColumnValue<T>(columnName);
            }
            // TODO: Log index out of range
            return default;
        }


        public static T TryGetColumnValue<T>(this DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
            {
                return default;
            }
            object value = row[columnName];
            if (value == DBNull.Value)
            {
                return default;
            }
            else
            {
                return (T)value;
            }
        }


        public static T Get<T>(this DataRow row, string columnName)
        {
            return (T)row[columnName];
        }


        public static T Get<T>(this DataRow row, int index)
        {
            return (T)row.ItemArray[index];
        }


        public static T Get<T>(this DataTable table, string columnName, int rowIndex = 0)
        {
            return (T)table.Rows[rowIndex][columnName];
        }


        public static T Get<T>(this DataTable table, int columnIndex, int rowIndex = 0)
        {
            return (T)table.Rows[rowIndex].ItemArray[columnIndex];
        }
    }


    public class PrimaryKeyNotDefinedException : Exception
    {
        public PrimaryKeyNotDefinedException(string tableName)
            : base($"Table {tableName} does not have a primary key defined")
        { }
    }
}
