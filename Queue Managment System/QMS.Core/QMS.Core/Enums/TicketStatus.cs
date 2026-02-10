using NpgsqlTypes;

namespace QMS.Core.Enums
{
    public enum TicketStatus
    {
        [PgName("Waiting")]
        Waiting,

        [PgName("InProgress")]
        InProgress,

        [PgName("Completed")]
        Completed,

        [PgName("Cancelled")]
        Cancelled
    }
}
