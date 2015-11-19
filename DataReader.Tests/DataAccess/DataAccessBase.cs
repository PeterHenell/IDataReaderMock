using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest
{
    public class DataAccessBase
    {
        public DataAccessBase()
        {

        }

        public TEntity ExecuteOne<TEntity>(string query, Func<IDataReader, TEntity> entityBuilder)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                using (var reader = ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        var entity = entityBuilder(reader);
                        return entity;
                    }
                    else
                    {
                        return default(TEntity);
                    }
                }
            }
        }

        public virtual IDataReader ExecuteReader(SqlCommand cmd)
        {
            cmd.Connection.Open();
            return cmd.ExecuteReader();
        }

        private SqlConnection GetConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            return new SqlConnection(builder.ToString());
        }
    }
}
