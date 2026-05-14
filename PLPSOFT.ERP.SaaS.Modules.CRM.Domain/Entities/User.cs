namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class User
    {
        public long UserID { get; set; }
        public long CompanyID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public Company? Company { get; set; }
    }
}
