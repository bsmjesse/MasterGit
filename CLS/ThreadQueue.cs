using System;
using System.Collections ;
using System.Diagnostics;
using System.Threading ;
using VLF.ERR ;

namespace VLF.CLS
{
	public delegate void OnQueueEventHandler(object message);

	/// <summary>
	/// ThreadQueue aggregates Queue with event fired on new item in the queue.
	/// Event is created in ThreadQueue's working thread.
	/// Content is synhronized and iteratable, remove function is available
	/// </summary>
	public class ThreadQueue : ThreadBase
	{
		protected Queue queue ;
		public  OnQueueEventHandler evtOnQueue ;
		protected PerformanceCounters performanceCounters;
		protected string counterName;

		public ThreadQueue(	OnQueueEventHandler evtOnQueue, 
			                string name,
			                TraceSwitch tsMain,
			                PerformanceCounters perfCounters,
			                string countName ) 
			                : base(name,tsMain)
		{
			this.evtOnQueue = evtOnQueue;
			queue = new Queue() ;
			performanceCounters = perfCounters;
			counterName = countName;
		}

		public void AddQueue( object item )
		{
         Queue syncQ = Queue.Synchronized(queue);
         syncQ.Enqueue(item);		
		}


		public int Count
		{
			get
			{
				return Queue.Synchronized( queue ).Count  ;
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
            Util.BTrace(Util.INF0, "ThreadQueue.ThreadProc ->  {0} started pool {1}", ownName, bThreads);
            

#if   FAULTY_SERVER
            EventWaitHandle hFaulty = new EventWaitHandle(false, EventResetMode.ManualReset, ownName + "Event");
            EventWaitHandle[] events = new EventWaitHandle[] 
               {
                   eventStopThread, 
                   hFaulty,
               };

            Util.BTrace(Util.INF1, "-- ThreadQueue.ThreadProc -> STOPEVENT[{0}Event]", ownName);
#else
            ManualResetEvent[] events = new ManualResetEvent[] { eventStopThread };
#endif            
         int queueSize = 0, chunks = 0 ;

			try
			{            
				while(true)
				{
					 UpdateHeartBeatTime();
               
                if ( WaitHandle.WaitTimeout != WaitHandle.WaitAny( events, Def.Const.mqsThreadDelay, true) )
                {
                   Util.BTrace( Util.INF1, "ThreadQueue.ThreadProc ->  {0} signalled to stop ", ownName ) ;
                   break ;
                }
               
					
					try
					{
                  Queue syncQ = Queue.Synchronized(queue);
                  queueSize = syncQ.Count;

                  if (performanceCounters != null)
                     performanceCounters.SetRawValue(counterName, queueSize);


                  if (queueSize > 0)
                  {
                     if (queueSize > 10)
                        Util.BTrace(Util.INF0, "{0} has {1} messages", ownName, queueSize);
                     chunks = queueSize > 10 ? 10 : queueSize;
                     for (int i = 0; i < chunks; ++i)
                        ManagedDequeThreadPool.QueueUserWorkItem(new WaitCallback(evtOnQueue), syncQ.Dequeue(), false);
                  }
					}
					catch(DASDbConnectionClosed)
					{
                  Util.BTrace( Util.ERR2, "ThreadQueue.ThreadProc -> DASDbConnectionClosed in {0}", ownName);
						break;
					}
					catch(Exception ex)
					{
                  Util.BTrace( Util.ERR2, "ThreadQueue.ThreadProc ->(1) EXCEPTION[{0} in {1}", ex.Message, ownName);
                  break ;
					}
				}
			}
			catch( ThreadAbortException ex)
			{
				 Util.BTrace( Util.ERR2, "ThreadQueue.ThreadProc -> THREADABORTEXCEPTION[{0} in {1}", ex.Message, ownName);
			}
			catch(Exception ex)
			{
             Util.BTrace( Util.ERR2, "ThreadQueue.ThreadProc ->(2) EXCEPTION[{0}] in {1}", ex.Message, ownName);
			}
			finally
			{
            ManagedThreadPool.Reset();
#if   FAULTY_SERVER
            hFaulty.Close();
#endif
            Util.BTrace( Util.ERR2, "ThreadQueue.ThreadProc -> stopped {0}", ownName);
			}
		}
	}
}
