using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Net;                ///< for IPEndPoint
using System.Net.Sockets;        ///< for PktFilter
using System.Diagnostics;
using VLF.CLS.Def;


namespace VLF.CLS
{
   /** \class  PktFilter
    *  \brief  this is a class which is trying to stay connected to a specific IP address, read from a configuration 
    *          file and PROCESS/HANDLES a message and place it in a PIPE/QUEUE for further processings
    *  \comment is in charge of sending keep_alive packets to SLSes 
    *          or when the connection is broken it will try to reconnect to SLS
    * 
    */
   public class PktFilter : ThreadBase
   {
      public delegate bool HasPacket() ;            ///< check the buffer
      public delegate byte[]  HandlePacket() ;      ///< extract and manipulate the packet from buffer
      public delegate void OnReceivedPacket();      ///< handle the result of fnHandlePacket

      /** \struct    SLSStatistics
       *  \brief     sent every CT_HEARTBEAT_INTERVAL 
       */
      struct SLSStatistics
      {
         ulong packetsSent;         ///< how many packets where sent to that SLS
         float packetsPerSecond;    ///< computed by the timer thread
         ulong packetsHandled;       ///< received on every request from the timer thread 
      }


      /** \struct       ChannelConfiguration
       *  \brief        read from an XML file or updated whenever to file it's updated ( by ACM )
       */
      struct ChannelConfiguration
      {
         public short port;            ///< port where you connect to ( this is the SLS port listening )
         public long ipAddress;        ///< address where you connect to ( this is the SLS ip address listening )
         public long minBoxId;         ///< min box id for which you sent to this SLS                                       
         public long maxBoxId;         ///< max box id for which you sent to this SLS                                                                              ///
      }

      static readonly ushort SLS_PORT = 9191;
      static readonly ulong CT_HEARTBEAT_INTERVAL = 30000;     // every 30 seconds
      static readonly byte[] HEARTBEAT_PKT = { 0xAA, 0xBB, 0xCC, 0xDD };

      /** \brief   this packet contains 
            *           - length of the packet
            *           - type of the packet
            *           - ID of the packet filter communicated upstream  
            */
      private byte[] PKT_ID = null;

      private int _dclID = 0;       ///< ID used in PKT_ID
                                    
      private Socket _slsSocket = null;     ///< connectivity with SLS, every packet is sent there
      //      private MCast  _terSocket = null 
      private IPAddress _ipAddress;          ///< ip addresses where the DCL could try to send the received packets
      IPEndPoint _slsEndPoint = null;        ///< IPEndPoint for SLS

      /// a receiver thread is feeding _packetsPerMinute
      private ulong _packetsPerMinute;    ///< this is coming from SLS showing how many packets per minute it can handle                                   
      private ushort _port = 0;

      // ManualResetEvent instances signal completion.
      private static AutoResetEvent _connectDone = new AutoResetEvent(false);
      private static AutoResetEvent _connectFailed = new AutoResetEvent(false);

      private ulong _timeoutInterval;         ///< used for the timer thread
      private Hashtable _lstTimeoutPackets;   ///< this is a sorted array of timeouts for every connection to SLS
      /// whenever the timer thread is sending a new packet it reset the value to MAX_TIMEOUT_VALUE
      /// and whenver it comes again is substracting the period _timeoutInterval
      /// when the ACK is coming back from SLS it flags that entry ( 0xFFFFFFFF ) and let the timer know
      /// the response was received
      //    private Hashtable _slsStats;          ///< this are statistics for every SLS 


      HasPacket            _fnHasPacket = null ;
      HandlePacket         _fnHandlePacket = null ;
      OnReceivedPacket     _fnOnReceivedPacket = null ;



      /// <summary>
      /// 
      /// </summary>
      /// <param name="ipAddress">this a list of potential IP addresses ready to handle the traffic
      /// </param>   
      /// <param name="port">the port is unique, of the server you send packets </param>        
      /// <param name="timeoutInterval"> the timeout interval where you send 
      /// </param>        
      public PktFilter(int id, string ipAddress, ushort port, ulong timeoutInterval)
         : base(ipAddress.ToString(), null)
      {

         _dclID = id;
         PKT_ID = new byte[12];
         BitConverter.GetBytes(12).CopyTo(PKT_ID, 0);
         BitConverter.GetBytes(Const.PKT_EXTERNAL).CopyTo(PKT_ID, 4);
         BitConverter.GetBytes(id).CopyTo(PKT_ID, 8);

         _packetsPerMinute = 0L;
         _timeoutInterval = timeoutInterval;
         _ipAddress = null;
         if (Util.IsIPAddress(ipAddress, out _ipAddress))
         {
            _port = port;
            _slsSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _slsEndPoint = new IPEndPoint(_ipAddress, _port);
            _lstTimeoutPackets = new Hashtable();
         }
         else
            throw new ApplicationException("PktFilter exception -> wrong IP address" + ipAddress);
      }

      /** \fn     public void Initialize(HasPacket hasPacket, 
       *                                 HandlePacket handlePacket, 
       *                                 OnReceivedPacket onReceivedPacket)
       *  \brief  some of the functions coudl be null, if you use the channel only in one way ( sending packets as
       *          I used it for IService
       */ 
      public void Initialize(HasPacket hasPacket, HandlePacket handlePacket, OnReceivedPacket onReceivedPacket)
      {
         Debug.Assert(null != _fnHasPacket && null != _fnHandlePacket && null != _fnOnReceivedPacket);
         _fnHasPacket = hasPacket;
         _fnHandlePacket = handlePacket;
         _fnOnReceivedPacket = onReceivedPacket;         
      }


      private void ChangeConfiguration()
      {
         lock (this)
         {
            // close any connection
            Close();
         }
      }
      public bool Connected
      {
         get
         {
            return (null != _slsSocket ? _slsSocket.Connected : false);
         }
      }
      /// <summary>
      ///      Init will start a resilient connection with the server as long as is not receiving any 
      ///      message to stop the service
      /// </summary>
      public void Init()
      {
         _slsSocket.BeginConnect(_slsEndPoint, new AsyncCallback(EstablishConnection), _slsSocket);
      }


      public void Close()
      {
         Util.BTrace(Util.INF0, ">> PktFilter.Close");

         try
         {
            // stop the working thread
            base.Stop();

            // close the socket
            if (null != _slsSocket)
            {
               _slsSocket.Shutdown(SocketShutdown.Both);
               _slsSocket.Close();
               _slsSocket = null;
            }
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.INF0, "PktFilter.Close -> EXC[{0}]", exc.Message);
         }

         Util.BTrace(Util.INF0, "<< PktFilter.Close");
      }

      /// <summary>
      ///      multiple threads could send data through this socket, that's the reason that 
      ///      the call is protected 
      ///      another idea is to have a queue only for incoming traffic and only one thread 
      ///      serving that queue by sending continuously packets
      /// </summary>
      /// <param name="data"></param>
      public void Send(byte[] data)
      {
         if (null == data || data.Length < 1)
         {
            Util.BTrace(Util.ERR0, " PktFilter.Send -> null arguments ");
            return;
         }

         if (Connected)
         {
            lock (_slsSocket)
            {
               try
               {
                  int size = data.Length;
                  int ret = 0;

                  while (ret < size)
                  {
                     ret += _slsSocket.Send(data);
                  }

               }
               catch (SocketException sExc)
               {
                  Util.BTrace(Util.ERR0, " PktFilter.Send -> SocketException [{0}]", sExc.Message);
                  try
                  {
                     _slsSocket.Shutdown(SocketShutdown.Both);
                     _slsSocket.Disconnect(true);
                  }
                  catch { }
                  finally
                  {
                     _connectFailed.Set();
                  }
               }
               catch (ObjectDisposedException exc)
               {
                  Util.BTrace(Util.ERR0, " PktFilter.Send -> ObjectDisposedException [{0}]", exc.Message);
               }
            }

         }
      }


      private void EstablishConnection(IAsyncResult ar)
      {
         bool bConnected = false;
         try
         {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);

            Util.BTrace(Util.WARN0, "-- PktFilter.EstablishConnection ->Socket connected to {0}",
                client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.
            _connectDone.Set();

            // send the ID of the DCL for SLS
            Send(PKT_ID);
            Thread.Sleep(1000);
            Send(PKT_ID);

            bConnected = true;
         }
         catch (SocketException exc)
         {
            Util.BTrace(Util.WARN0, "-- PktFilter.EstablishConnection -> SocketException[{0}]", exc.Message);
            _connectFailed.Set();
         }
         catch (Exception exc)
         {
            Util.BTrace(Util.WARN0, "-- PktFilter.EstablishConnection -> Exception[{0}]", exc.Message);
         }
         finally
         {
            if (false == bConnected)
               _connectFailed.Set();
         }
      }




      /////////////////////////////////////////////////////////////////////
      //       interfaces for ThreadBase abstract class
      //
      /////////////////////////////////////////////////////////////////////


      public override void Dispose()
      {
         Close();
      }

      public override void Start()
      {
         Util.BTrace(Util.INF1, ">> PktFilter.Start...");

         try
         {
            base.Start();
         }
         catch (Exception ex)
         {
            Util.BTrace(Util.ERR1, "-- PktFilter.Start -> Exception MSG[{0}]", ex.Message);
            Util.DumpStack(ex);
         }

         Util.BTrace(Util.INF1, "<< PktFilter.Start...");
      }

      public override void Stop()
      {
         Util.BTrace(Util.INF1, ">> PktFilter.Stop...");

         base.Stop();

         Util.BTrace(Util.INF1, "<< PktFilter.Stop...");
      }

      /// <summary>
      ///      this thread has a dual function
      ///      - if the socket is connected, read from the sockets and place them in a queue
      ///        else
      ///          try to connect to that ip address
      /// </summary>
      protected override void ThreadProc()
      {

         Util.BTrace(Util.INF1, ">> PktFilter.ThreadProc -> DCLChannel started.");

#if   FAULTY_SERVER
         EventWaitHandle hFaulty = new EventWaitHandle(false, EventResetMode.ManualReset, ownName + "Event");
         EventWaitHandle[] events = new EventWaitHandle[] 
               {
                   eventStopThread, 
                   hFaulty,
               };
         Util.BTrace(Util.INF1, "-- PktFilter.ThreadProc -> STOPEVENT[{0}Event]", ownName);
#else

         WaitHandle[] events = new WaitHandle[] { eventStopThread, _connectDone, _connectFailed }; // , asyncResult.AsyncWaitHandle };

#endif

         int cntBytes = 0;
         int idx = 0, capacity = 512;
         byte[] buffer = new byte[512];

         while (workerThread.ThreadState == System.Threading.ThreadState.Running)
         {
            try
            {
               UpdateHeartBeatTime();
               int iRes = WaitHandle.WaitAny(events, 10, true);
               if (0 == iRes)
               {
                  Util.BTrace(Util.INF1, "-- PktFilter.ThreadProc ->  {0} signalled to stop ", ownName);
                  break;
               }
               else
                  if (Connected)
                  {
                     if (_slsSocket.Poll(10, SelectMode.SelectRead))
                     {
                        // got new data
                        cntBytes = _slsSocket.Receive(buffer, idx, capacity - idx, SocketFlags.None);
                        idx += cntBytes;
/*
                        while (_fnHasPacket())
                           _fnOnReceivedPacket(_fnHandlePacket());
 */ 
                     }

                     continue;
                  }
                  else
                  {
                     if (1 == iRes || 2 == iRes) // you got notification but it's still disconnected, start a new connector
                        _slsSocket.BeginConnect(_slsEndPoint, new AsyncCallback(EstablishConnection), _slsSocket);
                     
                     Thread.Sleep(10000); 
                  }

            }
            catch (ThreadAbortException ex)
            {
               // nothing to do - aborting the thread
               Util.BTrace(Util.ERR1, "-- PktFilter.ThreadProc -> EXCEPTION thread aborted* [{0}]", ex.Message);
            }
            catch (SocketException ex)
            {
               Util.BTrace(Util.ERR1, "-- PktFilter.ThreadProc -> SOCKET_EXCEPTION[{0}]", ex.Message);
               try
               {
                  _slsSocket.Shutdown(SocketShutdown.Both);
                  _slsSocket.Disconnect(true);
               }
               finally
               {
                  _connectFailed.Set();
               }
            }
            catch (Exception ex)
            {
               Util.BTrace(Util.WARN1, "-- PktFilter.ThreadProc -> EXCEPTION [{0}]", ex.Message);
            }

         } // end while 

#if   FAULTY_SERVER
        hFaulty.Close();
#endif

         // here signal the SLSFeeder that this PktFilter is closed 
         //         OnReceivedPacket(_slsSocket);

         Util.BTrace(Util.INF1, "<< PktFilter.ThreadProc -> DCLChannel stopped.");

      }

   }
}
