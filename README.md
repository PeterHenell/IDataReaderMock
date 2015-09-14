# IDataReaderMock
Library for creating a IDataReader from an IEnumerable<T>.

Intended to be used to mock the IDataReader from SqlCommand.ExecuteReader() call.
Can also be used as a source for the SqlClient.SqlBulkCopy.WriteToServer method.

## Limitations
Only works for simple classes of T which provide all their data as Properties.
(Will not try to get any other kind of data from T).

## Details
Implemented to stream the IEnumerable while consuming the DataReaderCollection.
The source parameter will not be consumed/iterated until the IDataReader interfance of DataReaderCollection is being used.

## How to use
Simply create a new instance of DataReaderCollection<T>, supply any IEnumerable<T> as parameter.

        var col = new DataReaderCollection<T>(entities);

Now you can use this collection as a mock of an actual SqlCommand.ExecuteReader() call:
        public void Fill(IDataReader reader, List<Customer> customers)
        {
            while (reader.Read())
            {
                var entity = new Customer
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Age = reader.GetInt32(reader.GetOrdinal("Age")),
                    YearlyBonus = reader.GetInt64(reader.GetOrdinal("YBonus"))
                };
                customers.Add(entity);
            }
        }


## Example for SqlClient.SqlBulkCopy
Bulk inserting a list of Customers:

    public class IntegrationTests
    {
        BulkDataImporter _dao = new BulkDataImporter("Server=.;Initial Catalog=TestDB;Integrated Security=true;");

        [Test]
        public void ShouldInsert1000Rows()
        {
            // Using NBuilder to generate a list of customers
            // https://github.com/garethdown44/nbuilder/
            // Using Faker.Net for generating nicer values for each customer
            // https://github.com/jonwingfield/Faker.Net
            var customers = Builder<Customer>
                .CreateListOfSize(100000)
                .All()
                    .With(c => c.Name = Faker.Name.GetName())
                .Build();

            _dao.InsertMany<Customer>(customers, "Customer");
        }
    }
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
