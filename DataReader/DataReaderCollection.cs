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
        DataTable _items;
        int _current = -1;
        bool _closed = false;
        Dictionary<string, int> _ordinals;

        public DataReaderCollection(IEnumerable<T> items)
        {
            _items = CreateDataTable(items);
            _ordinals = PrepareOrdinals(_items);
        }

        public DataReaderCollection(DataTable items)
        {
            _items = items.Copy();
            _ordinals = PrepareOrdinals(_items);
        }

        private static Dictionary<string, int> PrepareOrdinals(DataTable table)
        {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn col = table.Columns[i];
                dict.Add(col.ColumnName, i);
            }
            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class</remarks>
        /// <param name="list"></param>
        /// <returns></returns>
        private static DataTable CreateDataTable(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            if (properties.Length == 0)
            {
                throw new ArgumentException("type does not have any properties");
            }

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(
                    new DataColumn(
                        info.Name,
                        Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType
                        )
                    );
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public void Close()
        {
            _closed = true;
        }

        public int Depth
        {
            get { return _items.Rows.Count; }
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
            return _items.Columns[ordinal].DataType.Name;
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

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _items.Rows.GetEnumerator();
        }

        public Type GetFieldType(int ordinal)
        {
            return _items.Columns[ordinal].DataType;
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
            return _items.Columns[ordinal].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            if (!_ordinals.ContainsKey(name))
                throw new ArgumentOutOfRangeException("name");

            return _ordinals[name];
        }

        public DataTable GetSchemaTable()
        {
            return _items.Clone();
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
            values = _items.Rows[_current].ItemArray;
            return _items.Rows[_current].ItemArray.Length;
        }

        public bool HasRows
        {
            get { return _items.Rows.Count > 0; }
        }

        public bool IsClosed
        {
            get { return _closed; }
        }

        public bool IsDBNull(int ordinal)
        {
            return _items.Rows[_current][ordinal] == null;
        }

        public bool NextResult()
        {
            return ++_current <= _items.Rows.Count - 1;
        }

        public bool Read()
        {
            return ++_current < _items.Rows.Count;
        }

        public int RecordsAffected
        {
            get { return _current; }
        }

        public object this[string name]
        {
            get
            {
                var ordinal = GetOrdinal(name);
                return _items.Rows[_current][ordinal];
            }
        }

        public object this[int ordinal]
        {
            get { return _items.Rows[_current][_current]; }
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
            if (_current == -1)
            {
                throw new InvalidOperationException("Cannot get value before calling Read()");
            }
            if (ordinal > _ordinals.Count - 1)
            {
                throw new ArgumentOutOfRangeException("ordinal");
            }
            
            return (TColType)_items.Rows[_current][ordinal];
        }
    }
}
