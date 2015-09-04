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
using System.IO;
using System.Configuration;
using VLF.CLS;
namespace SentinelFM
{
    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public partial class frmMesssageRotating : SentinelFMBasePage
    {

        public string strMessage;


        
        protected System.Web.UI.WebControls.Label lblTotalAlarms;
        

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);


                if ((Context.Session == null) || (sn == null) || (sn.UserName == ""))
                {
                    RedirectToLogin();
                    return;
                }

                if (!Page.IsPostBack)
                {
                   strMessage = sn.Map.MessagesHTML;
                   this.lblMessTotal.Text =  sn.Map.MessagesCount.ToString() ;
                    //MessagesList_Fill();
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

        // Changes for TimeZone Feature start
        private void MessagesList_Fill_NewTZ()
        {
            try
            {

                StringReader strrXML = null;


                string xml = "";
                string strFromDT = "";
                string strToDT = "";
                this.lblMessTotal.Text = "0";

                // changes for Timezone Feature start
                strFromDT = DateTime.Now.AddHours(-24 - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");  // changes for Timezone Feature end

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), false))
                    if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo_NewTZ(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);

                string str = "";
                string strStyle = "";

                //Unread Messages
                DataRow[] drCollections = null;
                drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
                foreach (DataRow rowItem in drCollections)
                {
                    strStyle = "style='{color:#000066;}'";
                    str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
                }

                if (drCollections != null)
                    this.lblMessTotal.Text = drCollections.Length.ToString();

                //Read Messages
                if (sn.User.ShowReadMess == 1)
                {
                    ds.Tables[0].Select();
                    drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

                    foreach (DataRow rowItem in drCollections)
                    {
                        strStyle = "style='{color:green;}'";
                        str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
                    }

                    if (drCollections != null)
                        this.lblMessTotal.Text = Convert.ToString((Convert.ToInt32(this.lblMessTotal.Text) + drCollections.Length));
                }



                strMessage = str;

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        // Changes for TimeZone Feature end


        private void MessagesList_Fill()
        {
            try
            {
                
                StringReader strrXML = null;


                string xml = "";
                string strFromDT = "";
                string strToDT = "";
                this.lblMessTotal.Text = "0";

                strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");

                ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();

                if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), false))
                    if (objUtil.ErrCheck(hist.GetUserTextMessagesShortInfo(sn.UserID, sn.SecId, strFromDT, strToDT, Convert.ToInt16(VLF.CLS.Def.Enums.TxtMsgDirectionType.In), ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                strrXML = new StringReader(xml);

                DataSet ds = new DataSet();
                ds.ReadXml(strrXML);

                string str = "";
                string strStyle = "";

                //Unread Messages
                DataRow[] drCollections = null;
                drCollections = ds.Tables[0].Select("UserId=-1", "", DataViewRowState.CurrentRows);
                foreach (DataRow rowItem in drCollections)
                {
                    strStyle = "style='{color:#000066;}'";
                   str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n","<br>").Replace("\r\n","<br>").Replace("\r","<br>")    + "]" + "</u></a></p><br>");
                }

                if (drCollections != null)
                    this.lblMessTotal.Text = drCollections.Length.ToString();

                //Read Messages
                if (sn.User.ShowReadMess == 1)
                {
                    ds.Tables[0].Select();
                    drCollections = ds.Tables[0].Select("UserId<>-1", "", DataViewRowState.CurrentRows);

                    foreach (DataRow rowItem in drCollections)
                    {
                        strStyle = "style='{color:green;}'";
                        str = str + ("<p><a href='#' " + strStyle + " onclick=NewWindow('" + rowItem["MsgId"].ToString().TrimEnd() + ";" + rowItem["VehicleId"].ToString().TrimEnd() + "') >" + Convert.ToDateTime(rowItem["MsgDateTime"].ToString().TrimEnd()).ToString() + " " + rowItem["Description"].ToString().TrimEnd() + "<br>" + "[" + rowItem["MsgBody"].ToString().TrimEnd().Replace("'", "''").Replace("\n", "<br>").Replace("\r\n", "<br>").Replace("\r", "<br>") + "]" + "</u></a></p><br>");
                    }

                    if (drCollections != null)
                        this.lblMessTotal.Text = Convert.ToString((Convert.ToInt32(this.lblMessTotal.Text) + drCollections.Length));
                }



                strMessage = str;

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
