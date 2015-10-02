using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.IO;
using VLF.CLS.Def;
using VLF.CLS.Interfaces;

namespace SentinelFM.Configuration
{
    /// <summary>
    /// Summary description for frmTimerGeoZone.
    /// </summary>
    public partial class frmTimerGeoZone : SentinelFMBasePage
    {

        
        protected bool reloadMap;
        




        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);




                if ((sn == null) || (sn.UserName == ""))
                {
                    string str = "";
                    str = "top.document.all('TopFrame').cols='0,*';";
                    Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
                    return;
                }

                if (sn.Cmd.BoxId == 0)
                    return;


                int cmdStatus = 0;

                LocationMgr.Location dbl = new LocationMgr.Location();
                

                if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
                    if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
                    {

                        foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                        {
                            if (rowItem["BoxId"].ToString() == sn.Cmd.BoxId.ToString())
                            {

                                if (rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone)
                                    rowItem["Status"] = clsGeoZone.FailedAddPendingGeoZone;

                                if (rowItem["Status"].ToString() == clsGeoZone.DeletePendingGeoZone)
                                    rowItem["Status"] = clsGeoZone.FailedDeletePendingGeoZone;

                                break;
                            }
                        }

                        Response.Write("<script language='javascript'> clearTimeout();  parent.main.location.href='frmVehicleGeoZone.aspx'; </script>");
                        return;
                    }

                sn.Cmd.Status = (CommandStatus)cmdStatus;

                if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout) || (cmdStatus == (int)CommandStatus.Pending))
                {
                    if (sn.Cmd.CommandId != Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs))
                    {
                        int RowsCount =sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count - 1;

                        // Delete all geozone 
                        if (sn.GeoZone.GeozoneId == VLF.CLS.Def.Const.allGeozones)
                        {
                            for (int index = RowsCount; index >= 0; --index)
                            {
                                DataRow rowItem =sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows[index];

                                if (cmdStatus == (int)CommandStatus.Ack)
                                    CutAssignGeozoneToUnassigned(rowItem);
                                else
                                    rowItem["Status"] = clsGeoZone.FailedDeletePendingGeoZone;
                            }
                        }
                        else
                        {
                            for (int index = RowsCount; index >= 0; --index)
                            {
                                DataRow rowItem =sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows[index];

                                if ((rowItem["BoxId"].ToString() == sn.Cmd.BoxId.ToString()) && (sn.GeoZone.GeozoneId == Convert.ToInt32(rowItem["GeozoneId"])))
                                {
                                    if (rowItem["Status"].ToString() == clsGeoZone.AddPendingGeoZone || rowItem["Status"].ToString() == clsGeoZone.FailedAddPendingGeoZone)
                                    {
                                        if (cmdStatus == (int)CommandStatus.Ack)
                                        {
                                            rowItem["Status"] = clsGeoZone.GeoZoneSync;
                                            rowItem["Assigned"] = "true";
                                        }
                                        else
                                        {
                                            rowItem["Status"] = clsGeoZone.FailedAddPendingGeoZone;
                                        }
                                    }

                                    if (rowItem["Status"].ToString() == clsGeoZone.DeletePendingGeoZone || rowItem["Status"].ToString() == clsGeoZone.FailedDeletePendingGeoZone)
                                    {
                                        if (cmdStatus == (int)CommandStatus.Ack)
                                            CutAssignGeozoneToUnassigned(rowItem);
                                        else
                                            rowItem["Status"] = clsGeoZone.FailedDeletePendingGeoZone;

                                    }
                                    break;
                                }
                            }
                        }
                    }
                     else if (sn.Cmd.CommandId == Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.GetGeoZoneIDs) && (cmdStatus != (int)CommandStatus.CommTimeout))
                    {
                        CheckSync();
                    }
                    Response.Write("<script language='javascript'> clearTimeout();  parent.main.location.href='frmVehicleGeoZone.aspx'; </script>");
                    return;
                }
                else
                {
                    Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>");
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                Response.Write("<script language='javascript'> clearTimeout();  parent.main.location.href='frmVehicleGeoZone.aspx'; </script>");
            }
        }

        protected void CutAssignGeozoneToUnassigned(DataRow rowItem)
        {
            //Add Geozone to Unassigned list						
            DataRow dr =sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].NewRow();
            dr["GeozoneId"] = rowItem["GeoZoneId"].ToString();
            dr["GeoZoneName"] = rowItem["GeoZoneName"].ToString();
            dr["SeverityId"] = rowItem["SeverityId"].ToString();
            dr["GeozoneNo"] = rowItem["GeozoneNo"];
            dr["OrganizationId"] = rowItem["OrganizationId"];
            dr["Type"] = rowItem["Type"];
            dr["Description"] = rowItem["Description"];
           sn.GeoZone.DsUnAssVehicleGeoZone.Tables[0].Rows.Add(dr);

            //Delete Geozone from assigned list
           sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Remove(rowItem);
        }
        private void CheckSync()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsGeoZone = new DataSet();
                
                string xml = "";

                //Get Box Geozones
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetLastGeoZoneIDsFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetLastGeoZoneIDsFromHistoryByBoxId(sn.UserID, sn.SecId, sn.Cmd.BoxId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " GetLastGeoZoneIDsFromHistoryByBoxId :" + sn.Cmd.BoxId.ToString() + " failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name)); ;
                    }


                if (xml == "")
                {
                    //delete Geozone from  database
                    int rowAffected = 0;
                    if ((sn.GeoZone.DsVehicleGeoZone != null) && (sn.GeoZone.DsVehicleGeoZone.Tables.Count > 0) &&sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count > 0)
                        foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                        {
                            if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicle(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt16(rowItem["GeozoneId"]), ref rowAffected), false))
                                if (objUtil.ErrCheck(dbv.DeleteGeozoneFromVehicle(sn.UserID, sn.SecId, sn.Cmd.BoxId, Convert.ToInt16(rowItem["GeozoneId"]), ref rowAffected), true))
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Delete Geozone:" + rowItem["GeozoneId"].ToString() + " failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                                    continue;
                                }
                        }

                   sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Clear();
                    return;
                }


                string CustomProp = "";
                strrXML = new StringReader(xml);

                dsGeoZone.ReadXml(strrXML);
                if (dsGeoZone.Tables[0].Rows.Count > 0)
                    CustomProp = dsGeoZone.Tables[0].Rows[0]["CustomProp"].ToString().TrimEnd();

                if ((CustomProp == "") && (sn.GeoZone.DsVehicleGeoZone != null))
                {
                    //Database geozone against box
                    foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                    {
                        rowItem["Status"] = clsGeoZone.DeletePendingGeoZone;
                    }
                    return;
                }
                //Create GeoZoneXML
                string GeoId = "";
                string strXML = "<ROOT>";
                int i = 1;
                while (VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneId.ToString() + i, CustomProp) != "")
                {
                    strXML += "<Geozones>";
                    GeoId = VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyGeozoneId.ToString() + i, CustomProp);
                    strXML += "<GeozoneId>" + GeoId + "</GeozoneId>";
                    strXML += "</Geozones>";
                    i++;
                }
                strXML += "</ROOT>";

                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, strXML, ref xml), false))
                    if (objUtil.ErrCheck(dbo.GetOrganizationGeozonesInfo(sn.UserID, sn.SecId, sn.User.OrganizationId, strXML, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                    return;

                DataSet dsBoxGeoZones = new DataSet();
                strrXML = new StringReader(xml);
                dsBoxGeoZones.ReadXml(strrXML);

                object[] newGeoRow = null;
                int rowIndex = 0;
                //Box Geozone against Database
                foreach (DataRow rowItem in dsBoxGeoZones.Tables[0].Rows)
                {
                   sn.GeoZone.DsVehicleGeoZone.Tables[0].Select();
                    //DataRows=sn.GeoZone.DsVehicleGeoZone.Tables[0].Select("GeozoneId="+rowItem["GeoZoneId"].ToString());
                    for (rowIndex = 0; rowIndex <sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count; ++rowIndex)
                    {
                        //if (DataRows==null || DataRows.Length==0)
                        if (Convert.ToInt32(rowItem["GeoZoneId"]) == Convert.ToInt32(sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows[rowIndex]["GeoZoneId"]))
                        {
                            // found
                            break;
                        }
                    }
                    // check if geozone does not exist in database
                    if (rowIndex ==sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Count)
                    {
                        newGeoRow = new Object[sn.GeoZone.DsVehicleGeoZone.Tables[0].Columns.Count];
                        newGeoRow[2] = Convert.ToInt32(rowItem["SeverityId"]);
                        newGeoRow[4] = Convert.ToInt32(rowItem["GeozoneId"]);
                        newGeoRow[5] = rowItem["Geozonename"].ToString().TrimEnd();
                        newGeoRow[14] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)Convert.ToInt32(rowItem["SeverityId"]));
                        newGeoRow[12] = sn.Cmd.BoxId;

                        if (rowItem["GeozoneName"].ToString() == VLF.CLS.Def.Const.unknownGeozoneName)
                            newGeoRow[15] = clsGeoZone.DeletePendingGeoZone;
                        else
                            newGeoRow[15] = clsGeoZone.AddPendingGeoZone;

                       sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows.Add(newGeoRow);
                    }
                }



                //Database geozone against box
                foreach (DataRow rowItem in sn.GeoZone.DsVehicleGeoZone.Tables[0].Rows)
                {
                    if (rowItem["Status"].ToString() == clsGeoZone.GeoZoneSync.ToString())
                    {
                        for (rowIndex = 0; rowIndex < dsBoxGeoZones.Tables[0].Rows.Count; ++rowIndex)
                        {
                            if (Convert.ToInt32(rowItem["GeoZoneId"]) == Convert.ToInt32(dsBoxGeoZones.Tables[0].Rows[rowIndex]["GeoZoneId"]))
                                break;
                        }
                        // check if geozone does not exist in box
                        if (rowIndex == dsBoxGeoZones.Tables[0].Rows.Count)
                            rowItem["Status"] = clsGeoZone.DeletePendingGeoZone;
                    }
                }

            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }
        }
        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
