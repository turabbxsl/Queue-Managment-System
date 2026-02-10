using NpgsqlTypes;

namespace QMS.Core.Enums
{
    public enum ServiceType
    {
        [PgName("Credit")]
        Credit,

        [PgName("Deposit")]
        Deposit,

        [PgName("Card")]
        Card,

        [PgName("Transfer")]
        Transfer,

        [PgName("Exchange")]
        Exchange
    }
}
