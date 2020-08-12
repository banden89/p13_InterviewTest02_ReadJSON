using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WG_test.Property;
using WG_test.Methods;

namespace WG_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //輸入JSON檔案路徑
            string file1_path = args[0];
            string file2_path = args[1];
            DataTable item_table = new DataTable();
            DataTable score_table = new DataTable();
            DataColumn column;
            DataRow row;

            using (StreamReader read = new StreamReader(file1_path))
            {
                string json = read.ReadToEnd();

                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                Item[] model = items.Cast<Item>().ToArray();
                //展開JSON並放進Dictionary
                Dictionary<string, object> flattenJson = FlatJsonConvert.Serialize<Item[]>(model);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "id";
                item_table.Columns.Add(column);
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "title";
                item_table.Columns.Add(column);
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Decimal");
                column.ColumnName = "score";
                item_table.Columns.Add(column);

                for (int i = 0; i < flattenJson.Count; i++)
                {
                    var item = flattenJson.ElementAt(i);

                    if (item.Key.Contains("id"))
                    {
                        row = item_table.NewRow();
                        row["id"] = item.Value;
                        row["title"] = flattenJson.ElementAt(i + 1).Value;
                        item_table.Rows.Add(row);
                    }
                }
            }

            using (StreamReader read = new StreamReader(file2_path))
            {
                string json = read.ReadToEnd();

                List<Item_score> items = JsonConvert.DeserializeObject<List<Item_score>>(json);
                Item_score[] model = items.Cast<Item_score>().ToArray();

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "id";
                score_table.Columns.Add(column);
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Int32");
                column.ColumnName = "score";
                score_table.Columns.Add(column);

                for (int index = 0; index < model.Length; index++)
                {
                    row = score_table.NewRow();
                    row["id"] = model[index].id;
                    row["score"] = model[index].score;
                    score_table.Rows.Add(row);
                }
            }

            //將輸入的成績放進item_table(可以想成做合併DataTable)
            foreach (DataRow rows in item_table.Rows)
            {
                foreach (DataRow rows1 in score_table.Rows)
                {
                    if (rows[0].ToString() == rows1[0].ToString())
                        rows["score"] = rows1["score"];
                }
            }

            //算出父項平均分數(read from bottom)
            for (int i = item_table.Rows.Count - 1; i >= 0; i--)
            {
                DataRow lastRow = item_table.Rows[i];
                if (lastRow["score"].ToString() == "")
                {
                    double sum = 0;
                    double count = 0;                  

                    for(int j = i+1; j < item_table.Rows.Count; j++)
                    {
                        if (lastRow["id"].ToString().Length + 2 == item_table.Rows[j]["id"].ToString().Length &&
                            lastRow["id"].ToString() == item_table.Rows[j]["id"].ToString().Substring(0, lastRow["id"].ToString().Length))
                        {
                            sum += Convert.ToDouble(item_table.Rows[j]["score"]);
                            count += 1;
                        }
                    }
                    lastRow["score"] = Math.Round(sum / count, 1);
                }
            }

            //Datatable轉JSON
            string str_json = DataTableToJson.DataTableToJsonWithJavaScriptSerializer(item_table);          
            Console.WriteLine(DataTableToJson.JsonPrettify(str_json));           
            Console.Read();
        }
    }
}