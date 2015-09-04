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

namespace SentinelFM.Map
{
    /// <summary>
    /// Summary description for frmMapOptions.
    /// </summary>
    public partial class frmMapOptions : SentinelFMBasePage
    {
        
        

        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMapOptionsForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                this.chkShowLandmark.Checked = sn.Map.ShowLandmark;
                this.chkShowLandmarkname.Checked = sn.Map.ShowLandmarkname;
                this.chkShowVehicleName.Checked = sn.Map.ShowVehicleName;
                this.chkShowGeoZones.Checked = sn.Map.ShowGeoZone;
                this.chkShowLandmarkRadius.Checked = sn.Map.ShowLandmarkRadius;

                if (chkShowLandmarkRadius.Checked)
                {
                    this.chkShowLandmark.Enabled = false;
                    this.chkShowLandmark.Checked = false;
                }
                else
                {
                    this.chkShowLandmark.Enabled = true;
                }
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

        protected void cmdSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                sn.Map.ShowLandmark = this.chkShowLandmark.Checked;
                sn.Map.ShowLandmarkname = this.chkShowLandmarkname.Checked;
                sn.Map.ShowVehicleName = this.chkShowVehicleName.Checked;
                sn.Map.ShowGeoZone = this.chkShowGeoZones.Checked;
                sn.Map.ShowLandmarkRadius = this.chkShowLandmarkRadius.Checked ;

                string ShowLandmark = "";
                string ShowLandmarkname = "";
                string ShowVehicleName = "";
                string ShowGeoZones = "";
                string ShowLandmarkRadius = "";

                ShowLandmark = sn.Map.ShowLandmark ? "1" : "0";
                ShowLandmarkname = sn.Map.ShowLandmarkname ? "1" : "0";
                ShowVehicleName = sn.Map.ShowVehicleName ? "1" : "0";
                ShowGeoZones = sn.Map.ShowGeoZone ? "1" : "0";
                ShowLandmarkRadius = sn.Map.ShowLandmarkRadius ? "1" : "0";

                



                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                

                //ShowLandmark



                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update (sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmark), ShowLandmark), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmark . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }



                //ShowLandmarkName

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkName), ShowLandmarkname), true))
                    {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmark . User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    }

                //ShowVehicleName

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowVehicleName), ShowVehicleName), true))
                    {
                           System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmark . User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    }



                //ShowGeoZones

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowGeoZone), ShowGeoZones), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowGeoZone), ShowGeoZones), true))
                    {
                             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowGeoZone . User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                    }



                //ShowLandmarkRadius

                if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkRadius), ShowLandmarkRadius ), false))
                    if (objUtil.ErrCheck(dbu.UserPreference_Add_Update(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.Preference.MapOptShowLandmarkRadius ), ShowLandmarkRadius), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " MapOptShowLandmarkRadius . User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    }

                Response.Write("<script language='javascript'>window.opener.location.href='frmVehicleMap.aspx'; window.close()</script>");

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

        }

        protected void chkShowLandmarkname_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkShowLandmarkname.Checked && !chkShowLandmarkRadius.Checked)
            {
                this.chkShowLandmark.Checked = true;
                this.chkShowLandmark.Enabled = false;
            }
            else
            {
                this.chkShowLandmark.Enabled = true;
            }
        }

        protected void chkShowLandmark_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkShowLandmark.Checked)
            {
                this.chkShowLandmarkname.Enabled = true;
            }
            else
            {
                this.chkShowLandmarkname.Enabled = false;
                this.chkShowLandmarkname.Checked = false;
            }
        }
        protected void chkShowLandmarkRadius_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowLandmarkRadius.Checked)
            {
                this.chkShowLandmark.Enabled = false;
                this.chkShowLandmark.Checked = false;
            }
            else
            {
                this.chkShowLandmark.Enabled = true;
            }
        }
}
}
