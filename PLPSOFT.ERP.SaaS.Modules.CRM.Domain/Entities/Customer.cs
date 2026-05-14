namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class Customer
    {
        public long CustomerID { get; set; }
        public long CompanyID { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public Company? Company { get; set; }
    }
}
