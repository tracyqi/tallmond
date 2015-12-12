﻿using System;
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
        private static int batchamount = 5000;

        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "I":
                    Import("I", @"D:\ReadyFolder");
                    break;
                case "O":
                    GenerateShopify();
                    break;
                case "OM":
                    GenerateShopify(360);
                    break;
                case "OMG":
                    GenerateGeneral(360);
                    break;
                case "OMN":
                    GenerateNew(360);
                    break;
                default:
                    throw new Exception("Wrong parameter");
            }
        }

        private static void Import(string operation,
            string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string f in files)
            {
                string filename = Path.GetFileNameWithoutExtension(f);
                string vendor = filename.Split('-')[0];
                string vendorshort = filename.Split('-')[1];
                string collection = vendor;

                var excelData = new ExcelData(f);
                var sheets = excelData.getWorksheetNames();

                TallamondEntities entityContext = new TallamondEntities();

                //PART NUMBER DESCRIPTION QTY U / M Price Condition   Cert Lead Time FOB

                foreach (var s in sheets)
                {
                    try
                    { 
                    Console.WriteLine("Importing from Sheet {0}:", s);
                    var sheet = excelData.getData(s);
                    //int i = 0;

                    // batch upload
                    var data = sheet
                         .Select((l, i) => new { Line = l, Index = i })
                         .GroupBy(x => x.Index / batchamount)
                         .Select(grp => new { FileIndex = grp.Key, Lines = grp.Select(x => x.Line) });


                        foreach (var d in data)
                        {
                            List<Inventory> inventories = new List<Inventory>();

                            foreach (var row in d.Lines)
                            {
                                #region insert record
                                Inventory inventory = new Inventory();

                                int index = row.Table.Columns.IndexOf("PartNumber");
                                string pn = row.ItemArray[index].ToString().Trim();
                                if (string.IsNullOrEmpty(pn))
                                    continue;

                                int qty = 1;
                                index = row.Table.Columns.IndexOf("Quantity");
                                if (index > 1)
                                {
                                    qty = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? 1 : Convert.ToInt32(row.ItemArray[index].ToString().Trim());
                                }

                                string description = string.Empty;
                                index = row.Table.Columns.IndexOf("DESCRIPTION");
                                if (index >= 0)
                                {
                                    string dec = row.ItemArray[index].ToString().Trim();
                                    description = Regex.Replace(dec, @"[^\u0000-\u007F]", string.Empty);
                                }

                                double originalPrice = 0;
                                index = row.Table.Columns.IndexOf("OriginalPrice");
                                if (index >= 0)
                                {
                                    originalPrice = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? 0 : Convert.ToDouble(row.ItemArray[index].ToString().Trim());
                                }

                                double finalPrice = 0;
                                index = row.Table.Columns.IndexOf("FinalPrice");
                                if (index >= 0)
                                {
                                    finalPrice = Convert.ToDouble(row.ItemArray[index].ToString().Trim());
                                }
                                else // Not find the price
                                {
                                    // =IF(NOT(ISNUMBER(E2)),"",ROUND(IF(E2<0.01,"",IF(E2<100,E2*1.3,IF(E2<1000,E2*1.2,IF(E2<3000,E2*1.15,IF(E2<10000,E2*1.1,IF(E2>=10000,E2*1.06,"")))))),2))

                                    //if (originalPrice < 0.01)
                                    //    finalPrice = 0;
                                    //else if (originalPrice < 100)
                                    //    finalPrice = originalPrice * 1.3;
                                    //else if (originalPrice < 1000)
                                    //    finalPrice = originalPrice * 1.2;
                                    //else if (originalPrice < 3000)
                                    //    finalPrice = originalPrice * 1.2;
                                    //else if (originalPrice < 10000)
                                    //    finalPrice = originalPrice * 1.15;
                                    //else
                                    //    finalPrice = originalPrice * 1.11;



                                    if (originalPrice < 0.01)
                                        finalPrice = 0;
                                    else if (originalPrice < 50)
                                        finalPrice = originalPrice * 2;
                                    else if (originalPrice < 100)
                                        finalPrice = originalPrice + 30;
                                    else if (originalPrice < 1000)
                                        finalPrice = originalPrice * 1.25;
                                    //else if (originalPrice < 3000)
                                    //    finalPrice = originalPrice * 1.2;
                                    else if (originalPrice < 10000)
                                        finalPrice = originalPrice * 1.15;
                                    else
                                        finalPrice = originalPrice * 1.11;
                                }
                                finalPrice = Math.Round(finalPrice, 2);

                                //AR – As Removed
                                //NE – New Equipment
                                //NS – New Surplus
                                //OH – Overhauled
                                //SV – Serviceable
                                string defaultCondition = "NE";
                                index = row.Table.Columns.IndexOf("Condition");
                                if (index >= 0)
                                {
                                    defaultCondition = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? defaultCondition : row.ItemArray[index].ToString().Trim();
                                }

                                string defaultCerts = "";
                                index = row.Table.Columns.IndexOf("Certificate");
                                if (index >= 0)
                                {
                                    defaultCerts = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? defaultCerts : row.ItemArray[index].ToString().Trim();
                                }
                                // Special rule, NS means no Cersts
                                //if (defaultCondition == "NS")
                                //{
                                //    defaultCerts = string.Empty;
                                //}

                                string defaultLeadtime = "5-7 ";
                                index = row.Table.Columns.IndexOf("LeadTime");
                                if (index >= 0)
                                {
                                    defaultLeadtime = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? defaultLeadtime : row.ItemArray[index].ToString().Trim();
                                }

                                // FOB 
                                string defaultFob = "";
                                index = row.Table.Columns.IndexOf("FOB");
                                if (index >= 0)
                                {
                                    defaultFob = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? defaultFob : row.ItemArray[index].ToString().Trim();
                                }

                                string defaultMOQ = "$300";
                                index = row.Table.Columns.IndexOf("MOQ");
                                if (index >= 0)
                                {
                                    defaultMOQ = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? defaultMOQ : row.ItemArray[index].ToString().Trim();
                                }

                                string notes = "";
                                index = row.Table.Columns.IndexOf("Notes");
                                if (index >= 0)
                                {
                                    notes = string.IsNullOrEmpty(row.ItemArray[index].ToString().Trim()) ? notes : row.ItemArray[index].ToString().Trim();
                                }


                                StringBuilder sb = new StringBuilder();
                                string note = "<li><b>Condition:</b> " + defaultCondition + "</li> " +
                                                    (string.IsNullOrEmpty(defaultCerts) ? "" : ("<li>Comes with " + defaultCerts + " </li> ")) +
                                                    "<li>All parts are subject to prior sale </li> " +
                                                    //((finalPrice >= 200) ? string.Empty : "<li>Minimum Order Amount:" + defaultMOQ + "</li>") +
                                                    "<li>Sale price is effective for available inventory only </li> " +
                                                    (string.IsNullOrEmpty(defaultFob) ? "" : ("<li><b> FOB " + defaultFob + " </b></li> ")) +
                                                    "<li>Lead time is " + defaultLeadtime + " based on available inventory </li> "
                                                    //+ notes
                                                    ;

                                sb.AppendFormat("{0}{1}{2}", "<b>Description: </b>", description, "<br>");
                                sb.AppendFormat("{0}{1}{2}", "<b>Part Number: </b>", pn, "<br>");
                                sb.AppendFormat("{0}{1}", "<b> Note:</b>", "<br>");
                                sb.AppendFormat("{0}{1}", "", note);

                                inventory.Handle = pn + "-" + vendorshort;
                                inventory.Title = pn;
                                inventory.Body__HTML_ = sb.ToString();
                                inventory.Type = "Aircraft Parts";
                                inventory.Tags = string.Join(",", description, vendor, inventory.Type, defaultCondition, pn);
                                inventory.Published = true;
                                inventory.Option1_Name = "Title";
                                inventory.Option1_Value = "Default Title";
                                inventory.Option2_Name = "Leadtime";
                                inventory.Option2_Value = defaultLeadtime;
                                inventory.Option3_Name = "MOQ";
                                inventory.Option3_Value = defaultMOQ;
                                inventory.Original_Price = originalPrice;
                                inventory.Variant_SKU = pn;
                                inventory.Variant_Price = finalPrice;
                                inventory.Variant_Inventory = qty;
                                inventory.Qty = qty;
                                inventory.Variant_Taxable = false;
                                inventory.Vendor = vendor;
                                inventory.VendorShort = vendorshort;
                                inventory.Lead_Time = defaultLeadtime;
                                inventory.MOQ = defaultMOQ;
                                inventory.Condition = defaultCondition;
                                inventory.Collection = collection;
                                inventory.SEO_Title = string.Join(",", description, "Part Number", pn);
                                inventory.SEO_Description = inventory.SEO_Title;
                                inventory.fob = defaultFob;
                                inventory.Certification = defaultCerts;
                                inventory.Notes = notes;
                                //inventory.Image_Src = "https://cdn.shopify.com/s/files/1/0416/3905/files/comingsoon_small_bw.gif";


                                inventories.Add(inventory);
                                //if (entityContext.Inventories.)
                                #endregion
                                //entityContext.Inventories.Add(inventory);
                                //entityContext.SaveChanges();

                            }
                            entityContext.Inventories.AddRange(inventories);
                            entityContext.SaveChanges();
                            Console.WriteLine("----- Total Count:", sheet.Count());
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine (((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors);
                    }

                }
            }
        }


        private static void GenerateShopify(int days = 0)
        {
            #region retrieve records from db
            TallamondEntities entityContext = new TallamondEntities();

            int maxLineCount = 10000;
            string theDirectory = System.Reflection.Assembly.GetAssembly(typeof(Inventory)).Location; ;

            string outputfolder = Path.GetDirectoryName(theDirectory);

            DateTime modDate = DateTime.Now.AddDays(days * -1);

            var vendors = entityContext.Vendors;



            #endregion

            int i = 0;
            foreach (var ven in vendors)
            {
                var inventories = entityContext.Inventories.Where(o => string.Compare(o.Vendor, ven.VendorName, true) == 0 && o.Published == true  && o.ModifiedDate >= modDate);

                var fileGroups = inventories.GroupBy(x => x.Id / maxLineCount);
                foreach (var grp in fileGroups)
                {
                    string outfile = Path.Combine(Path.Combine(outputfolder, grp.First().Vendor + "_" + "Shopify_" + i++ + ".csv"));


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
                                csvwriter.WriteRecord<Inventory>(record);
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private static void GenerateGeneral(int days = 0)
        {
            #region retrieve records from db
            TallamondEntities entityContext = new TallamondEntities();

            int maxLineCount = Int32.MaxValue;
            string theDirectory = System.Reflection.Assembly.GetAssembly(typeof(Inventory)).Location; ;

            string outputfolder = Path.GetDirectoryName(theDirectory);

            DateTime modDate = DateTime.Now.AddDays(days * -1);

            var vendors = entityContext.Vendors;



            #endregion

            int i = 0;
            foreach (var ven in vendors)
            {
                var inventories = entityContext.Inventories.Where(o => string.Compare(o.Vendor, ven.VendorName, true) == 0 && o.Published == true && o.ModifiedDate >= modDate);

                var fileGroups = inventories.GroupBy(x => x.Id / maxLineCount);
                foreach (var grp in fileGroups)
                {
                    string outfile = Path.Combine(Path.Combine(outputfolder, grp.First().Vendor + "_" + "General_" + i++ + ".csv"));


                    #region transform files
                    //string outfile_error = Path.Combine(Path.Combine(outputfolder, "Shopify_error_" + fn));
                    using (TextWriter writer = File.CreateText(outfile))
                    {
                        using (var csvwriter = new CsvWriter(writer))
                        {
                            csvwriter.Configuration.RegisterClassMap<OutputGeneral>();


                            csvwriter.WriteHeader<Inventory>();

                            foreach (var record in grp)
                            {
                                csvwriter.WriteRecord<Inventory>(record);
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private static void GenerateNew(int days = 0)
        {
            #region retrieve records from db
            TallamondEntities entityContext = new TallamondEntities();

            int maxLineCount = Int32.MaxValue;
            string theDirectory = System.Reflection.Assembly.GetAssembly(typeof(Inventory)).Location; ;

            string outputfolder = Path.GetDirectoryName(theDirectory);

            DateTime modDate = DateTime.Now.AddDays(days * -1);

            var vendors = entityContext.Vendors;



            #endregion

            int i = 0;
            foreach (var ven in vendors)
            {
                var inventories = entityContext.Inventories.Where(o => string.Compare(o.Vendor, ven.VendorName, true) == 0 && o.Published == true && o.ModifiedDate >= modDate);

                var fileGroups = inventories.GroupBy(x => x.Id / maxLineCount);
                foreach (var grp in fileGroups)
                {
                    string outfile = Path.Combine(Path.Combine(outputfolder, grp.First().Vendor + "_" + "Tallamond_" + i++ + ".csv"));


                    #region transform files
                    //string outfile_error = Path.Combine(Path.Combine(outputfolder, "Shopify_error_" + fn));
                    using (TextWriter writer = File.CreateText(outfile))
                    {
                        using (var csvwriter = new CsvWriter(writer))
                        {
                            csvwriter.Configuration.RegisterClassMap<OutputNew>();


                            csvwriter.WriteHeader<Inventory>();

                            foreach (var record in grp)
                            {
                                csvwriter.WriteRecord<Inventory>(record);
                            }
                        }
                    }
                    #endregion
                }
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
            Map(m => m.SEO_Description).Name("Description");
            Map(m => m.Original_Price).Name("Original Price");
            Map(m => m.Original_Price).TypeConverterOption(NumberStyles.Currency);
            Map(m => m.Vendor).Name("Vendor");
            Map(m => m.Type).Name("Type");
            Map(m => m.Tags).Name("Tags");
            Map(m => m.Variant_SKU).Name("Variant SKU");
            Map(m => m.Variant_Inventory).Name("Variant Inventory Qty");
            Map(m => m.Variant_Price).Name("Variant Price");
            Map(m => m.Variant_Taxable).Name("Variant Taxable");
            //Map(m => m.VendorShort).Name("VendorShort");
            //Map(m => m.Lead_Time).Name("Lead Time");
            //Map(m => m.MOQ).Name("MOQ");
            //Map(m => m.Condition).Name("Condition");
            //Map(m => m.Collection).Name("Collection");
            //Map(m => m.Option1_Name).Name("Option1 Name");
            //Map(m => m.Option1_Value).Name("Option1 Value");
            //Map(m => m.Option2_Name).Name("Option2 Name");
            //Map(m => m.Option2_Value).Name("Option2 Value");
            //Map(m => m.Option3_Name).Name("Option3 Name");
            //Map(m => m.Option3_Value).Name("Option3 Value");
            Map(m => m.Image_Src).Name("Image Src");
            Map(m => m.SEO_Title).Name("SEO Title");
            Map(m => m.SEO_Description).Name("SEO Description");
            //Map(m => m.Note).Name("Note");
            //Map(m => m.).Name("Total Value");
        }
    }

    public sealed class OutputGeneral : CsvClassMap<Inventory>
    {
        public OutputGeneral()
        {
            Map(m => m.Title).Name("Part Number");
            //Map(m => m.Body__HTML_).Name("Body (HTML)");
            Map(m => m.SEO_Description).Name("Description");
            //Map(m => m.Original_Price).Name("Original Price");
            //Map(m => m.Original_Price).TypeConverterOption(NumberStyles.Currency);
            //Map(m => m.Vendor).Name("Vendor");
            //Map(m => m.Type).Name("Type");
            //Map(m => m.Tags).Name("Tags");
            //Map(m => m.Variant_SKU).Name("Variant SKU");
            Map(m => m.Variant_Inventory).Name("Qty Available");
            //Map(m => m.Variant_Price).Name("Variant Price");
            //Map(m => m.Variant_Taxable).Name("Variant Taxable");
            //Map(m => m.VendorShort).Name("VendorShort");
            //Map(m => m.Lead_Time).Name("Lead Time");
            //Map(m => m.MOQ).Name("MOQ");
            Map(m => m.Condition).Name("Condition");
            //Map(m => m.Collection).Name("Collection");
            //Map(m => m.Option1_Name).Name("Option1 Name");
            //Map(m => m.Option1_Value).Name("Option1 Value");
            //Map(m => m.Option2_Name).Name("Option2 Name");
            //Map(m => m.Option2_Value).Name("Option2 Value");
            //Map(m => m.Option3_Name).Name("Option3 Name");
            //Map(m => m.Option3_Value).Name("Option3 Value");
            //Map(m => m.Image_Src).Name("Image Src");
            //Map(m => m.SEO_Title).Name("SEO Title");
            //Map(m => m.SEO_Description).Name("SEO Description");
            //Map(m => m.Note).Name("Note");
            //Map(m => m.).Name("Total Value");
        }
    }

    public sealed class OutputNew : CsvClassMap<Inventory>
    {
        public OutputNew()
        {
            Map(m => m.category).Name("category");
            Map(m => m.Title).Name("name");
            Map(m => m.Variant_SKU).Name("description");
            Map(m => m.SEO_Description).Name("short_description");
            Map(m => m.Handle).Name("sku");
            Map(m => m.Variant_Price).Name("Price");
            Map(m => m.tax_class_id).Name("tax_class_id");
            Map(m => m.is_in_stock).Name("is_in_stock");
            Map(m => m.Variant_Inventory ).Name("stock");
            Map(m => m.weight).Name("weight");
            Map(m => m.Image_Src).Name("image");
            Map(m => m.combine).Name("Condition,FOB,Certification,lead_time");

            //Map(m => m.Title).Name("PartNumber");
            //Map(m => m.Condition).Name("Condition");
            //Map(m => m.fob).Name("FOB");
            //Map(m => m.Certification ).Name("Certification");
            //Map(m => m.Lead_Time).Name("LeadTime");
            //Map(m => m.Variant_Inventory).Name("Inventory Qty");
            //Map(m => m.Dimensions).Name("Dimensions");
            //Map(m => m.Notes).Name("Notes");
            //Map(m => m.Technical_Documentation).Name("Technical Documentation");


            //category name    description short_description   sku Price   tax_class_id is_in_stock stock weight  image Condition, FOB, Certification, lead_time

            //Map(m => m.Original_Price).Name("Original Price");
            //Map(m => m.Original_Price).TypeConverterOption(NumberStyles.Currency);
            //Map(m => m.Vendor).Name("Vendor");
            //Map(m => m.Type).Name("Type");
            //Map(m => m.Tags).Name("Tags");
            //Map(m => m.Variant_SKU).Name("Variant SKU");
            //Map(m => m.Variant_Inventory).Name("Qty Available");
            //Map(m => m.Variant_Price).Name("Variant Price");
            //Map(m => m.Variant_Taxable).Name("Variant Taxable");
            //Map(m => m.VendorShort).Name("VendorShort");
            //Map(m => m.Lead_Time).Name("Lead Time");
            //Map(m => m.MOQ).Name("MOQ");
            //Map(m => m.Collection).Name("Collection");
            //Map(m => m.Option1_Name).Name("Option1 Name");
            //Map(m => m.Option1_Value).Name("Option1 Value");
            //Map(m => m.Option2_Name).Name("Option2 Name");
            //Map(m => m.Option2_Value).Name("Option2 Value");
            //Map(m => m.Option3_Name).Name("Option3 Name");
            //Map(m => m.Option3_Value).Name("Option3 Value");
            //Map(m => m.Image_Src).Name("Image Src");
            //Map(m => m.SEO_Title).Name("SEO Title");
            //Map(m => m.SEO_Description).Name("SEO Description");
            //Map(m => m.Note).Name("Note");
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

        public int getColumnIndex(string sheet, string columnName)
        {
            var reader = this.getExcelReader();
            var workSheet = reader.AsDataSet().Tables[sheet];
            return workSheet.Columns.IndexOf(columnName);
        }


    }
}
