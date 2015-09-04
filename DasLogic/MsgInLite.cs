using System;
using System.Collections.Generic;
using System.Text;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.CLS.Def;            // CMFOut

namespace VLF.DAS.Logic
{
   /** \class  MsgInLite
    *  \brief  this class is only to operate insertion/deletion in vlfMsgIn/vlfMsgOut
    *          and as a helper for bulk operations
    */
   public class MsgInLite : Das
   {
      private VLF.DAS.DB.MsgInLite _msgIn = null;

      public MsgInLite(string connectionString)
         : base(connectionString)
      {
         _msgIn = new VLF.DAS.DB.MsgInLite(sqlExec);

      }

      public MsgInLite(string connectionString, bool forHistory)
         : base(connectionString)
      {
         _msgIn = new VLF.DAS.DB.MsgInLite(forHistory, sqlExec);

      }
      public new void Dispose()
      {
         base.Dispose();
      }

      /** \fn     public void AddMsgIn(CMFIn cmfIn)
       *  \brief  this is the 
       */
      public void AddMsgIn(CMFIn cmfIn )
      {
         _msgIn.AddMsg(cmfIn);
      }

      public void Add2MsgIn(CMFIn cmfIn)
      {
         _msgIn.AppendMsg(cmfIn, false);
      }

      public void DeleteMsg(CMFIn cmfIn)
      {
         if (cmfIn.isDuplicatedMsg)
            _msgIn.DeleteMsgTimeRange(cmfIn);
         else
             _msgIn.DeleteMsg(cmfIn);
      }
      public void DeleteMsg_TmpSLS(CMFIn cmfIn)
      {
          if (cmfIn.isDuplicatedMsg)
              _msgIn.DeleteMsgTimeRange_TmpSLS(cmfIn);
          else
              _msgIn.DeleteMsg_TmpSLS(cmfIn);
      }

      /// <summary>
      ///      TO BE IMPLEMENTED
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="applicationId"></param>
      /// <param name="xmlData"></param>
      /// <param name="boxId"></param>
      /// <param name="dtOriginDateTime"></param>
      /// <returns></returns>
      public int DecodeThirdPartyMessage(int userId, int applicationId, string xmlData,
                                          out int boxId, out DateTime dtOriginDateTime)
      {
         boxId = 0;
         dtOriginDateTime = DateTime.Now.ToUniversalTime();
         return 0;
      }
      /**
       *  \brief  used to save messages received from MDT
       */  
      public void AppendThirdPartyMessage(DateTime dtOriginated, int boxID, double lat, double lon, int seqNum,
                                          short boxProtocolId, string customProperties)
      {
         _msgIn.AppendThirdPartyMessage(dtOriginated, boxID, lat, lon, seqNum, boxProtocolId, customProperties);
      }

      /** \brief  the message is only to add simulated messages to the history through a web service call
       *  \ret    0 - for success
       *          1 - for duplicate message
       *          2 - box is not present in the system
       *          3 - invalid parameters 
       */ 
      public int AppendFakeGPSMessage(DateTime dtOriginated, string VINNumber, double lat, double lon)
      {
         return _msgIn.AppendFakeGPSMessage(dtOriginated, VINNumber, lat, lon);
      }
      /** \fn     public DataSet RunDynamicSQL(string query, string condition)
       *  \brief  run a normal query with a condition
       */ 
      public DataSet RunDynamicSQL(string query, string condition)
      {
         return _msgIn.RunDynamicSQL(query, condition);
      }

      public DataSet RunDynamicSQL(string query, string condition, int timeout)
      {
         return _msgIn.RunDynamicSQL(query, condition, timeout);
      }
      
      /**
       * \brief   updates tha address in vlfMsgInHst
       */
      public void UpdateAddress(int boxId, DateTime origin, string address, string nearestLandmark)
      {
         _msgIn.UpdateStreetAddressInHistory(boxId, origin, address, 5000, nearestLandmark);
      }

      /**
       * \brief   updates tha address in vlfMsgInHst
       */
      public void UpdateAddress(int boxId, DateTime origin, DateTime received, string address, string nearestLandmark)
      {
         _msgIn.UpdateStreetAddressInHistory(boxId, origin, received, address, 5000, nearestLandmark);
      }


      /**
     * \brief   update custom property in vlfMsgInHst
     */
      public void UpdateCustomPropInHistory(int boxId, DateTime origin,Int16 boxMsgInTypeId,  string customPropertyAddons)
      {
         _msgIn.UpdateCustomPropInHistory(boxId, origin, boxMsgInTypeId,customPropertyAddons, 5000);
      }


   }
}
