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

namespace SentinelFM
{
   public partial class frmFullScreenHistGrid : SentinelFMBasePage
   {
      
      

      protected void Page_Load(object sender, EventArgs e)
      {


         if (!Page.IsPostBack)
         {
               if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
               {
                  dgStops.ClearCachedDataSource();
                  dgStops.RebindDataSource();
                  dgHistoryDetails.LayoutSettings.ClientVisible = false;
                  dgStops.LayoutSettings.ClientVisible = true;
               }
               else
               {
                  dgHistoryDetails.ClearCachedDataSource();
                  dgHistoryDetails.RebindDataSource();
                  dgHistoryDetails.LayoutSettings.ClientVisible = true;
                  dgStops.LayoutSettings.ClientVisible = false;
               }

            cboRows.SelectedIndex = cboRows.Items.IndexOf(cboRows.Items.FindByValue(sn.History.DgVisibleRows.ToString()));
            cboGridPaging.SelectedIndex = cboGridPaging.Items.IndexOf(cboGridPaging.Items.FindByValue(sn.History.DgItemsPerPage.ToString()));
            this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
            this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

         }
      }
      protected void cboGridPaging_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.dgStops.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);
         this.dgHistoryDetails.LayoutSettings.PagingSize = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

         sn.History.DgItemsPerPage = Convert.ToInt32(this.cboGridPaging.SelectedItem.Value);

         if ((sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            dgStops.ClearCachedDataSource();
            dgStops.RebindDataSource();
         }
         else
         {
            dgHistoryDetails.ClearCachedDataSource();
            dgHistoryDetails.RebindDataSource();
         }
      }

      protected void dgHistoryDetails_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
         if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
         {
            e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
               e.Layout.TextSettings.UseLanguage = "fr-FR";
            //else
            //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
         }
      }
      protected void dgStops_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
         if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
         {
            e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
               e.Layout.TextSettings.UseLanguage = "fr-FR";
            //else
            //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
         }
      }
  
      protected void dgStops_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && (sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            e.DataSource = sn.History.DsHistoryInfo.Tables["StopData"];
         }
      }

      protected void dgHistoryDetails_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.History.DsHistoryInfo != null) && (sn.History.DsHistoryInfo.Tables[0] != null)
            && !(sn.History.ShowStops || sn.History.ShowStopsAndIdle || sn.History.ShowIdle))
         {
            e.DataSource = sn.History.DsHistoryInfo;
         }
      }
}
}
