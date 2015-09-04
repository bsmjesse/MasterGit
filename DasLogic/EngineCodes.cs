using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for SortedList
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.Logic
{
   public class EngineCodes : Das
	{
      public enum EngineCodeType
      {
         Unknown = 0,
         OBD2    = 1,
         J1708   = 2
      }

      private DB.J1708Codes _j1708Codes = null;
      private DB.OBD2Codes  _obd2Codes  = null;
			
		#region General Interfaces

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
      public EngineCodes(string connectionString) : base(connectionString)
		{
         _j1708Codes = new DB.J1708Codes(sqlExec);
         _obd2Codes = new DB.OBD2Codes(sqlExec);
      }

		/// <summary>
		/// Distructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion


      /// <summary>
      ///      the 
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="originDateTime"></param>
      /// <param name="type"></param>
      /// <param name="code1"></param>
      /// <param name="code2"></param>
      /// <param name="code3"></param>
      public void AddEngineCode(int boxId, DateTime originDateTime, EngineCodeType type, 
                                string code1, string code2, string code3)
      {
         Util.BTrace(Util.INF0, "EngineCodes.AddEngineCode -> bid={0} odt={1} EngineCodeType={2} [{3}, {4}, {5}]",
            boxId, originDateTime, type, code1, code2, code3);

         if (type == EngineCodeType.J1708)
            _j1708Codes.AddJ1708Code(boxId, originDateTime, code1, code2, code3);
         else if (type == EngineCodeType.OBD2)
            _obd2Codes.AddOBD2Code(boxId, originDateTime, code1);
         else // code unknown
            Util.BTrace(Util.INF0, "EngineCodes.AddEngineCode -> bid={0} odt={1} EngineCodeType={2}",
               boxId, originDateTime, type);
      }


      /// <summary>
      ///      it returns
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      /// <param name="boxId"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <returns></returns>
      public DataSet GetEngineCodes(int userId, int boxId, DateTime from, DateTime to, string lang)
      {
         DataSet ds1 = _j1708Codes.GetJ1708Codes(userId, boxId, from, to, lang);
         if (Util.IsDataSetValid(ds1))
         {
            // add a column
            ds1.Tables[0].Columns.Add("CodeType", Type.GetType("System.String"));
            foreach(DataRow dr in ds1.Tables[0].Rows)
               dr["CodeType"] = "J1708" ;
         }

         DataSet ds2 = _obd2Codes.GetOBD2Codes(userId, boxId, from, to, lang);
         if (Util.IsDataSetValid(ds2))
         {
            ds1.Tables[0].Columns.Add("CodeType", Type.GetType("System.String"));
            foreach (DataRow dr in ds1.Tables[0].Rows)
               dr["CodeType"] = "OBD2";

            if (Util.IsDataSetValid(ds1))
            {
               ds1.Merge(ds2);
               return ds1;
            }

            return ds2;
         }

         if (Util.IsDataSetValid(ds1))
            return ds1;

         return null;

            
      }

      /// <summary>
      ///      it returns the whole info containing
      ///         OriginDateTime, BoxId, Code1, Code2, Code3, Translation, 
      ///            Address, latitude, longitude, speed, heading, sensorMask (coming from the same record in vlfMagInHst)
      /// </summary>
      /// <comment>
      ///      this could be generated from History page by filtering for SendDTC message type
      /// </comment>
      /// <param name="userId"></param>
      /// <param name="boxId"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="lang"></param>
      /// <returns></returns>
      public DataSet GetEngineCodesWithInfo(int userId, int boxId, DateTime from, DateTime to, string lang)
      {
         return null;
      }

      /// <summary>
      ///      it returns
      ///         MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation, CodeType (OBD2/J1708)
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="lang"></param>
      /// <returns></returns>
      public DataSet GetEngineCodesByUser(int userId, DateTime from, DateTime to, string lang)
      {
         DataSet ds1 = _j1708Codes.GetJ1708CodesByUser(userId, from, to, lang);
         if (Util.IsDataSetValid(ds1))
         {
            // add a column
            ds1.Tables[0].Columns.Add("CodeType", Type.GetType("System.String"));
            foreach (DataRow dr in ds1.Tables[0].Rows)
               dr["CodeType"] = "J1708";
         }

         DataSet ds2 = _obd2Codes.GetOBD2CodesByUser(userId, from, to, lang);
         if (Util.IsDataSetValid(ds2))
         {
            ds1.Tables[0].Columns.Add("CodeType", Type.GetType("System.String"));
            foreach (DataRow dr in ds1.Tables[0].Rows)
               dr["CodeType"] = "OBD2";

            if (Util.IsDataSetValid(ds1))
            {
               ds1.Merge(ds2);
               return ds1;
            }

            return ds2;
         }

         if (Util.IsDataSetValid(ds1))
            return ds1;

         return null;
      }
   }
}
