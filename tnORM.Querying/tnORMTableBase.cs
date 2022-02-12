﻿using System.Reflection;

namespace tnORM.Querying
{
    public abstract class tnORMTableBase
    {
        public abstract string DatabaseName { get; }
        public abstract string SchemaName { get; }
        public abstract string TableName { get; }
        public abstract string TableAlias { get; }
        public abstract tnORMTableFieldCollection Fields { get; }
        public abstract tnORMTableDataCollection Data { get; }

        public string FullyQualifiedTableName
        {
            get
            {
                return $"{DatabaseName}.[{SchemaName}].[{TableName}]";
            }
        }
    }


    public abstract class tnORMTableFieldCollection
    {
        public bool HasIdentityColumn()
        {
            if (!hasIdentityColumn.HasValue)
            {
                hasIdentityColumn = false;
                var properties = GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (Attribute.IsDefined(property, typeof(Identity)))
                    {
                        hasIdentityColumn = true;
                        return true;
                    }
                }
            }
            return hasIdentityColumn.Value;
        }
        private bool? hasIdentityColumn;


        public string[] GetFieldNames()
        {
            return GetFieldNames(true);
        }


        public string[] GetFieldNames(bool includePrimaryKeyFields)
        {
            List<string> result = new();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(DatabaseColumn))
                    && (includePrimaryKeyFields || !Attribute.IsDefined(property, typeof(PrimaryKey))))
                {
                    result.Add(property.Name);
                }
            }
            return result.ToArray();
        }


        public PropertyInfo[] GetPrimaryKeyFields()
        {
            List<PropertyInfo> primaryKeysList = new();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(PrimaryKey)))
                {
                    primaryKeysList.Add(property);
                }
            }
            if (primaryKeysList.Count == 0)
            {
                string tableName = GetType().Name[..^6];
                throw new PrimaryKeyNotDefinedException(tableName);
            }
            return primaryKeysList.ToArray();
        }
    }

    public abstract class tnORMTableDataCollection
    {
    }
}