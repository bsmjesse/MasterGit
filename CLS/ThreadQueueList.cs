using System;
using System.Diagnostics;
using System.Collections ;
using System.Threading ;

namespace VLF.CLS
{
//	public delegate void OnQueueEventHandler(object sender);
	/// <summary>
	/// ThreadQueue aggregates ArrayList with event fired on new item in the queue.
	/// Event is created in TreadQueue's working thread.
	/// Content is synhronized and iteratable, remove function is available
	/// </summary>
	
	public class ThreadQueueList : ThreadBase
	{
		private const int ThreadDelay = 50 ;
        private TraceSwitch tsMain;

        public ThreadQueueList(string name_, TraceSwitch tsMain_): base(name_,tsMain_)
		{
			Queue = new ArrayList() ;
            base.Start() ;
		}

		public void Dispose()
		{
			base.Stop() ;
		}
		
		protected ArrayList Queue ;
		
		public event OnQueueEventHandler OnQueue ;
		
		protected Thread workerThread ;

		public void AddQueue( object item )
		{
			ArrayList syncQ = ArrayList.Synchronized( Queue ) ;
			syncQ.Add( item ) ;		
		}
		
		public object this[int index]
		{
			get
			{
				ArrayList syncQ = ArrayList.Synchronized( Queue ) ;
				return syncQ[index] ;
			}
		}
		
		public void RemoveAt( int index )
		{
			ArrayList syncQ = ArrayList.Synchronized( Queue ) ;		
			syncQ.RemoveAt( index ) ;
		}
		
		public int Count
		{
			get
			{
				ArrayList syncQ = ArrayList.Synchronized( Queue ) ;
				return syncQ.Count ;
			}
		}

        protected override void ThreadProc()
        {
            Util.BTrace(Util.INF1, ">> ThreadQueueList.ThreadProc ->  {0} started ", ownName);

#if   FAULTY_SERVER
            EventWaitHandle hFaulty = new EventWaitHandle(false, EventResetMode.ManualReset, ownName + "Event");
            EventWaitHandle[] events = new EventWaitHandle[] 
               {
                   eventStopThread, 
                   hFaulty,
               };

           Util.BTrace(Util.INF1, "-- ThreadQueueList.ThreadProc -> STOPEVENT[{0}Event]", ownName);
#else
            ManualResetEvent[] events = new ManualResetEvent[] { eventStopThread };
#endif

            try
            {
                while (workerThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    if (WaitHandle.WaitTimeout != WaitHandle.WaitAny(events, ThreadDelay, true))
                    {
                        Util.BTrace(Util.INF1, "-- ThreadQueueList.ThreadProc ->  {0} signalled to stop ", ownName);
                        break;
                    }

                    ArrayList syncQ = ArrayList.Synchronized(Queue);

                    if (syncQ.Count > 0)
                        OnQueueAvailable();
                }
            }
            catch (ThreadAbortException)
            {
                // just exit here
            }

#if   FAULTY_SERVER
            hFaulty.Close();
#endif
            Util.BTrace(Util.INF1, "<< ThreadQueueList.ThreadProc ->  {0} stopped ", ownName);

        }

		protected virtual void OnQueueAvailable() 
		{
			if (OnQueue != null)
			{
				object item ;
				lock( Queue.SyncRoot )
				{
					item = Queue[0] ;
					Queue.RemoveAt(0) ;
				}
				OnQueue( item );
			}
		}

	}

}
