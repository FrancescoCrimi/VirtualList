using System;

namespace CiccioSoft.VirtualList.Data
{
    public enum DbType
    {
        SqLite = 0,
        MsLocalDb
    }

    public class MyAppOptions
    {
        public DbType DbType { get; set; }
    }
}
