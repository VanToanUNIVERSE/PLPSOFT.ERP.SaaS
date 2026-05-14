namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class SystemTypeValue
    {
        public long TypeValueID { get; set; }
        public long TypeID { get; set; }
        public string ValueCode { get; set; } = string.Empty;
        public string ValueName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public SystemType? Type { get; set; }
    }
}
