using System;
using System.Threading ;
using System.Diagnostics;

namespace VLF.CLS
{
	/// <summary>
	/// 	Provides basic common behavior of all SentinelFM threads.
	/// </summary>
	/// <remarks>
	/// 	Every single thread of SentinelFM system is derived from this class. It becomes
	/// 	very useful when we are making array of dependent threads to control their behavior.
	///	2005/05/31	bgv
	///		- added the gracefully shutdown instead of calling Abort with the help of events
	/// </remarks>
	public abstract class ThreadBase : IDisposable
	{
		private DateTime lastUpdatedTime;	///< this is used by a montoring thread to recognize when a 
							                     ///< thread is stucked 

		#region Protected members
		/// <summary>
		/// 	      Referrence to the running thread.
		/// </summary>
		protected Thread workerThread ;
		/// <summary>
		/// 	   Threads own name. 
		/// </summary>
		protected string ownName;
		/// <summary>
		/// 	      Multilevel switch to control tracing and debug output.
		/// </summary>
		protected TraceSwitch tsMain;
      ///<summary>
      ///   the thread is signalled to exit its own procedure
      ///</summary>
      protected ManualResetEvent eventStopThread;
      ///<summary>
      ///   the thread signalls when before leaving its own procedure
      ///</summary>      
      protected ManualResetEvent eventThreadStopped;      
		#endregion

		/// <summary>
 		///     Initializes a new instance of the class derived from the ThreadBase, 
      ///     passed in the constructor 
		/// </summary>
		/// <param name="ownName">Thread's own name.</param>
		/// <param name="tsMain">Multilevel switch to control tracing and debug output.</param>
		public ThreadBase( string ownName, TraceSwitch tsMain )
		{
			this.ownName = ownName ;
			this.tsMain = tsMain ;
			workerThread = new Thread( new ThreadStart( ThreadProc ) );
			workerThread.Name = ownName;
         // initialize events
         eventStopThread = new ManualResetEvent(false);
         eventThreadStopped = new ManualResetEvent(false);
		}
      
		/// <summary>
		/// Starts current thread.
		/// </summary>
		public virtual void Start()
		{
			workerThread.Start() ;
		}	
      
     
		/// <summary>
		/// Stops current thread.
		/// </summary>
		public virtual void Stop()
		{
			if( isRunning() )
			{
              eventStopThread.Set() ;
/*              
              // wait when thread  will stop or finish
              while (workerThread.IsAlive)
              {
                // Instead of this we wait for event some appropriate time
                // (and by the way give time to worker thread) and process events.
                if ( WaitHandle.WaitAll( (new ManualResetEvent[] {eventThreadStopped}), 1000, true) )
                {
                    break;
                }
              }             
*/
              workerThread.Join();
  
			}
			workerThread = null;
		}
		/// <summary>
		/// Dispose method for ThreadBase class
		/// </summary>
		public virtual void Dispose()
		{
         Util.BTrace(Util.INF1, ">> ThreadBase.Dispose");
			Stop();
         eventStopThread.Close();
         eventThreadStopped.Close();
         Util.BTrace(Util.INF1, "<< ThreadBase.Dispose");

		}
		/// <summary>
		///      Checks current thread status.
		/// </summary>
      /// <comment>
      ///   [gb, 2006/28/07]
      ///   this condition is very tricky 
      ///    if you think you can return 
      ///       return (workerThread == null)?false:( (workerThread.ThreadState == ThreadState.Running) || (workerThread.ThreadState == ThreadState.WaitSleepJoin) );
      ///    the state of the thread could change in between comparisons !!
      ///    that's the reason you read only once !!!
      /// </comment>
		/// <returns> Returns true if thread is running. Otherwise it returns false.</returns>
		public bool isRunning()
		{        
			if( null != workerThread )
         {
            System.Threading.ThreadState thState = workerThread.ThreadState ;
            return ((thState == System.Threading.ThreadState.Running) || (thState == System.Threading.ThreadState.WaitSleepJoin) );
         }

			return false;
		}
		/// <summary>
		/// Updates heart beat info of current thread.
		/// </summary>
		protected void UpdateHeartBeatTime()
		{
			lastUpdatedTime = System.DateTime.Now;
		}
		/// <summary>
		/// Checks current thread health.
		/// </summary>
		/// <returns>Returns true if thread has not been updating heart beat value for 
		/// 'threshold' seconds period of time.</returns>
		public bool isLocked( double threshold )
		{
			// by default
			if( threshold == 0 )
				return false;

			return ( lastUpdatedTime.AddSeconds( threshold ) < DateTime.Now );
		}
		/// <summary>
		/// Gets a name of current thread.
		/// </summary>
		/// <returns>Returns current thread name.</returns>
		public string GetThreadName()
		{
			return ownName;
		}
		/// <summary>
		/// Gets a current thread state.
		/// </summary>
		/// <returns>Returns state of the current thread.</returns>
		public System.Threading.ThreadState GetThreadState()
		{
			return workerThread.ThreadState;
		}
		/// <summary>
		/// Thread routine - main thread procedure.
		/// </summary>
		protected abstract void ThreadProc();
	}
}
