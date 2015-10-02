using System;
using System.Collections ;
using System.Diagnostics;
using System.Threading ;
using VLF.ERR ;

namespace VLF.CLS
{
	/// <summary>
   ///      DBWriterThread is a queue to store multiple write operation in a queue and write once in a SQL table
   ///      instead of multiple accesses to the table
	///      Content is synchronized and iteratable
	/// </summary>
   /// <comment> 
   ///       this function attempt to simulate a bulk insert and is useful for operations
   ///       used in "vlfmsgInHist" and "vlfMsgOutHst" and vlfMsgIn
   /// 
   ///     this is useful when opertions are delayed with database are delayed for certain handling
   ///     - update the address in bulk
   ///     - decode a packet
   ///      NEW
   ///        - the thread has an interface to receive messages from remote dispatchers
   ///          and once a certain numbers of jobs are in queue, then distpatch a server to handle them in block
   ///        - you eliminate the polling mechanism of the database and the worker it is doing the job for
   ///            - getting N packets, filling the address, saving in history, deleting from repository
   /// </comment>   
	public class DBWriterThread: ThreadQueue 
	{
      int _maximumNumRequests;

      public DBWriterThread(OnQueueEventHandler evtOnQueue, 
			                   string name,
                            int maximumNumRequests,         ///< how many request it accepts
			                   TraceSwitch tsMain,
			                   PerformanceCounters perfCounters,
			                   string countName ) 
			                   : base(evtOnQueue, name,tsMain, perfCounters, countName)
		{
         _maximumNumRequests = maximumNumRequests;
		}		

		/// <summary>
		/// main thread procedure
		/// </summary>
		protected override void ThreadProc()
		{
         bool bThreads = false;
         if (ownName.Contains("QueueIn"))
            bThreads = true;
         Util.BTrace(Util.INF0, "DBWriterThread.ThreadProc ->  {0} started pool {1}", ownName, bThreads);


#if   FAULTY_SERVER
            EventWaitHandle hFaulty = new EventWaitHandle(false, EventResetMode.ManualReset, ownName + "Event");
            EventWaitHandle[] events = new EventWaitHandle[] 
               {
                   eventStopThread, 
                   hFaulty,
               };

            Util.BTrace(Util.INF1, "-- DBWriterThread.ThreadProc -> STOPEVENT[{0}Event]", ownName);
#else
         ManualResetEvent[] events = new ManualResetEvent[] { eventStopThread };
#endif

         try
         {
            while (true)
            {
               UpdateHeartBeatTime();

               if (WaitHandle.WaitTimeout != WaitHandle.WaitAny(events, Def.Const.mqsThreadDelay, true))
               {
                  Util.BTrace(Util.INF1, "DBWriterThread.ThreadProc ->  {0} signalled to stop ", ownName);
                  break;
               }

               if (performanceCounters != null)
                  performanceCounters.SetRawValue(counterName, Count);
               try
               {
                  Queue syncQ = Queue.Synchronized(queue);
                  int queueSize = syncQ.Count;
                  if (queueSize > 0)
                  {
                     // here we should implement a throttling mechanism to delete messages which don't have a chance 
                     // of being handled in real time 
                     if ((queueSize > _maximumNumRequests) && bThreads)
                     {
                        Util.BTrace(Util.INF0, "DELETE MESSAGES | {0} has {1} messages", ownName, queueSize);
                        syncQ.Clear();
                     }
                     else
                     {
                        if (queueSize > 20)
                           Util.BTrace(Util.INF0, "{0} has {1} messages", ownName, queueSize);
                        ManagedThreadPool.QueueUserWorkItem(new WaitCallback(evtOnQueue), syncQ.Dequeue());
                     }
                  }

               }
               catch (DASDbConnectionClosed)
               {
                  Util.BTrace(Util.ERR2, "ThreadQueue.ThreadProc -> DASDbConnectionClosed in {0}", ownName);
                  break;
               }
               catch (Exception ex)
               {
                  Util.BTrace(Util.ERR2, "ThreadQueue.ThreadProc ->(1) EXCEPTION[{0} in {1}", ex.Message, ownName);
                  break;
               }
            }
         }
         catch (ThreadAbortException ex)
         {
            Util.BTrace(Util.ERR2, "ThreadQueue.ThreadProc -> THREADABORTEXCEPTION[{0} in {1}", ex.Message, ownName);
         }
         catch (Exception ex)
         {
            Util.BTrace(Util.ERR2, "ThreadQueue.ThreadProc ->(2) EXCEPTION[{0}] in {1}", ex.Message, ownName);
         }
         finally
         {
            ManagedThreadPool.Reset();
#if   FAULTY_SERVER
            hFaulty.Close();
#endif
            Util.BTrace(Util.ERR2, "ThreadQueue.ThreadProc -> stopped {0}", ownName);
         }
      }
	}
}
