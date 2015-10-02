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
    public partial class DBoard_Pie : SentinelFMBasePage
    {
        public string strData = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            //strData = LoadMaintenance();
        }



        private string LoadDTCcodes()
        {
            string xml = "";
            ServerDBVehicle.DBVehicle vehicleProxy = new ServerDBVehicle.DBVehicle();
            try
            {
                if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, -999, sn.User.DefaultFleet, System.DateTime.Now.AddDays(-1), System.DateTime.Now, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    if (objUtil.ErrCheck(vehicleProxy.GetJ1708CodesVehicleFleet(sn.UserID, sn.SecId, -999, sn.User.DefaultFleet, System.DateTime.Now.AddDays(-1), System.DateTime.Now, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                    {
                        return "";
                    }

                if (xml == "")
                    return "";

                StringReader strrXML = new StringReader(xml);
                DataSet ds = new DataSet();

                string strPath = Server.MapPath("../Datasets/DTCcodes.xsd");
                ds.ReadXmlSchema(strPath);
                ds.ReadXml(strrXML);



                StringBuilder builder = new StringBuilder();
                builder.Append("data.push(");
                string tmp = "";

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    tmp = "{name:'" + ds.Tables[0].Rows[i]["Description"].ToString() + " - [" + ds.Tables[0].Rows[i]["code1"].ToString() + "]',data: " + ds.Tables[0].Rows[i]["counter"].ToString() + "}";
                    builder.Append(tmp);
                    if (i != ds.Tables[0].Rows.Count - 1)
                        builder.Append(",");

                }
                builder.Append(")");

                return builder.ToString();
            }
            catch
            {
                return "";
            }
        }



        //private string LoadMaintenance()
        //{
        //    try
        //    {
        //        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        //        MCCManager mccMgr = new MCCManager(sConnectionString);

         
        //        DataSet ds = new DataSet();

        //        ds=mccMgr.MaintenanceGetVehicleServices_DashBoard(sn.UserID);
        //        if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 1))
        //        {
        //            StringBuilder builder = new StringBuilder();
        //            builder.Append("data.push(");
        //            string tmp = "";
        //            tmp = "{name: 'Over 50',data: " + ds.Tables[0].Rows[0]["Over50"].ToString() + "},";
        //            builder.Append(tmp);

        //            tmp = "{name: 'Over 90',data: " + ds.Tables[0].Rows[0]["Over90"].ToString() + "},";
        //            builder.Append(tmp);


        //            tmp = "{name: 'Overdue',data: " + ds.Tables[0].Rows[0]["Overdue"].ToString() + "}";
        //            builder.Append(tmp);


        //            builder.Append(")");
        //            return builder.ToString();
        //        }
        //        else return "";
                
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}
    }
}


