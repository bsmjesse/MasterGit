using System;
using System.Collections.Generic;
using System.Text;
using System.Data;			   // for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.DAS.DB;
using VLF.CLS.Def;            // CMFOut

namespace VLF.DAS.Logic
{
   /** \class  MsgOutLite
    *  \brief  this class is only to operate insertion/deletion in vlfMsgIn/vlfMsgOut
    *          and as a helper for bulk operations
    */ 
   public class MsgOutLite: Das
   {
      private VLF.DAS.DB.MsgOutLite _msgOut = null;

      /// <summary>
      ///      used to write in vlfMsgOut
      /// </summary>
      /// <param name="connectionString"></param>
      public MsgOutLite(string connectionString)
         : base(connectionString)
      {
         _msgOut = new VLF.DAS.DB.MsgOutLite(sqlExec);

      }

      public new void Dispose()
      {
         base.Dispose();
      }

      /** \fn     public void AddMsgOut(CMFOut cMFOut, SByte priority, short dclId, short aslId, int userId)
       *  \brief  this is the 
       */ 
      public void AddMsgOut(CMFOut cMFOut, SByte priority, short dclId, short aslId, int userId)
      {
         _msgOut.AddMsg(cMFOut, priority, dclId, aslId, userId);
      }
      
   }
}
