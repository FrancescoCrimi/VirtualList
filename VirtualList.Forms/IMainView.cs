using System;

namespace VirtualList.Forms
{
    public interface IMainView : IView
    {
        event EventHandler SearchEvent;
    }
}
