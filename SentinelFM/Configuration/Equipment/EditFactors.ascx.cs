using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VLF.DAS.Logic;
using System.Configuration;
using Telerik.Web.UI;

namespace SentinelFM
{
    public partial class Configuration_Equipment_EditFactors : System.Web.UI.UserControl
    {
        public string selectMsg = "Select media type";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        MediaManager mediaMg = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (combTypeName.Items.Count == 0)
            {
                combTypeName.DataSource = GetMediaTypes();
                combTypeName.DataBind();
                //combTypeName.Items.Add(new Telerik.Web.UI.RadComboBoxItem(selectMsg, "0"));
                valReqcombTypeName.InitialValue = selectMsg;
            }
            if (combMeasureUnit.Items.Count == 0)
            {
                combMeasureUnit.DataSource = GetMeasureUnits();
                combMeasureUnit.DataBind();
                combMeasureUnit.SelectedIndex = 0;
                //combTypeName.Items.Add(new Telerik.Web.UI.RadComboBoxItem(selectMsg, "0"));
                //valReqcombTypeName.InitialValue = selectMsg;
            }

            if (!IsPostBack)
            {
                try
                {
                    int MediaFactorDecimalDigits = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MediaFactorDecimalDigits"].ToString());
                    txtFactor1.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
                    txtFactor2.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
                    txtFactor3.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
                    txtFactor4.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
                    txtFactor5.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
                }
                catch (Exception ex) { }
            }
           
        }

        private DataSet GetMeasureUnits()
        {
            SentinelFMSession sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
            }

            if (mediaMg == null) mediaMg = new MediaManager(sConnectionString);
            return mediaMg.GetUnitOfMeasuresByUserId(sn.UserID);
        }

        private DataTable GetMediaTypes()
        {
            DataTable dt = null;
            if (mediaMg == null) mediaMg  = new MediaManager(sConnectionString);
            DataSet ds = mediaMg.GetMediaTypes();
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                DataRow dr = dt.NewRow();
                dr["TypeName"] = selectMsg;
                dr["MediaTypeId"] = "-1";
                dt.Rows.InsertAt(dr, 0);
            }
            return dt;
        }
        protected void combTypeName_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (combTypeName.SelectedIndex > 0)
            {
                if (mediaMg == null) mediaMg = new MediaManager(sConnectionString);
                DataSet ds = mediaMg.GetMediaFactorNamesByMediaTypeId(int.Parse(combTypeName.SelectedValue));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    SetControlsValues(lblFactorName1, txtFactor1,
                        ds.Tables[0].Rows[0]["FactorName1"], DBNull.Value);
                    SetControlsValues(lblFactorName2, txtFactor2,
                        ds.Tables[0].Rows[0]["FactorName2"], DBNull.Value);
                    SetControlsValues(lblFactorName3, txtFactor3,
                        ds.Tables[0].Rows[0]["FactorName3"], DBNull.Value);
                    SetControlsValues(lblFactorName4, txtFactor4,
                        ds.Tables[0].Rows[0]["FactorName4"], DBNull.Value);
                    SetControlsValues(lblFactorName5, txtFactor5,
                        ds.Tables[0].Rows[0]["FactorName5"], DBNull.Value);
                }
            }

        }
        private void SetControlsValues(Label lblFactorName, RadNumericTextBox txtFactor, object factorName, object factor)
        {
            if (factorName is DBNull || string.IsNullOrEmpty(factorName.ToString()))
            {
                lblFactorName.Visible = false;
                lblFactorName.Text = string.Empty;
                txtFactor.Visible = false;
                return;
            }
            lblFactorName.Text = factorName.ToString();
            lblFactorName.Visible = true;
            if (!(factor is DBNull) && !string.IsNullOrEmpty(factor.ToString()))
                txtFactor.Value = double.Parse(factor.ToString());
            else txtFactor.Value = null;
            txtFactor.Visible = true;
        }
    }
}