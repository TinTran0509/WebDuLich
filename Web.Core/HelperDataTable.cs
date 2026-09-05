using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Web.Core
{
    public class HelperDataTable
    {
        /// <summary>
        /// Chuyển đổi IList sang DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">List Object</param>
        /// <returns>Trả lại một DataTable</returns>
        public static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            var table = CreateTable<T>();
            var entityType = typeof(T);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (var item in list)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = rows.Select(CreateItem<T>).ToList();
            }

            return list;
        }
        /// <summary>
        /// Chuyển đổi một DataTable sang List object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Tham số là một DataTable</param>
        /// <returns>Trả lại một List object</returns>
        public static IList<T> ConvertToList<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            var rows = table.Rows.Cast<DataRow>().ToList();

            return ConvertTo<T>(rows);
        }
        private static T CreateItem<T>(DataRow row)
        {
            var obj = default(T);
            if (row == null) return obj;
            obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in row.Table.Columns)
            {
                var prop = obj.GetType().GetProperty(column.ColumnName);
                var value = row[column.ColumnName];
                prop.SetValue(obj, value, null);
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            var entityType = typeof(T);
            var table = new DataTable(entityType.Name);
            var properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }
    }
}