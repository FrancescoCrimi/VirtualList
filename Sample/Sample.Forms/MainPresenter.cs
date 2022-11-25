using System;

namespace CiccioSoft.VirtualList.Sample.Forms
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
