using System;
using System.Data ;			// for DataSet
using System.Data.SqlClient ;	//for SqlException
using VLF.ERR;
using VLF.CLS;
using System.Text ;


namespace VLF.DAS.DB
{

   

   public class Peripheral : TblGenInterfaces
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
		public Peripheral(SQLExecuter sqlExec) : base ("vlfPeripheralBoxAssigment",sqlExec)
		{
		}

      /// <summary>
      ///      it executes two operation
      ///      based on the peripheralType it inserts a record in
      ///       . for MDT                 in vlfMdt
      ///       . for Garmin              in vlfGarmin
      /// 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="peripheralType"></param>
      /// <param name="peripheralId"></param>
      /// <param name="MajorVersion"></param>
      /// <param name="MinorVersion"></param>
      /// <returns>
      ///      the peripheralid from vlfMdt or vlfGarmin which is the same as PeripheralId from vlfPeripheralBoxAssigment
      /// </returns>
      public int PeripheralAssignment(int boxId, DateTime when, int peripheralType, string peripheralId, int MajorVersion, int MinorVersion )
      {
         string prefixMsg = "Unable to assign peripherals " + peripheralType + " " + peripheralId;
         int ret = -1;

        
            try
            {
               string sql = "PeripheralAssignment";
               SqlParameter[] sqlParams = new SqlParameter[6];
               sqlParams[0] = new SqlParameter("@boxId", boxId);
               sqlParams[1] = new SqlParameter("@dtWhen", when);
               sqlParams[2] = new SqlParameter("@peripheralType", peripheralType);
               sqlParams[3] = new SqlParameter("@peripheralId", peripheralId);
               sqlParams[4] = new SqlParameter("@MajorVersion", MajorVersion);
               sqlParams[5] = new SqlParameter("@MinorVersion", MinorVersion);

               //Executes SQL statement
               ret = sqlExec.SPExecuteNonQuery(sql, sqlParams);
            }
            catch (SqlException objException)
            {
               Util.ProcessDbException(prefixMsg, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
               throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
               throw new DASException(prefixMsg + " " + objException.Message);
            }

         return ret;
      }
 
   }

   public class PeripheralHistory : TblGenInterfaces
   {
      /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sqlExec"></param>
      public PeripheralHistory(SQLExecuter sqlExec)
         : base("vlfPeripheralBoxAssigmentHist", sqlExec)
		{

		}
   }
}
