namespace PLPSOFT.ERP.SaaS.Modules.CRM.Domain.Entities
{
    public class SystemType
    {
        public long TypeID { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;

        public ICollection<SystemTypeValue> Values { get; set; } = new List<SystemTypeValue>();
    }
}
