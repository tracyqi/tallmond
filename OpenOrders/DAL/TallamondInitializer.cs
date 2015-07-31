using OpenOrders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenOrders.DAL
{
    public class TallamondInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<TallamondContext>
    {
        protected override void Seed(TallamondContext context)
        {
            var products = new List<Product>
            {
                new Product { ProductTitle="412000021154", Description ="CIRCUIT BREAKER", PartNumber="412000021154", Vendor="hainan", LeadTime ="3-5 days", Certification ="", FOB ="FOB China",  Image ="", MOQ ="$150",  ProductType="Aircraft Parts", Condition ="NS", OriginalPrice = 129.14, VariantPrice = 150.15 },
                new Product { ProductTitle="084741010100017",   Description =" HOOK",   PartNumber="084741010100017", Vendor="hainan", LeadTime ="3-5 days", Certification ="", FOB ="FOB China",  Image ="", MOQ ="$150",  ProductType="Aircraft Parts", Condition ="NS", OriginalPrice = 0.78, VariantPrice = 3.20 },
                new Product { ProductTitle="0133008800000",   Description =" Catch Double Rev. Assy",   PartNumber="0133008800000", Vendor="Airbus Helicopter", LeadTime ="3-5 days", Certification ="", FOB ="FOB China",  Image ="", MOQ ="$150",  ProductType="Aircraft Parts", Condition ="NE", OriginalPrice = 0.78, VariantPrice = 3.20 },
           };

            products.ForEach(s => context.Products.Add(s));
            context.SaveChanges();

            var orders = new List<Order>
            {
                new Order{CustomerPONum ="123-45-XXX", CustomerPOCreated =DateTime.Parse("2015/7/20"),   CustomerInvoiceNum ="", CustomerInvoiceCreated = DateTime.Parse("2015/7/20"),MfrPONum ="", MfrPOCreated  = DateTime.Parse("2015/7/20"), MfrInvoiceNum ="", MfrInvoiceDate   = DateTime.Parse("2015/7/20"),  NextFollowupDate   = DateTime.Parse("2015/7/20"), Status =OrderStatus.MfrPOed, Owner="tracy", Note ="XXXX supplieres contacted" },
                new Order{CustomerPONum ="TestPO45678", CustomerPOCreated =DateTime.Parse("2015/7/20"),   CustomerInvoiceNum ="", CustomerInvoiceCreated = DateTime.Parse("2015/7/20"),MfrPONum ="", MfrPOCreated  = DateTime.Parse("2015/7/20"), MfrInvoiceNum ="", MfrInvoiceDate   = DateTime.Parse("2015/7/20"),  NextFollowupDate   = DateTime.Parse("2015/7/20"), Status =OrderStatus.MfrPOed, Owner="tracy", Note ="XXXX supplieres contacted" },
            };
            orders.ForEach(s => context.Orders.Add(s));
            context.SaveChanges();
        }
    }
}