using System;

namespace CiccioSoft.VirtualList.Data.Infrastructure
{
    public enum DbType
    {
        SqLite = 0,
        SqlServer,
        MsLocalDb,
        MySql,
        FakeDb
    }
}