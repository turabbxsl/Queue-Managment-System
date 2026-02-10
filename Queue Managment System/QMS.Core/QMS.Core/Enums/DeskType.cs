using NpgsqlTypes;

namespace QMS.Core.Enums
{
    public enum DeskType
    {
        [PgName("Operation")]
        Operation,

        [PgName("Cash")]
        Cash,

    }
}
