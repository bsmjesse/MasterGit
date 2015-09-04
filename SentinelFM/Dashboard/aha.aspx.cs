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
    public partial class DBoard_aha : SentinelFMBasePage
    {
        public string strData ="";

        protected void Page_Load(object sender, EventArgs e)
        {
            strData = LoadAHA();
        }



        private string LoadAHA()
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            Report rpt = new Report(sConnectionString);
            DataSet ds = new DataSet();
            StringBuilder builder = new StringBuilder();

            try
            {
                //ds = rpt.DashBoard_AHA (sn.UserID);
                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 1))
                {
                    builder.Append("data.push(");
                    string tmp = "";

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmp = "{name:'" + ds.Tables[0].Rows[i]["FleetName"].ToString() + "',data: " + ds.Tables[0].Rows[i]["Counter"].ToString() + "}";
                        builder.Append(tmp);
                        if (i != ds.Tables[0].Rows.Count - 1)
                            builder.Append(",");

                    }
                    builder.Append(")");
                }
                return builder.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}


