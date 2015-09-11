using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderTest.Converters
{
    public class GenericListToDataTableConverter<T>
    {
        private PropertyInfo[] _properties;

        public GenericListToDataTableConverter()
        {
            Type type = typeof(T);
            this._properties = type.GetProperties();

            if (_properties.Length == 0)
            {
                throw new ArgumentException("type does not have any properties");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/18746064/using-reflection-to-create-a-datatable-from-a-class</remarks>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in _properties)
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
                object[] values = GetRowFrom<T>(entity);
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private object[] GetRowFrom<T>(T entity)
        {
            object[] values = new object[_properties.Length];
            for (int i = 0; i < _properties.Length; i++)
            {
                values[i] = _properties[i].GetValue(entity);
            }
            return values;
        }

        private void Fill<T>(DataRow row, T entity)
        {
            row.ItemArray = GetRowFrom(entity);
        }

        public DataTable CreateSchemaOnlyTable<T>()
        {
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in _properties)
            {
                dataTable.Columns.Add(
                    new DataColumn(
                        info.Name,
                        Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType
                        )
                    );
            }
            return dataTable;
        }

        public IEnumerable<DataRow> StreamRows<T>(IEnumerable<T> list, DataTable dataTable)
        {
            foreach (T entity in list)
            {
                var row = GetRow(entity, dataTable);
                yield return row;
            }
        }

        public DataRow GetRow<T>(T entity, DataTable schemaTable)
        {
            var row = schemaTable.NewRow();
            Fill(row, entity);
            return row;
        }
    }
}
