using System;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class Semaphore
    {
        private static bool _isBusy = false;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task FetchData()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (_isBusy)
                {
                    return;
                }
                _isBusy = true;
            }
            finally
            {
                semaphoreSlim.Release();
            }

            await Task.Delay(300);

            await semaphoreSlim.WaitAsync();
            try
            {
                _isBusy = false;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
