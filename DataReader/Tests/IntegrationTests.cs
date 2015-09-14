using DataReaderTest.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest.Tests
{
    public class IntegrationTests
    {
        BulkDataImporter _dao = new BulkDataImporter("Server=.;Initial Catalog=TestDB;Integrated Security=true;");
        
        [Test]
        public void ShouldInsert1000Rows()
        {
            // Using NBuilder from Fizzware to generate a list of customers
            // https://github.com/garethdown44/nbuilder/
            var customers = FizzWare.NBuilder
                .Builder<Customer>
                .CreateListOfSize(100000)
                .Build();

            _dao.InsertMany<Customer>(customers, "Customer");
        }
    }
}
