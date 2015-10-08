using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM.GeomarkServiceRef;

namespace SentinelFM
{
    public partial class Widgets_BulkVehicleStateUpdate : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        
        public int DefaultState = 100;
        public string SelectedVehicles = string.Empty;
        public bool UpdateNotes = true;

        public string VEHICLE_STATE_DATA = "{}";

        private string sConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

                int.TryParse(Request["defaultState"], out DefaultState);
                SelectedVehicles = Request["selectedVehicles"].Replace(",,", ",").Trim(',');

                VehicleState_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name) + " " + Ex.StackTrace.ToString());
            }
            
        }

        private void VehicleState_Fill()
        {
            string filter = string.Format("VehicleId IN ({0})", SelectedVehicles);
            DataRow[] foundRows = sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Select(filter);
            if(foundRows.Length == 0)
            {
                VEHICLE_STATE_DATA = @"{
                              ""draw"": 1,
                              ""recordsTotal"": 0,
                              ""recordsFiltered"": 0,
                              ""data"": []}";

                return;
            }
            else
            {
                string sdata = "";
                foreach (DataRow rowItem in foundRows)
                {
                    string operationStateName = "";
                    int operationState = 100;
                    int originOperationState = 100;
                    if (sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                    {
                        if (rowItem["OperationalState"].ToString() == "100")
                        {
                            operationStateName = "Unavailable";
                            operationState = 200;
                            originOperationState = 100;
                        }
                        else if (rowItem["OperationalState"].ToString() == "200")
                        {
                            operationStateName = "Available";
                            operationState = 100;
                            originOperationState = 200;
                        }
                    }

                    long landmarkId = 0;
                    if (sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkID") && rowItem["LandmarkID"].ToString().Trim() != "")
                    {
                        long.TryParse(rowItem["LandmarkID"].ToString(), out landmarkId);
                    }

                    long vehicleId = 0;
                    long.TryParse(rowItem["VehicleId"].ToString(), out vehicleId);

                    int boxId = 0;
                    int.TryParse(rowItem["BoxId"].ToString(), out boxId);

                    int serviceConfigId = 0;

                    int landmarkDuration = -1;
                    string shouldSendEmailImmediately = "";

                    long landmarkEventId = 0;
                    if (sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkEventId") && rowItem["LandmarkEventId"].ToString().Trim() != "")
                    {
                        long.TryParse(rowItem["LandmarkEventId"].ToString(), out landmarkEventId);
                    }

                    string landmarkInDatetime = "";
                    if (sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("LandmarkInDateTime") && rowItem["LandmarkInDateTime"].ToString().Trim() != "")
                    {
                        landmarkInDatetime = rowItem["LandmarkInDateTime"].ToString();
                    }

                    DataSet dsService = new DataSet();
                    if (landmarkId > 0 && vehicleId > 0)
                    {
                        VLF.DAS.Logic.Vehicle _vehicle = new VLF.DAS.Logic.Vehicle(sConnectionString);
                        dsService = _vehicle.GetServiceConfigurationsByLandmarkAndVehicle(sn.User.OrganizationId, vehicleId, landmarkId);
                        string[,] serviceList = new string[dsService.Tables[0].Rows.Count, 2];

                        if (dsService.Tables[0].Rows.Count > 1)
                        {
                            for (int i = 0; i < dsService.Tables[0].Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    int.TryParse(dsService.Tables[0].Rows[i]["ServiceConfigID"].ToString(), out serviceConfigId);
                                }
                                else if (dsService.Tables[0].Rows[i]["ServiceConfigID"].ToString() == "531") {
                                    serviceConfigId = 531;
                                }                                
                            }

                        }
                        else if (dsService.Tables[0].Rows.Count == 1)
                        {
                            int.TryParse(dsService.Tables[0].Rows[0]["ServiceConfigID"].ToString(), out serviceConfigId);
                        }

                        //landmarkDuration = serviceConfigId == 0 ? -1 : getDuration(serviceConfigId, vehicleId, landmarkId, boxId);
                        Dictionary<string, string> dictVehicleAvailableEmailSetting = GetVehicleAvailableEmailSetting(serviceConfigId, vehicleId, landmarkId, boxId);
                        int.TryParse(dictVehicleAvailableEmailSetting["PeriodicEmailDurationInMinute"], out landmarkDuration);
                        if (landmarkDuration > 0)
                        {
                            landmarkDuration = landmarkDuration / 60;
                        }
                        shouldSendEmailImmediately = dictVehicleAvailableEmailSetting["ShouldSendEmailImmediately"].ToLower() == "false" ? "" : "checked";
                    }

                    sdata += "<tr>" +
                                "<td id='VehicleDescription_" + vehicleId.ToString() + "'>" + rowItem["Description"].ToString() +
                                "   <input type='hidden' id='LandmarkId_" + vehicleId.ToString() + "' value='" + landmarkId.ToString() + "' />" +
                        //"   <input type='hidden' id='ServiceConfigId_" + vehicleId.ToString() + "' value='" + serviceConfigId.ToString() + "' />" +
                                "   <input type='hidden' id='vehicleId_" + vehicleId.ToString() + "' value='" + vehicleId.ToString() + "' />" +
                                "   <input type='hidden' id='boxId_" + vehicleId.ToString() + "' value='" + boxId.ToString() + "' />" +
                                "   <input type='hidden' id='landmarkEventId_" + vehicleId.ToString() + "' value='" + landmarkEventId.ToString() + "' />" +
                                "   <input type='hidden' id='landmarkInDatetime_" + vehicleId.ToString() + "' value='" + landmarkInDatetime + "' />" +
                                "   <input type='hidden' id='originOperationState_" + vehicleId.ToString() + "' value='" + originOperationState.ToString() + "' />" +
                                "</td>" +
                                "<td>";
                    if (landmarkId > 0 && vehicleId > 0 && serviceConfigId > 0)
                    {
                        sdata += "<select id='ServiceConfigId_" + vehicleId.ToString() + "' name='ServiceConfigId_" + vehicleId.ToString() + "' onchange='getLandmarkDuration(" + vehicleId + "," + landmarkId + "," + boxId + ")'>";
                        foreach (DataRow dr in dsService.Tables[0].Rows)
                        {
                            string selected = (dr["ServiceConfigID"].ToString() == serviceConfigId.ToString()) ? "selected" : "";
                            sdata += "<option value='" + dr["ServiceConfigID"].ToString() + "' " + selected + ">" + dr["ServiceConfigName"].ToString() + "</option>";
                        }
                        sdata += "</select>";
                    }
                    else
                    {
                        sdata += "   <input type='hidden' id='ServiceConfigId_" + vehicleId.ToString() + "' value='" + serviceConfigId.ToString() + "' />";
                    }
                    sdata +=    "</td>" +
                                "<td width='20'><select id='OperationalState_" + vehicleId.ToString() + "' name='OperationalState" + vehicleId.ToString() + "' onchange=\"OnOperationalStateChange(this)\">" +
                                "       <option value='100' " + (operationState == 100 ? "Selected" : "") + ">Available</option>" +
                                "       <option value='200' " + (operationState == 200 ? "Selected" : "") + ">Unavailable</option>" +
                                "    </select>" +
                                "</td>" +
                                "<td><input size='30' type='text' id='OperationalStateNotes_" + vehicleId.ToString() + "' value=\"" + rowItem["OperationalStateNotes"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"/>" +
                                "</td>" +
                                "<td><div id='selLandmarkDuration_" + vehicleId.ToString() + "' " + ((landmarkId != 0 && serviceConfigId != 0 && operationState != 200) ? "" : "style='display:none'") + "><select id='LandmarkDuration_" + vehicleId.ToString() + "' name='LandmarkDuration" + vehicleId.ToString() + "'>" +
                                "       <option value='0' " + (landmarkDuration == 0 ? "selected" : "") + ">0</option>" +
                                "       <option value='12' " + (landmarkDuration == 12 ? "selected" : "") + ">12</option>" +
                                "       <option value='24' " + (landmarkDuration == 24 ? "selected" : "") + ">24</option>" +
                                "       <option value='36' " + (landmarkDuration == 36 ? "selected" : "") + ">36</option>" +
                                "       <option value='48' " + (landmarkDuration == 48 ? "selected" : "") + ">48</option>" +
                                "       <option value='60' " + (landmarkDuration == 60 ? "selected" : "") + ">60</option>" +
                                "       <option value='72' " + (landmarkDuration == 72 ? "selected" : "") + ">72</option>" +
                                " </select> Hours &nbsp;<input type='checkbox' name='chkSendEmailImmediately_" + vehicleId.ToString() + "' id='chkSendEmailImmediately_" + vehicleId.ToString() + "' " + shouldSendEmailImmediately + " /> Send Email Immediately</div></td>" +
                                "<td><div id='statud_" + vehicleId.ToString() + "'><img class='imgVehicleUpdateStatus' src='' id='imgStatus_" + vehicleId.ToString() + "' width='12' height='12' style='display:none;' /></div></td>" +
                             "</tr>";
                }


                VEHICLE_STATE_DATA = "<tbody>" + sdata + "</tbody>";
            }
        }

        //private int getDuration(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        //{            
        //    int rvDuration = -1;

        //    try
        //    {
        //        GeomarkServiceClient clientGeomarkService = new GeomarkServiceClient("httpbasic");
        //        rvDuration = clientGeomarkService.GetPostpone(boxId, (int)landmarkId, serviceConfigId);
        //        if (rvDuration == -1)
        //        {
        //            rvDuration = 24;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        //    }

        //    return rvDuration;
        //}

        private Dictionary<string, string> GetVehicleAvailableEmailSetting(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        {
            Dictionary<string, string> dictVehicleAvailableEmailSetting = new Dictionary<string, string>();

            try
            {
                GeomarkServiceClient clientGeomarkService = new GeomarkServiceClient("httpbasic");
                dictVehicleAvailableEmailSetting = clientGeomarkService.GetVehicleAvailableEmailSetting(boxId, (int)landmarkId, serviceConfigId);
                if (dictVehicleAvailableEmailSetting == null)
                {
                    dictVehicleAvailableEmailSetting = new Dictionary<string, string>();
                    dictVehicleAvailableEmailSetting.Add("PeriodicEmailDurationInMinute", "-1");
                    dictVehicleAvailableEmailSetting.Add("ShouldSendEmailImmediately", "True");
                }

                if (!dictVehicleAvailableEmailSetting.ContainsKey("PeriodicEmailDurationInMinute"))
                {
                    dictVehicleAvailableEmailSetting.Add("PeriodicEmailDurationInMinute", "-1");
                }

                if (!dictVehicleAvailableEmailSetting.ContainsKey("ShouldSendEmailImmediately"))
                {
                    dictVehicleAvailableEmailSetting.Add("ShouldSendEmailImmediately", "True");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return dictVehicleAvailableEmailSetting;
        }

    }
}