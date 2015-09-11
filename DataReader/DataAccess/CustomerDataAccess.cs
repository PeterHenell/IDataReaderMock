using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest.DataAccess
{
    public class CustomerDataAccess 
    {
        DataAccessBase _dao = new DataAccessBase();


        public void InsertMany(IEnumerable<Customer> customers)
        {
            var col = new DataReaderCollection<Customer>(customers);
            using (var bulk = new SqlBulkCopy(GetconnectionString(), SqlBulkCopyOptions.TableLock))
            {
                bulk.DestinationTableName = "Customer";

                bulk.WriteToServer(col);
            }
        }

        private string GetconnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = ".";
            builder.InitialCatalog = "DWH_BASE";
            builder.IntegratedSecurity = true;
            return builder.ToString();
        }

        //protected override Customer BuildEntity(System.Data.IDataReader dataReader)
        //{
        //    return new Customer
        //    {
        //        Name = dataReader.GetString(dataReader.GetOrdinal("Name")),
        //        Age = dataReader.GetInt32(dataReader.GetOrdinal("Age")),
        //        YearlyBonus = dataReader.GetInt64(dataReader.GetOrdinal("YearlyBonus"))
        //    };
        //}
    }
}
