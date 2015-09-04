using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text;
using VLF.DAS.Logic;

namespace SentinelFM
{
    public partial class Configuration_frmExportDrivers : System.Web.UI.Page
    {
        SentinelFMSession sn;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        ContactManager contactMsg = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["SentinelFMSession"] != null)
            {
                sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];

                contactMsg = new ContactManager(sConnectionString);

                DataSet dsDrivers = new DataSet();

                try
                {

                    dsDrivers = contactMsg.GetOrganizationDrivers(sn.User.OrganizationId);
                    StringBuilder driverlist = new StringBuilder();
                    driverlist.Append("DriverID,First Name,Last Name,License,Vehicle Description,Class,Gender,Height,Cell Phone,Additional Phone,City,State,Zipcode,Country,Home Phone,License issued,License expired,E-mail,Address,Description,Termination Date,Postion Info,Key Fob Id,SMS Id,SMS Pwd,US Cycle,CA Cycle,Timezone");
                    driverlist.Append(Environment.NewLine);
                    foreach (DataRow r in dsDrivers.Tables[0].Rows)
                    {
                        driverlist.Append(r["DriverId"].ToString());
                        driverlist.Append("," + r["FirstName"].ToString());
                        driverlist.Append("," + r["LastName"].ToString());
                        driverlist.Append(",\"" + r["License"].ToString().Replace("\"", "\"\"") + "\"");
                        driverlist.Append(",\"" + r["VehicleDescription"].ToString().Replace("\"", "\"\"") + "\"");
                        driverlist.Append("," + r["Class"].ToString());
                        driverlist.Append("," + r["Gender"].ToString());
                        driverlist.Append("," + r["Height"].ToString());
                        driverlist.Append("," + r["CellPhone"].ToString());
                        driverlist.Append(",\"" + r["AdditionalPhone"].ToString().Replace("\"", "\"\"") + "\"");
                        driverlist.Append("," + r["City"].ToString());
                        driverlist.Append(",\"" + r["State"].ToString().Replace("\"", "\"\"") + "\"");
                        driverlist.Append("," + r["ZipCode"].ToString());
                        driverlist.Append("," + r["Country"].ToString());
                        driverlist.Append("," + r["HomePhone"].ToString());
                        driverlist.Append("," + r["LicenseIssued"].ToString());
                        driverlist.Append("," + r["LicenseExpired"].ToString());
                        driverlist.Append("," + r["Email"].ToString());
                        driverlist.Append("," + r["Address"].ToString());
                        driverlist.Append("," + r["Description"].ToString());
                        driverlist.Append("," + r["TerminationDate"].ToString());
                        driverlist.Append("," + r["PositionInfo"].ToString());
                        driverlist.Append("," + r["KeyFobId"].ToString());
                        driverlist.Append("," + r["SMSID"].ToString());
                        driverlist.Append("," + r["SMSPwd"].ToString());
                        driverlist.Append("," + r["USCycle"].ToString());
                        driverlist.Append("," + r["CACycle"].ToString());
                        driverlist.Append("," + r["TimeZone"].ToString());

                        driverlist.Append(Environment.NewLine);

                    }

                    Response.Clear();
                    Response.ContentType = "application/csv";
                    Response.AddHeader("Content-Disposition", "attachment; filename=driverlist.csv");
                    Response.Write(driverlist.ToString());
                    Response.Flush();
                    Response.End();


                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                    //lblMessage.Text = Ex.Message;                    
                }

                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                }
            }
        }
    }
}