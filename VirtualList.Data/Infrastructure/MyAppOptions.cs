using System;

namespace CiccioSoft.VirtualList.Data.Infrastructure
{
    public enum DbType
    {
        SqLite = 0,
        MsLocalDb,
        FakeDb
    }

    public class MyAppOptions
    {
        public DbType DbType { get; set; }
    }
}
