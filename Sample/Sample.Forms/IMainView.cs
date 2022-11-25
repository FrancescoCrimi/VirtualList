using System;

namespace CiccioSoft.VirtualList.Sample.Forms
{
    public interface IMainView : IView
    {
        event EventHandler SearchEvent;
    }
}
