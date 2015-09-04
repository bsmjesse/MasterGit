using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using VLF.DAS.Logic;
using System.Configuration;
using System.Text;
using System.Data;

/// <summary>
/// Summary description for TestService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class dashboardService : System.Web.Services.WebService
{
    private VLF.DAS.Logic.DashBoard _dashboard = null;


    public class dataAHAItem
    {
        public string FleetName;
        public string Counter;
    }



    public class dataVItem
    {
        public string description;
        public int harshBraking;
        public int harshAcc;
        public int Speeding;
        public int Seatbelt;
        public int Reverse;
        public int TotalHarshBraking;
        public int TotalHarshAcceleration;
        public int TotalSpeeding;
        public int TotalSeatbelt;
        public int TotalReverse;
        public int Total;
    }



    public class dataIItem
    {
        public string description;
        public float idling;
        public float fuel;
        public float perc;
        public float TotalIdling;
        public float TotalFuel;
    }



    public class dataMItem
    {
        public string VehicleDescription;
        public string Description;
        public float ServicePerc;
        public int StatusId;
    }



    public class dataItem
    {
        public string name;
        public int data;
    }


    public dashboardService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }





    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadAHA(int TopFleets, int TopHours)
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        Report rpt = new Report(sConnectionString);
        DataSet ds = new DataSet();
        StringBuilder builder = new StringBuilder();
        SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];

        List<dataAHAItem> list = new List<dataAHAItem>();

        try
        {
            ds = rpt.DashBoard_AHA(sn.UserID, TopFleets, TopHours);
            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    list.Add(new dataAHAItem() { FleetName = ds.Tables[0].Rows[i]["FleetName"].ToString(), Counter = ds.Tables[0].Rows[i]["Counter"].ToString() });
                }

            }
            return list;
        }
        catch
        {
            return "";
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadViolationsDetails(int FleetId, int TopHours)
    {
        //string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        //Report rpt = new Report(sConnectionString);
        //DataSet ds = new DataSet();
        //SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        //List<dataVItem> list = new List<dataVItem>();
        //try
        //{

        //    DataSet dsFleets = new DataSet();
        //    dsFleets = sn.User.GetUserFleets(sn);
        //    //  DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
        //    // ds = rpt.Dashboard_Violations(Convert.ToInt32(drArr[0]["FleetId"]), System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now, sn.UserID);

        //    ds = rpt.Dashboard_Violations(FleetId, Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-TopHours), Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM"), sn.UserID);
        //    // Session["dsViolations"] = ds;

        //    if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        //    {

        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            list.Add(new dataVItem()
        //            {
        //                description = ds.Tables[0].Rows[i]["description"].ToString(),
        //                harshBraking = Convert.ToInt32(ds.Tables[0].Rows[i]["harshBraking"])
        //                ,
        //                harshAcc = Convert.ToInt32(ds.Tables[0].Rows[i]["harshAcc"])
        //                ,
        //                Speeding = Convert.ToInt32(ds.Tables[0].Rows[i]["Speeding"])
        //                ,
        //                Seatbelt = Convert.ToInt32(ds.Tables[0].Rows[i]["seatBelt"])
        //                ,
        //                Reverse = Convert.ToInt32(ds.Tables[0].Rows[i]["reverseViolations"])
        //                ,
        //                TotalHarshBraking = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalHarshBraking"])
        //                ,
        //                TotalHarshAcceleration = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalHarshAcceleration"])
        //                ,
        //                TotalSpeeding = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalSpeeding"])
        //                ,
        //                TotalSeatbelt = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalSeatbelt"])
        //               ,
        //                TotalReverse = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalReverse"])
        //                ,
        //                Total = Convert.ToInt32(ds.Tables[0].Rows[i]["Total"])
        //            });
        //        }

        //    }
        //    return list;
        //}
        //catch
        //{
        //    return "";
        //}

        return "";
    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadIdlingDetails(int FleetId, int TopHours)
    {
        //string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        //Report rpt = new Report(sConnectionString);
        //DataSet ds = new DataSet();
        //SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        //List<dataIItem> list = new List<dataIItem>();
        //try
        //{

        //    DataSet dsFleets = new DataSet();
        //    dsFleets = sn.User.GetUserFleets(sn);
        //    //  DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
        //    // ds = rpt.Dashboard_Violations(Convert.ToInt32(drArr[0]["FleetId"]), System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now, sn.UserID);

        //    //ds = rpt.Dashboard_Idling(FleetId, System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now);
        //    ds = rpt.Dashboard_Idling(FleetId, Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-TopHours), Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM"));
        //    // Session["dsIdling"] = ds; 

        //    if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        //    {

        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            list.Add(new dataIItem()
        //            {
        //                description = ds.Tables[0].Rows[i]["description"].ToString(),
        //                idling = Convert.ToSingle(ds.Tables[0].Rows[i]["idling"])
        //                ,
        //                fuel = Convert.ToSingle(ds.Tables[0].Rows[i]["idleFuel"])
        //                ,
        //                perc = Convert.ToSingle(ds.Tables[0].Rows[i]["perc"])
        //                ,
        //                TotalIdling = Convert.ToSingle(ds.Tables[0].Rows[i]["TotalIdling"])
        //                ,
        //                TotalFuel = Convert.ToSingle(ds.Tables[0].Rows[i]["TotalFuel"])
        //            });
        //        }

        //    }
        //    return list;
        //}
        //catch
        //{
        //    return "";
        //}

        return "";
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadViolationsSummary(int FleetId, int TopHours)
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        Report rpt = new Report(sConnectionString);
        DataSet ds = new DataSet();
        StringBuilder builder = new StringBuilder();
        SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        List<dataItem> list = new List<dataItem>();
        try
        {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            //DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
            //ds = rpt.Dashboard_Violations(Convert.ToInt32(drArr[0]["FleetId"]), System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now, sn.UserID);
            ds = rpt.Dashboard_Violations(FleetId, Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-TopHours), Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM"), sn.UserID);

            // ds = (DataSet)Session["dsViolations"]; 

            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {

                list.Add(new dataItem() { name = "Speeding", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalSpeeding"]) });
                list.Add(new dataItem() { name = "Harsh. Braking", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalHarshBraking"]) });
                list.Add(new dataItem() { name = "Harsh. Acc.", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalHarshAcceleration"]) });

                if (sn.User.OrganizationId == 951)
                {
                    list.Add(new dataItem() { name = "Seatbelt", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalSeatbelt"]) });
                    list.Add(new dataItem() { name = "Reverse", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalReverse"]) });
                }


            }
            else
            {
                list.Add(new dataItem() { name = "Speeding", data = 0 });
                list.Add(new dataItem() { name = "Harsh. Braking", data = 0 });
                list.Add(new dataItem() { name = "Harsh. Acc.", data = 0 });
            }
            return list;
        }
        catch
        {
            return "";
        }
    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadIdlingSummary(int FleetId, int TopHours)
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
        Report rpt = new Report(sConnectionString);
        DataSet ds = new DataSet();
        StringBuilder builder = new StringBuilder();
        SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        List<dataItem> list = new List<dataItem>();
        try
        {

            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);
            //DataRow[] drArr = dsFleets.Tables[0].Select("FleetName='All Vehicles'");
            //ds = rpt.Dashboard_Violations(Convert.ToInt32(drArr[0]["FleetId"]), System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now, sn.UserID);

            //ds = rpt.Dashboard_Idling(FleetId, System.DateTime.Now.AddHours(-TopHours), System.DateTime.Now);

            ds = rpt.Dashboard_Idling(FleetId, Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM").AddHours(-TopHours), Convert.ToDateTime(System.DateTime.Now.ToShortDateString() + " 12:00 AM"));


            // ds = (DataSet)Session["dsIdling"];

            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {

                list.Add(new dataItem() { name = "Total Fuel", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalFuel"]) });
                list.Add(new dataItem() { name = "Idling Hrs.", data = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalIdling"]) });


            }
            else
            {
                list.Add(new dataItem() { name = "Total Fuel", data = 0 });
                list.Add(new dataItem() { name = "Idling Hrs.", data = 0 });
            }
            return list;
        }
        catch
        {
            return "";
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadMaintenanceSummary(int FleetId)
    {
        try
        {
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            MCCManager mccMgr = new MCCManager(sConnectionString);
            List<dataItem> list = new List<dataItem>();
            SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
            DataSet ds = new DataSet();

            ds = mccMgr.MaintenanceGetVehicleServices_DashBoard(sn.UserID, FleetId);

            //ds=(DataSet)Session["dsMaintenance"];

            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {

                list.Add(new dataItem() { name = "Over 90", data = Convert.ToInt32(ds.Tables[0].Rows[0]["Over90"]) });
                list.Add(new dataItem() { name = "Over 95", data = Convert.ToInt32(ds.Tables[0].Rows[0]["Over95"]) });
                list.Add(new dataItem() { name = "Overdue", data = Convert.ToInt32(ds.Tables[0].Rows[0]["Overdue"]) });

            }
            else
            {
                list.Add(new dataItem() { name = "Over 90", data = 0 });
                list.Add(new dataItem() { name = "Over 95", data = 0 });
                list.Add(new dataItem() { name = "Overdue", data = 0 });

            }

            return list;
        }
        catch
        {
            return null;
        }

    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadMaintenanceDetails(int FleetId)
    {
        //try
        //{
        //    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        //    MCCManager mccMgr = new MCCManager(sConnectionString);
        //    List<dataMItem> list = new List<dataMItem>();
        //    SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        //    DataSet ds = new DataSet();

        //    ds = mccMgr.MaintenanceGetVehicleServices_DashBoard(sn.UserID, FleetId);

        //    //Session["dsMaintenance"] = ds;

        //    if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
        //    {


        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {

        //            list.Add(new dataMItem()
        //            {
        //                VehicleDescription = ds.Tables[0].Rows[i]["VehicleDescription"].ToString().Replace("'", ""),
        //                Description = ds.Tables[0].Rows[i]["Description"].ToString().Replace("'", "")
        //                ,
        //                ServicePerc = Convert.ToSingle(ds.Tables[0].Rows[i]["ServicePerc"])
        //                ,
        //                StatusId = Convert.ToInt16(ds.Tables[0].Rows[i]["StatusId"])

        //            });


        //        }
        //    }

        //    return list;
        //}
        //catch
        //{
        //    return null;
        //}
        return null;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,
          UseHttpGet = false, XmlSerializeString = false)]
    public object LoadFleets()
    {
        DataSet ds = new DataSet();
        SentinelFM.SentinelFMSession sn = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
        List<dataItem> list = new List<dataItem>();
        try
        {

            ds = sn.User.GetUserFleets(sn);
            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    list.Add(new dataItem() { name = ds.Tables[0].Rows[i]["FleetName"].ToString(), data = Convert.ToInt32(ds.Tables[0].Rows[i]["FleetId"]) });
                }

            }
            return list;
        }
        catch
        {
            return "";
        }
    }
}
