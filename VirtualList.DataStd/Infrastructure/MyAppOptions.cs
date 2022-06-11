using System;

namespace CiccioSoft.VirtualList.DataStd.Infrastructure
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
