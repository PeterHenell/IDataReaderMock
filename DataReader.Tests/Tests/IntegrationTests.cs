using DataReaderTest.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using System.Data;

namespace DataReaderTest.Tests
{
    public class IntegrationTests
    {
        BulkDataImporter _dao = new BulkDataImporter("Server=.;Initial Catalog=TestDB;Integrated Security=true;");

        [Test]
        [Ignore]
        public void ShouldInsert1000Rows()
        {
            // Using NBuilder to generate a list of customers
            // https://github.com/garethdown44/nbuilder/
            // Using Faker.Net for generating nicer values
            // https://github.com/jonwingfield/Faker.Net
            var customers = Builder<Customer>
                .CreateListOfSize(100000)
                .All()
                    .With(c => c.Name = Faker.Company.Name())
                    .With(c => c.Longname = Faker.Company.Name())
                .Build();

            //_dao.InsertMany<Customer>(customers, "Customer");
        }

        [Test]
        public void CreateMany()
        {
            var customers = Builder<Person>
                .CreateListOfSize(100000)
                .All()
                    .With(c => c.Name = Faker.Name.FullName())
                .Build();
            var count = customers.Count();
        }

        class Person
        {
            public string Name { get; set; }
        }
    }
}
