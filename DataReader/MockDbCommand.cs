using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest
{
    public class MockDbCommand<T> : IDbCommand
    {
        public MockDbCommand(string commandText, DataReaderCollection<T> commandResult)
        {
            this.CommandText = commandText;
            this.CommandResult = commandResult;
        }

        public void Cancel()
        {
            
        }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return CommandResult;
        }

        public IDataReader ExecuteReader()
        {
            return CommandResult;
        }

        public object ExecuteScalar()
        {
            return CommandResult[0];
        }

        public IDataParameterCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }

        public void Prepare()
        {
        }

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Dispose()
        {
            
        }

        public DataReaderCollection<T> CommandResult { get; set; }
    }
}
