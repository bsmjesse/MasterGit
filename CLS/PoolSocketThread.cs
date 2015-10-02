/** \file    PoolSocketThread.cs
 *  \brief   this is the file implementing a pool of threads with a simple job of
 *           waiting on a queue of accepted sockets and reading on them 
 *  \author  gb
 *  \date    2006/05/06
 *  \comment you have to change a little bit the class in order to support connected sockets
 */
using System;
using System.Threading;
using System.Collections;
using System.Net.Sockets;



namespace VLF.CLS
{
    public delegate void Functor(Socket s);   // definitions of the delegate 

    public class PoolSocketThreads
    {
        public const int MAX_THREADS_IN_POOL = 8;

        private int count = 0;          // initial number of threads in the pool
        private long running = 0;        // the # of busy threads
        private Thread[] arrThreads;
        private ArrayList listSockets;
        private ManualResetEvent eventStop;
        private AutoResetEvent eventHandleTask;

        private Functor DoReadSocket;             // the pointer to the delegate

        /** \fn       public void HandleTask()
         *  \brief    handle one request whenever is signalled by the producer (acceptor thread in TCPServer)
         *  \comment  in fact the queue of threads should wait on a semaphore and on a stop event 
        */
        public void HandleTask()
        {
            Util.BTrace(Util.INF1, ">> PoolSocketThreads.ReadSocket -> ID{0} DateTime[{1}]", Thread.CurrentThread.GetHashCode(), DateTime.Now.ToString());
            WaitHandle[] events = new WaitHandle[] { eventStop, eventHandleTask };


            while (true)
            {
                int iRet = WaitHandle.WaitAny(events, 20, true);
                if (0 == iRet)
                {
                    // signalled to exit
                    Util.BTrace(Util.WARN1, "-- PoolSocketThreads.ReadSocket -> {0} out", Thread.CurrentThread.GetHashCode());
                    break;
                }
                Interlocked.Increment(ref running);
                if (1 == iRet)
                {
                    // get the socket and do the job
                    DoReadSocket(GetSocket());      // this function shouldn't throw in ANY CASE
                }
                Interlocked.Decrement(ref running);

                // and here you should listen on all open sockets and signal the pool when there is read data
            }

            Util.BTrace(Util.INF1, "<< PoolSocketThreads.ReadSocket -> ID{0} DateTime[{1}]", Thread.CurrentThread.GetHashCode(), DateTime.Now.ToString());
        }

        public PoolSocketThreads(ManualResetEvent evtStop,
                                 Functor func)
        {
            eventStop = evtStop;
            eventHandleTask = new AutoResetEvent(false);
            DoReadSocket = func;
        }

        public void Init(int size)
        {
            Util.BTrace(Util.WARN1, ">> PoolSocketThreads.Init -> size[{0}]", size);

            lock (this)
            {
                if (0 < size && size <= MAX_THREADS_IN_POOL)
                {
                    if (0 == count)
                    {
                        arrThreads = new Thread[size];
                        for (int i = 0; i < size; ++i)
                        {
                            arrThreads[i] = new Thread(new ThreadStart(HandleTask));
                            arrThreads[i].Name = "Worker" + i.ToString();
                            arrThreads[i].Start();
                        }

                        listSockets = new ArrayList(32);
                        count = size;
                    }
                    else
                        throw new ArgumentException("PoolSocketThreads.Init -> pool already initialized");
                }
                else
                    throw new ArgumentException("PoolSocketThreads.Init -> illegal argument");
            }

            Util.BTrace(Util.WARN1, "<< PoolSocketThreads.Init -> size[{0}]", size);
        }

        public void Stop()
        {
            Util.BTrace(Util.WARN1, ">> PoolSocketThreads.Stop ...");

            eventStop.Set();       // signal all threads to get out 

            lock (listSockets)
            {
                int size = listSockets.Count;

                if (0 != size)
                    for (int i = 0; i < size; ++i)
                        ((Socket)listSockets[i]).Close();
            }

            Util.BTrace(Util.WARN1, "<< PoolSocketThreads.Stop ...");
        }

        // number of threads in the pool
        public int Count
        {
            get { return count; }
        }

        // number of threads running 
        public long Running
        {
           get { return Interlocked.Read( ref running ); }
        }

        // number of sockets/requests in the list
        public int Requests
        {
            get
            {
                lock (listSockets)
                {
                    return listSockets.Count;
                }
            }
        }

        public void AddSocket(Socket s)
        {
            Util.BTrace(Util.INF1, ">> PoolSocketThreads.AddSocket -> socket[{0}]", s.GetHashCode());

            lock (listSockets)
            {
                listSockets.Add(s);
                if (listSockets.Count >= MAX_THREADS_IN_POOL)
                   Util.BTrace(Util.WARN2, "-- PoolSocketThreads.AddSocket -> more than {1} sockets, socket[{0}]", s.GetHashCode(), MAX_THREADS_IN_POOL);

                eventHandleTask.Set();
            }

            Util.BTrace(Util.INF1, "<< PoolSocketThreads.AddSocket -> socket[{0}]", s.GetHashCode());
        }

        public Socket GetSocket()
        {
            Util.BTrace(Util.INF1, ">> PoolSocketThreads.GetSocket ...");

            Socket s = null;
            lock (listSockets)
            {
                if (0 != listSockets.Count)
                {
                    s = (Socket)listSockets[0];
                    listSockets.RemoveAt(0);
                }
                else
                    throw new ArgumentException("PoolSocketThreads.GetSocket empty list");
            }
            Util.BTrace(Util.INF1, "<< PoolSocketThreads.GetSocket -> socket[{0}]", s.GetHashCode());
            return s;

        }

        // print out the threads, the sockets and when they took over that socket
        public void Dump()
        {
            Util.BTrace(Util.INF1, "-- PoolSocketThreads.Dump -> running threads[{0}]", running );

            /*
                     for (int i = 0 ; i < count ; ++i)
                        if ( null != arrSockets[i] )
                        {
                           Util.BTrace(Util.INF1, "-- PoolSocketThreads.Dump -> running threads[{0}] pair<{1}, {2}>", i, arrSockets[i], arrThreads[i]) ;
                        }
            */
        }
    }
}