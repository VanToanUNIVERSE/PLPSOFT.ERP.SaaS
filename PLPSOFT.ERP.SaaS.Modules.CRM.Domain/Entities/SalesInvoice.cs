namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class SalesInvoice
    {
        public long InvoiceID { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Customer? Customer { get; set; }
    }
}
