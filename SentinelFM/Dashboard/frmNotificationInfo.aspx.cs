using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Xml; 

namespace SentinelFM
{
    public partial class Dashboard_frmNotificationInfo : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string NotificationId = Request.QueryString["NotificationId"];
            string xmlData = "";
            Int16  NotificationTypeId = 0;
            DataRow[] drArr = sn.History.DsNotifications.Tables[0].Select("NotificationId='" + NotificationId + "'");

            if (drArr == null && drArr.Length == 0)
                return;
 
            xmlData = drArr[0]["Data"].ToString();
            NotificationTypeId = Convert.ToInt16(drArr[0]["NotificationType"].ToString());
            this.lblVehicle.Text = Convert.ToString(drArr[0]["Description"]);
            this.lblLicensePlate.Text = Convert.ToString(drArr[0]["LicensePlate"]);
            this.lblDateTimeValue.Text =Convert.ToDateTime(drArr[0]["DateTimeCreated"]).ToString();
            this.lblAddress.Text  = drArr[0]["Address"].ToString();
            Int64 vehicleId = Convert.ToInt64(drArr[0]["vehicleId"]);

            if ((drArr[0]["Address"].ToString() == "N/A" && Convert.ToDouble(drArr[0]["Latitude"]) != 0))
            {
                try
                {
                    this.lblAddress.Text = clsMap.ResolveStreetAddress(sn, drArr[0]["Latitude"].ToString(), drArr[0]["Longitude"].ToString());
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }

            }


            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xmlData));
            XmlElement root = doc.DocumentElement;


            if (NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.DTC_Codes || NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.J1708_Codes)
            {

                string xml = "";
                DataSet ds = new DataSet();

              

                XmlNodeList nodes = root.SelectNodes("/d/ex");
                if (nodes.Count == 0)
                    return; 
            
                //this.lblDTCSource.Text = nodes[0]["DTCSource"].InnerText;
                //this.lblDTCCount.Text = nodes[0]["DTCCnt"].InnerText;


                if (nodes.Count == 0)
                    return;

                try
                {
                    this.lblDTCSourceValue.Text = nodes[0].Attributes.GetNamedItem("DTCSource").Value;
                    this.lblDTCCnt.Text = nodes[0].Attributes.GetNamedItem("DTCCnt").Value;
                    this.lblDTCInVehicle.Text = nodes[0].Attributes.GetNamedItem("DTCInVehicle").Value;
                }
                catch
                {
                    lblDTC.Text = nodes[0].InnerText.Replace("=", ": ").Replace("DTC", "DTC ");
                }

                if (NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.DTC_Codes)
                    {
                        using (ServerDBVehicle.DBVehicle dbVehicle = new ServerDBVehicle.DBVehicle())
                        {

                            if (objUtil.ErrCheck(dbVehicle.GetDTCCodesDescription(sn.UserID, sn.SecId, ref xml), false))
                                if (objUtil.ErrCheck(dbVehicle.GetDTCCodesDescription(sn.UserID, sn.SecId, ref xml), true))
                                {
                                }

                            if (xml != "")
                            {
                                StringReader strrXML = new StringReader(xml);
                                ds.ReadXml(strrXML);
                            }

                        }
                    }


               
                    XmlNodeList nodesV = root.SelectNodes("/d/v");
                    foreach (XmlNode node in nodesV)
                    {
                        //this.lblDTCcode.Text += node.InnerText + " ; ";

                        if (NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.DTC_Codes)
                        {
                            if (ds != null && ds.Tables.Count > 0)
                            {
                                DataRow[] drArrCode = ds.Tables[0].Select("DTCCode='" + node.InnerText + "'");
                                if (drArrCode != null && drArrCode.Length > 0)
                                    this.lblDTCcodeDescription.Text += node.InnerText + "-" + drArrCode[0]["text"] + "\r\n";
                                else
                                    this.lblDTCcodeDescription.Text += node.InnerText + ";" + "\r\n";
                            }
                        }
                        else
                        {
                            this.lblDTCcodeDescription.Text +=node.InnerText+ "\r\n";
                        }

                    }
                
               
                

                this.viewNotications.ActiveViewIndex = 0;  
            }
            else if (NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.MIL_Light_On)
            {
                XmlNodeList nodes = root.SelectNodes("/d");
                if (nodes.Count == 0)
                    return;

                //this.lblMILLigh.Text = nodes[0].InnerText=="1"? "On": "Off";
                this.lblMILLigh.Text = nodes[0].InnerText;
                this.viewNotications.ActiveViewIndex = 1;
            }
            else if (NotificationTypeId == (Int16)VLF.CLS.Def.Enums.NotificationType.Service_Maintenance)
            {
                XmlNodeList nodes = root.SelectNodes("/d");
                if (nodes.Count == 0)
                    return;

                try
                {
                    this.lblBoxIdNotification.Text = nodes[0].Attributes.GetNamedItem("boxId").Value;
                    this.lblStatusNotification.Text = Convert.ToString((VLF.DAS.Logic.Vehicle.VehicleServiceStatusType)Convert.ToInt16(nodes[0].Attributes.GetNamedItem("status").Value));
                    this.lblServiceNotification.Text = nodes[0].Attributes.GetNamedItem("serviceId").Value;
                    this.lblMessageNotification.Text = nodes[0].Attributes.GetNamedItem("msg").Value;



                    string xml = "";
                    DataSet ds = new DataSet();
                    using (ServerDBVehicle.DBVehicle dbVehicle = new ServerDBVehicle.DBVehicle())
                    {

                        if (objUtil.ErrCheck(dbVehicle.VehicleMaintenancePlan_Get(sn.UserID, sn.SecId, sn.User.OrganizationId, vehicleId, Convert.ToInt32(nodes[0].Attributes.GetNamedItem("serviceId").Value), ref xml), false))
                            if (objUtil.ErrCheck(dbVehicle.VehicleMaintenancePlan_Get(sn.UserID, sn.SecId, sn.User.OrganizationId, vehicleId, Convert.ToInt32(nodes[0].Attributes.GetNamedItem("serviceId").Value), ref xml), true))
                            {
                            }

                        if (xml != "")
                        {
                            StringReader strrXML = new StringReader(xml);
                            ds.ReadXml(strrXML);
                        }

                    }

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        this.lblServiceNotification.Text = ds.Tables[0].Rows[0]["ServiceDescription"].ToString();
                    }

                }
                catch
                {
                    this.lblMaintenanceValue.Text = nodes[0].InnerText.Replace("=", ": ").Replace("vid", "vehicle").Replace('"', ' ');
                }

                this.viewNotications.ActiveViewIndex = 2;
            }
        }
    }
}
