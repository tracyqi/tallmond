using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;
using System.IO;
using System.Data;
using System.Reflection;

namespace Inventory
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine(System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"2015-06-23.xlsx");

            var excelData = new ExcelData(path);
            var sheets = excelData.getWorksheetNames();

            TallamondEntities entityContext = new TallamondEntities();

            foreach (var s in sheets)
            {
                var sheet = excelData.getData(s);
                foreach (var row in sheet)
                {
                    Console.WriteLine(Environment.NewLine);

                    Inventory inventory = new Inventory();

                    string pn = row.ItemArray[0].ToString();
                    string vendorshort = "hn";
                    string fob = "FOB China";
                    string vendor = "hainan";
                    string collection = "Hainan";
                    string condition = "NS";
                                //AR – As Removed
                                //NE – New Equipment
                                //NS – New Surplus
                                //OH – Overhauled
                                //SV – Serviceable

                    int leadtime = 3;

                    //=IF(E2>=200,1,ROUNDUP(200/E2,0))
                    int MOQ = 1;
                    if (Convert.ToDouble(row.ItemArray[4]) == 0)
                    {
                        MOQ = 0;
                    }
                    else if (Convert.ToDouble(row.ItemArray[4]) >= 200)
                    {
                        MOQ = 1;
                    }
                    else
                    {
                        MOQ = Convert.ToInt32((200 / Convert.ToDouble(row.ItemArray[4])) )> Convert.ToInt32(row.ItemArray[1]) ? Convert.ToInt32(200 / Convert.ToDouble(row.ItemArray[4])) : Convert.ToInt32(row.ItemArray[1]);
                    }

                    inventory.Handle = pn + "-" + vendorshort;
                    inventory.Title = pn;

                    StringBuilder sb = new StringBuilder();
                    string description = row.ItemArray[2].ToString();
                    string note ="<li><b>Condition:</b> FN </li>" +
                                        "<li>Comes with FAA 8130 form 3 </li>" +
                                        "<li>All parts are subject to prior sale </li>" +
                                        "<li>Sale price is effective for available inventory only </li>" +
                                        "<li><b>" + fob + "</b> </li>";

                    sb.AppendFormat("{0}{1}{2}", "<b>Description: </b>", description, "<br>");
                    sb.AppendFormat("{0}{1}{2}", "<b>Part Number: </b>", inventory.Title, "<br>");
                    sb.AppendFormat("{0}{1}", "<b> Note:</b>", "<br>");
                    sb.AppendFormat("{0}{1}", "", note);
                    inventory.Body__HTML_ = sb.ToString();

                    inventory.Type = "Aircraft Parts";

                    inventory.Tags = string.Join(",", description, vendor, inventory.Type, condition, pn);

                    inventory.Published = true;

                    inventory.Option1_Name = "Title";

                    inventory.Option1_Value = "Default Title";

                    inventory.Option2_Name = "Leadtime";

                    inventory.Option2_Value = leadtime.ToString();

                    inventory.Option3_Name = "MOQ";

                    inventory.Option3_Value = MOQ;

                    inventory.Original_Price = Convert.ToDouble(row.ItemArray[4]);

                    inventory.Variant_SKU =pn;

                    inventory.Variant_Price = Convert.ToDouble(row.ItemArray[4]);

                    inventory.Variant_Inventory_Qty = Convert.ToInt32(row.ItemArray[1]);

                    inventory.Variant_Taxable = false;

                    inventory.Vendor = vendor;

                    inventory.VendorShort = vendorshort;

                    inventory.Lead_Time = leadtime;

                    inventory.MOQ = MOQ;

                    inventory.Condition = condition;

                    inventory.Collection = collection;

                    inventory.SEO_Title = string.Join(",", description, "Part Number", pn);

                    inventory.SEO_Description = inventory.SEO_Title;

                    inventory.Image_Src = "https://cdn.shopify.com/s/files/1/0416/3905/files/comingsoon_small_bw.gif";



                    entityContext.Inventories.Add(inventory);
                    entityContext.SaveChanges();



                }
            }
        }

    }

    public class ExcelData
    {
        string _path;

        public ExcelData(string path)
        {
            _path = path;
        }

        public IExcelDataReader getExcelReader()
        {
            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
            // to get started. This is how we avoid dependencies on ACE or Interop:
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read);

            // We return the interface, so that
            IExcelDataReader reader = null;
            try
            {
                if (_path.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                if (_path.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<string> getWorksheetNames()
        {
            var reader = this.getExcelReader();
            var workbook = reader.AsDataSet();
            var sheets = from DataTable sheet in workbook.Tables select sheet.TableName;
            return sheets;
        }

        public IEnumerable<DataRow> getData(string sheet, bool firstRowIsColumnNames = true)
        {
            var reader = this.getExcelReader();
            reader.IsFirstRowAsColumnNames = firstRowIsColumnNames;
            var workSheet = reader.AsDataSet().Tables[sheet];
            var rows = from DataRow a in workSheet.Rows select a;
            return rows;
        }
    }
}
