using System;
using System.Collections;
using System.Text;
using System.Threading;
using VLF.CLS.Interfaces;


namespace VLF.CLS
{
   /** \class     QueueServer
    *  \brief     this server is a pool of threads doing some jobs
    *             you add handlers to it based on the type of packets in the queue
    */
   // what you are doing with the outgoing messages
//   public delegate bool OnMessageOut(object obj);

   // what are you doing with the incoming messages
//   public delegate bool OnMessageIn(byte[] arr);

   public class QueueServer
   {
      ulong reqInPackets = 0;
      ulong reqOutPackets = 0;
      double reqInPerSec = .0;
      double reqOutPerSec = .0;

      ManagedThreadPool2 _threadPool = null;
      Hashtable _filters = null;
      Queue _request = null;

      public QueueServer()
      {
         //         LOG log = new LOG(getClass()); 
         _threadPool = new ManagedThreadPool2();
         _filters = new Hashtable();

         if (null == _threadPool || null == _filters)
            throw new ApplicationException("Queueserver (ctor) -> not enough memory ");
      }

      public bool Init()
      {
         return true;
      }

      public void Close()
      {
         _threadPool.Reset();
      }

      public void Add(object obj)
      {
         lock (_filters)
         {
            // 
            WaitCallback callback = (WaitCallback)_filters[obj.GetType()];
            if (null != callback)
               _threadPool.QueueUserWorkItem(callback);
            else
               Util.BTrace(Util.WARN0, "QueueServer.Add -> no VALUE for KEY[{0}]", obj.ToString());
         }
      }

      public void Delete(object obj)
      {
         lock (_filters)
         {

         }
      }

      public void Clear()
      {
         _threadPool.Clear();
      }

      // add filters for the incoming messages
      public void AddFilter(System.Type type, OnDASMsgOutEventHandler fnMsgOut)
      {
         lock (_filters)
         {
            _filters.Add(type, fnMsgOut);
         }
      }

      // add filters for the outgoing messages
      public void AddFilter(System.Type type, OnChannelDataEvent  fnMsgIn)
      {
         lock (_filters)
         {
            _filters.Add(type, fnMsgIn);
         }
      }


   }
}
