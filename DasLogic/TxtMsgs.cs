using System;
using System.Collections.Generic;
using System.Text;
using VLF.CLS.Def;
using System.Data; 

namespace VLF.DAS.Logic
{
   public class TxtMsgs :Das
   {
      private DB.TxtMsgs textMsg = null;
			
		#region General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
      public TxtMsgs(string connectionString) : base(connectionString)
		{
			textMsg = new DB.TxtMsgs(sqlExec);
		}
		/// <summary>
		/// Destructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

      /// <summary>
      ///       add latitude/longitude info for every message
      /// </summary>
      /// <param name="cmfIn"></param>
      /// <param name="txtMsgType"></param>
      /// <param name="msgDirection"></param>
      /// <param name="userId"></param>
      /// <param name="strAck"></param>
      /// <returns></returns>
      public int AddTextMsg(CMFIn cmfIn, short txtMsgType)
      {
         if (null != cmfIn)
         {
            return textMsg.AddMsg(cmfIn.boxID,
                                  cmfIn.originatedDateTime,
                                  cmfIn.latitude,
                                  cmfIn.longitude,
                                  txtMsgType,
                                  CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyMessage, cmfIn.customProperties),
                                  (short)VLF.CLS.Def.Enums.TxtMsgDirectionType.In,
                                  VLF.CLS.Def.Const.unassignedIntValue,
                                  VLF.CLS.Def.Const.txtMsgAckNA);
         }
         else
            return -1;
      }

      /// <summary>
      /// Add new text message
      /// </summary>
      /// <exception cref="DASAppDataAlreadyExistsException">Throws DASAppDataAlreadyExistsException information already exist.</exception>
      /// <exception cref="DASException">Throws DASException in all other error cases.</exception>
      /// <returns> current message id or -1 in case of error</returns>
      public int AddTextMsg(int boxId, DateTime msgDateTime, short txtMsgTypeId, string msgBody, short msgDirection, int userId, string ack)
      {
         return textMsg.AddMsg(boxId, msgDateTime, txtMsgTypeId, msgBody, msgDirection, userId, ack);
      }

       /// <summary>
      /// Get MDT Manufacturer and Model
       /// </summary>
       /// <param name="MdtTypeId"></param>
       /// <returns></returns>
       public DataSet GetMDTInfoByTypeId(int MdtTypeId)
       {
           return textMsg.GetMDTInfoByTypeId(MdtTypeId);  
       }

       // Changes for TimeZone Feature start
       /// <summary>
       /// GetMessagesShortInfoCheckSum
       /// </summary>
       /// <param name="from"></param>
       /// <param name="to"></param>
       /// <param name="requestUserId"></param>
       /// <returns></returns>
       public string GetMsgsShortInfoCheckSum_NewTZ(DateTime from, DateTime to, int requestUserId)
       {
           return textMsg.GetMessagesShortInfoCheckSum_NewTZ(from, to, requestUserId);
       }
       // Changes for TimeZone Feature end

       /// <summary>
       /// GetMessagesShortInfoCheckSum
       /// </summary>
       /// <param name="from"></param>
       /// <param name="to"></param>
       /// <param name="requestUserId"></param>
       /// <returns></returns>
       public string GetMsgsShortInfoCheckSum(DateTime from, DateTime to, int requestUserId)
       {
           return textMsg.GetMessagesShortInfoCheckSum(from, to, requestUserId);
       }
   }
}
