using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VLF.ERR;
using VLF.CLS;

namespace VLF.DAS.DB
{
   /// <summary>
   /// Manages report schedules
   /// 
   /// \brief Add, delete and get records from the table vlfReportSchedules
   /// </summary>
   public class ReportScheduler : TblOneIntPrimaryKey, IDisposable
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="sqlExec"></param>
      public ReportScheduler(SQLExecuter sqlExec) : base("vlfReportSchedules", sqlExec)
      {
      }

      /// <summary>
      /// Add new schedule record
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="isfleet"></param>
      /// <param name="fleetid"></param>
      /// <param name="parameters"></param>
      /// <param name="email"></param>
      /// <param name="userid"></param>
      /// <param name="guiid"></param>
      /// <param name="status"></param>
      /// <param name="statusdate"></param>
      /// <param name="frequency"></param>
      /// <param name="freqparam"></param>
      /// <param name="startdate"></param>
      /// <param name="enddate"></param>
      /// <param name="deliveryMethod">Email or file download</param>
      /// <param name="lang">Report language</param>
      public void AddReportSchedule(DateTime from, DateTime to, bool isfleet, int fleetid, string parameters, string email,
         int userid, int guiid, string status, DateTime statusdate, short frequency, short freqparam, DateTime startdate, DateTime enddate,
         short deliveryMethod, string lang)
      {
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@from", System.Data.SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", System.Data.SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@isfleet", System.Data.SqlDbType.Bit, isfleet);
            sqlExec.AddCommandParam("@fleetid", System.Data.SqlDbType.Int, fleetid);
            sqlExec.AddCommandParam("@params", System.Data.SqlDbType.VarChar, parameters);
            sqlExec.AddCommandParam("@email", System.Data.SqlDbType.VarChar, email);
            sqlExec.AddCommandParam("@userid", System.Data.SqlDbType.Int, userid);
            sqlExec.AddCommandParam("@guiid", System.Data.SqlDbType.Int, guiid);
            sqlExec.AddCommandParam("@status", System.Data.SqlDbType.VarChar, status);
            sqlExec.AddCommandParam("@statusdate", System.Data.SqlDbType.DateTime, statusdate);
            sqlExec.AddCommandParam("@frequency", System.Data.SqlDbType.SmallInt, frequency);
            sqlExec.AddCommandParam("@freqparam", System.Data.SqlDbType.SmallInt, freqparam);
            sqlExec.AddCommandParam("@startdate", System.Data.SqlDbType.DateTime, startdate);
            sqlExec.AddCommandParam("@enddate", System.Data.SqlDbType.DateTime, enddate);
            sqlExec.AddCommandParam("@delivery", System.Data.SqlDbType.SmallInt, deliveryMethod);
            sqlExec.AddCommandParam("@lang", System.Data.SqlDbType.VarChar, lang);
            sqlExec.SPExecuteNonQuery("ReportScheduleAdd");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
      }

      /// <summary>
      /// Add new scheduled report, incl. file format
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="isfleet"></param>
      /// <param name="fleetid"></param>
      /// <param name="parameters"></param>
      /// <param name="email"></param>
      /// <param name="userid"></param>
      /// <param name="guiid"></param>
      /// <param name="status"></param>
      /// <param name="statusdate"></param>
      /// <param name="frequency"></param>
      /// <param name="freqparam"></param>
      /// <param name="startdate"></param>
      /// <param name="enddate"></param>
      /// <param name="deliveryMethod">Email or file download</param>
      /// <param name="lang">Report language</param>
      /// <param name="format">Report format (pdf = 1, xls = 2, doc = 3)</param>
      public int AddReportSchedule(DateTime from, DateTime to, bool isfleet, int fleetid, string parameters, string email,
         int userid, int guiid, string status, DateTime statusdate, short frequency, short freqparam, DateTime startdate, DateTime enddate,
         short deliveryMethod, string lang, short format)
      {
         int rowsAdded = 0;
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@from", System.Data.SqlDbType.DateTime, from);
            sqlExec.AddCommandParam("@to", System.Data.SqlDbType.DateTime, to);
            sqlExec.AddCommandParam("@isfleet", System.Data.SqlDbType.Bit, isfleet);
            sqlExec.AddCommandParam("@fleetid", System.Data.SqlDbType.Int, fleetid);
            sqlExec.AddCommandParam("@params", System.Data.SqlDbType.VarChar, parameters);
            sqlExec.AddCommandParam("@email", System.Data.SqlDbType.VarChar, email);
            sqlExec.AddCommandParam("@userid", System.Data.SqlDbType.Int, userid);
            sqlExec.AddCommandParam("@guiid", System.Data.SqlDbType.Int, guiid);
            sqlExec.AddCommandParam("@status", System.Data.SqlDbType.VarChar, status);
            sqlExec.AddCommandParam("@statusdate", System.Data.SqlDbType.DateTime, statusdate);
            sqlExec.AddCommandParam("@frequency", System.Data.SqlDbType.SmallInt, frequency);
            sqlExec.AddCommandParam("@freqparam", System.Data.SqlDbType.SmallInt, freqparam);
            sqlExec.AddCommandParam("@startdate", System.Data.SqlDbType.DateTime, startdate);
            sqlExec.AddCommandParam("@enddate", System.Data.SqlDbType.DateTime, enddate);
            sqlExec.AddCommandParam("@delivery", System.Data.SqlDbType.SmallInt, deliveryMethod);
            sqlExec.AddCommandParam("@lang", System.Data.SqlDbType.VarChar, lang);
            sqlExec.AddCommandParam("@format", System.Data.SqlDbType.SmallInt, format);

            rowsAdded = sqlExec.SPExecuteNonQuery("ReportScheduleAddFmt");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
         return rowsAdded;
      }

      /// <summary>
      /// Delete a record
      /// </summary>
      /// <param name="reportid"></param>
      /// <param name="userid"></param>
      public int DeleteReportSchedule(int reportid, int userid)
      {
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@reportid", System.Data.SqlDbType.Int, reportid);
            sqlExec.AddCommandParam("@userid", System.Data.SqlDbType.Int, userid);
            return sqlExec.SPExecuteNonQuery("ReportScheduleDelete");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
      }

      /// <summary>
      /// Get all active report schedules
      /// </summary>
      /// <param name="userid"></param>
      public DataSet GetActiveReportSchedules()
      {
         DataSet ds = null;
         try
         {
            sqlExec.ClearCommandParameters();
            ds = sqlExec.SPExecuteDataset("ReportSchedulesGetActive");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
         return ds;
      }

      /// <summary>
      /// Get scheduled reports by user id
      /// </summary>
      /// <param name="userid"></param>
      public DataSet GetScheduledReportsByUserID(int userid)
      {
         DataSet ds = null;

         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@UserId", System.Data.SqlDbType.Int, userid);
            ds = sqlExec.SPExecuteDataset("ReportSchedulesGetByUserID");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }

         return ds;
      }


      /// <summary>
      /// Delete a scheduled report by report id
      /// </summary>
      /// <param name="reportid"></param>
      public void DeleteScheduledReportByReportID(int reportid)
      {
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@ReportId", System.Data.SqlDbType.Int, reportid);
            sqlExec.SQLExecuteNonQuery("DELETE FROM vlfReportSchedules WHERE [ReportID] = @ReportId");
         }

         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }

         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }

         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }

         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
      }

      /// <summary>
      /// Get report files for download
      /// </summary>
      /// <param name="userid"></param>
      /// <returns></returns>
      public DataSet GetReportFilesByUserId(int userid)
      {
         DataSet ds = null;

         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@UserId", System.Data.SqlDbType.Int, userid);
            ds = sqlExec.SPExecuteDataset("ReportScheduleGetFiles");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }

         return ds;
      }

      /// <summary>
      /// Get report files for download
      /// </summary>
      /// <param name="userid"></param>
      /// <returns></returns>
      public DataSet GetReportFilesByReportId(int reportId)
      {
         DataSet ds = null;

         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@reportid", System.Data.SqlDbType.Int, reportId);
            ds = sqlExec.SPExecuteDataset("ReportScheduleGetReportFiles");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }

         return ds;
      }

      /// <summary>
      /// Delete a record from report files table
      /// </summary>
      /// <param name="rowid">Row Id</param>
      public int DeleteReportFile(int rowid)
      {
         try
         {
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@rowid", System.Data.SqlDbType.Int, rowid);
            return sqlExec.SQLExecuteNonQuery("DELETE FROM vlfReportFiles WHERE RowID = @rowid");
         }
         catch (DASAppException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASDbConnectionClosed e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (DASException e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Das, e.Message);
            throw e;
         }
         catch (Exception e)
         {
            Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, e.Message);
            throw e;
         }
      }
 
      #region IDisposable Members

      void IDisposable.Dispose()
      {
         sqlExec.Dispose();
      }

      #endregion
   }
}
