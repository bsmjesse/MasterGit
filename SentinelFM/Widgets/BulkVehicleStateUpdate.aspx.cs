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
                    if (sn.Map.DsFleetInfoNew.Tables["VehiclesLastKnownPositionInformation"].Columns.Contains("OperationalStateName"))
                    {
                        if (rowItem["OperationalState"].ToString() == "100")
                        {
                            operationStateName = "Unavailable";
                            operationState = 200;
                        }
                        else if (rowItem["OperationalState"].ToString() == "200")
                        {
                            operationStateName = "Available";
                            operationState = 100;
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

                    if (landmarkId > 0 && vehicleId > 0)
                    {
                        VLF.DAS.Logic.Vehicle _vehicle = new VLF.DAS.Logic.Vehicle(sConnectionString);
                        DataSet dsService = _vehicle.GetServiceConfigurationsByLandmarkAndVehicle(sn.User.OrganizationId, vehicleId, landmarkId);
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

                        landmarkDuration = serviceConfigId == 0 ? -1 : getDuration(serviceConfigId, vehicleId, landmarkId, boxId);
                    }

                    //sdata += (sdata == "" ? "" : ",") + "{" + 
                    //            "\"VehicleId\":" + vehicleId.ToString() + 
                    //            ",\"VehiceDescription\": \"" + rowItem["Description"].ToString() + "\"" +
                    //            ",\"OperationStateName\":  \"" + operationStateName + "\"" +
                    //            ",\"OperationalStateNotes\":  \"" + rowItem["OperationalStateNotes"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"" +
                    //            ",\"LandmarkDuration\":  \"" + ((landmarkId != 0 && serviceConfigId != 0) ? landmarkDuration.ToString() : "") + "\"" +
                    //            ",\"OperationState\":  \"" + operationState.ToString() + "\"" +
                    //            ",\"LandmarkId\":  \"" + landmarkId.ToString() + "\"" +
                    //            ",\"ServiceConfigId\":  \"" + serviceConfigId.ToString() + "\"" +
                    //        "}";


                    sdata += "<tr>" +
                                "<td id='VehicleDescription_" + vehicleId.ToString() + "'>" + rowItem["Description"].ToString() + 
                                "   <input type='hidden' id='LandmarkId_" + vehicleId.ToString() + "' value='" + landmarkId.ToString() + "' />" +
                                "   <input type='hidden' id='ServiceConfigId_" + vehicleId.ToString() + "' value='" + serviceConfigId.ToString() + "' />" +
                                "   <input type='hidden' id='vehicleId_" + vehicleId.ToString() + "' value='" + vehicleId.ToString() + "' />" +
                                "   <input type='hidden' id='boxId_" + vehicleId.ToString() + "' value='" + boxId.ToString() + "' />" +
                                "</td>" +
                                "<td width='20'><select id='OperationalState_" + vehicleId.ToString() + "' name='OperationalState" + vehicleId.ToString() + "' onchange=\"OnOperationalStateChange(this)\">" +
                                "       <option value='100' " + (operationState == 100 ? "Selected" : "") + ">Available</option>" +
                                "       <option value='200' " + (operationState == 200 ? "Selected" : "") + ">Unavailable</option>" +
                                "    </select>" +
                                "</td>" +
                                "<td><input size='50' type='text' id='OperationalStateNotes_" + vehicleId.ToString() + "' value=\"" + rowItem["OperationalStateNotes"].ToString().Replace("\"", "\\\"").Replace("\r", " ").Replace("\n", " ") + "\"/>" +
                                "</td>" +
                                "<td><div id='selLandmarkDuration_" + vehicleId.ToString() + "' " + ((landmarkId != 0 && serviceConfigId != 0 && operationState != 200) ? "" : "style='display:none'") + "><select id='LandmarkDuration_" + vehicleId.ToString() + "' name='LandmarkDuration" + vehicleId.ToString() + "'>" +
                                "       <option value='0' " + (landmarkDuration == 0 ? "selected" : "") + ">0</option>" +
                                "       <option value='12' " + (landmarkDuration == 12 ? "selected" : "") + ">12</option>" +
                                "       <option value='24' " + (landmarkDuration == 24 ? "selected" : "") + ">24</option>" +
                                "       <option value='36' " + (landmarkDuration == 36 ? "selected" : "") + ">36</option>" +
                                "       <option value='48' " + (landmarkDuration == 48 ? "selected" : "") + ">48</option>" +
                                "       <option value='60' " + (landmarkDuration == 60 ? "selected" : "") + ">60</option>" +
                                "       <option value='72' " + (landmarkDuration == 72 ? "selected" : "") + ">72</option>" +
                                " </select> Hours</div></td>" +
                                "<td><div id='statud_" + vehicleId.ToString() + "'><img class='imgVehicleUpdateStatus' src='' id='imgStatus_" + vehicleId.ToString() + "' width='12' height='12' style='display:none;' /></div></td>" +
                             "</tr>";
                }

//                VEHICLE_STATE_DATA = @"{
//                              ""draw"": 1,
//                              ""recordsTotal"": " + foundRows.Length.ToString() + @",
//                              ""recordsFiltered"": " + foundRows.Length.ToString() + @",
//                              ""data"": [" + sdata;


//                VEHICLE_STATE_DATA += "]}";

                VEHICLE_STATE_DATA = "<tbody>" + sdata + "</tbody>";
            }
        }

        private int getDuration(int serviceConfigId, long vehicleId, long landmarkId, int boxId)
        {            
            int rvDuration = -1;

            try
            {
                GeomarkServiceClient clientGeomarkService = new GeomarkServiceClient("httpbasic");
                rvDuration = clientGeomarkService.GetPostpone(boxId, (int)landmarkId, serviceConfigId);
                if (rvDuration == -1)
                {
                    rvDuration = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return rvDuration;
        }

    }
}