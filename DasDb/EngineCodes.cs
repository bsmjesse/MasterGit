using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using System.Collections;	// for SortedList
using VLF.ERR;
using VLF.CLS;


namespace VLF.DAS.DB
{

   /// <summary>
   ///      
   /// </summary>
   public class J1708Codes : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public J1708Codes(SQLExecuter sqlExec)
         : base("vlfJ1708History", sqlExec)
      {
      }

      public void AddJ1708Code(int boxId, DateTime originDateTime, string code1, string code2, string code3)
      {
         string prefixMsg = string.Format("Unable to add J1708Code: bid={0} odt={1} MID={2} PID={3} FMI={4}",
                              boxId, originDateTime, code1, code2, code3);
         if (false == string.IsNullOrEmpty(code1) ||
             false == string.IsNullOrEmpty(code2) ||
             false == string.IsNullOrEmpty(code3))
         {
            try
            {
               string sql = "INSERT INTO " + tableName + "( " +
                           "boxId" +
                           ",originDateTime" +
                           ",code1" +
                           ",code2" +
                           ",code3) VALUES (@boxId, @originDateTime, @code1, @code2, @code3)";

               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
               sqlExec.AddCommandParam("@originDateTime", SqlDbType.DateTime, originDateTime);
               if (code1 == null)
                  sqlExec.AddCommandParam("@code1", SqlDbType.VarChar, System.DBNull.Value);
               else
                  sqlExec.AddCommandParam("@code1", SqlDbType.VarChar, code1);

               if (code2 == null)
                  sqlExec.AddCommandParam("@code2", SqlDbType.VarChar, System.DBNull.Value);
               else
                  sqlExec.AddCommandParam("@code2", SqlDbType.VarChar, code2);

               if (code3 == null)
                  sqlExec.AddCommandParam("@code3", SqlDbType.VarChar, System.DBNull.Value);
               else
                  sqlExec.AddCommandParam("@code3", SqlDbType.VarChar, code3);


               sqlExec.SQLExecuteNonQuery(sql);
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
         }
      }

      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      public DataSet GetJ1708Codes(int userId, int boxId, DateTime from, DateTime to, string lang)
      {
         string prefixMsg = string.Format("Unable to get J1708 codes bid={0} from={1} to={2} lang={3}",
                              boxId, from, to, lang);

         DataSet sqlDataSet = null;

         try
         {
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

            sqlDataSet = sqlExec.SPExecuteDataset("GetJ1708Codes");
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

         return sqlDataSet;
      }



      // Changes for TimeZone Feature start
      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      public DataSet GetJ1708CodesVehicleFleet_NewTZ(int userId, int vehicleId, int fleetId, DateTime from, DateTime to, string lang)
      {
          string prefixMsg = string.Format("Unable to get J1708 codes vehicleId={0} from={1} to={2} lang={3}",
                              vehicleId, from, to, lang);

          DataSet sqlDataSet = null;

          try
          {
              // Set SQL command
              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
              sqlExec.AddCommandParam("@vehicleId", SqlDbType.Int, vehicleId);
              sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
              sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, from);
              sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, to);
              sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

              sqlDataSet = sqlExec.SPExecuteDataset("GetCodesByVehicleFleet_NewTimeZone");
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

          return sqlDataSet;
      }
      // Changes for TimeZone Feature end


      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      public DataSet GetJ1708CodesVehicleFleet(int userId, int vehicleId, int fleetId, DateTime from, DateTime to, string lang)
      {
          string prefixMsg = string.Format("Unable to get J1708 codes vehicleId={0} from={1} to={2} lang={3}",
                              vehicleId, from, to, lang);

          DataSet sqlDataSet = null;

          try
          {
              // Set SQL command
              sqlExec.ClearCommandParameters();
              sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
              sqlExec.AddCommandParam("@vehicleId", SqlDbType.Int, vehicleId);
              sqlExec.AddCommandParam("@fleetId", SqlDbType.Int, fleetId);
              sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, from);
              sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, to);
              sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

              sqlDataSet = sqlExec.SPExecuteDataset("GetCodesByVehicleFleet");
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

          return sqlDataSet;
      }

      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///         MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      /// <comment>
      ///      uses SP GetJ1708CodesByBox
      /// </comment>
      public DataSet GetJ1708CodesByUser(int userId, DateTime from, DateTime to, string lang)
      {
         string prefixMsg = string.Format("Unable to get J1708 codes userid={0} from={1} to={2} lang={3}",
                              userId, from, to, lang);

         DataSet sqlDataSet = null;

         try
         {
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

            sqlDataSet = sqlExec.SPExecuteDataset("GetJ1708CodesByUser");
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

         return sqlDataSet;
      }

      public string GetTranslation(string mid, string pid, string fmi, string lang)
      {
         string prefixMsg = string.Format("Unable to get J1708 transalation  MID={0} PID={1} FMI={2} lang={3}",
                             mid, pid, fmi, lang);

         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);
            sqlExec.AddCommandParam("@PID", SqlDbType.VarChar, pid);
            sqlExec.AddCommandParam("@MID", SqlDbType.VarChar, mid);
            sqlExec.AddCommandParam("@FMI", SqlDbType.VarChar, fmi);
            object obj = sqlExec.SPExecuteScalar("GetJ1708Translation");
            return ((obj != System.DBNull.Value) ? obj.ToString() : null) ;
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

         return null;
      }
   }


   /// <summary>
   ///      
   /// </summary>
   public class OBD2Codes : TblOneIntPrimaryKey
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public OBD2Codes(SQLExecuter sqlExec)
         : base("vlfOBD2History", sqlExec)
      {
      }

      public void AddOBD2Code(int boxId, DateTime originDateTime, string code)
      {
         string prefixMsg = string.Format("Unable to add OBD2 Code: bid={0} odt={1} ",
                             boxId, originDateTime, code);

         if (!string.IsNullOrEmpty(code))
         {
            try
            {
               string sql = "INSERT INTO " + tableName + "(boxId,originDateTime,code) VALUES (@boxId, @originDateTime, @code)";

               sqlExec.ClearCommandParameters();
               sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
               sqlExec.AddCommandParam("@originDateTime", SqlDbType.DateTime, originDateTime);
               sqlExec.AddCommandParam("@code", SqlDbType.VarChar, code);

               sqlExec.SQLExecuteNonQuery(sql);
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
         }
      }

      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      public DataSet GetOBD2Codes(int userId, int boxId, DateTime from, DateTime to, string lang)
      {
         string prefixMsg = string.Format("Unable to get OBD2 codes bid={0} from={1} to={2} lang={3}", 
                              boxId, from, to, lang);

         DataSet sqlDataSet = null;

         try
         {            
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@boxId", SqlDbType.Int, boxId);
            sqlExec.AddCommandParam("@dtFrom", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@dtTo", SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

            sqlDataSet = sqlExec.SPExecuteDataset("GetOBD2CodesByBox");
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

         return sqlDataSet;
      }

      /// <summary>
      ///      it returns
      ///      LP, VehicleDescription, Vin#, 
      ///          MaxDateTime, BoxId, Counter, Code1, Code2, Code3, Translation
      /// </summary>
      public DataSet GetOBD2CodesByUser(int userId, DateTime from, DateTime to, string lang)
      {
         string prefixMsg = string.Format("Unable to get OBD2 codes userId={0} from={1} to={2} lang={3}",
                             userId, from, to, lang);

         DataSet sqlDataSet = null;

         try
         {
            // Set SQL command
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@userId", SqlDbType.Int, userId);
            sqlExec.AddCommandParam("@fromDate", SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@toDate", SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);

            sqlDataSet = sqlExec.SPExecuteDataset("GetOBD2CodesByUser");
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

         return sqlDataSet;
      }

      public string GetOBD2Translation(string code, string lang)
      {
         string prefixMsg = string.Format("Unable to get OBD2 transalation  CODE={0} lang={1}", code, lang);

         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@lang", SqlDbType.VarChar, lang);
            sqlExec.AddCommandParam("@code", SqlDbType.VarChar, code);
            object obj = sqlExec.SPExecuteScalar("GetOBD2Translation");
            return ((obj != System.DBNull.Value) ? obj.ToString() : null);
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

         return null;
      }
   }
   
}
