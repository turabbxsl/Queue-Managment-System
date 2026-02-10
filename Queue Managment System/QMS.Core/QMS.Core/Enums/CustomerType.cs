using NpgsqlTypes;

namespace QMS.Core.Enums
{
    public enum CustomerType
    {
        [PgName("Regular")]
        Regular,

        [PgName("VIP")]
        VIP
    }
}
