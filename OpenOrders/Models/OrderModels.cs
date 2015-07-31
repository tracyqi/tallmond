using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenOrders.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CustomerPONum { get; set; }
        public DateTime CustomerPOCreated { get; set; }
        public String CustomerInvoiceNum { get; set; }
        public DateTime CustomerInvoiceCreated { get; set; }
        public string MfrPONum { get; set; }
        public DateTime MfrPOCreated { get; set; }
        public String MfrInvoiceNum { get; set; }
        public DateTime MfrInvoiceDate { get; set; }
        public DateTime NextFollowupDate { get; set; }

        public OrderStatus Status { get; set; }
        public string Note { get; set; }
        public string Owner { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

    public enum OrderStatus
    {
        POCreated, 
        CustomerPaymentRecved,
        MfrPOed,
        MfrShipped, 
        Closed
    }
}