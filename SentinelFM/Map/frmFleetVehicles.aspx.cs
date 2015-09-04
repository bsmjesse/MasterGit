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
using System.Data;
using System.IO;

namespace SentinelFM
{
   public partial class Map_frmFleetVehicles : SentinelFMBasePage
   {

      
      

      protected void Page_Load(object sender, EventArgs e)
      {
        
         if (!Page.IsPostBack)
            CboFleet_Fill();
         
      }

      private void CboFleet_Fill()
      {
         try
         {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));

         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }
      protected void cmdSave_Click(object sender, EventArgs e)
      {
         try
         {
            
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            this.lblMessage.Text = "";

            if (this.cboFleet.SelectedIndex == -1)
            {
               this.lblMessage.Visible = true;
               this.lblMessage.Text = (string)base.GetLocalResourceObject("MessageText_SelectFleet");
               return;
            }

            //DumpBeforeCall(sn, string.Format("frmFleetVehicles -- Save_Click"));

            foreach (DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
            {
               if (rowItem["chkBoxShow"].ToString().ToLower()  == "true")
               {
                  if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["VehicleId"])), false))
                     if (objUtil.ErrCheck(dbf.AddVehicleToFleet(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToInt32(rowItem["VehicleId"])), true))
                     {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("MessageText_AddFailed");
                        return;
                     }
               }
            }
            this.lblMessage.Visible = true;
            this.lblMessage.Text = (string)base.GetLocalResourceObject("MessageText_AddSuccess");
         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
}
}
