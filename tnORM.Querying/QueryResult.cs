using System.Data;

namespace tnORM.Querying
{
    public class QueryResult
    {
        public string[] Columns { get; private set; }
        public object[][] Data { get; private set; }
        public int RowCount { get; private set; }


        public QueryResult(DataTable data)
        {
            int columnCount = data.Columns.Count;
            RowCount = data.Rows.Count;
            Columns = new string[columnCount];
            Data = new object[columnCount][];
            for(int i = 0; i < Columns.Length; i++)
            {
                Columns[i] = data.Columns[i].ColumnName;
            }
            for (int i = 0; i < Columns.Length; i++)
            {
                Data[i] = new object[RowCount];
                for (int j = 0; j < RowCount; j++)
                {
                    Data[i][j] = data.Rows[j].ItemArray[i];
                }
            }
        }


        public string ToJson()
        {
            string result = $"{{\"RowCount\": {RowCount}, \"Columns\":[";
            if(Columns.Length > 0)
            {
                foreach(string column in Columns)
                {
                    result += $"\"{column.Replace("\"", "\\\"")}\",";
                }
                result = result[..^1];
            }
            result += "],\"Data\":[";
            if(RowCount > 0)
            {
                for(int i = 0; i < RowCount; i++)
                {
                    result += "[";
                    for (int j = 0; j < Columns.Length; j++)
                    {
                        string dataValue = Data[j][i].ToString()
                            .Replace("\\", "\\\\")
                            .Replace("\"", "\\\"");
                        result += $"\"{dataValue}\",";
                    }
                    result = result[..^1] + "],";
                }
                result = result[..^1];
            }
            result += "]}";
            return result;
        }
    }
}
