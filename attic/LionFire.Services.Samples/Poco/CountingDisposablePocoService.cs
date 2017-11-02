using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services.Samples
{
    public class CountingDisposablePocoService : IDisposable
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        public CountingDisposablePocoService()
        {
            Console.Write(" --- Starting CountingDisposablePocoService starting --- ");
            int i = 0;
            Task.Factory.StartNew(() =>
            {
                Console.Write(i++ + " ");
                Debug.Write(i++ + " ");
            }, cts.Token).ContinueWith(t =>
            {
                Console.WriteLine();
                Console.WriteLine(" --- Starting CountingDisposablePocoService finished --- ");
            });

        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
