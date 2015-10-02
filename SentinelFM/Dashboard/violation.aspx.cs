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

    public partial class DashBoard_violation : SentinelFMBasePage
    {
  

         public string strGraphData = "";
         public string strGridData = "";
         public string strFleets = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            LoadViolations(ref strFleets, ref strGraphData, ref strGridData);
        }


        
        private void LoadViolations(ref string strFleets , ref string strGraphData, ref string strGridData)
        {
            try
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;
                Report rpt = new Report(sConnectionString);
                DataSet ds = new DataSet();
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
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


                     ds = rpt.Dashboard_Violations(Convert.ToInt32(drArr[0]["FleetId"]), System.DateTime.Now.AddHours (-36), System.DateTime.Now, sn.UserID);
                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                {
                     builder = new StringBuilder();
                    builder.Append("data.push(");


                    tmp = "{name: 'Speeding',data: " + ds.Tables[0].Rows[0]["TotalSpeeding"].ToString() + "},";
                    builder.Append(tmp);

                    tmp = "{name: 'Ext. Braking',data: " + ds.Tables[0].Rows[0]["TotalExtrBraking"].ToString() + "},";
                    builder.Append(tmp);

                    tmp = "{name: 'Ext. Acc.',data: " + ds.Tables[0].Rows[0]["TotalExtrAcceleration"].ToString() + "}";
                    builder.Append(tmp);


                               

                    builder.Append(")");
                    strGraphData = builder.ToString();


                    builder = new StringBuilder();
                    builder.Append("dataGrid.push([");

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmp = "['" + ds.Tables[0].Rows[i]["description"].ToString().Replace("'", "`") + "'," + ds.Tables[0].Rows[i]["Speeding"].ToString() + "," +  ds.Tables[0].Rows[i]["extBraking"].ToString() + "," + ds.Tables[0].Rows[i]["extAcc"].ToString() + "," + ds.Tables[0].Rows[i]["Total"].ToString() + "]";

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
            catch
            {
                strGraphData = "";
                strGridData = "";
                strFleets = "";
            }
        }
    }
}