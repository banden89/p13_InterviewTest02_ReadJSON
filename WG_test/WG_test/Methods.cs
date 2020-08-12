using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Nancy.Json;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace WG_test.Methods
{
    public static class FlatJsonConvert
    {
        static void ExploreAddProps(Dictionary<string, object> props, string name, Type type, object value)
        {
            if (value == null || type.IsPrimitive || type == typeof(DateTime) || type == typeof(string))
                props.Add(name, value);
            else if (type.IsArray)
            {
                var a = (Array)value;
                for (var i = 0; i < a.Length; i++)
                    ExploreAddProps(props, $"{name}[{i}]", type.GetElementType(), a.GetValue(i));
            }
            else
            {
                type.GetProperties().ToList()
                    .ForEach(p =>
                    {
                        var prefix = string.IsNullOrEmpty(name) ? string.Empty : name + ".";
                        ExploreAddProps(props, $"{prefix}{p.Name}", p.PropertyType, p.GetValue(value));
                    });
            }
        }

        public static Dictionary<string, object> Serialize<T>(T data)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            ExploreAddProps(props, string.Empty, typeof(T), data);
            return props;
        }
    }

    public static class DataTableToJson
    {
        public static string DataTableToJsonWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;

            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        public static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }
    }
}
