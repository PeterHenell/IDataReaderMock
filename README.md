# IDataReaderMock
Library for creating a IDataReader from an IEnumerable<T>.

## Limitations
Only works for simple classes of T which provide all their data as Properties.
(Will not try to get any other kind of data from T).

## Details
Implemented to stream the IEnumerable while consuming the DataReaderCollection.

## How to use
Simply create a new instance of DataReaderCollection<T>, supply any IEnumerable<T> as parameter.

    var col = new DataReaderCollection<T>(entities);

The source parameter will not be consumed/iterated until the IDataReader interfance of DataReaderCollection is being used.


## Example
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
