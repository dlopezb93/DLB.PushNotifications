using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Options
{
    public class AsyncLock : IDisposable
    {
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<AsyncLock> LockAsync()
        {
            await _semaphoreSlim.WaitAsync();
            return this;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
