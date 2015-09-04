using System;
// using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VLF.ERR;

namespace VLF.CLS
{
   /// <summary>
   /// ThreadDeque aggregates Deque with event fired on new item in the queue.
   /// Event is created in ThreadDeque's working thread.
   /// Content is synhronized and iteratable, remove function is available
   /// </summary>
   public class ThreadList : ThreadBase
   {
      protected LinkedList<object> queue;
      public OnQueueEventHandler evtOnQueue;
      protected PerformanceCounters performanceCounters;
      protected string counterName;

      public ThreadList(OnQueueEventHandler evtOnQueue,
                        string name,
                        TraceSwitch tsMain,
                        PerformanceCounters perfCounters,
                        string countName)
         : base(name, tsMain)
      {
         this.evtOnQueue = evtOnQueue;
         queue = new LinkedList<object>();
         performanceCounters = perfCounters;
         counterName = countName;
      }

      public void AddHead(object item)
      {
         lock (queue)
         {
            queue.AddFirst(item);
         }
      }

      public void AddLast(object item)
      {
         lock (queue)
         {
            queue.AddLast(item);
         }
      }
      public int Count
      {
         get
         {
            lock (queue)
            {
               return queue.Count;
            }
         }
      }

      /// <summary>
      /// main thread procedure
      /// </summary>
      protected override void ThreadProc()
      {
         bool bThreads = false;
         if (ownName.Contains("QueueIn"))
            bThreads = true;
         Util.BTrace(Util.INF0, "ThreadList.ThreadProc ->  {0} started pool {1}", ownName, bThreads);


#if   FAULTY_SERVER
            EventWaitHandle hFaulty = new EventWaitHandle(false, EventResetMode.ManualReset, ownName + "Event");
            EventWaitHandle[] events = new EventWaitHandle[] 
               {
                   eventStopThread, 
                   hFaulty,
               };

            Util.BTrace(Util.INF1, "-- ThreadList.ThreadProc -> STOPEVENT[{0}Event]", ownName);
#else
         ManualResetEvent[] events = new ManualResetEvent[] { eventStopThread };
#endif
         int queueSize = 0, chunks = 0;

         try
         {
            while (true)
            {
               UpdateHeartBeatTime();

               if (WaitHandle.WaitTimeout != WaitHandle.WaitAny(events, Def.Const.mqsThreadDelay, true))
               {
                  Util.BTrace(Util.INF1, "ThreadList.ThreadProc ->  {0} signalled to stop ", ownName);
                  break;
               }


               try
               {
                  queueSize = Count;

                  if (performanceCounters != null)
                     performanceCounters.SetRawValue(counterName, queueSize);

                  if (queueSize > 0)
                  {

                     // here we should find a method of throttling the messages for SLS
                     // 
                     if (queueSize > 300)
                     {
                        Util.BTrace(Util.INF0, "LIST MESSAGES {0} has {1} messages, sleep ...", ownName, queueSize);
                        Thread.Sleep(500);
                     }
                     else
                     {

                        if (queueSize > 25)
                           Util.BTrace(Util.INF0, "{0} has {1} messages", ownName, queueSize);
                        chunks = queueSize > 10 ? 10 : queueSize;
                        lock (queue)
                        {
                           for (int i = 0; i < chunks; ++i)
                           {
                              ManagedThreadPool.QueueUserWorkItem(new WaitCallback(evtOnQueue),
                                                queue.First.Value);
                              queue.RemoveFirst();
                           }
                        }
                     }
                  }

               }
               catch (DASDbConnectionClosed)
               {
                  Util.BTrace(Util.ERR2, "ThreadList.ThreadProc -> DASDbConnectionClosed in {0}", ownName);
                  break;
               }
               catch (Exception ex)
               {
                  Util.BTrace(Util.ERR2, "ThreadList.ThreadProc ->(1) EXCEPTION[{0} in {1}", ex.Message, ownName);
                  break;
               }
            }
         }
         catch (ThreadAbortException ex)
         {
            Util.BTrace(Util.ERR2, "ThreadList.ThreadProc -> THREADABORTEXCEPTION[{0} in {1}", ex.Message, ownName);
         }
         catch (Exception ex)
         {
            Util.BTrace(Util.ERR2, "ThreadList.ThreadProc ->(2) EXCEPTION[{0}] in {1}", ex.Message, ownName);
         }
         finally
         {
            ManagedThreadPool.Reset();
#if   FAULTY_SERVER
            hFaulty.Close();
#endif
            Util.BTrace(Util.ERR2, "ThreadList.ThreadProc -> stopped {0}", ownName);
         }
      }
   }
}
