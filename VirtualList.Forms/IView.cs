using System;
using System.Windows.Forms;

namespace VirtualList.Forms
{
    public interface IView : IWin32Window
    {
        event EventHandler LoadEvent;
        event EventHandler CloseEvent;
        void Show();
        DialogResult ShowDialog(IWin32Window owner);
        void Close();
    }
}
