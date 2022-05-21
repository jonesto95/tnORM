namespace tnORM.Querying
{
    public class SqlTransaction
    {
        public Exception Exception { get; set; }

        public bool IdentityInsert { get; set; } = false;

        private readonly List<string> Statements = new();

        public SqlTransaction() { }


        public SqlTransaction(bool identityInsert)
        {
            IdentityInsert = identityInsert;
        }


        public SqlTransaction Select(SqlSelect select)
        {
            return AddQueryText(select);
        }


        public SqlTransaction Insert<T>(T entity) where T : tnORMTableBase
        {
            string insertString = entity.InsertString(IdentityInsert);
            AddQueryText(insertString);
            return this;
        }


        public SqlTransaction InsertList<T>(IEnumerable<T> entities) where T : tnORMTableBase
        {
            foreach(T entity in entities)
            {
                Insert(entity);
            }
            return this;
        }


        public SqlTransaction Update(SqlUpdate update)
        {
            AddQueryText(update);
            return this;
        }


        public SqlTransaction Update<T>(T entity) where T : tnORMTableBase
        {
            string updateString = entity.UpdateString();
            AddQueryText(updateString);
            return this;
        }


        public SqlTransaction UpdateList<T>(IEnumerable<T> entities) where T : tnORMTableBase
        {
            foreach (T entity in entities)
            {
                Update(entity);
            }
            return this;
        }


        public SqlTransaction Delete(SqlDelete delete)
        {
            AddQueryText(delete);
            return this;
        }


        public SqlTransaction Delete<T>(T entity) where T : tnORMTableBase
        {
            string DeleteString = entity.DeleteString();
            AddQueryText(DeleteString);
            return this;
        }


        public SqlTransaction DeleteList<T>(IEnumerable<T> entities) where T : tnORMTableBase
        {
            foreach (T entity in entities)
            {
                Delete(entity);
            }
            return this;
        }


        public SqlTransaction AppendTransaction(SqlTransaction transaction)
        {
            Statements.AddRange(transaction.Statements);
            return this;
        }


        public SqlTransaction AddQueryText(string sqlText)
        {
            Statements.Add(sqlText);
            return this;
        }


        public override string ToString()
        {
            return ToSqlString(true);
        }


        public string ToSqlString()
        {
            return ToSqlString(true);
        }


        public string ToSqlString(bool commitTransaction)
        {
            string endAction = commitTransaction ? "COMMIT" : "ROLLBACK";
            string result = "BEGIN TRANSACTION BEGIN TRY ";
            foreach (string statement in Statements)
            {
                result += $"{statement} ";
            }
            result += $" {endAction} END TRY BEGIN CATCH ROLLBACK; THROW END CATCH";
            return result;
        }


        #region Execution methods

        public void Execute()
        {
            Execute(true);
        }


        public int Execute(bool commitTransaction)
        {
            string transactionSql = ToSqlString(commitTransaction);
            return tnORMQueryInterface.ExecuteNonQueryText(transactionSql);
        }


        public bool TryExecute()
        {
            bool result = false;
            try
            {
                Execute(true);
                return true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            return result;
        }


        public bool TestExecute()
        {
            bool result = false;
            try
            {
                Execute(false);
                return true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            return result;
        }

        #endregion
    }
}
