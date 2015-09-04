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
using VLF.CLS.Def;
using System.IO;

namespace SentinelFM
{
    public partial class Map_frmVehicleAttributes : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (!Page.IsPostBack)
                {
                   this.lblVehicleId.Text  = Request.QueryString["VehicleId"];
                   VehicleInfoLoad(Convert.ToInt64(this.lblVehicleId.Text));
                   LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleAttr, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                }
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        private void VehicleInfoLoad(Int64 VehicleId)
        {
            try
            {
                DataSet ds = new DataSet();
                string xml = "";
                ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
                if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), false))
                    if (objUtil.ErrCheck(dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, VehicleId, ref xml), true))
                    {
                        return;
                    }

                if (xml == "")
                {
                    return;
                }

                ds.ReadXml(new StringReader(xml));
                long mask= Convert.ToInt64(ds.Tables[0].Rows[0]["FwAttributes1"]);
                this.lblBoxId.Text = ds.Tables[0].Rows[0]["BoxId"].ToString() ;
                LoadAttributes(mask);
                


            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }


        /// <summary>
        /// Load all fw attributes and check according to a mask
        /// </summary>
        /// <param name="mask"></param>
        internal void LoadAttributes(long mask)
        {
            chkAttrList.Items.Clear();  
            Array featArray = Enum.GetValues(typeof(Enums.FwAttributes));
            bool checkedAttr = false;
            ListItem ls;
            for (int i = 0; i < featArray.Length; i++)
            {
                long element = (long)featArray.GetValue(i);
                if ((element & mask) == element)
                    checkedAttr = true;
                else
                    checkedAttr = false;

                ls=new ListItem();
                ls.Text =Enum.GetName(typeof(Enums.FwAttributes) , element).Replace("_"," ") ;
                ls.Value= featArray.GetValue(i).ToString(); 
                ls.Selected =checkedAttr;
                chkAttrList.Items.Add(ls);
            }
        }


        /// <summary>
        /// Get total mask value from checkbox checked items
        /// </summary>
        /// <param name="chkListFeatures">CheckedListBox</param>
        /// <returns>Feature Mask</returns>
        internal long GetFeatureMask()
        {
            long totalMask = 0;
            for (int i = 0; i < this.chkAttrList.Items.Count; i++)
            {
                if (this.chkAttrList.Items[i].Selected)
                    totalMask |= (long)Enum.Parse(typeof(Enums.FwAttributes),
                  this.chkAttrList.Items[i].Value);
            }
            return totalMask;
        }


        protected void cmdSave_Click(object sender, EventArgs e)
        {
            long mask = GetFeatureMask();

            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            if (objUtil.ErrCheck(dbv.UpdateBoxFeatures (sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text),mask), false))
                if (objUtil.ErrCheck(dbv.UpdateBoxFeatures(sn.UserID, sn.SecId, Convert.ToInt32(this.lblBoxId.Text), mask), true))
                {
                    this.lblMsg.Text = "Update failed.";  
                    return;
                }
            this.lblMsg.Text = "Vehicle information was updated successfully.";  
        }

        protected void cmdVehicleInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleDescription.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdSensorCommands_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmSensorMain.aspx?LicensePlate=" +sn.Cmd.SelectedVehicleLicensePlate);
        }
        protected void cmdServices_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraServices.aspx?VehicleId=" + lblVehicleId.Text);
        }
        protected void cmdUnitInfo_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleExtraInfo.aspx?VehicleId=" + lblVehicleId.Text);
        }
}




}
