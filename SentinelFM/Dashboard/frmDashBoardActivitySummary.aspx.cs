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
using ISNet.WebUI.WebGrid.Chart;
using ISNet.WebUI.WebGrid;
using System.Drawing;
using Bar;
using System.Text;
using System.Collections.Generic;

namespace SentinelFM
{
    public partial class Dashboard_frmActivitySummary : SentinelFMBasePage 
    {
        public string _xml = "";
        public SortedDictionary<int, StackBarCollection> _BarData;

        protected void Page_Load(object sender, EventArgs e)
        
        {
            
             
           // SeriesArea settings = this.dgActivitySummary.ChartSettings.SeriesAreaSettings.Settings;

            if (!Page.IsPostBack)
                dgActivitySummary_Fill_NewTZ();

            this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            this.lblLastUpdated.Text = System.DateTime.Now.ToShortTimeString();    
        }


        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            //dgAlarms.ClearCachedDataSource();
            //dgAlarms.RebindDataSource(); 
        }

        // Changes for TimeZone Feature start

        private void dgActivitySummary_Fill_NewTZ()
        {
            try
            {


                string strFromDT = "";
                string strToDT = "";

                strFromDT = DateTime.Now.AddHours(-24 - sn.User.NewFloatTimeZone - sn.User.DayLightSaving).AddMinutes(-5).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");


                ServerDBOrganization.DBOrganization dbOrg = new ServerDBOrganization.DBOrganization();
                float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

                dbOrg.GetActivitySummaryPerOrganizationCompleted +=
               new ServerDBOrganization.GetActivitySummaryPerOrganizationCompletedEventHandler(ActivitySummaryXML);
                dbOrg.GetActivitySummaryPerOrganization_NewTZAsync(sn.UserID, sn.SecId, sn.User.OrganizationId, strFromDT, strToDT, 3, _xml);



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        // Changes for TimeZone Feature end

        private void dgActivitySummary_Fill()
        {
            try
            {


                string strFromDT = "";
                string strToDT = "";

                strFromDT = DateTime.Now.AddHours(-24 - sn.User.TimeZone - sn.User.DayLightSaving).AddMinutes(-5).ToString("MM/dd/yyyy HH:mm:ss");
                strToDT = DateTime.Now.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss");


                ServerDBOrganization.DBOrganization dbOrg = new ServerDBOrganization.DBOrganization();
                Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

                dbOrg.GetActivitySummaryPerOrganizationCompleted +=
               new ServerDBOrganization.GetActivitySummaryPerOrganizationCompletedEventHandler(ActivitySummaryXML);
                dbOrg.GetActivitySummaryPerOrganizationAsync(sn.UserID, sn.SecId, sn.User.OrganizationId, strFromDT,strToDT,3, _xml);



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }


        void ActivitySummaryXML(Object source, ServerDBOrganization.GetActivitySummaryPerOrganizationCompletedEventArgs  e)
        {
            try
            {

            //Validate if key expired
            if ((VLF.ERRSecurity.InterfaceError)e.Result == VLF.ERRSecurity.InterfaceError.PassKeyExpired)
            {
                SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
                string secId = "";
                int result = sec.ReloginMD5ByDBName (sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
                if (result != 0)
                {
                    sn.SecId = secId;
                    dgActivitySummary_Fill_NewTZ();
                }
            }

            _xml = e.xml;
            if (_xml == null || _xml == "")
                return;


          
            StringReader strrXML = new StringReader(_xml);
            DataSet ds = new DataSet();
            string strPath = MapPath("..\\Dashboard\\Datasets") + @"\dstActivitySummaryReportPerFleet.xsd";
            ds.ReadXmlSchema(strPath);

            ds.ReadXml(strrXML);
            sn.History.DsActivitySummary = ds;
            dgActivitySummary.ClearCachedDataSource();
            dgActivitySummary.RebindDataSource();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
        protected void dgActivitySummary_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            if (sn.History.DsActivitySummary != null)
                e.DataSource = sn.History.DsActivitySummary;
        }
        protected void dgActivitySummary_ChartImageProcessing(object sender, ISNet.WebUI.WebGrid.Chart.WebGridChartEventArgs e)
        {


          ChartSettings settings = e.ChartConfig.ChartSettings;

          switch (settings.ChartType)
          {
              case ChartType.Bar:
                  SeriesBar barSettings = settings.SeriesBarSettings.Settings;
                  switch (settings.SeriesBarSettings.Type)
                  {
                      case SeriesBarType.ClusteredBar:
                          settings.SeriesColorCollection.StartIndex = 12;
                          break;
                      case SeriesBarType.StackedBar:
                          settings.SeriesColorCollection.StartIndex = 14;
                          break;
                      case SeriesBarType.StackedPercentageBar:
                          settings.SeriesColorCollection.StartIndex = 16;
                          break;
                      case SeriesBarType.Basic3DBar:
                      case SeriesBarType.Clustered3DBar:
                          barSettings.ChartFillType = ChartFillType.ColorFill;
                          barSettings.BarShape = BarShape.SmoothEdgeBar;
                          break;
                      case SeriesBarType.Stacked3DBar:
                          barSettings.ChartFillType = ChartFillType.ColorFill;
                          barSettings.BarShape = BarShape.Pyramid;
                          barSettings.BorderStyle.Width = 0;
                          break;
                      case SeriesBarType.StackedPercentage3DBar:
                          barSettings.ChartFillType = ChartFillType.ColorFill;
                          barSettings.BarShape = BarShape.InvertedCone;
                          barSettings.BorderStyle.Width = 0;
                          break;
                  }
                  break;
          }
            
        }
        protected void optView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optView.SelectedItem.Value == "1")
            {
                GraphView();
                this.dgActivitySummary.Visible = false;
               // this.ctrlChart.Visible = true;
                this.ScriptValues.Visible = true;  
            }
            else
            {
                this.dgActivitySummary.Visible = true ;
                this.ScriptValues.Visible = false ;
               // this.ctrlChart.Visible = false ;

            }
        }

        private void GraphView()
        {
            //ColumnChart chart = new ColumnChart();
            //chart.MaxColumnWidth = 20;
            //chart.Fill.Color = Color.FromArgb(100, Color.Green);

            //foreach (DataRow dr in sn.History.DsActivitySummary.Tables[0].Rows)
            //{
            //    if (Convert.ToSingle(dr["EngineOnMin"])>0)
            //        chart.Data.Add(new ChartPoint(dr["fleetName"].ToString() ,Convert.ToSingle(dr["EngineOnMin"])));
            //}
            


            //ctrlChart.Charts.Add(chart);
            //ctrlChart.RedrawChart();


            int columns = 1;
            int rows = sn.History.DsActivitySummary.Tables[0].Rows.Count-1 ;
            _BarData = new SortedDictionary<int, StackBarCollection>();
            int graphCount = sn.History.DsActivitySummary.Tables[0].Rows.Count - 1;
            foreach (DataRow dr in sn.History.DsActivitySummary.Tables[0].Rows)
                FillDictionary(dr);

            StringBuilder s = new StringBuilder();
            s.AppendLine(CreateGraphArray());

            string script = string.Format("<script type=\"text/javascript\">{0}</script>\n", s.ToString());
            string controls = CreateGraphContainers(columns, rows);
            this.ScriptValues.Text = string.Format("{0}{1}", controls, script);

        }


        void FillDictionary(DataRow dr)
        {
            //load your values here...
            StackBarCollection sbc = new StackBarCollection("Time Well Spent");
            sbc.Scale = 1;
            //sbc.Colors = new string[] { "#ff0000", "#00c800" }; 

            List<BarValue> bvs = new List<BarValue>();
            BarValueCollection bvc = new BarValueCollection(dr["fleetName"].ToString());
            List<BarValue> values = new List<BarValue>();
            values.Add(new BarValue(dr["EngineOnMin"].ToString()+"min" , Convert.ToDouble(dr["EngineOnMin"])));
            bvc.AddRange(values);
            sbc.Add(bvc);
            _BarData.Add(1, sbc);
        }

        #region Graph

        string CreateGraphContainers(int columns, int rows)
        {
            HtmlGenericControl mc = new HtmlGenericControl("div");
            mc.ID = "d5B_mc";
            HtmlGenericControl t = new HtmlGenericControl("table");
            t.Attributes.Add("cellpadding", "0");
            t.Attributes.Add("cellspacing", "0");
            int index = 0;
            for (int a = 0; a < rows; a++)
            {
                HtmlGenericControl r = new HtmlGenericControl("tr");
                for (int b = 0; b < columns; b++)
                {
                    HtmlGenericControl c = new HtmlGenericControl("td");
                    HtmlGenericControl cd = new HtmlGenericControl("div");
                    cd.Attributes.Add("class", "d5B_cc");

                    HtmlGenericControl h = new HtmlGenericControl("div");
                    h.Attributes.Add("class", "d5B_h");
                    HtmlGenericControl ht = new HtmlGenericControl("table");
                    ht.Attributes.Add("cellpadding", "0");
                    ht.Attributes.Add("cellspacing", "0");
                    HtmlGenericControl hr = new HtmlGenericControl("tr");

                    HtmlGenericControl hcl = new HtmlGenericControl("td");
                    hcl.Attributes.Add("class", "d5B_hcl");
                    HtmlGenericControl hdivl = new HtmlGenericControl("div");
                    hdivl.InnerHtml = _BarData[index].Title;


                    HtmlGenericControl hcr = new HtmlGenericControl("td");
                    hcr.Attributes.Add("class", "d5B_hcr");
                    hcr.Attributes.Add("style", "text-align:right;");
                    HtmlGenericControl hdivr = new HtmlGenericControl("div");
                    HtmlGenericControl hnbr = new HtmlGenericControl("nobr");
                    HtmlGenericControl hs = new HtmlGenericControl("span");
                    hs.InnerHtml = "Scale:&nbsp;";
                    hs.Attributes.Add("style", "font-size:7pt;font-weight:normal;");
                    HtmlGenericControl hin = new HtmlGenericControl("select");
                    hin.ID = string.Format("d5B_gin_" + index);
                    hin.Attributes.Add("onchange", "d5B_sel(this);");
                    StringBuilder options = new StringBuilder();
                    double[] sfs = new double[] { 0.01, 0.1, 1.0, 10, 100 };
                    for (int sf = 0; sf < sfs.Length; sf++)
                    {
                        options.Append("<option");
                        if (_BarData[index].Scale == sfs[sf])
                            options.Append(" selected");
                        options.Append(">");
                        options.Append(sfs[sf]);
                        options.Append("</option>");
                    }

                    hin.InnerHtml = options.ToString();

                    hcl.Controls.Add(hdivl);
                    hr.Controls.Add(hcl);

                    hs.Controls.Add(hnbr);
                    hs.Controls.Add(hin);
                    hdivr.Controls.Add(hs);
                    hcr.Controls.Add(hdivr);
                    hr.Controls.Add(hcr);
                    ht.Controls.Add(hr);
                    h.Controls.Add(ht);

                    //div.Controls.Add(h);

                    cd.Controls.Add(h);

                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.ID = string.Format("d5B_{0}", index);
                    div.Attributes.Add("class", "d5B_c");
                    cd.Controls.Add(div);
                    c.Controls.Add(cd);
                    index++;
                    r.Controls.Add(c);
                }
                t.Controls.Add(r);
            }

            mc.Controls.Add(t);

            return GetRenderedControl(mc);
        }


        public static string GetRenderedControl(Control control)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            control.RenderControl(writer);
            return writer.InnerWriter.ToString();
        }

        string CreateGraphArray()
        {
            //var valArr = { "101": new Array(new Array(50, "50%"), new Array(25, "25%"), new Array(10, "10%")), "102a": new Array(new Array(30, "30%"), new Array(34, "34%"), new Array(22, "22%")), "Unit Dallas 1": new Array(new Array(26, "26%"), new Array(33, "33%"), new Array(10, "10%")) };
            StringBuilder s = new StringBuilder();
            s.Append("var d5B_gda = new Array (");

            for (int a = 0; a < _BarData.Count; a++)
            {
                StackBarCollection sb = _BarData[a];
                StringBuilder colors = new StringBuilder(" new Array(");
                for (int b = 0; b < sb.Colors.Length; colors.AppendFormat(" \"{0}\",", sb.Colors[b++])) ;
                colors.Remove(colors.Length - 1, 1);
                colors.Append(")");

                StringBuilder values = new StringBuilder(" {");
                for (int c = 0; c < sb.Values.Length; c++)
                {
                    StringBuilder value = new StringBuilder();
                    BarValueCollection bvc = sb.Values[c];
                    values.AppendFormat(" \"{0}\": new Array(", bvc.Caption);
                    for (int d = 0; d < bvc.Count; d++)
                    {
                        BarValue bv = bvc[d];
                        value.AppendFormat(" new Array( \"{0}\", {1}),", bv.Caption, bv.Percentage);
                    }
                    value.Remove(value.Length - 1, 1);
                    value.Append(") }");
                    values.AppendLine(value.ToString());
                }
                values.Remove(values.Length - 1, 1);
                s.AppendFormat(" new Array( \"{0}\", {1}, {2}, {3} ),", sb.Title, sb.Scale, colors, values);
            }
            s.Remove(s.Length - 1, 1);
            s.AppendLine(" );");
            return s.ToString();
        }


        #endregion
    }
}
