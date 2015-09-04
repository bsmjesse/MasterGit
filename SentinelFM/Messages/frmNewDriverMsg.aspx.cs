using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Net.Mail;
using VLF.MailLib; 

namespace SentinelFM
{
    public partial class Messages_frmNewDriverMsg : SentinelFMBasePage 
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DsDriversFill();
                string driverId = Request.QueryString["driverId"];
                if ((driverId != "") && clsUtility.IsNumeric(driverId))
                {
                    DataSet ds = new DataSet();
                    ds = (DataSet)ViewState["Drivers"];
                    DataRow[] drArr = ds.Tables[0].Select("DriverId=" + driverId);
                    if (drArr != null && drArr.Length > 0)
                        this.cboDrivers.SetMultipleValues(drArr[0]["FullNameAndEmail"].ToString(), drArr[0]["DriverId"].ToString());
                }
            }

            
           
        }
        protected void cboDrivers_InitializeDataSource(object sender, ISNet.WebUI.WebCombo.DataSourceEventArgs e)
        {
            e.DataSource =(DataSet)ViewState["Drivers"];

          
        }


       
        protected void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {

                if (cboDrivers.GetMultipleValuesValue() == "")
                {
                    this.lblMessage.Text = "Please enter a Driver";  
                    return;
                }

                lblMessage.Text="";

                VLF.MailLib.EMailMessageAuth eMail = new VLF.MailLib.EMailMessageAuth(
                  System.Configuration.ConfigurationSettings.AppSettings["UserName"],
                  System.Configuration.ConfigurationSettings.AppSettings["Password"],
                  System.Configuration.ConfigurationSettings.AppSettings["SMTPServer"]);


                string val = cboDrivers.GetMultipleValuesValue();
                string[] arrDrivers = val.Split(';');
                string errMsg = "";
                DataSet ds=new DataSet();  
                ds=(DataSet)ViewState["Drivers"];
                
                for (int i = 0; i<arrDrivers.Length-1; i++)
                {
                    DataRow[] drArr = ds.Tables[0].Select("DriverId=" + arrDrivers[i]);
                    if (drArr != null && drArr.Length > 0)
                    {
                        if (drArr[0]["Email"].ToString().TrimEnd()  == "")
                        {
                            errMsg += " Email not sent to Driver " + drArr[0]["FullName"].ToString()+ " due to empty email";
                            continue; 
                        }

                        try
                        {

                            eMail.SendMail(drArr[0]["Email"].ToString(), "Text Message", this.txtSubject.Text);

                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo , VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info ,
                                      "Email has been sent to '" + drArr[0]["Email"].ToString() + "' successfully." + " Form:" + Page.GetType().Name));
                        }
                        catch
                        {
                            errMsg += " Failed to send Email to Driver: " + drArr[0]["FullName"].ToString();

                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                                      " Send email failed: " + drArr[0]["Email"].ToString() + " Form:" + Page.GetType().Name));
                        }
   
                        using (ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver())
                        {

                            // Changes for TimeZone Feature start
                            if (objUtil.ErrCheck(driver.AddDriverMsg(sn.UserID, sn.SecId, Convert.ToInt32(drArr[0]["DriverId"]),drArr[0]["Email"].ToString() ,this.txtSubject.Text,System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving)), false))
                                if (objUtil.ErrCheck(driver.AddDriverMsg(sn.UserID, sn.SecId, Convert.ToInt32(drArr[0]["DriverId"]), drArr[0]["Email"].ToString(), this.txtSubject.Text, System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving)), false))  // Changes for TimeZone Feature end
                                {
                                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                                       " Failed to store driver text Msg: " + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                                    

                                }
                        }
                    }

                }

                if (errMsg.TrimEnd() != "")
                {
                    this.lblMessage.Text = errMsg;
                    return;
                }
                else
                {
                    Response.Write("<script language='javascript'>window.close()</script>");
                }
              }
              catch (NullReferenceException Ex)
              {
                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                 RedirectToLogin();
              }

              catch (Exception Ex)
              {
                 System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
              }
        }

        private void DsDriversFill()
        {
            StringReader strrXML = null;
            string xml = "";

            using (ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver())
            {
                if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                           " No drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                    }
            }

            DataSet dsDrivers = new DataSet();
            strrXML = new StringReader(xml);
            dsDrivers.ReadXml(strrXML);

            DataColumn dc = new DataColumn();
            dc.ColumnName = "FullNameAndEmail";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsDrivers.Tables[0].Columns.Add(dc);

            string email = "";
            foreach (DataRow dr in dsDrivers.Tables[0].Rows)
            {
                email = dr["Email"].ToString().TrimEnd() == "" ? " No email setup " : dr["Email"].ToString().TrimEnd();
                dr["FullNameAndEmail"] = dr["FullName"] + " [" + email + "]";
            }

            
            ViewState["Drivers"] = dsDrivers;
            
        }
      
}
}
