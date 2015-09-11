using DataReaderTest.DataAccess;
using FizzWare.NBuilder;
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
        CustomerDataAccess _dao = new CustomerDataAccess();

        [Test]
        public void ShouldInsert1000Rows()
        {
            var customers = Builder<Customer>.CreateListOfSize(100000).Build();

            _dao.InsertMany(customers);
        }
    }
}
