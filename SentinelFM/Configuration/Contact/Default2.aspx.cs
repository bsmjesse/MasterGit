using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Configuration_Contact_Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
        //TimeZoneInfo.FindSystemTimeZoneById()
        //VLF.DAS.Logic.ContactManager cm = new VLF.DAS.Logic.ContactManager(sConnectionString);
        //System.Data.DataSet ds = cm.GetTimeZones();

        //foreach(DataRow dr in ds.Tables[0].Rows )
        //{
        //    var t = (from o in timeZones where o.Id == dr["TimeZoneId"].ToString() select o).Single();
        //    if (t == null)
        //    {
        //        string ss = "1";
        //    }
        //    else
        //    { 
        //        if ( t.DisplayName !=  dr["DisplayName"].ToString() ||
        //             t.StandardName != dr["StandardName"].ToString() ||
        //             t.DaylightName != dr["DaylightName"].ToString() ||
        //            t.SupportsDaylightSavingTime.ToString() != dr["IsDaylightSaving"].ToString() 
        //           )
        //        {
        //            string ss = "1";
        //        }
        //    }
        //}
        dg.DataSource =  timeZones;
        dg.DataBind();


    }
}