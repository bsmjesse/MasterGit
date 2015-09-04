using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Globalization;
using System.Text;
using VLF.DAS.Logic;
using System.Configuration;

namespace SentinelFM
{

    public partial class DashBoard_idling : SentinelFMBasePage
    {
  

         public string strGraphData = "";
         public string strGridData = "";
         public string strFleets = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            LoadIdling(ref strFleets, ref strGraphData, ref strGridData);
        }


        
        private void LoadIdling(ref string strFleets , ref string strGraphData, ref string strGridData)
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
                Report rpt = new Report(sConnectionString);
                DataSet ds = new DataSet();
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                //DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
                Int32 fleetId = 0;
                DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
                if (drArr != null)
                    fleetId = Convert.ToInt32(drArr[0]["FleetId"]);
                else
                    fleetId = Convert.ToInt32(dsFleets.Tables[0].Rows[0]["FleetId"]); 

                string tmp = "";
                StringBuilder builder = new StringBuilder();
                builder.Append("fleetList.push([");


                     for (int i = 0; i < dsFleets.Tables[0].Rows.Count; i++)
                     {
                         tmp = "[" + dsFleets.Tables[0].Rows[i]["FleetId"].ToString() + ",'" + dsFleets.Tables[0].Rows[i]["FleetName"].ToString().Replace("'", "`") + "']";

                         builder.Append(tmp);
                         if (i != dsFleets.Tables[0].Rows.Count - 1)
                             builder.Append(",");

                     }
                     builder.Append(" ]);");

                     strFleets = builder.ToString();


                     ds = rpt.Dashboard_Idling(fleetId, System.DateTime.Now.AddHours(-36), System.DateTime.Now);
                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                {
                     builder = new StringBuilder();
                    builder.Append("data.push(");
                    
                    tmp = "{name: 'Idling Hrs.',data: " + ds.Tables[0].Rows[0]["TotalIdling"].ToString() + "},";
                    builder.Append(tmp);

                    tmp = "{name: 'Engine Hrs.',data: " + ds.Tables[0].Rows[0]["TotalEngine"].ToString() + "}";
                    builder.Append(tmp);


                    builder.Append(")");
                    strGraphData = builder.ToString();


                    builder = new StringBuilder();
                    builder.Append("dataGrid.push([");

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmp = "['" + ds.Tables[0].Rows[i]["description"].ToString().Replace("'","`")  + "'," + ds.Tables[0].Rows[i]["engine"].ToString() + "," + ds.Tables[0].Rows[i]["idling"].ToString() + ",'" + Math.Round(Convert.ToSingle(ds.Tables[0].Rows[i]["perc"]), 2).ToString() + "']";

                        builder.Append(tmp);
                        if (i != ds.Tables[0].Rows.Count - 1)
                            builder.Append(",");

                    }
                    builder.Append(" ]);");

                    strGridData = builder.ToString();
                  
                }
                else
                {
                    strGraphData = "";
                    strGridData = "";
                    strFleets = "";
                }
                
            }
            catch (Exception Ex)
            {

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //strGraphData = "";
                //strGridData = "";
                //strFleets = "";
            }
        }
    }
}