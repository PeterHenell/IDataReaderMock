using DataReaderTest.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataReaderTest
{
    public class DataReaderCollection<T> : IDataReader
    {
        GenericListToDataTableConverter<T> _converter = new GenericListToDataTableConverter<T>();

        DataTable _schema;
        IEnumerable<T> _items;
        T _currentItem;
        bool _closed = false;
        Dictionary<string, int> _ordinals;
        private IEnumerator<T> _enumerator;

        public DataReaderCollection(IEnumerable<T> items)
        {
            _items = items;
            _schema = _converter.CreateSchemaOnlyTable<T>();
            _ordinals = GetOrdinalsFrom(_schema);
            _enumerator = _items.GetEnumerator();
            _currentItem = _enumerator.Current;
        }

        public DataReaderCollection(DataTable items)
        {
            _schema = items.Copy();
            _ordinals = GetOrdinalsFrom(_schema);
        }

        private static Dictionary<string, int> GetOrdinalsFrom(DataTable table)
        {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn col = table.Columns[i];
                dict.Add(col.ColumnName, i);
            }
            return dict;
        }

        public void Close()
        {
            _closed = true;
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public int FieldCount
        {
            get { return _ordinals.Count; }
        }

        public bool GetBoolean(int ordinal)
        {
            return GetFieldValue<bool>(ordinal);
        }

        public byte GetByte(int ordinal)
        {
            return GetFieldValue<byte>(ordinal);
        }

        public long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int ordinal)
        {
            return GetFieldValue<char>(ordinal);
        }

        public long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int ordinal)
        {
            return _schema.Columns[ordinal].DataType.Name;
        }

        public DateTime GetDateTime(int ordinal)
        {
            return GetFieldValue<DateTime>(ordinal);
        }

        public decimal GetDecimal(int ordinal)
        {
            return GetFieldValue<Decimal>(ordinal);
        }

        public double GetDouble(int ordinal)
        {
            return GetFieldValue<double>(ordinal);
        }

        //public System.Collections.IEnumerator GetEnumerator()
        //{
        //    return _enumerator;
        //}

        public Type GetFieldType(int ordinal)
        {
            return _schema.Columns[ordinal].DataType;
        }

        public float GetFloat(int ordinal)
        {
            return GetFieldValue<float>(ordinal);
        }

        public Guid GetGuid(int ordinal)
        {
            return GetFieldValue<Guid>(ordinal);
        }

        public short GetInt16(int ordinal)
        {
            return GetFieldValue<short>(ordinal);
        }

        public int GetInt32(int ordinal)
        {
            return GetFieldValue<int>(ordinal);
        }

        public long GetInt64(int ordinal)
        {
            return GetFieldValue<long>(ordinal);
        }

        public string GetName(int ordinal)
        {
            return _schema.Columns[ordinal].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            if (!_ordinals.ContainsKey(name))
                throw new ArgumentOutOfRangeException("name");

            return _ordinals[name];
        }

        public DataTable GetSchemaTable()
        {
            return _schema.Clone();
        }

        public string GetString(int ordinal)
        {
            return GetFieldValue<string>(ordinal);
        }

        public object GetValue(int ordinal)
        {
            return GetFieldValue<object>(ordinal);
        }

        public int GetValues(object[] values)
        {
            values = _converter.GetRow(_currentItem, _schema).ItemArray;
            return values.Length;
        }

        public bool HasRows
        {
            get { return _items.Any(); }
        }

        public bool IsClosed
        {
            get { return _closed; }
        }

        public bool IsDBNull(int ordinal)
        {
            return _currentItem == null;
        }

        public bool NextResult()
        {
            return MoveNext();
        }

        private bool MoveNext()
        {
            var didMove = _enumerator.MoveNext();
            _currentItem = _enumerator.Current;
            return didMove;
        }

        public bool Read()
        {
            return MoveNext();
        }

        public int RecordsAffected
        {
            get { return 0; }
        }

        public object this[string name]
        {
            get
            {
                var ordinal = GetOrdinal(name);
                return GetFieldValue<object>(ordinal);
            }
        }

        public object this[int ordinal]
        {
            get { return GetFieldValue<object>(ordinal); }
        }

        public void Dispose()
        {

        }

        public IDataReader GetData(int i)
        {
            return this;
        }

        private TColType GetFieldValue<TColType>(int ordinal)
        {
            if (_currentItem == null)
            {
                throw new InvalidOperationException("Cannot get value before calling Read()");
            }
            if (ordinal > _ordinals.Count - 1)
            {
                throw new ArgumentOutOfRangeException("ordinal");
            }

            var current = _converter.GetRow(_currentItem, _schema);
            return (TColType)current[ordinal];
        }
    }
}
