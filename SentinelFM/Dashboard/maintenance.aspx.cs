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

    public partial class DashBoard_maintenance : SentinelFMBasePage
    {
  

         public string strGraphData = "";
         public string strGridData = "";


        protected void Page_Load(object sender, EventArgs e)
        {
           // LoadMaintenance(ref strGraphData, ref strGridData);
        }




        //private void  LoadMaintenance(ref string strGraphData,ref string strGridData)
        //{
        //    try
        //    {
        //        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        //        MCCManager mccMgr = new MCCManager(sConnectionString);

         
        //        DataSet ds = new DataSet();

        //        ds=mccMgr.MaintenanceGetVehicleServices_DashBoard(sn.UserID);
        //        if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        //        {
        //            StringBuilder builder = new StringBuilder();
        //            builder.Append("data.push(");
        //            string tmp = "";
        //            tmp = "{name: 'Over 90',data: " + ds.Tables[0].Rows[0]["Over90"].ToString() + "},";
        //            builder.Append(tmp);

        //            tmp = "{name: 'Over 95',data: " + ds.Tables[0].Rows[0]["Over95"].ToString() + "},";
        //            builder.Append(tmp);


        //            tmp = "{name: 'Overdue',data: " + ds.Tables[0].Rows[0]["Overdue"].ToString() + "}";
        //            builder.Append(tmp);


        //            builder.Append(")");
        //            strGraphData = builder.ToString();


        //            builder = new StringBuilder();
        //            builder.Append("dataGrid.push([");

        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                tmp = "['" + ds.Tables[0].Rows[i]["VehicleDescription"].ToString().Replace("'", "`") + "','" + ds.Tables[0].Rows[i]["Description"].ToString().Replace("'", "`") + "','" + Math.Round(Convert.ToSingle(ds.Tables[0].Rows[i]["ServicePerc"]), 2).ToString() + "'," + ds.Tables[0].Rows[i]["StatusId"].ToString() + "]";

        //                builder.Append(tmp);
        //                if (i != ds.Tables[0].Rows.Count - 1)
        //                    builder.Append(",");

        //            }
        //            builder.Append(" ]);");

        //            strGridData = builder.ToString();
        //        }
        //        else
        //        {
        //            strGraphData = "";
        //            strGridData = "";
        //        }
                
        //    }
        //    catch
        //    {
        //        strGraphData = "";
        //        strGridData = "";
        //    }
        //}
    }
}