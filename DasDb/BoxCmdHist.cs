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
    /// Provides interface for the Outgoing and Incoming Commands
    /// </summary>
    public class BoxCmdHist : TblGenInterfaces
    {
          
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlExec"></param>
        public BoxCmdHist(SQLExecuter sqlExec)
            : base("vlfBoxCmdHist", sqlExec)
        {
        }


        public DataSet GetCmdSend(int userId, int fleetId, string cmdType)
        {
            DataSet sqlDataSet = null;
            try
            {
                //Prepares SQL statement
                string sql =    " Declare @timezone int " +
                    " Select @timezone=dbo.GetTimeZoneDayLight("+userId+") " +
            " SELECT dbo.vlfBoxCmdHist.BoxId , dbo.vlfVehicleInfo.Description, dbo.vlfBoxCmdOutType.BoxCmdOutTypeName , DATEADD(hour,@timezone,dbo.vlfBoxCmdHist.DateTimeSent ) as DateTimeSent, " +
                   "  DATEADD(hour,@timezone,dbo.vlfBoxCmdHist.DateTimeAck) as DateTimeAck ,dbo.vlfUser.UserName , dbo.vlfBoxCmdHist.CustomProp, dbo.vlfFleetVehicles.FleetId " +
            " FROM   dbo.vlfBoxCmdHist INNER JOIN " +
                   " dbo.vlfBoxCmdOutType ON dbo.vlfBoxCmdHist.BoxCmdOutTypeId = dbo.vlfBoxCmdOutType.BoxCmdOutTypeId INNER JOIN " +
                   " dbo.vlfVehicleAssignment ON dbo.vlfBoxCmdHist.BoxId = dbo.vlfVehicleAssignment.BoxId INNER JOIN " +
                   " dbo.vlfFleetVehicles ON dbo.vlfVehicleAssignment.VehicleId = dbo.vlfFleetVehicles.VehicleId INNER JOIN " +
                   " dbo.vlfVehicleInfo ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleInfo.VehicleId INNER JOIN " +
                   " dbo.vlfUser ON dbo.vlfBoxCmdHist.UserId = dbo.vlfUser.UserId " +
                   " WHERE     (dbo.vlfFleetVehicles.FleetId =" + fleetId + ") " +
                    " AND     dbo.vlfBoxCmdHist.BoxCmdOutTypeID in (29,59,86,103) " +
                   " AND (vlfBoxCmdOutType.BoxCmdOutTypeName LIKE '%" + cmdType + "%') ";

                sqlDataSet = sqlExec.SQLExecuteDataset(sql);
               /*sqlDataSet.Tables[0].Columns[4].ColumnName = "DateTimeAck";
               sqlDataSet.Tables[0].Columns[4].Caption = "DateTimeAck";*/
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get command history information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get command history information. " + objException.Message);
            }
            return sqlDataSet;
        }
         
        public DataSet GetCmdRec(int userId, int fleetId, int msgTypeId)
        {
            DataSet sqlDataSet = null;
            try
            {
                 //Prepares SQL statement
                string sql =
                      "DECLARE @timezone int " +
                    " Select @timezone=dbo.GetTimeZoneDayLight(" + userId + ")" +
            " SELECT   dbo.vlfBoxExtraInfo.BoxId, dbo.vlfVehicleInfo.Description,  DATEADD(hour,@timezone,dbo.vlfBoxExtraInfo.Timestamp) as Timestamp,dbo.vlfBoxMsgInType.BoxMsgInTypeName,  " +
                     " dbo.vlfBoxExtraInfo.CustomProp, dbo.vlfFleetVehicles.FleetId "+
            "FROM      dbo.vlfFleetVehicles INNER JOIN "+
                     " dbo.vlfVehicleAssignment ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleAssignment.VehicleId INNER JOIN "+
                     " dbo.vlfVehicleInfo ON dbo.vlfFleetVehicles.VehicleId = dbo.vlfVehicleInfo.VehicleId INNER JOIN "+
                     " dbo.vlfBoxExtraInfo ON dbo.vlfVehicleAssignment.BoxId = dbo.vlfBoxExtraInfo.BoxId INNER JOIN "+
                     " dbo.vlfBoxMsgInType ON dbo.vlfBoxExtraInfo.MsgTypeId = dbo.vlfBoxMsgInType.BoxMsgInTypeId "+
            "WHERE     (dbo.vlfFleetVehicles.FleetId =" + fleetId + ") ";
                
                if (msgTypeId != -1)
                    sql += " AND (dbo.vlfBoxExtraInfo.MsgTypeId= " + msgTypeId + ") ";
                
               sqlDataSet = sqlExec.SQLExecuteDataset(sql);
                
            }
            catch (SqlException objException)
            {
                Util.ProcessDbException("Unable to get command history information.", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                throw new DASException("Unable to get command history information. " + objException.Message);
            }
            return sqlDataSet;
        }
    }
}
