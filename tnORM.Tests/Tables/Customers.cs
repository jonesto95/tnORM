using tnORM.Shared;
using tnORM.Querying;
using tnORM.Querying.TableData;
using tnORM.Querying.TableFields;

namespace tnORM.Tests.Tables
{
    public class Customers : tnORMTableBase
    {
        public override string DatabaseName
        {
            get
            {
                if (!string.IsNullOrEmpty(databaseName))
                {
                    return databaseName;
                }
                string database = tnORMConfig.GetDatabaseName("TestDb");
                if (database != null)
                {
                    databaseName = database;
                    return database;
                }
                databaseName = "TestDb";
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
                return "Customers";
            }
        }

        public override string TableAlias
        {
            get
            {
                return "c";
            }
        }


        public override CustomersFields Fields => new();


        public new CustomersData Data = new();

        public override void SetDataField(string field, object value)
        {
            Data = Data.SetProperty(field, value);
        }
    }
}


namespace tnORM.Querying.TableFields
{
    public partial class CustomersFields : tnORMTableFieldCollection
    {
        [PrimaryKey]
        [DatabaseColumn]
        public static SqlField CustomersGuid { get; } = new("c", "CustomersGuid", "CustomersGuid");
        
        [DatabaseColumn]
        public static SqlField CreatedDate { get; } = new("c", "CreatedDate", "CreatedDate");

        [DatabaseColumn]
        public static SqlField LastModifiedTime { get; } = new("c", "LastModifiedTime", "LastModifiedTime");

        [DatabaseColumn]
        public static SqlField Name { get; } = new("c", "Name", "Name");

        [DatabaseColumn]
        public static SqlField EmployeesGuid { get; } = new("c", "EmployeesGuid", "EmployeesGuid");

    }
}


namespace tnORM.Querying.TableData
{
    public partial class CustomersData : tnORMTableDataCollection
    {
        public decimal CustomersGuid { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public string Name { get;  set; }

        public decimal? EmployeesGuid { get; set; }


        public CustomersData()
        {
            CustomersGuid = tnORMShared.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }
    }
}
