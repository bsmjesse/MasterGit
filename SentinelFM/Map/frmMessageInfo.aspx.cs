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

namespace SentinelFM
{
    /// <summary>
    /// Summary description for frmMessageInfo.
    /// </summary>
    public partial class frmMessageInfo : SentinelFMBasePage
    {

        
        

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                if (!Page.IsPostBack)
                {

                    GuiSecurity(this);
                    string MsgKey = Request.QueryString["MsgKey"].ToString();
                    string[] indexArray = MsgKey.Split(';');

                    int MessageId = Convert.ToInt32(indexArray[0]);
                    int VehicleId = Convert.ToInt32(indexArray[1]);
                    int PeripheralId = Convert.ToInt32(indexArray[2]);
                    Int16  MsgTypeId = Convert.ToInt16(indexArray[3]);
                    DateTime MsgDateTime = Convert.ToDateTime(indexArray[4]);
                    Int64 checksumId = Convert.ToInt64(indexArray[5]);

                    ViewState["MessageId"] = MessageId;
                    ViewState["MsgDateTime"] = MsgDateTime;
                    ViewState["PeripheralId"] = PeripheralId;
                    ViewState["MsgTypeId"] = MsgTypeId;
                    ViewState["checksumId"] = checksumId;
                    MessageInfoLoad(MessageId, PeripheralId, MsgTypeId, MsgDateTime, VehicleId);

                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
            }

        }

        private void MessageInfoLoad(int MessageId, int PeripheralId, Int16 MsgTypeId, DateTime MsgDateTime, int VehicleId)
        {
            try
            {
                
                DataSet ds = new DataSet();
                StringReader strrXML = null;
                string xml = "";
                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesFullInfo(sn.UserID, sn.SecId, MessageId, PeripheralId,MsgTypeId,MsgDateTime,VehicleId ,  ref xml), false))
                    if (objUtil.ErrCheck(hist.GetUserIncomingTextMessagesFullInfo(sn.UserID, sn.SecId, MessageId, PeripheralId, MsgTypeId, MsgDateTime,VehicleId,  ref xml), true))
                    {
                        return;
                    }
                strrXML = new StringReader(xml);


                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(strrXML);

                if (ds.Tables[0].Columns.IndexOf("To") == -1)
                {
                    DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                    colTo.DefaultValue = "";
                    ds.Tables[0].Columns.Add(colTo);
                }

                this.lblBoxId.Text = ds.Tables[0].Rows[0]["BoxId"].ToString().TrimEnd();
                this.lblLicensePlate.Text = ds.Tables[0].Rows[0]["LicensePlate"].ToString().TrimEnd();
                this.lblTimeCreated.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["MsgDateTime"].ToString().TrimEnd()).ToString();
                this.lblFrom.Text = ds.Tables[0].Rows[0]["From"].ToString().TrimEnd();
                this.lblTo.Text = ds.Tables[0].Rows[0]["To"].ToString().TrimEnd();
                this.lblStreetAddress.Text = ds.Tables[0].Rows[0]["StreetAddress"].ToString().TrimEnd();
                if (MsgTypeId ==(Int16)VLF.CLS.Def.Enums.PeripheralTypes.MDT) 
                    this.txtMessage.Text = ds.Tables[0].Rows[0]["MsgBody"].ToString().TrimEnd();
                else
                {
                    foreach (DataRow rowItem in ds.Tables[0].Rows)
                        txtMessage.Text += Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + "-" + VLF.CLS.Util.PairFindValue("TXT", rowItem["MsgBody"].ToString()) + "\r";
                }
                
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
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



        protected void cmdMarkAsRead_Click(object sender, System.EventArgs e)
        {
            
            DataSet ds = new DataSet();
            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

            #if MDT_NEW
            if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["MessageId"]), Convert.ToInt32(lblBoxId.Text)), false))
                if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["MessageId"]), Convert.ToInt32(lblBoxId.Text)), true))
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                }
            #else
            // Changes for TimeZone Feature end
            if (objUtil.ErrCheck(hist.SetMsgUserIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["MessageId"]), Convert.ToInt32(ViewState["MsgTypeId"]), Convert.ToDateTime(ViewState["MsgDateTime"]).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToInt32(ViewState["PeripheralId"]), Convert.ToInt64(ViewState["checksumId"])), false))
                if (objUtil.ErrCheck(hist.SetMsgUserIdExtended(sn.UserID, sn.SecId, Convert.ToInt32(ViewState["MessageId"]), Convert.ToInt32(ViewState["MsgTypeId"]), Convert.ToDateTime(ViewState["MsgDateTime"]).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), System.DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToInt32(ViewState["PeripheralId"]), Convert.ToInt64(ViewState["checksumId"])), true)) // Changes for TimeZone Feature end
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                }
#endif
          
             string str = "";
            sn.Map.LoadMessages_NewTZ(sn, ref str);
            sn.Map.MessagesHTML = str;

            Response.Write("<script language='javascript'>window.close()</script>");
        }

 

    }
}
