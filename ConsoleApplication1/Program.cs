using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upload
{
    class Program
    {
        static void Main(string[] args)
        {

            string inputfoler = @"F:\Git\test\data\input";

            string[] filePaths = Directory.GetFiles(inputfoler);

            foreach (string fileName in filePaths)
            {
                GenerateShopify(fileName);
                GenerateILSmart(fileName);
            }
        }

        private static void GenerateILSmart(string fileName)
        {
            #region Parse fileName
            string fn = Path.GetFileName(fileName);
            string vendor = fn.Split('_')[0];
            string vendorshort = fn.Split('_')[1];
            string outputfolder = Path.Combine(Path.GetDirectoryName(fileName).Replace("input", "output"));
            string outfile = Path.Combine(Path.Combine(outputfolder, "ILSmart_" + fn));
            #endregion

            //string outfile_error = Path.Combine(Path.Combine(outputfolder, "Shopify_error_" + fn));
            using (TextReader reader = File.OpenText(fileName))
            {
                using (TextWriter writer = File.CreateText(outfile))
                {
                    using (var csvwriter = new CsvWriter(writer))
                    {
                        csvwriter.Configuration.RegisterClassMap<OutputILSmartMap>();

                        using (var csv = new CsvReader(reader))
                        {
                            csv.Configuration.RegisterClassMap<InputMap>();
                            csv.Configuration.TrimFields = true;
                            csv.Configuration.SkipEmptyRecords = true;
                            csv.Configuration.WillThrowOnMissingField = false;
                            //csv.Configuration.IgnoreHeaderWhiteSpace = true;
                            //var recs = csv.GetRecords<Input>().ToList();

                            csvwriter.WriteHeader<OutputILSmart>();

                            while (csv.Read())
                            {
                                var record = csv.GetRecord<Input>();
                                //Input record = csv.GetRecord<Input>();

                                var o = new OutputILSmart();
                                o.ILSId = "C08H";
                                o.TransCode = "A";
                                o.PartNumber = record.PN;
                                o.AlternatePartNumber = string.Empty;
                                o.Condition = String.IsNullOrEmpty(record.Condition) ? "NE" : record.Condition;
                                o.Quantity = record.Quantity;
                                o.Description = record.Description + " For details, please check http://www.business.aero";
                                o.CAGE = string.Empty;
                                o.Category = string.Empty;
                                o.ControlCode = string.Empty;
                                o.Value = string.Empty;
                                o.ListCode = "Y";
                                //o.Price = record.UnitPrice; TODO: list price in the future
                                o.UoM = String.IsNullOrEmpty(record.UoM) ? "EA" : record.UoM;
                                o.TotalPrice = o.Quantity * record.UnitPrice;

                                csvwriter.WriteRecord<OutputILSmart>(o);
                            }
                        }
                    }
                }
            }
        }

        private static void GenerateShopify(string fileName)
        {
            #region Parse fileName
            string fn = Path.GetFileName(fileName);
            string vendor = fn.Split('_')[0];
            string vendorshort = fn.Split('_')[1];
            string outputfolder = Path.Combine(Path.GetDirectoryName(fileName).Replace("input", "output"));
            string outfile = Path.Combine(Path.Combine(outputfolder, "Shopify_" + fn));
            #endregion

            #region transform files
            //string outfile_error = Path.Combine(Path.Combine(outputfolder, "Shopify_error_" + fn));
            using (TextReader reader = File.OpenText(fileName))
            {
                using (TextWriter writer = File.CreateText(outfile))
                {
                    using (var csvwriter = new CsvWriter(writer))
                    {
                        csvwriter.Configuration.RegisterClassMap<OutputShopifyMap>();

                        using (var csv = new CsvReader(reader))
                        {
                            csv.Configuration.RegisterClassMap<InputMap>();
                            csv.Configuration.TrimFields = true;
                            csv.Configuration.SkipEmptyRecords = true;
                            csv.Configuration.WillThrowOnMissingField = false;
                            //csv.Configuration.IgnoreHeaderWhiteSpace = true;
                            //var recs = csv.GetRecords<Input>().ToList();

                            csvwriter.WriteHeader<OutputShopify>();

                            while (csv.Read())
                            {
                                var record = csv.GetRecord<Input>();
                                //Input record = csv.GetRecord<Input>();

                                var o = new OutputShopify();
                                o.Handle = record.PN + "-" + vendorshort;
                                o.Title = record.PN;
                                //o.Body = record.PN;//(HTML)	//TODO
                                o.Description = (String.IsNullOrEmpty(record.Description) || record.Description.Contains("?")) ? record.PN : record.Description
                                    + (String.IsNullOrEmpty(record.UoM) ? string.Empty : "<b> Unit of Measure: </b>" + record.UoM);
                                o.OriginalPrice = record.UnitPrice;
                                o.Vendor = vendor;
                                o.VendorShort = vendorshort;
                                o.LeadTime = 1;
                                //o.MOQ = 1; //TODO: total price 
                                o.Condition = "NE";
                                //AR – As Removed
                                //NE – New Equipment
                                //NS – New Surplus
                                //OH – Overhauled
                                //SV – Serviceable
                                o.Type = "Aircraft Parts";
                                o.Collection = vendor;
                                //o.VariantPrice = o.OriginalPrice * 1.5;
                                o.VariantSKU = record.PN;
                                o.VariantTaxable = "FALSE";
                                o.Option1Name = "Title";
                                o.Option1Value = "Default Title";
                                o.Option2Name = "Leadtime";
                                o.Option2Value = "1";
                                o.Option3Name = "MOQ";
                                o.Option3Value = o.MOQ;
                                o.Tags = string.Join(",", (String.IsNullOrEmpty(record.Description) || record.Description.Contains("?")) ? record.PN : record.Description, o.Vendor, o.Type, o.Condition, record.PN);
                                o.SEOTitle = string.Join(",", (String.IsNullOrEmpty(record.Description) || record.Description.Contains("?")) ? record.PN : record.Description, "Part Number", record.PN);
                                o.SEODescription = o.SEOTitle;
                                o.ImageSrc = "https://cdn.shopify.com/s/files/1/0416/3905/files/comingsoon_small_bw.gif";
                                o.Note = "<li><b>Condition:</b> FN </li>" +
                                        "<li>Comes with FAA 8130 form 3 </li>" +
                                        "<li>All parts are subject to prior sale </li>" +
                                        "<li>Sale price is effective for available inventory only </li>" +
                                        "<li><b>FOB China </b> </li>";
                                o.Quantity = record.Quantity;

                                csvwriter.WriteRecord<OutputShopify>(o);
                            }
                        }
                    }
                }
            }
            #endregion

            #region split the files if too big
            string[] filePaths = Directory.GetFiles(outputfolder);

            foreach (string file in filePaths)
            {
                string f = Path.Combine(outputfolder, Path.GetFileNameWithoutExtension(file));
                Split(file, outputfolder);
            }
            #endregion
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
                    Directory.CreateDirectory (outputfolder + "_new");

                string f = Path.Combine(outputfolder +"_new", Path.GetFileNameWithoutExtension(inputfile) + "_" + grp.FileIndex + ".csv");

                // Write headers for splitted file (Skip the first file)
                if (grp.FileIndex > 0)
                    File.WriteAllLines(f, lines);

                File.AppendAllLines(f, grp.Lines);
            }
        }
    }
}

class Input
{
    public string PN { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public double UnitPrice { get; set; }
    public string Condition { get; set; }
    public string UoM { get; set; }
    public string Option1 { get; set; }//ATA;
    public string Option2 { get; set; }//EngineType;
    public string Option3 { get; set; }//AircraftType;
    public string Option4 { get; set; }//
    public string Option5 { get; set; }//
    public string Option6 { get; set; }//
}

class InputMap : CsvClassMap<Input>
{
    public InputMap()
    {
        Map(m => m.PN).Name("P/N");
        Map(m => m.Quantity).Name("QTY");
        Map(m => m.Description).Name("DESCRIPTION");
        Map(m => m.UnitPrice).Name("UNIT PRICE");
        Map(m => m.UnitPrice).TypeConverterOption(NumberStyles.Currency);
        Map(m => m.Option1).Name("ATA");
        Map(m => m.UoM).Name("UoM");
        Map(m => m.Option2).Name("Engine Type");
        Map(m => m.Option3).Name("Aircraft type");
    }
}

public sealed class OutputShopifyMap : CsvClassMap<OutputShopify>
{
    public OutputShopifyMap()
    {
        Map(m => m.Handle).Name("Handle");
        Map(m => m.Title).Name("Title");
        Map(m => m.Body).Name("Body (HTML)");
        Map(m => m.Description).Name("Description");
        Map(m => m.OriginalPrice).Name("Original Price");
        Map(m => m.OriginalPrice).TypeConverterOption(NumberStyles.Currency);
        Map(m => m.Vendor).Name("Vendor");
        Map(m => m.VendorShort).Name("VendorShort");
        Map(m => m.LeadTime).Name("Lead Time");
        Map(m => m.MOQ).Name("MOQ");
        Map(m => m.Condition).Name("Condition");
        Map(m => m.Type).Name("Type");
        Map(m => m.Collection).Name("Collection");
        Map(m => m.VariantPrice).Name("Variant Price");
        Map(m => m.VariantSKU).Name("Variant SKU");
        Map(m => m.VariantTaxable).Name("Variant Taxable");
        Map(m => m.Option1Name).Name("Option1 Name");
        Map(m => m.Option1Value).Name("Option1 Value");
        Map(m => m.Option2Name).Name("Option2 Name");
        Map(m => m.Option2Value).Name("Option2 Value");
        Map(m => m.Option3Name).Name("Option3 Name");
        Map(m => m.Option3Value).Name("Option3 Value");
        Map(m => m.Tags).Name("Tags");
        Map(m => m.SEOTitle).Name("SEO Title");
        Map(m => m.SEODescription).Name("SEO Description");
        Map(m => m.ImageSrc).Name("Image Src");
        Map(m => m.Note).Name("Note");
        Map(m => m.Quantity).Name("Variant Inventory Qty");
        Map(m => m.TotalValue).Name("Total Value");
    }
}

public sealed class OutputILSmartMap : CsvClassMap<OutputILSmart>
{
    //ILS ID (4)	Trans Code (1)	Part Number (20)	Alternate Part Number (20)	Condition (2)	Quantity (5)	Description (20)	CAGE (5)	Category (9)	Control Code (9)	Value (9)	List Code (1)	Price (9)	U/M (2)

    public OutputILSmartMap()
    {
        Map(m => m.ILSId).Name("ILS ID (4)");
        Map(m => m.TransCode).Name("Trans Code (1)");
        Map(m => m.PartNumber).Name("Part Number (20)");
        Map(m => m.AlternatePartNumber).Name("Alternate Part Number (20)");
        Map(m => m.Condition).Name("Condition (2)");
        //Map(m => m.Quantity).TypeConverterOption(NumberStyles.Currency);
        Map(m => m.Quantity).Name("Quantity (5)");
        Map(m => m.Description).Name("Description (20)");
        Map(m => m.CAGE).Name("CAGE (5)");
        Map(m => m.Category).Name("Category (9)");
        Map(m => m.ControlCode).Name("Control Code (9)");
        Map(m => m.Value).Name("Value (9)");
        Map(m => m.ListCode).Name("List Code (1)");
        Map(m => m.Price).Name("Price (9)");
        Map(m => m.Price).TypeConverterOption(NumberStyles.Currency);
        Map(m => m.UoM).Name("U/M (2)");
        Map(m => m.TotalPrice).Name("Total Price");
    }
}

public class OutputShopify
{
    public string Handle { get; set; }
    public string Title { get; set; }
    public string Body
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}{2}", "<b>Description: </b>", this.Description, "<br>");
            sb.AppendFormat("{0}{1}{2}", "<b>Part Number: </b>", this.Title, "<br>");
            sb.AppendFormat("{0}{1}", "<b>Note:</b>", "<br>");
            sb.AppendFormat("{0}{1}{2}", "<ul>", this.Note, "</ul><br>");
            return sb.ToString();
        }
    }        //(HTML)	
    public string Description { get; set; }
    public double OriginalPrice { get; set; }
    public string Vendor { get; set; }
    public string VendorShort { get; set; }
    public int LeadTime { get; set; }
    public int MOQ
    {
        get
        {
            //=IF(E2>=200,1,ROUNDUP(200/E2,0))
            if (OriginalPrice == 0) return 0;
            if (OriginalPrice >= 200) return 1;
            return Convert.ToInt32(200 / OriginalPrice) > this.Quantity ? Convert.ToInt32(200 / OriginalPrice) : this.Quantity;
        }
    }
    public string Condition { get; set; }
    public string Type { get; set; }
    public string Collection { get; set; }
    public string VariantPrice
    {
        get
        {
            //=IF(NOT(ISNUMBER(E2)),"",ROUND(IF(E2<0.01,"",IF(E2<100,E2*1.3,IF(E2<1000,E2*1.2,IF(E2<3000,E2*1.15,IF(E2<10000,E2*1.1,IF(E2>=10000,E2*1.06,"")))))),2))

            if (double.IsNaN(OriginalPrice) || OriginalPrice <= 0.01) return "0";

            if (OriginalPrice < 100) return Math.Round((OriginalPrice * 1.3), 2).ToString();

            if (OriginalPrice < 1000) return Math.Round((OriginalPrice * 1.2), 2).ToString();

            if (OriginalPrice < 3000) return Math.Round((OriginalPrice * 1.15), 2).ToString();

            if (OriginalPrice < 10000) return Math.Round((OriginalPrice * 1.1), 2).ToString();

            if (OriginalPrice >= 10000) return Math.Round((OriginalPrice * 1.06), 2).ToString();

            return string.Empty;
        }
    }
    public string VariantSKU { get; set; }
    public string VariantTaxable { get; set; }
    public string Option1Name { get; set; }
    public string Option1Value { get; set; }
    public string Option2Name { get; set; }
    public string Option2Value { get; set; }
    public string Option3Name { get; set; }
    public double Option3Value { get; set; }
    public string Tags { get; set; }
    public string SEOTitle { get; set; }
    public string SEODescription { get; set; }
    public string ImageSrc { get; set; }
    public string Note { get; set; }
    public int Quantity { get; set; }
    public double TotalValue { get { return this.OriginalPrice * this.Quantity; } }
}

public class OutputILSmart
{
    public string ILSId { get; set; }
    public string TransCode { get; set; }
    public string PartNumber { get; set; }
    public string AlternatePartNumber { get; set; }
    public string Condition { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string CAGE { get; set; }
    public string Category { get; set; }
    public string ControlCode { get; set; }
    public string Value { get; set; }
    public string ListCode { get; set; }
    public double Price { get; set; }
    public string UoM { get; set; }
    public double TotalPrice { get; set; }


    //ILS ID (4)	Trans Code (1)	Part Number (20)	Alternate Part Number (20)	Condition (2)	Quantity (5)	Description (20)	CAGE (5)	Category (9)	Control Code (9)	Value (9)	List Code (1)	Price (9)	U/M (2)

}




//Handle	
//Title	
//Body (HTML)	
//Description	
//Original 
//Price	
//Vendor	
//VendorShort	
//Lead Time	
//MOQ	
//Condition	
//Type	
//Collection	
//Variant Price	
//Variant SKU	
//Variant Taxable	
//Option1 Name	
//Option1 Value	
//Option2 Name	
//Option2 Value	
//Option3 Name	
//Option3 Value	
//Tags	
//SEO Title	
//SEO Description	
//Image Src	
//Note