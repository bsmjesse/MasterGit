using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Text;


namespace SentinelFM
{
    public partial class Configuration_ScheduledReportsService : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (!Page.IsPostBack)
                {
                    string request = Request.QueryString["st"];
                    string email = Request.QueryString["email"];

                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = Encoding.Default;

                    if (string.IsNullOrEmpty(request))
                    {
                        request = "GetVehicleEmailsByEmail";
                    }

                    if (request == "GetVehicleEmailsByEmail")
                    {
                        GetVehicleEmailsByEmail(email);
                    }
                    else if (request == "GetCostCenterFleetEmailsByEmail")
                    {
                        GetCostCenterFleetEmailsByEmail(email);
                    }
                    else if (request == "GetScheduledReportsByEmail")
                    {
                        GetScheduledReportsByEmail(email);
                    }
                    else if (request == "GetVehicleEmailsByVehicle")
                    {
                        GetVehicleEmailsByVehicle();
                    }
                    else if (request == "GetCostCenterFleetEmailsByVehicle")
                    {
                        GetCosterFleetEmailByVehicle();
                    }
                    else if (request == "GetScheduledReportsByVehicle")
                    {
                        GetScheduledReportsByVehicle();
                    }
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        private void GetVehicleEmailsByEmail(string email)
        {
            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetScheduledReportsVehicleEmail(sn.User.OrganizationId, email);
            vehicleEmails.DataSetName = "VehicleEmailsDataSet";
            vehicleEmails.Tables[0].TableName = "VehicleEmails";
            Response.Write(vehicleEmails.GetXml());            
        }

        private void GetCostCenterFleetEmailsByEmail(string email)
        {
            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetScheduledReportsCostCenterFleetEmail(sn.User.OrganizationId, email);
            vehicleEmails.DataSetName = "ScheduledReportsCostCenterFleetEmailDataSet";
            vehicleEmails.Tables[0].TableName = "ScheduledReportsCostCenterFleetEmail";
            Response.Write(vehicleEmails.GetXml());
        }

        private void GetScheduledReportsByEmail(string email)
        {
            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetScheduledReports(sn.User.OrganizationId, email);
            vehicleEmails.DataSetName = "ScheduledReportsDataSet";
            vehicleEmails.Tables[0].TableName = "ScheduledReports";
            string xml = vehicleEmails.GetXml();
            xml = xml.Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
            Response.Write(xml);
        }

        private void GetVehicleEmailsByVehicle()
        {
            //string vehicleType = Request.QueryString["vehicleTypeValue"];
            int vehicleType = -1;
            int.TryParse(Request.QueryString["vehicleTypeValue"], out vehicleType);
            string searchParam = Request.QueryString["textSearchParam"];

            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetVehicleEmailByVehicle(sn.User.OrganizationId, vehicleType, searchParam);
            vehicleEmails.DataSetName = "VehicleEmailsDataSet";
            vehicleEmails.Tables[0].TableName = "VehicleEmails";
            Response.Write(vehicleEmails.GetXml());
        }

        private void GetCosterFleetEmailByVehicle()
        {
            int vehicleType = -1;
            int.TryParse(Request.QueryString["vehicleTypeValue"], out vehicleType);
            string searchParam = Request.QueryString["textSearchParam"];
            
            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetCosterFleetEmailByVehicle(sn.User.OrganizationId, vehicleType, searchParam);
            vehicleEmails.DataSetName = "ScheduledReportsCostCenterFleetEmailDataSet";
            vehicleEmails.Tables[0].TableName = "ScheduledReportsCostCenterFleetEmail";
            Response.Write(vehicleEmails.GetXml());
        }

        private void GetScheduledReportsByVehicle()
        {
            int vehicleType = -1;
            int.TryParse(Request.QueryString["vehicleTypeValue"], out vehicleType);
            string searchParam = Request.QueryString["textSearchParam"];
            
            VLF.PATCH.Logic.PatchScheduledReports sr = new VLF.PATCH.Logic.PatchScheduledReports(sConnectionString);
            DataSet vehicleEmails = sr.GetScheduledReportsByVehicle(sn.User.OrganizationId, vehicleType, searchParam);
            vehicleEmails.DataSetName = "ScheduledReportsDataSet";
            vehicleEmails.Tables[0].TableName = "ScheduledReports";
            string xml = vehicleEmails.GetXml();
            xml = xml.Replace("-05:00", getUserTimezone()).Replace("-04:00", getUserTimezone());
            Response.Write(xml);
        }

        private string getUserTimezone()
        {
            return "";

            if (sn.User.TimeZone >= 0 && sn.User.TimeZone < 10)
                return "+0" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone >= 10)
                return "+" + sn.User.TimeZone + ":00";
            else if (sn.User.TimeZone < 0 && sn.User.TimeZone > -10)
                return "-0" + Math.Abs(sn.User.TimeZone) + ":00";
            else
                return sn.User.TimeZone + ":00";
        }
    }
}