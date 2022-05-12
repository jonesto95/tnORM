using System.Data;
using System.Data.SqlClient;
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

        public static T FirstOrDefault<T>(this SqlSelect select) where T: tnORMTableBase
        {
            var dataTable = ExecuteQueryText(select);
            if(dataTable.Rows.Count == 0)
            {
                return default;
            }
            return ConvertToModel<T>(dataTable.Rows[0]);
        }


        public static T SingleOrDefault<T>(this SqlSelect select) where T : tnORMTableBase
        {
            var dataTable = ExecuteQueryText(select);
            if (dataTable.Rows.Count == 0)
            {
                return default;
            }
            if(dataTable.Rows.Count > 1)
            {
                throw new TooManyResultsException(dataTable.Rows.Count);
            }
            return ConvertToModel<T>(dataTable.Rows[0]);
        }


        public static T[] ToModelArray<T>(this SqlSelect select) where T : tnORMTableBase
        {
            var dataTable = ExecuteQueryText(select);
            return ConvertToModelArray<T>(dataTable);
        }


        public static T ToScalarValueOrDefault<T>(this SqlSelect select)
        {
            var dataTable = ExecuteQueryText(select);
            if (dataTable.Rows.Count == 0)
            {
                return default;
            }
            return (T)dataTable.Rows[0].ItemArray[0];
        }


        public static T[] ToPrimitiveArray<T>(this SqlSelect select)
        {
            var dataTable = ExecuteQueryText(select);
            T[] result = new T[dataTable.Rows.Count];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = (T)(dataTable.Rows[i].ItemArray[0]);
            }
            return result;
        }


        public static T ConvertToModel<T>(this DataRow row) where T : tnORMTableBase
        {
            T entity = Activator.CreateInstance<T>();
            string[] fields = entity.Fields.GetFieldNames();
            foreach (string field in fields)
            {
                object cellValue = row.TryGetColumnValue<object>(field);
                entity.SetDataField(field, cellValue);
            }
            return entity;
        }


        public static T[] ConvertToModelArray<T>(this DataTable table) where T : tnORMTableBase
        {
            T[] result = new T[table.Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ConvertToModel<T>(table.Rows[i]);
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
                property = entity.GetDataField<object>(field);
                result += property.ToSqlString() + ", ";
            }
            result = result[..^2] + ")";
            return result;
        }


        public static void Insert<T>(this T entity) where T : tnORMTableBase
        {
            string insertSql = entity.InsertString();
            ExecuteNonQueryText(insertSql);
        }


        public static string UpdateString<T>(this T entity) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string[] fields = entity.Fields.GetFieldNames(false);
            string updateString =
                $"UPDATE {tableInstance.FullyQualifiedTableName} SET ";
            foreach(string field in fields)
            {
                object value = entity.GetDataField<object>(field);
                updateString += $"{field} = {value.ToSqlString()},";
            }
            updateString = updateString[..^1];

            var primaryKeys = entity.Fields.GetPrimaryKeyFields();
            object primaryKeyValue = entity.GetDataField<object>(primaryKeys[0].Name);
            updateString += $" WHERE {primaryKeys[0].Name} = {primaryKeyValue.ToSqlString()} ";
            for(int i = 1; i < primaryKeys.Length; i++)
            {
                primaryKeyValue = entity.GetDataField<object>(primaryKeys[i].Name);
                updateString += $"AND {primaryKeys[i].Name} = {primaryKeyValue.ToSqlString()} ";
            }
            return updateString;
        }


        public static void Update<T>(this T entity) where T : tnORMTableBase
        {
            string updateSql = entity.UpdateString();
            ExecuteNonQueryText(updateSql);
        }


        public static string DeleteString<T>(this T entity) where T : tnORMTableBase
        {
            T tableInstance = Activator.CreateInstance<T>();
            string deleteString = $"DELETE FROM {tableInstance.FullyQualifiedTableName} ";
            var primaryKeys = entity.Fields.GetPrimaryKeyFields();
            object primaryKeyValue = entity.GetDataField<object>(primaryKeys[0].Name);
            deleteString += $"WHERE {primaryKeys[0].Name} = {primaryKeyValue.ToSqlString()} ";
            for (int i = 1; i < primaryKeys.Length; i++)
            {
                primaryKeyValue = entity.GetDataField<object>(primaryKeys[i].Name);
                deleteString += $"AND {primaryKeys[i].Name} = {primaryKeyValue.ToSqlString()} ";
            }
            return deleteString;
        }


        public static void Delete<T>(this T entity) where T : tnORMTableBase
        {
            string deleteSql = entity.DeleteString();
            ExecuteNonQueryText(deleteSql);
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


    public class TooManyResultsException : Exception
    {
        public TooManyResultsException(int resultCount)
            : base($"{resultCount} results were found when at most one was expected")
        { }
    }
}
