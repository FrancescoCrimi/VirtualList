using System;
using System.Windows.Forms;

namespace CiccioSoft.VirtualList.Sample.Forms
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
