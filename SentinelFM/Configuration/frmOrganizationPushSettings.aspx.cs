using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using OboutInc.ColorPicker;
using VLF.CLS; 
namespace SentinelFM
{
    public partial class Configuration_frmOrganizationPushSettings : SentinelFMBasePage
    {
        string confirm; 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetOrganizationPushSettings();
                GuiSecurity(this);
                
                HideAllTable();
                cboPushType_Fill();
                this.tblAddUpdate.Visible = false;
                tblUpdateButtons.Visible = false;
                ViewState["ConfirmDelete"] = "0";
                
               

            }
        }

        private void GetOrganizationPushSettings()
        {
            StringReader strrXML = null;
            DataSet ds = new DataSet();
            string xml = "";

            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.GetPushConfigurationByOrg(sn.UserID, sn.SecId,sn.User.OrganizationId,ref xml), false))
                    if (objUtil.ErrCheck(dbOrganization.GetPushConfigurationByOrg(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
            }


            if (xml == "")
            {
                this.dgPushConfiguration.DataSource = null;
                this.dgPushConfiguration.DataBind();
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);


            this.dgPushConfiguration.DataSource = ds;
            this.dgPushConfiguration.DataBind();
            
        }
       
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }
        protected void cmdUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }
        protected void cmdVehicles_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }
        protected void cmdFleets_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmEmails.aspx"); 
        }

     


        
        private void HideAllTable()
        {
          
            this.tblFTP.Visible = false;
            this.tblHTTP.Visible = false;
            this.tblHTTPS.Visible = false;
            this.tblSMTP.Visible = false;
            this.tblTCP_IP.Visible = false;
            this.tblWebService.Visible = false;   
        }


        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            this.tblAddUpdate.Visible = true;
            tblUpdateButtons.Visible = true ;
            cmdAdd.Visible = false ;
            this.lblPushId.Text = "0";
            lblPushTypeId.Text = "";
            this.lblMessage.Text = "";  
            this.cboPushType.Visible = true;
            this.lblPushTypeName.Visible = false;
            HideAllTable(); 
 
        }


        private void cboPushType_Fill()
        {
            StringReader strrXML = null;
            DataSet ds = new DataSet();
            string xml = "";

            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.GetUnassignedPushTypesByOrg(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                    if (objUtil.ErrCheck(dbOrganization.GetUnassignedPushTypesByOrg(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                    {
                        return;
                    }
            }


            if (xml == "")
            {
                this.cboPushType.DataSource = null;
                this.cboPushType.DataBind();
                return;
            }

            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);


            this.cboPushType.DataSource = ds;
            this.cboPushType.DataBind();

        }
        protected void cboPushType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTables("");
        }
        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            this.tblAddUpdate.Visible = false;
            tblUpdateButtons.Visible = false;
            cmdAdd.Visible = true; 
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            
            string PushTypeId = "";
            if (lblPushTypeId.Text != "")
                PushTypeId = lblPushTypeId.Text;
            else
                PushTypeId = cboPushType.SelectedItem.Value;

            if (PushTypeId == "0" || PushTypeId=="")
            {
                this.lblMessage.Visible = true;  
                this.lblMessage.Text = "Please select a Push Type";
                return; 
            }

            lblMessage.Text = "";
            string configuration = "";
            switch (PushTypeId)
            {
                case "1":
                    configuration = String.Format("url={0};msg={1};uname={2};pwd={3};", this.txtUrlFTP.Text, "", this.txtUserNameFTP.Text, this.txtPasswordFTP.Text);  
                    break;
                case "2":
                    configuration = String.Format("email={0};msg={1};subj={2};uname={3};pwd={4};", this.txtEmailSMTP.Text,"",this.txtSubjSMTP.Text,   this.txtUserNameSMTP.Text, this.txtPasswordSMTP.Text);  
                    break;
                case "3":
                    configuration = String.Format("url={0};msg={1};uname={2};pwd={3};", this.txtUrlHTTP.Text, "", this.txtUserNameHTTP.Text, this.txtPasswordHTTP.Text);  
                    break;
                case "4":
                    configuration = String.Format("url={0};msg={1};uname={2};pwd={3};", this.txtUrlHTTPS.Text, "", this.txtUserNameHTTPS.Text, this.txtPasswordHTTPS.Text);  
                    break;
                case "5":
                    configuration = String.Format("ip={0};port={1};msg={2};", this.txtIpTCP.Text, this.txtPortTCP.Text, "");  
                    break;
                case "6":
                    configuration = String.Format("url={0};msg={1};uname={2};pwd={3};", this.txtUrlWebService.Text, "", this.txtPasswordWebService.Text , this.txtUserNameWebService.Text);  
                    break;
            }


            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (this.lblPushId.Text == "0")
                {
                    if (objUtil.ErrCheck(dbOrganization.AddPush(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(cboPushType.SelectedItem.Value), configuration), false))
                        if (objUtil.ErrCheck(dbOrganization.AddPush(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(cboPushType.SelectedItem.Value), configuration), true))
                        {
                            return;
                        }
                }
                else
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdatePushConfiguration(sn.UserID, sn.SecId, Convert.ToInt64(this.lblPushId.Text), configuration), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdatePushConfiguration(sn.UserID, sn.SecId, Convert.ToInt64(this.lblPushId.Text), configuration), true))
                        {
                            return;
                        }
                }
            }
            cboPushType_Fill();

            GetOrganizationPushSettings();
            tblUpdateButtons.Visible = false;
            this.tblAddUpdate.Visible = false;
            cmdAdd.Visible = true;
            HideAllTable(); 
        }
        protected void cmdSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmOrganizationSettings.aspx"); 
        }
        protected void dgPushConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            this.lblPushId.Text = dgPushConfiguration.DataKeys[dgPushConfiguration.SelectedIndex].ToString();
            this.lblPushTypeId.Text = dgPushConfiguration.SelectedItem.Cells[2].Text;
            string configuration = dgPushConfiguration.SelectedItem.Cells[0].Text;
            this.lblPushTypeName.Text  = dgPushConfiguration.SelectedItem.Cells[1].Text;
            this.cboPushType.SelectedIndex = -1;
            this.cboPushType.Visible = false;
            lblPushTypeName.Visible = true; 
            ShowTables(configuration);
            this.lblMessage.Text = "";  
            
        }

        private void ShowTables(string configuration)
        {
            HideAllTable();
            this.tblAddUpdate.Visible = true;
            this.tblUpdateButtons.Visible = true;
            string PushTypeId = "0";

            if (lblPushTypeId.Text != "")
                PushTypeId = lblPushTypeId.Text;
            else
                PushTypeId = cboPushType.SelectedItem.Value;
            
            switch (PushTypeId)
            {
                case "1":
                    this.txtUrlFTP.Text = Util.PairFindValue("url", configuration);
                    this.txtUserNameFTP.Text = Util.PairFindValue("uname", configuration);
                    this.txtPasswordFTP.Text = Util.PairFindValue("pwd", configuration);
                    this.tblFTP.Visible = true;
                    break;
                case "2":
                    this.txtEmailSMTP.Text = Util.PairFindValue("email", configuration);
                    this.txtSubjSMTP.Text = Util.PairFindValue("subj", configuration);
                    this.txtUserNameSMTP.Text = Util.PairFindValue("uname", configuration);
                    this.txtPasswordSMTP.Text = Util.PairFindValue("pwd", configuration);
                    this.tblSMTP.Visible = true;
                    break;
                case "3":
                    this.txtUrlHTTP.Text = Util.PairFindValue("url", configuration);
                    this.txtUserNameHTTP.Text = Util.PairFindValue("uname", configuration);
                    this.txtPasswordHTTP.Text = Util.PairFindValue("pwd", configuration);
                    this.tblHTTP.Visible = true;
                    break;
                case "4":
                    this.txtUrlHTTPS.Text = Util.PairFindValue("url", configuration);
                    this.txtUserNameHTTPS.Text = Util.PairFindValue("uname", configuration);
                    this.txtPasswordHTTPS.Text = Util.PairFindValue("pwd", configuration);
                    this.tblHTTPS.Visible = true;
                    break;
                case "5":
                    this.txtIpTCP.Text = Util.PairFindValue("ip", configuration);
                    this.txtPortTCP.Text = Util.PairFindValue("port", configuration);
                    this.tblTCP_IP.Visible = true;
                    break;
                case "6":
                    this.txtUrlWebService.Text = Util.PairFindValue("url", configuration);
                    this.txtPasswordWebService.Text = Util.PairFindValue("pwd", configuration);
                    this.txtUserNameWebService.Text = Util.PairFindValue("uname", configuration);
                    this.tblWebService.Visible = true;
                    break;
            }
        }
        protected void dgPushConfiguration_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                confirm = "return confirm('Are you sure you want to delete?')";
                LinkButton deleteBtn = (LinkButton)e.Item.Cells[4].Controls[0];
                deleteBtn.Attributes.Add("onclick", confirm);
            }
        }
        protected void dgPushConfiguration_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (confirm == "")
                    return;

                lblMessage.Visible = true;
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();


                Int64  PushId = Convert.ToInt64(dgPushConfiguration.DataKeys[e.Item.ItemIndex].ToString());

                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.DeletePush(sn.UserID, sn.SecId, sn.User.OrganizationId, PushId), false))
                        if (objUtil.ErrCheck(dbOrganization.DeletePush(sn.UserID, sn.SecId, sn.User.OrganizationId, PushId), true))
                        {
                            return;
                        }
                }


                dgPushConfiguration.SelectedIndex = -1;
                lblMessage.Text = "Push Type deleted";
                dgPushConfiguration.CurrentPageIndex = 0;
                GetOrganizationPushSettings();
                cboPushType_Fill();  
                ViewState["ConfirmDelete"] = "0";
                confirm = "";
                HideAllTable();
                this.tblAddUpdate.Visible = false;
                this.tblUpdateButtons.Visible = false;   

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
        
}
}
