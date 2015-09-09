using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest
{
    class Program
    {

//        CREATE TYPE	ListOfInts AS TABLE ( num BIGINT NOT NULL);
//GO

//CREATE TABLE BatchStage (i BIGINT NOT NULL);

//ALTER PROCEDURE BatchInsert @IDS ListOfInts READONLY
//AS 
//BEGIN
//    INSERT dbo.BatchStage ( i )
//    SELECT num FROM @IDS
//END	

        static void Main(string[] args)
        {
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = "localhost";
            //builder.InitialCatalog = "DWH_PRES";
            //builder.IntegratedSecurity = true;
            //var connectionString = builder.ConnectionString;

            //using (var con = new SqlConnection(connectionString))
            //using (var cmd = new SqlCommand(" [REPORT].[127_NyaMotparter]", con) 
            //    { CommandType = CommandType.StoredProcedure })
            //{
            //    con.Open();
            //    cmd.Parameters.Add()
            //}

        }

        static DataTable CallReportProcedure(string connectionString, DateTime startDate, DateTime endDate, string departmentID, string columnFilter)
        {
            var drInts = new DataReaderCollection<long>(new List<long> { 1L, 2L });

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("BatchInsert", conn) { CommandType = System.Data.CommandType.StoredProcedure })
            {
                conn.InfoMessage += conn_InfoMessage;
                var param = new SqlParameter
                {
                    SqlDbType = System.Data.SqlDbType.Structured,
                    TypeName = "ListOfInts",
                    Value = drInts,
                    ParameterName = "@IDS"
                };
                cmd.Parameters.Add(param);

                var da = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                da.Fill(ds);
                return ds.Tables[0];
            }
        }

        static void conn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
