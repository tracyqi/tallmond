using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;
using System.IO;
using System.Data;
using System.Reflection;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Inventory
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "I-hn":
                    ImportHainan();
                    break;
                case "I-ah":
                    ImportAH();
                    break;
                case "O":
                    GenerateShopify();
                    break;
                default:
                    throw new Exception("Wrong parameter");
            }
        }

        private static void ImportAH()
        {
            //string path = Path.Combine(System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"2015-06-23.xlsx");

            string path = Path.Combine(@"C:\Users\tracyqi.REDMOND\Google Drive\DOCUMENT\Inventory\InstockFromSuppliers\Airbus Helicopters\Airbus-Helicopters-2015.xlsx");

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

                    string pn = row.ItemArray[0].ToString().Trim();

                    if (string.IsNullOrEmpty(pn))
                        continue;

                    string vendorshort = "ah";
                    string fob = "FOB Washington, USA ";
                    string vendor = "Airbus Helicopters";
                    string collection = "Airbus Helicopters";

                    string condition = "NE";
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "condition", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        condition = string.IsNullOrEmpty(row.ItemArray[7].ToString().Trim()) ? "NE" : row.ItemArray[7].ToString().Trim();
                    }

                    //AR – As Removed
                    //NE – New Equipment
                    //NS – New Surplus
                    //OH – Overhauled
                    //SV – Serviceable

                    string certification = "FAA 8130 form 3";
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "certification", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        certification = string.IsNullOrEmpty(row.ItemArray[8].ToString().Trim()) ? "FAA 8130 form 3" : row.ItemArray[8].ToString().Trim();
                    }

                    int leadtime = 2;
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "Lead Time", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        leadtime = Convert.ToInt32(string.IsNullOrEmpty(row.ItemArray[9].ToString().Trim()) ? "2" : row.ItemArray[9].ToString().Trim());
                    }

                    string description = Regex.Replace(row.ItemArray[2].ToString(), @"[^\u0000-\u007F]", string.Empty);
                        
                        //ContainsUnicodeCharacter() ? row.ItemArray[2].ToString() : "N/A";
                    double price = Convert.ToDouble(row.ItemArray[6]);
                    int qty = Convert.ToInt32(row.ItemArray[3]);


                    //=IF(E2>=200,1,ROUNDUP(200/E2,0))
                    int MOQ = 1;
                    if (price == 0)
                    {
                        MOQ = 1;
                    }
                    else if (price >= 200)
                    {
                        MOQ = 1;
                    }
                    else
                    {
                        MOQ = Convert.ToInt32(200 / price) < qty ? Convert.ToInt32(200 / price) : qty;
                    }


                    StringBuilder sb = new StringBuilder();
                    string note = "<li><b>Condition:</b> " + condition + "</li>" +
                                        "<li>Comes with " + certification + " </li>" +
                                        "<li>All parts are subject to prior sale </li>" +
                                        "<li>Sale price is effective for available inventory only </li>" +
                                        "<li><b>" + fob + "</b> </li>";

                    sb.AppendFormat("{0}{1}{2}", "<b>Description: </b>", description, "<br>");
                    sb.AppendFormat("{0}{1}{2}", "<b>Part Number: </b>", pn, "<br>");
                    sb.AppendFormat("{0}{1}", "<b> Note:</b>", "<br>");
                    sb.AppendFormat("{0}{1}", "", note);

                    inventory.Handle = pn + "-" + vendorshort;
                    inventory.Title = pn;
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
                    inventory.Original_Price = price;
                    inventory.Variant_SKU = pn;
                    inventory.Variant_Price = price;
                    inventory.Variant_Inventory_Qty = qty;
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
        private static void ImportHainan()
        {
            //string path = Path.Combine(System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"2015-06-23.xlsx");

            string path = Path.Combine(@"C:\Users\tracyqi.REDMOND\Google Drive\DOCUMENT\Inventory\InstockFromSuppliers\Hainan Airline", @"2015-06-23.xlsx");

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

                    string pn = row.ItemArray[0].ToString().Trim();

                    if (string.IsNullOrEmpty(pn))
                        continue;

                    string vendorshort = "hn";
                    string fob = "FOB China";
                    string vendor = "hainan";
                    string collection = "Hainan";
                    string condition = "NE";
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "condition", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        condition = string.IsNullOrEmpty(row.ItemArray[11].ToString().Trim()) ? "NE" : row.ItemArray[11].ToString().Trim();
                    }

                    //AR – As Removed
                    //NE – New Equipment
                    //NS – New Surplus
                    //OH – Overhauled
                    //SV – Serviceable

                    string certification = "FAA 8130 form 3";
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "certification", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        certification = string.IsNullOrEmpty(row.ItemArray[12].ToString().Trim()) ? "FAA 8130 form 3" : row.ItemArray[12].ToString().Trim();
                    }

                    int leadtime = 3;
                    if (row.ItemArray.Any(o => string.Equals(o.ToString(), "Lead Time", StringComparison.InvariantCultureIgnoreCase) == true))
                    {
                        leadtime = Convert.ToInt32(string.IsNullOrEmpty(row.ItemArray[13].ToString().Trim()) ? "2" : row.ItemArray[13].ToString().Trim());
                    }

                    string description = Regex.Replace(row.ItemArray[2].ToString(), @"[^\u0000-\u007F]", string.Empty);
                    double price = Convert.ToDouble(row.ItemArray[6]);
                    int qty = Convert.ToInt32(row.ItemArray[1]);


                    //=IF(E2>=200,1,ROUNDUP(200/E2,0))
                    int MOQ = 1;
                    if (price == 0)
                    {
                        MOQ = 1;
                    }
                    else if (price >= 200)
                    {
                        MOQ = 1;
                    }
                    else
                    {
                        MOQ = Convert.ToInt32(200 / price) < qty ? Convert.ToInt32(200 / price) : qty;
                    }


                    StringBuilder sb = new StringBuilder();
                    string note = "<li><b>Condition:</b> " + condition + "</li>" +
                                        "<li>Comes with " + certification + " </li>" +
                                        "<li>All parts are subject to prior sale </li>" +
                                        "<li>Sale price is effective for available inventory only </li>" +
                                        "<li><b>" + fob + "</b> </li>";

                    sb.AppendFormat("{0}{1}{2}", "<b>Description: </b>", description, "<br>");
                    sb.AppendFormat("{0}{1}{2}", "<b>Part Number: </b>", pn, "<br>");
                    sb.AppendFormat("{0}{1}", "<b> Note:</b>", "<br>");
                    sb.AppendFormat("{0}{1}", "", note);

                    inventory.Handle = pn + "-" + vendorshort;
                    inventory.Title = pn;
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
                    inventory.Original_Price = price;
                    inventory.Variant_SKU = pn;
                    inventory.Variant_Price = price;
                    inventory.Variant_Inventory_Qty = qty;
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

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        private static void GenerateShopify()
        {
            #region retrieve records from db
            TallamondEntities entityContext = new TallamondEntities();
            int maxLineCount = 10000;
            string outputfolder = @"F:\Git\TallamondProduct\Inventory\Inventory\bin";

            var vendors = entityContext.Inventories.GroupBy(x =>x.Vendor);

            //    .Select(grp => new { FileIndex = grp.Key, Lines = grp.Select(x => x.Line) });
            #endregion

            int i = 0;
            foreach (var ven in vendors)
            {
                var fileGroups = ven.GroupBy(x => x.ID / maxLineCount);
                foreach (var grp in fileGroups)
                {
                    string outfile = Path.Combine(Path.Combine(outputfolder, grp.First().Vendor + "_"+ "Shopify_" + i++ + ".csv"));


                    #region transform files
                    //string outfile_error = Path.Combine(Path.Combine(outputfolder, "Shopify_error_" + fn));
                    using (TextWriter writer = File.CreateText(outfile))
                    {
                        using (var csvwriter = new CsvWriter(writer))
                        {
                            csvwriter.Configuration.RegisterClassMap<OutputShopifyMap>();


                            csvwriter.WriteHeader<Inventory>();

                            foreach (var record in grp)
                            {
                                //var record = csv.GetRecord<Input>();
                                //Input record = csv.GetRecord<Input>();


                                csvwriter.WriteRecord<Inventory>(record);
                            }
                        }
                    }
                    #endregion

                    //#region split the files if too big
                    //string[] filePaths = Directory.GetFiles(outputfolder);

                    //foreach (string file in filePaths)
                    //{
                    //    string f = Path.Combine(outputfolder, Path.GetFileNameWithoutExtension(file));
                    //    Split(file, outputfolder);
                    //}
                    //#endregion
                }
            }
        }


        public static void Split(string inputfile, string outputfolder)
        {
            int maxLineCount = 5000;
            FileInfo file = new FileInfo(inputfile);

            var lines = File.ReadLines(inputfile).Take(1).ToArray();

            var fileGroups = File.ReadLines(file.FullName)
                .Select((l, i) => new { Line = l, Index = i })
                .GroupBy(x => x.Index / maxLineCount)
                .Select(grp => new { FileIndex = grp.Key, Lines = grp.Select(x => x.Line) });

            foreach (var grp in fileGroups)
            {
                if (!Directory.Exists(outputfolder + "_new"))
                    Directory.CreateDirectory(outputfolder + "_new");

                string f = Path.Combine(outputfolder + "_new", Path.GetFileNameWithoutExtension(inputfile) + "_" + grp.FileIndex + ".csv");

                // Write headers for splitted file (Skip the first file)
                if (grp.FileIndex > 0)
                    File.WriteAllLines(f, lines);

                File.AppendAllLines(f, grp.Lines);
            }
        }
    }

    public sealed class OutputShopifyMap : CsvClassMap<Inventory>
    {
        public OutputShopifyMap()
        {
            Map(m => m.Handle).Name("Handle");
            Map(m => m.Title).Name("Title");
            Map(m => m.Body__HTML_).Name("Body (HTML)");
            //Map(m => m.Description).Name("Description");
            Map(m => m.Original_Price).Name("Original Price");
            Map(m => m.Original_Price).TypeConverterOption(NumberStyles.Currency);
            Map(m => m.Vendor).Name("Vendor");
            Map(m => m.VendorShort).Name("VendorShort");
            Map(m => m.Lead_Time).Name("Lead Time");
            Map(m => m.MOQ).Name("MOQ");
            Map(m => m.Condition).Name("Condition");
            Map(m => m.Type).Name("Type");
            Map(m => m.Collection).Name("Collection");
            Map(m => m.Variant_Price).Name("Variant Price");
            Map(m => m.Variant_SKU).Name("Variant SKU");
            Map(m => m.Variant_Taxable).Name("Variant Taxable");
            Map(m => m.Option1_Name).Name("Option1 Name");
            Map(m => m.Option1_Value).Name("Option1 Value");
            Map(m => m.Option2_Name).Name("Option2 Name");
            Map(m => m.Option2_Value).Name("Option2 Value");
            Map(m => m.Option3_Name).Name("Option3 Name");
            Map(m => m.Option3_Value).Name("Option3 Value");
            Map(m => m.Tags).Name("Tags");
            Map(m => m.SEO_Title).Name("SEO Title");
            Map(m => m.SEO_Description).Name("SEO Description");
            Map(m => m.Image_Src).Name("Image Src");
            //Map(m => m.Note).Name("Note");
            Map(m => m.Variant_Inventory_Qty).Name("Variant Inventory Qty");
            //Map(m => m.).Name("Total Value");
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
