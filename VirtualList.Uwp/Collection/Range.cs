namespace CiccioSoft.VirtualList.Uwp.Collection
{
    internal class Range
    {
        public int FirstVisible { get; set; }
        public int LastVisible { get; set; }
        public int LengthVisible { get; set; }
        public int FirstTracked { get; set; }
        public int LastTracked { get; set; }
        public int LengthTracked { get; set; }
    }
}
