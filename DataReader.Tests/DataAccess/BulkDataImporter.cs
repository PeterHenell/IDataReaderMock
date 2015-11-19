using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest.DataAccess
{
    public class BulkDataImporter
    {
        private string _connectionString;

        public BulkDataImporter(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void InsertMany<T>(IEnumerable<T> entities, string destinationTable)
        {
            var col = new DataReaderCollection<T>(entities);
            using (var bulk = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.TableLock))
            {
                bulk.DestinationTableName = destinationTable;
                bulk.EnableStreaming = true;
                bulk.WriteToServer(col);
            }
        }
    }
}
