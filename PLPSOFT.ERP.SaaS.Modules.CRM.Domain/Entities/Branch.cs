namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class Branch
    {
        public long BranchID { get; set; }
        public long CompanyID { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;

        public Company? Company { get; set; }
    }
}
