using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessQueueWithMultiThread
{
    class Program
    {

        private static void AddQue(ConcurrentQueue<int> cq)
        {
            for (int i = 0; i < 500; i++)
            {
                cq.Enqueue(i);
                Thread.Sleep(20);
                _queueNotifier.Set();
            }
        }

        private static AutoResetEvent _queueNotifier = new AutoResetEvent(false);

        static void Main(string[] args)
        {

            ConcurrentQueue<int> cq = new ConcurrentQueue<int>();

            var addTask = Task.Run(() => { Program.AddQue(cq);});

            Thread.Sleep(50);
          
            // An action to consume the ConcurrentQueue.
            Action action = () =>
            {
                int localValue;
                while (cq.TryDequeue(out localValue))
                {
                    Thread.Sleep(10);
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Queue count= {cq.Count} Decode get "+ localValue );
                }
            };

            while(true)
            {
                // 沒設定會占用CPU資源
                _queueNotifier.WaitOne();
                
                // Start 3 concurrent consuming actions
                Parallel.Invoke(action, action, action, action, action);
            }        
        }
      
    }
}
