using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO; 
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Diagnostics; 

namespace SentinelFM
{
    public partial class frmAlarmRotatingClient : System.Web.UI.Page
   {
      public string strAlarms;
      protected SentinelFMSession sn = null;
      protected clsUtility objUtil;
      public string alarmsScreenHight = "105";
      private Stopwatch Pagewatch = new Stopwatch();
      private Stopwatch watch = new Stopwatch();
      public string headerColor = "#009933";
      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            
            //Clear IIS cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);

            if (sn.User.ViewAlarmScrolling == 0)
                return; 
           
            if (sn.User.ViewMDTMessagesScrolling == 0)
               alarmsScreenHight = "290";

          

            if (!Page.IsPostBack)
            {

               if (sn.Map.ReloadMap)
               {
                  if (sn.User.MapType == VLF.MAP.MapType.LSD)
                      Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmLSDmap.aspx';</SCRIPT>");
                  else if (sn.User.MapType == VLF.MAP.MapType.VirtualEarth)
                      Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='../MapVE/VEMap.aspx';</SCRIPT>");
                      
                  else
                     Response.Write("<SCRIPT Language='javascript'>parent.frmFleetInfo.location.href='frmFleetInfoNew.aspx'; parent.frmVehicleMap.location.href='frmvehiclemap.aspx';</SCRIPT>");

                  sn.Map.ReloadMap = false;
               }

            }

          
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





        protected override void InitializeCulture()
        {

            if (Session["PreferredCulture"] != null)
            {
                string UserCulture = Session["PreferredCulture"].ToString();
                if (UserCulture != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
                }
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }

            if (sn.User.MenuColor != "")
                headerColor = sn.User.MenuColor;

           

        }



        public void RedirectToLogin()
        {
        
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
            return;
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

