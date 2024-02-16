// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

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
