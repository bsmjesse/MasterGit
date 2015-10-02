// Stephen Toub
// stoub@microsoft.com
//
// Very simple threadpool in C#.
// 4/27/04

#region Namespaces
using System;
using System.Threading;
using System.Collections;
#endregion

namespace VLF.CLS
{
   
   /// <summary>Managed thread pool.</summary>
   public class ManagedThreadPool2
   {
      #region Constants
      /// <summary>Maximum number of threads the thread pool has at its disposal.</summary>
      private const int _maxWorkerThreads = 10;
      #endregion

      #region Member Variables
      /// <summary>Queue of all the callbacks waiting to be executed.</summary>
      private Queue _waitingCallbacks;
      /// <summary>
      /// Used to signal that a worker thread is needed for processing.  Note that multiple
      /// threads may be needed simultaneously and as such we use a semaphore instead of
      /// an auto reset event.
      /// </summary>
      private Semaphore _workerThreadNeeded;
      /// <summary>List of all worker threads at the disposal of the thread pool.</summary>
      private ArrayList _workerThreads;
      /// <summary>Number of threads currently active.</summary>
      private int _inUseThreads;
      /// <summary>Lockable object for the pool.</summary>
      private object _poolLock = new object();
      #endregion

      #region Construction and Finalization
      /// <summary>Initialize the thread pool.</summary>
      public ManagedThreadPool2() { Initialize(); }

      /// <summary>Initializes the thread pool.</summary>
      private void Initialize()
      {
         // Create our thread stores; we handle synchronization ourself
         // as we may run into situtations where multiple operations need to be atomic.
         // We keep track of the threads we've created just for good measure; not actually
         // needed for any core functionality.
         _waitingCallbacks = new Queue();
         _workerThreads = new ArrayList();
         _inUseThreads = 0;

         // Create our "thread needed" event
         _workerThreadNeeded = new Semaphore(0);

         // Create all of the worker threads
         for (int i = 0; i < _maxWorkerThreads; i++)
         {
            // Create a new thread and add it to the list of threads.
            Thread newThread = new Thread(new ThreadStart(ProcessQueuedItems));
            _workerThreads.Add(newThread);

            // Configure the new thread and start it
            newThread.Name = "ManagedPoolThread #" + i.ToString();
            newThread.IsBackground = true;
            newThread.Start();
         }
      }
      #endregion

      #region Public Methods
      /// <summary>Queues a user work item to the thread pool.</summary>
      /// <param name="callback">
      /// A WaitCallback representing the delegate to invoke when the thread in the 
      /// thread pool picks up the work item.
      /// </param>
      public void QueueUserWorkItem(WaitCallback callback)
      {
         // Queue the delegate with no state
         QueueUserWorkItem(callback, null);
      }

      /// <summary>Queues a user work item to the thread pool.</summary>
      /// <param name="callback">
      /// A WaitCallback representing the delegate to invoke when the thread in the 
      /// thread pool picks up the work item.
      /// </param>
      /// <param name="state">
      /// The object that is passed to the delegate when serviced from the thread pool.
      /// </param>
      public void QueueUserWorkItem(WaitCallback callback, object state)
      {
         // Create a waiting callback that contains the delegate and its state.
         // At it to the processing queue, and signal that data is waiting.
         WaitingCallback waiting = new WaitingCallback(callback, state);
         lock (_poolLock) { _waitingCallbacks.Enqueue(waiting); }
         _workerThreadNeeded.AddOne();
      }

      /// <summary>
      ///      empty the queue from the existing jobs
      /// </summary>
      public void Clear()
      {
         lock (_poolLock) { _waitingCallbacks.Clear(); }
      }

      /// <summary>
      ///      remove only packets of a certain type
      /// </summary>
      /// <comment>
      ///      mostly used when the queue is used for multiple purposes and certain requests should be purged
      /// </comment>
      /// <param name="type"></param>
      public void Clear(Type type)
      {
         lock (_poolLock)
         {
            if (_waitingCallbacks.Count > 0)
            {
               Queue newWaitingCallbacks = new Queue(_waitingCallbacks.Count);
               foreach (object obj in _waitingCallbacks)
               {
                  WaitingCallback callback = (WaitingCallback)obj;
                  if (type != callback.Callback.GetType())
                     newWaitingCallbacks.Enqueue(callback);
                  else
                    if (callback.State is IDisposable) ((IDisposable)callback.State).Dispose();
               }

               _waitingCallbacks = newWaitingCallbacks;
            }
         }
      }
      /// <summary>Empties the work queue of any queued work items.  Resets all threads in the pool.</summary>
      /// <comment> reset is not used anywhere </comment>
      public void Reset()
      {
         lock (_poolLock)
         {
            // Cleanup any waiting callbacks
            try
            {
               // Try to dispose of all remaining state
               foreach (object obj in _waitingCallbacks)
               {
                  WaitingCallback callback = (WaitingCallback)obj;
                  if (callback.State is IDisposable) ((IDisposable)callback.State).Dispose();
               }
            }
            catch { }

            // Shutdown all existing threads
            try
            {
               foreach (Thread thread in _workerThreads)
               {
                  if (thread != null) thread.Abort("reset");
               }
            }
            catch { }

            /*
                        // Reinitialize the pool (create new threads, etc.)
                        Initialize();
             */
         }
      }
      #endregion

      #region Properties
      /// <summary>Gets the number of threads at the disposal of the thread pool.</summary>
      public int MaxThreads { get { return _maxWorkerThreads; } }
      /// <summary>Gets the number of currently active threads in the thread pool.</summary>
      public int ActiveThreads { get { return _inUseThreads; } }
      /// <summary>Gets the number of callback delegates currently waiting in the thread pool.</summary>
      public int WaitingCallbacks { get { lock (_poolLock) { return _waitingCallbacks.Count; } } }
      #endregion

      #region Thread Processing
      /// <summary>Event raised when there is an exception on a threadpool thread.</summary>
      public event UnhandledExceptionEventHandler UnhandledException;

      /// <summary>A thread worker function that processes items from the work queue.</summary>
      private void ProcessQueuedItems()
      {
         // Process indefinitely
         while (true)
         {
            _workerThreadNeeded.WaitOne();

            // Get the next item in the queue.  If there is nothing there, go to sleep
            // for a while until we're woken up when a callback is waiting.
            WaitingCallback callback = null;

            // Try to get the next callback available.  We need to lock on the 
            // queue in order to make our count check and retrieval atomic.
            lock (_poolLock)
            {
               if (_waitingCallbacks.Count > 0)
               {
                  try { callback = (WaitingCallback)_waitingCallbacks.Dequeue(); }
                  catch { } // make sure not to fail here
               }
            }

            if (callback != null)
            {
               // We now have a callback.  Execute it.  Make sure to accurately
               // record how many callbacks are currently executing.
               try
               {
                  Interlocked.Increment(ref _inUseThreads);
                  callback.Callback(callback.State);
               }
               catch (Exception exc)
               {
                  try
                  {
                     UnhandledExceptionEventHandler handler = UnhandledException;
                     if (handler != null) handler(typeof(ManagedThreadPool), new UnhandledExceptionEventArgs(exc, false));
                  }
                  catch { }
               }
               finally
               {
                  Interlocked.Decrement(ref _inUseThreads);
               }
            }
         }
      }
      #endregion

      /// <summary>Used to hold a callback delegate and the state for that delegate.</summary>
      private class WaitingCallback
      {
         #region Member Variables
         /// <summary>Callback delegate for the callback.</summary>
         private WaitCallback _callback;
         /// <summary>State with which to call the callback delegate.</summary>
         private object _state;
         #endregion

         #region Construction
         /// <summary>Initialize the callback holding object.</summary>
         /// <param name="callback">Callback delegate for the callback.</param>
         /// <param name="state">State with which to call the callback delegate.</param>
         public WaitingCallback(WaitCallback callback, object state)
         {
            _callback = callback;
            _state = state;
         }
         #endregion

         #region Properties
         /// <summary>Gets the callback delegate for the callback.</summary>
         public WaitCallback Callback { get { return _callback; } }
         /// <summary>Gets the state with which to call the callback delegate.</summary>
         public object State { get { return _state; } }
         #endregion
      }
   }
}
