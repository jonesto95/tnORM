using tnORM.Shared;
using tnORM.Querying;
using tnORM.Querying.TableData;
using tnORM.Querying.TableFields;

namespace tnORM.Tests.Tables
{
    public class Employees : tnORMTableBase
    {
        public override string DatabaseName
        {
            get
            {
                string database = tnORMConfig.GetDatabaseName("TestDb");
                if(database != null)
                {
                    return database;
                }
                return "TestDb";
            }
        }
        private string databaseName;

        public override string SchemaName
        {
            get
            {
                return "dbo";
            }
        }

        public override string TableName
        {
            get
            {
                return "Employees";
            }
        }

        public override string TableAlias
        {
            get
            {
                return "e";
            }
        }

        public override EmployeesFields Fields => new EmployeesFields();

        public EmployeesData Data { get; protected set; } = new();


        public override void SetDataField(string field, object value)
        {
            Data = Data.SetProperty(field, value);
        }

        public override T GetDataField<T>(string field)
        {
            return Data.GetProperty<T>(field);
        }
    }
}


namespace tnORM.Querying.TableFields
{
    public partial class EmployeesFields : tnORMTableFieldCollection
    {
        [PrimaryKey]
        [DatabaseColumn]
        public static SqlField EmployeesGuidField { get; } = new("e", "EmployeesGuid", "EmployeesGuid");

        [DatabaseColumn]
        public static SqlField CreatedDate { get; } = new("e", "CreatedDate", "CreatedDate");

        [DatabaseColumn]
        public static SqlField LastModifiedTime { get; } = new("e", "LastModifiedTime", "LastModifiedTime");

        [DatabaseColumn]
        public static SqlField HiredDate { get; } = new("e", "HiredDate", "HiredDate");

        [DatabaseColumn]
        public static SqlField Name { get; } = new("e", "Name", "Name");
    }
}


namespace tnORM.Querying.TableData
{
    public partial class EmployeesData : tnORMTableDataCollection
    {
        public decimal EmployeesGuid { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public DateTime HiredDate { get; set; }

        public string Name { get; set; }


        public EmployeesData()
        {
            EmployeesGuid = tnORMShared.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }
    }
}
