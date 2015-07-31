using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenOrders.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public string PartNumber { get; set; }
        public double OriginalPrice { get; set; }
        public double VariantPrice { get; set; }
        public string Vendor { get; set; }
        public string LeadTime { get; set; }
        public string Certification { get; set; }
        public string FOB { get; set; }
        public string Image { get; set; }
        public string MOQ { get; set; }
        public string ProductType { get; set; }
        public string Tags { get; set; }
        public string Condition { get; set; }
        public DateTime Created
        {
            get { return DateTime.Now; }
        }
        public DateTime Modified
        {
            get { return DateTime.Now; }
        }
    }
}
