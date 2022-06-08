using System;

namespace VirtualList.Forms
{
    public class MainPresenter : PresenterBase, IDisposable
    {
        public MainPresenter(IView view)
            : base(view)
        {
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
