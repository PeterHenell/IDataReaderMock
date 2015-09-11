using FizzWare.NBuilder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest
{
    [TestFixture]
    public class DataReaderCollectionTests
    {
        [Test]
        public void ShouldGetItemsByNameAndOrdinalFromCollection()
        {
            var customers = new List<Customer> { 
                new Customer {
                    Name = "Peter",
                    Age = 29
                }
            };

            var col = new DataReaderCollection<Customer>(customers);

            Assert.That(col.HasRows, Is.True);

            col.NextResult();
            Assert.That(col.GetString(col.GetOrdinal("Name")), Is.EqualTo("Peter"));
            Assert.That(col.GetString(0), Is.EqualTo("Peter"));

            Assert.That(col.GetInt32(col.GetOrdinal("Age")), Is.EqualTo(29));
            Assert.That(col.GetInt32(1), Is.EqualTo(29));

        }

        [Test]
        public void ShouldInstantiateCollection()
        {
            var customers = Builder<Customer>.CreateListOfSize(10).Build();

            var col = new DataReaderCollection<Customer>(customers);
            Assert.That(col.HasRows, Is.True);
            int i = 0;
            while (col.NextResult())
            {
                i++;
                Assert.That(col.GetString(col.GetOrdinal("Name")), Is.EqualTo("Name" + i.ToString()));
                Assert.That(col.GetString(0), Is.EqualTo("Name" + i.ToString()));

                Assert.That(col.GetInt32(col.GetOrdinal("Age")), Is.EqualTo(i));
                Assert.That(col.GetInt32(1), Is.EqualTo(i));
            }
            Assert.That(i, Is.EqualTo(10));
        }

        [Test]
        public void ShouldGetOrdinal()
        {
            var customers = new List<Customer> { 
                new Customer {
                    Name = "Peter",
                    Age = 29
                }
            };
            var col = new DataReaderCollection<Customer>(customers);

            Assert.That(col.GetOrdinal("Name"), Is.EqualTo(0));
            Assert.That(col.GetOrdinal("Age"), Is.EqualTo(1));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowExceptionWhenTryingToGetOrdinalOfColumnThatDoesNotExist()
        {
            var customers = new List<Customer> { 
                new Customer {
                    Name = "Peter",
                    Age = 29
                }
            };
            var col = new DataReaderCollection<Customer>(customers);

            var notFound = col.GetOrdinal("Salary");

            Assert.Fail("Should have throw exception when trying to get ordinal of non-column");
        }

        [Test]
        public void ShouldGetDataTypeOfOrdinal()
        {
            var customers = new List<Customer> { 
                new Customer {
                    Name = "Peter",
                    Age = 29,
                    YearlyBonus = 100000000L
                }
            };
            var col = new DataReaderCollection<Customer>(customers);

            Assert.That(col.GetFieldType(0), Is.EqualTo(typeof(string)));
            Assert.That(col.GetFieldType(1), Is.EqualTo(typeof(int)));
            Assert.That(col.GetFieldType(2), Is.EqualTo(typeof(long)));
        }

        [Test]
        [ExpectedException(ExpectedException= typeof(ArgumentException))]
        public void ShouldNotInstatiateForTypeWithNoProperties()
        {
            var empty = new List<EmptyClass> { 
                new EmptyClass ()
            };
            var col = new DataReaderCollection<EmptyClass>(empty);

            Assert.Fail("Should throw exception when trying to create collection for type with no properties");
        }

        [Test]
        public void ShouldGetValuesOfEachType()
        {
            var objs = Builder<ComplexObject>.CreateListOfSize(1).Build();
            var col = new DataReaderCollection<ComplexObject>(objs);
            col.Read();
            Assert.That(col.GetInt32(col.GetOrdinal("AInt")), Is.EqualTo(1));
            Assert.That(col.GetInt16(col.GetOrdinal("BInt16")), Is.EqualTo(1));
            Assert.That(col.GetInt64(col.GetOrdinal("CInt64")), Is.EqualTo(1));
            Assert.That(col.GetDecimal(col.GetOrdinal("DDecimal")), Is.EqualTo(1.0m));
            Assert.That(col.GetFloat(col.GetOrdinal("EFloat")), Is.EqualTo(1.0f));
            Assert.That(col.GetDouble(col.GetOrdinal("FDouble")), Is.EqualTo(1.0d));
            Assert.That(col.GetBoolean(col.GetOrdinal("GBoolean")), Is.EqualTo(false));
            Assert.That(col.GetByte(col.GetOrdinal("HByte")), Is.EqualTo(1));
            Assert.That(col.GetChar(col.GetOrdinal("IChar")), Is.EqualTo('A'));
            Assert.That(col.GetString(col.GetOrdinal("JString")), Is.EqualTo("JString1"));
            Assert.That(col.GetDateTime(col.GetOrdinal("KDateTime")), Is.GreaterThan(DateTime.Now.AddDays(-1)));
            Assert.That(col.GetGuid(col.GetOrdinal("LGuid")), Is.EqualTo(Guid.Parse("00000000-0000-0000-0000-000000000001")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException))]
        public void ShouldFailTryingToGetDataBeforeCallingRead()
        {
            var objs = Builder<ComplexObject>.CreateListOfSize(1).Build();
            var col = new DataReaderCollection<ComplexObject>(objs);
            
            col.GetInt32(col.GetOrdinal("AInt"));

            Assert.Fail("Should have thrown exception when trying to get data from datareader that have not been called .Read() first.");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void ShouldFailWhenTryingToReadOrdinalThatDoNotExist()
        {
            var objs = Builder<ComplexObject>.CreateListOfSize(1).Build();
            var col = new DataReaderCollection<ComplexObject>(objs);
            col.Read();
            Assert.That(col.GetGuid(11), Is.Not.Null);

            // Col 12 does not exist since we are zero based.
            col.GetInt32(12);

            Assert.Fail("Should have thrown exception when trying to get value from ordinal that is out of range");
        }

        [Test]
        public void ShouldIterateOverCollection()
        {
            var customers = Builder<Customer>.CreateListOfSize(10).Build();

            var col = new DataReaderCollection<Customer>(customers);
            int counter = 0;
            foreach (var item in col)
            {
                Console.WriteLine(item.GetType());
                counter++;
            }
            Assert.That(counter, Is.EqualTo(10));
        }

        public class EmptyClass { }

        public class ComplexObject
        {
            public int AInt { get; set; }
            public short BInt16 { get; set; }
            public long CInt64 { get; set; }
            public Decimal DDecimal { get; set; }
            public float EFloat { get; set; }
            public double FDouble { get; set; }
            public bool GBoolean { get; set; }
            public byte HByte { get; set; }
            public char IChar { get; set; }
            public string JString { get; set; }
            public DateTime KDateTime { get; set; }
            public Guid LGuid { get; set; }
        }
    }
}
