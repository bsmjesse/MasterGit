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
using SentinelFM;

public partial class HOS_frmMDTVersion : SentinelFMBasePage
{
    protected SentinelFM.ServerDBFleet.DBFleet dbf;
    string confirm;


    public string showFilter = "Show Filter";
    public string hideFilter = "Hide Filter";

    protected DataSet dsTimeZone = new DataSet();


    clsHOSManager hosManager = new clsHOSManager();

    public int DefaultOrganizationHierarchyFleetId = 0;
    public string DefaultOrganizationHierarchyFleetName = string.Empty;
    public string DefaultOrganizationHierarchyNodeCode = string.Empty;
    public string OrganizationHierarchyPath = "";
    public bool MutipleUserHierarchyAssignment = false;
    private VLF.PATCH.Logic.PatchOrganizationHierarchy poh;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
            bool ShowOrganizationHierarchy = false;
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
            }
            if (ShowOrganizationHierarchy)
            {
                trFleetSelectOption.Visible = true;
                string defaultnodecode = string.Empty;

                SentinelFM.ServerDBUser.DBUser dbu = new SentinelFM.ServerDBUser.DBUser();
                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {
                    }
                StringReader strrXML = new StringReader(xml);
                DataSet dsPref = new DataSet();
                dsPref.ReadXml(strrXML);
                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    {
                        defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                    }
                }
                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);
                }
                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                    hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();
                    if (MutipleUserHierarchyAssignment)
                    {
                        hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
                        hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                        //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    }
                }
                else
                {
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                    btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                }
                if (MutipleUserHierarchyAssignment)
                {
                    if (hidOrganizationHierarchyFleetId.Value.Trim() == string.Empty)
                        DefaultOrganizationHierarchyFleetName = "";
                    else if (hidOrganizationHierarchyFleetId.Value.Contains(","))
                        DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                    else
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                }
                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
            }
            else
            {
                trFleetSelectOption.Visible = false;
            }

            if (!Page.IsPostBack)
            {
                //LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
                CboFleet_Fill();

                DgEmails_Fill();
            }
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


    private void DgEmails_Fill()
    {
        try
        {
            DataSet dsEmails = new DataSet();

            int fleetid = -1;
            if (optAssignBased.SelectedItem.Value == "0")
            {
                if (this.cboFleet.SelectedItem != null)
                    fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
            }
            else
            {
                if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyFleetId.Value.Contains(","))
                {
                    this.dgEmails.DataSource = null;
                    this.dgEmails.DataBind();
                    return;
                }
                fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
            }

            dsEmails = hosManager.GetVehiclesHOSVersion(fleetid, sn.User.OrganizationId);

            if (dsEmails == null || dsEmails.Tables.Count == 0 ||
                dsEmails.Tables[0].Rows.Count == 0)
            {
                this.dgEmails.DataSource = dsEmails.Tables[0];
                this.dgEmails.DataBind();
                return;
            }



            this.dgEmails.DataSource = dsEmails;
            this.dgEmails.DataBind();
            this.dgEmails.Visible = true;
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





    protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (this.cboFleet.SelectedItem.Value != "-1")
        {
            dgEmails.CurrentPageIndex = 0;
            DgEmails_Fill();
            ViewState["ConfirmDelete"] = "0";
            

        }
        else
        {
            this.dgEmails.DataSource = null;
            this.dgEmails.DataBind();
        }
    }

    protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
    {
        if (hidOrganizationHierarchyFleetId.Value != "-1")
        {
            dgEmails.CurrentPageIndex = 0;
            DgEmails_Fill();
            ViewState["ConfirmDelete"] = "0";            
        }
        else
        {
            this.dgEmails.DataSource = null;
            this.dgEmails.DataBind();
        }
    }

    private void CboFleet_Fill()
    {
        try
        {

            DataSet dsFleets = new DataSet();
            StringReader strrXML = null;




            string xml = "";
            dbf = new SentinelFM.ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbf.GetFleetsInfoXMLByUserIdByLang(sn.UserID, sn.SecId, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                {
                    cboFleet.DataSource = null;
                    return;
                }
            strrXML = new StringReader(xml);
            dsFleets.ReadXml(strrXML);

            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();

            cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectFleet"), "-1"));




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








    protected void dgEmails_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        DgEmails_Fill();
    }

    protected void optAssignBased_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (optAssignBased.SelectedItem.Value == "0")
        {
            trBasedOnNormalFleet.Visible = true;
            trBasedOnHierarchyFleet.Visible = false;
        }
        else
        {
            trBasedOnNormalFleet.Visible = false;
            trBasedOnHierarchyFleet.Visible = true;
        }
        DgEmails_Fill();
    }
}