using System.Windows.Forms;

namespace VirtualList.Forms
{
    public abstract class PresenterBase
    {
        private readonly IView view;

        protected PresenterBase(IView view)
        {
            this.view = view;
        }

        public object View => view;

        public void Show() => view.Show();

        public DialogResult ShowDialog(IWin32Window owner) => view.ShowDialog(owner);
    }
}
