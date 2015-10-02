using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VLF.DAS;
using SentinelMobile.Models;
using System.Data;
using SentinelMobile.Models;
using System.Collections;
using System.Web.Script.Serialization;
using System.Text;
using System.Globalization;
using System.IO;
using System.Data.SqlClient;
namespace SentinelMobile.Controllers
{
    public class HOSController : Controller
    {
        //
        // GET: /HOS/
        string hosConstr = ConfigurationManager.ConnectionStrings["SentinelHOSConnection"].ToString();
        protected SQLExecuter sqlExec;
        User user;
        public HOSController()
        {
            this.sqlExec = new SQLExecuter(hosConstr);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(user.DefaultLanguage);


            if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
            {
                ViewBag.SelectedLanguage = "fr";
                ViewBag.SelectedLanguageUrl = "../Scripts/jquery.mobile.datebox.i18n.fr.utf8.js";
            }
            else
            {
                ViewBag.SelectedLanguage = "en";
                ViewBag.SelectedLanguageUrl = "../Scripts/jquery.mobile.datebox.i18n.en.utf8.js";
            }

        }

        public ActionResult Index()
        {
            AddScript();
            return View();
        }

        [Authorize]
        public ActionResult getAllPending()
        {
            AddScript();
            List<DriverPending> dpl = new List<DriverPending>();

            ////for (int i = 0; i < 10;i++)
            ////{
            ////    DriverPending d = new DriverPending();
            ////    d.DriverID = i.ToString();
            ////    d.DriverName = "Name" + i.ToString();
            ////    d.time = "22/11/2013";
            ////    dpl.Add(d);
            ////}

            string dateFormat = "M/d/yyyy";
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
            {
                dateFormat = "d/M/yyyy";
            }
            try
            {
                string sql = "usp_mb_GetPendingManualLogsByUser";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, user.UserId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, user.OrganizationId);

                DataSet ds = sqlExec.SPExecuteDataset(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        DriverPending d = new DriverPending();
                       
                        if (dr["DriverName"] != DBNull.Value &&
                            dr["Date"] != DBNull.Value)
                        {
                            d.dID = dr["DriverId"].ToString();
                            d.dName = dr["DriverName"].ToString();
                            d.rID = dr["refID"].ToString();
                            string s = ((DateTime)dr["Date"]).ToString(dateFormat);
                            //try
                            //{
                            //    string[] s1 = s.Split(' ');
                            //    if (s1.Length >= 1)
                            //        s = s1[0];
                            //}
                            //catch (Exception) { }
                            d.time = s; 
                            
                            dpl.Add(d);
                            
                        }
                    }
                }
            }
            catch (Exception )
            {
                return Json("-1", JsonRequestBehavior.AllowGet);
            }

            JavaScriptSerializer JSS1 = new JavaScriptSerializer();
            return Json(JSS1.Serialize(dpl), JsonRequestBehavior.AllowGet);

        }
        [Authorize]
        public ActionResult DriverPendingList()
        {

            AddScript();
            return View();
        }
        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult Approve_LogData_AllPending(string alldpl)
        {
            List<Duty> modelDuty = new List<Duty>();

            //List<DriverPending> dpl = ViewBag.DriverPendingList;
            JavaScriptSerializer JSS = new JavaScriptSerializer();
            List<DriverPending> dpl = JSS.Deserialize<List<DriverPending>>(alldpl);


            bool rv=true;
            if (dpl.Count > 0)
            {
                SqlConnection conn = null;
                SqlCommand cmd = null;
                try{
                    conn = new SqlConnection(sqlExec.ConnectionString);
                    conn.Open();
                    cmd = new SqlCommand("usp_hos_Approve_LogData_Manual", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (DriverPending obj in dpl)
                    {
                        try
                        {
                            string dataFormat = "M/d/yyyy";
                            if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                            {
                                dataFormat = "d/M/yyyy";
                            }

                            //JavaScriptSerializer JSS = new JavaScriptSerializer();
                            //List<String> dutiesObj = JSS.Deserialize<List<String>>(duties);
                            DateTime curDate = DateTime.MinValue;
                            DateTime dateStart = DateTime.MaxValue;
                            DateTime dateEnd = DateTime.MinValue;
                           
                            {
                                curDate = DateTime.ParseExact(obj.time,dataFormat, new CultureInfo("en-US"));
                                if (dateStart > curDate) dateStart = curDate;
                                if (dateEnd < curDate) dateEnd = curDate;
                            }

                            DataSet ds = null;
                            
                           
                                
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@Driver", SqlDbType.VarChar).Value = obj.dID/*user.DriverId*/;
                                    cmd.Parameters.Add("@DateStart", SqlDbType.DateTime).Value = dateStart;
                                    cmd.Parameters.Add("@DateEnd", SqlDbType.DateTime).Value = dateEnd;
                                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user.UserId;
                                    
                                    
                                    cmd.ExecuteNonQuery();
                                    
                                }
                           
                        }
                        catch (Exception ex)
                        {
                            rv=false;
                        }
                    }//foreach
                }
                catch(Exception e)
                {
                   
                    rv=false;
                }
                finally
                {
                     if(conn!=null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                    if (cmd != null)
                        cmd.Dispose();
                }
    

            }
            else 
                rv=false;

           
           

            if(rv)
                 return Json("1", JsonRequestBehavior.AllowGet);
            else
                return Json("-1", JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult DriverList()
        {
            AddScript();
            List<Driver> driverList = new List<Driver>();
            try
            {
                string sql = "usp_mb_GetDriversByUser";
                sqlExec.ClearCommandParameters();
                sqlExec.AddCommandParam("@UserId", SqlDbType.Int, user.UserId);
                sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, user.OrganizationId);

                DataSet ds = sqlExec.SPExecuteDataset(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["DriverName"] != DBNull.Value && 
                            dr["DriverID"] != DBNull.Value)
                            driverList.Add(new Driver(dr["DriverID"].ToString(), dr["DriverName"].ToString()));
                    }
                }
            }
            catch(Exception ex) {
                //Need to log
                throw new Exception(ex.Message);
            }
            ViewBag.DriverList = driverList;
            user.DriverId = null;
            user.DriverName = null;
            return View();
        }

         [Authorize]
         public ActionResult DriverLogsPdf(string filename)
         {
             filename = Server.UrlDecode(filename);
             Hos_Ws.HOSFileReteriver ws = new Hos_Ws.HOSFileReteriver();
             string[] files = filename.Split('@');
             byte[] filedata = ws.ReadFile(files[0]);
             string downloadName = files[0];
             if (files.Length > 1) downloadName = files[1];
             return File(filedata, "application/pdf");
         }

         [Authorize]
         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult DriverLogsWritePdf(string filename)
         {
             try
             {
                 DeletePDF();
                 filename = Server.UrlDecode(filename);
                 string[] files = filename.Split('@');
                 filename = files[0];
                 Hos_Ws.HOSFileReteriver ws = new Hos_Ws.HOSFileReteriver();

                 byte[] filedata = ws.ReadFile(filename);
                 filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                 string path = Server.MapPath("~/PDF/") + filename ;
                 if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                 using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, filedata.Length, false  ))
                 {
                     fileStream.Write(filedata, 0, filedata.Length);
                 }
                 System.Threading.Thread.Sleep(2000);
                 filename = filename + "?time=" + System.DateTime.Now.Ticks.ToString();
             }
             catch (Exception ex)
             {
                 return Json("0", JsonRequestBehavior.AllowGet);
             }
             return Json(filename, JsonRequestBehavior.AllowGet);
         }

         [Authorize]
         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult Approve_LogData_Manual(string duties)
         {
             List<Duty> modelDuty = new List<Duty>();
             if (user.DriverId != null)
             {

                 try
                 {
                     string dataFormat = "M/d/yyyy";
                     if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                     {
                         dataFormat = "d/M/yyyy";
                     }

                     JavaScriptSerializer JSS = new JavaScriptSerializer();
                     List<String> dutiesObj = JSS.Deserialize<List<String>>(duties);
                     DateTime curDate = DateTime.MinValue;
                     DateTime dateStart = DateTime.MaxValue;
                     DateTime dateEnd = DateTime.MinValue;
                     foreach (String duty in dutiesObj)
                     {
                         curDate = DateTime.ParseExact(duty,
                                                      dataFormat, new CultureInfo("en-US"));
                         if (dateStart > curDate) dateStart = curDate;
                         if (dateEnd < curDate) dateEnd = curDate;
                     }

                     DataSet ds = null;
                     string sql = "usp_hos_Approve_LogData_Manual";
                     using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
                     {
                         using (SqlCommand cmd = new SqlCommand("usp_hos_Approve_LogData_Manual", conn))
                         {
                             cmd.Parameters.Add("@Driver", SqlDbType.VarChar).Value = user.DriverId;
                             cmd.Parameters.Add("@DateStart", SqlDbType.DateTime).Value = dateStart;
                             cmd.Parameters.Add("@DateEnd", SqlDbType.DateTime).Value = dateEnd;
                             cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user.UserId;
                             conn.Open();
                             cmd.CommandType = CommandType.StoredProcedure;
                             cmd.ExecuteNonQuery();
                             conn.Close();
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     return Json("-1", JsonRequestBehavior.AllowGet);
                 }
             }
             else return Json("-1", JsonRequestBehavior.AllowGet);

             JavaScriptSerializer JSS1 = new JavaScriptSerializer();
             return Json("1", JsonRequestBehavior.AllowGet);
         }

         [Authorize]
         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult GetLogData_Reference_ManualDutyByDate(string duties)
         {
             List<Duty> modelDuty = new List<Duty>();
             if (user.DriverId != null)
             {

                 try
                 {
                     string dataFormat = "M/d/yyyy";
                     if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                     {
                         dataFormat = "d/M/yyyy";
                     }

                     JavaScriptSerializer JSS = new JavaScriptSerializer();
                     List<String> dutiesObj = JSS.Deserialize<List<String>>(duties);
                     DateTime curDate = DateTime.MinValue;
                     DateTime dateStart = DateTime.MaxValue;
                     DateTime dateEnd = DateTime.MinValue;
                     foreach (String duty in dutiesObj)
                     {
                         curDate = DateTime.ParseExact(duty,
                                                      dataFormat, new CultureInfo("en-US"));
                         if (dateStart > curDate) dateStart = curDate;
                         if (dateEnd < curDate) dateEnd = curDate;
                     }

                     DataSet ds = null;
                     string sql = "usp_hos_GetLogData_Reference_ManualDutyByDate";
                     sqlExec.ClearCommandParameters();
                     sqlExec.AddCommandParam("@DateStart", SqlDbType.DateTime, dateStart);
                     sqlExec.AddCommandParam("@DateEnd", SqlDbType.DateTime, dateEnd);
                     sqlExec.AddCommandParam("@driver", SqlDbType.VarChar, user.DriverId);
                     ds = sqlExec.SPExecuteDataset(sql);

                     string timeFormat = "HH:mm";

                     foreach (DataRow drDuty in ds.Tables[0].Rows)
                     {
                         Duty duty = new Models.Duty();
                         duty.date = ((DateTime)drDuty["time"]).Date.ToString(dataFormat);
                         if (drDuty["Cycle"] != DBNull.Value)
                             duty.cycle = drDuty["Cycle"].ToString();
                         if (drDuty["time"] != DBNull.Value)
                             duty.time = ((DateTime)drDuty["time"]).ToString(timeFormat);

                         if (drDuty["duty"] != DBNull.Value)
                             duty.duty = drDuty["duty"].ToString();

                         if (drDuty["action"] != DBNull.Value)
                             duty.action = drDuty["action"].ToString();

                         if (drDuty["province"] != DBNull.Value)
                             duty.province = drDuty["province"].ToString();

                         if (drDuty["city"] != DBNull.Value)
                             duty.city = drDuty["city"].ToString();

                         if (drDuty["remark"] != DBNull.Value)
                             duty.remark = drDuty["remark"].ToString();

                         if (drDuty["odometer"] != DBNull.Value)
                             duty.odometer = drDuty["odometer"].ToString();

                         if (drDuty["actionTxt"] != DBNull.Value)
                             duty.actionTxt = drDuty["actionTxt"].ToString();

                         if (drDuty["cityTxt"] != DBNull.Value)
                             duty.cityTxt = drDuty["cityTxt"].ToString();

                         if (drDuty["provinceTxt"] != DBNull.Value)
                             duty.provinceTxt = drDuty["provinceTxt"].ToString();

                         if (drDuty["provinceTxt"] != DBNull.Value)
                             duty.provinceTxt = drDuty["provinceTxt"].ToString();

                         if (drDuty["TLID"] != DBNull.Value)
                             duty.TLID = drDuty["TLID"].ToString();


                         if (duty.duty == "101") duty.dutyTxt = Resources.HOSResources.OffDuty;
                         if (duty.duty == "102") duty.dutyTxt = Resources.HOSResources.Sleep;
                         if (duty.duty == "103") duty.dutyTxt = Resources.HOSResources.Driving;
                         if (duty.duty == "104") duty.dutyTxt = Resources.HOSResources.OnDuty;
                         modelDuty.Add(duty);
                     }


                 }
                 catch (Exception ex)
                 {
                     return Json("-1", JsonRequestBehavior.AllowGet);
                 }
             }
             else return Json("-1", JsonRequestBehavior.AllowGet);

             JavaScriptSerializer JSS1 = new JavaScriptSerializer();
             return Json(JSS1.Serialize(modelDuty), JsonRequestBehavior.AllowGet);
         }

        //--------------------------------------------------------
         ActionResult _addDriverLogByRef(string refid,string user_DriverId,string user_DriverName,int flag=0)
         {
             Driver driver = new Models.Driver(user_DriverId, user_DriverName);
             ViewBag.Driver = driver;
             List<States> states = new List<States>();
             string dataFormat = "M/d/yyyy";
             if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
             {
                 dataFormat = "d/M/yyyy";
             }
             try
             {
                 DataSet ds = null;
                 string sql = "usp_hos_GetLogData_Reference_ManualByRefId";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@refId", SqlDbType.BigInt, Int64.Parse(refid));
                 sqlExec.AddCommandParam("@driver", SqlDbType.VarChar, driver.DriverID);
                 ds = sqlExec.SPExecuteDataset(sql);

                 if (ds.Tables[0].Rows.Count == 0) return RedirectToAction("DriverList", "HOS");
                 TripInfo tripInfo = new Models.TripInfo();
                 DataRow drTripInfo = ds.Tables[0].Rows[0];
                 if (drTripInfo["CompanyName"] != DBNull.Value)
                     tripInfo.company = drTripInfo["CompanyName"].ToString();

                 if (drTripInfo["Date"] != DBNull.Value)
                     tripInfo.date = ((DateTime)drTripInfo["Date"]).ToString(dataFormat);

                 //dataFormat = dataFormat + " HH:mm";
                 dataFormat = "HH:mm";
                 if (drTripInfo["ShiftStart"] != DBNull.Value)
                     tripInfo.shiftStart = ((DateTime)drTripInfo["ShiftStart"]).ToString(dataFormat);

                 if (drTripInfo["OdometerStart"] != DBNull.Value)
                     tripInfo.odoStart = drTripInfo["OdometerStart"].ToString();

                 if (drTripInfo["OdometerEnd"] != DBNull.Value)
                     tripInfo.odoEnd = drTripInfo["OdometerEnd"].ToString();

                 if (drTripInfo["CoDriverName"] != DBNull.Value)
                     tripInfo.coDriver = drTripInfo["CoDriverName"].ToString();

                 if (drTripInfo["HomeTerminalAddress"] != DBNull.Value)
                     tripInfo.hometerm = drTripInfo["HomeTerminalAddress"].ToString();

                 if (drTripInfo["EquipmentNo"] != DBNull.Value)
                     tripInfo.equipmentNo = drTripInfo["EquipmentNo"].ToString();

                 if (drTripInfo["ShippingDocument"] != DBNull.Value)
                     tripInfo.shippingDoc = drTripInfo["ShippingDocument"].ToString();

                 if (drTripInfo["TruckNo"] != DBNull.Value)
                     tripInfo.truckNo = drTripInfo["TruckNo"].ToString();

                 if (drTripInfo["TrailerNo"] != DBNull.Value)
                     tripInfo.tailerNo = drTripInfo["TrailerNo"].ToString();

                 if (drTripInfo["Cycle"] != DBNull.Value)
                     tripInfo.cycle = drTripInfo["Cycle"].ToString();

                 if (drTripInfo["Deffered"] != DBNull.Value)
                     tripInfo.deferredHours = drTripInfo["Deffered"].ToString();

                 if (drTripInfo["DeferedDay"] != DBNull.Value)
                     tripInfo.deferredDay = drTripInfo["DeferedDay"].ToString();

                 if (drTripInfo["Emergency"] != DBNull.Value)
                     tripInfo.emergency = drTripInfo["Emergency"].ToString();

                 if (drTripInfo["Remark"] != DBNull.Value)
                     tripInfo.remark = drTripInfo["Remark"].ToString();

                 if (drTripInfo["PersonalStartOdo"] != DBNull.Value)
                     tripInfo.personalStart = drTripInfo["PersonalStartOdo"].ToString();

                 if (drTripInfo["PersonalEndOdo"] != DBNull.Value)
                     tripInfo.personalEnd = drTripInfo["PersonalEndOdo"].ToString();

                 if (drTripInfo["AdverseDrivingCondition"] != DBNull.Value)
                     tripInfo.adverseDriving = drTripInfo["AdverseDrivingCondition"].ToString();

                 if (drTripInfo["Approved"] != DBNull.Value)
                     tripInfo.approved = drTripInfo["Approved"].ToString().ToLower();
                 //if (user.IsDriver) tripInfo.approved = "true";

                 ViewBag.tripInfo = tripInfo;

                 List<Duty> duties = new List<Models.Duty>();
                 foreach (DataRow drDuty in ds.Tables[1].Rows)
                 {
                     Duty duty = new Models.Duty();
                     duty.date = tripInfo.date;
                     duty.cycle = tripInfo.cycle;
                     if (drDuty["time"] != DBNull.Value)
                         duty.time = ((DateTime)drDuty["time"]).ToString(dataFormat);

                     if (drDuty["duty"] != DBNull.Value)
                         duty.duty = drDuty["duty"].ToString();

                     if (drDuty["action"] != DBNull.Value)
                         duty.action = drDuty["action"].ToString();

                     if (drDuty["province"] != DBNull.Value)
                         duty.province = drDuty["province"].ToString();

                     if (drDuty["city"] != DBNull.Value)
                         duty.city = drDuty["city"].ToString();

                     if (drDuty["remark"] != DBNull.Value)
                         duty.remark = drDuty["remark"].ToString();

                     if (drDuty["odometer"] != DBNull.Value)
                         duty.odometer = drDuty["odometer"].ToString();

                     if (drDuty["actionTxt"] != DBNull.Value)
                         duty.actionTxt = drDuty["actionTxt"].ToString();

                     if (drDuty["cityTxt"] != DBNull.Value)
                         duty.cityTxt = drDuty["cityTxt"].ToString();

                     if (drDuty["provinceTxt"] != DBNull.Value)
                         duty.provinceTxt = drDuty["provinceTxt"].ToString();

                     if (drDuty["provinceTxt"] != DBNull.Value)
                         duty.provinceTxt = drDuty["provinceTxt"].ToString();

                     if (drDuty["TLID"] != DBNull.Value)
                         duty.TLID = drDuty["TLID"].ToString();


                     if (duty.duty == "101") duty.dutyTxt = Resources.HOSResources.OffDuty;
                     if (duty.duty == "102") duty.dutyTxt = Resources.HOSResources.Sleep;
                     if (duty.duty == "103") duty.dutyTxt = Resources.HOSResources.Driving;
                     if (duty.duty == "104") duty.dutyTxt = Resources.HOSResources.OnDuty;
                     duties.Add(duty);
                 }

                 ViewBag.duties = duties;

                 ds = null;
                 sql = "usp_hos_GetStates";
                 sqlExec.ClearCommandParameters();
                 ds = sqlExec.SPExecuteDataset(sql);

                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                     {
                         DataRow dr = ds.Tables[0].Rows[index];
                         states.Add(new States(dr["FullName"].ToString(), dr["ID"].ToString()));
                     }
                 }
             }
             catch (Exception ex)
             {
                 //Need to log
                 throw new Exception(ex.Message);
             }
             ViewBag.States = states;
             ViewBag.back_state = flag;
             user.RefId = refid;
             user.DriverId = user_DriverId;
             return View("ModifyDriverLog");
         }

         [Authorize]
         public ActionResult AddDplLogByRef(string rid,string did,string dName)
         {
             //user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
             if (string.IsNullOrEmpty(did)) return RedirectToAction("DriverList", "HOS");
             AddScript();
             return _addDriverLogByRef(rid,did,dName,4);
         }

         [Authorize]
         public ActionResult AddDriverLogByRef(string refid)
         {
             //user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
             if (string.IsNullOrEmpty(user.DriverId)) return RedirectToAction("DriverList", "HOS");
             AddScript();
             return _addDriverLogByRef(refid,user.DriverId,user.DriverName);
             //Driver driver = new Models.Driver(user.DriverId, user.DriverName);
             //ViewBag.Driver = driver;
             //List<States> states = new List<States>();
             //string dataFormat = "M/d/yyyy";
             //if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
             //{
             //    dataFormat = "d/M/yyyy";
             //}
             //try
             //{
             //    DataSet ds = null;
             //    string sql = "usp_hos_GetLogData_Reference_ManualByRefId";
             //    sqlExec.ClearCommandParameters();
             //    sqlExec.AddCommandParam("@refId", SqlDbType.BigInt, Int64.Parse(refid));
             //    sqlExec.AddCommandParam("@driver", SqlDbType.VarChar, driver.DriverID);
             //    ds = sqlExec.SPExecuteDataset(sql);

             //    if (ds.Tables[0].Rows.Count == 0) return RedirectToAction("DriverList", "HOS");
             //    TripInfo tripInfo = new Models.TripInfo();
             //    DataRow drTripInfo = ds.Tables[0].Rows[0];
             //    if (drTripInfo["CompanyName"] != DBNull.Value)
             //        tripInfo.company = drTripInfo["CompanyName"].ToString();

             //    if (drTripInfo["Date"] != DBNull.Value)
             //        tripInfo.date = ((DateTime)drTripInfo["Date"]).ToString(dataFormat);

             //    //dataFormat = dataFormat + " HH:mm";
             //    dataFormat = "HH:mm";
             //    if (drTripInfo["ShiftStart"] != DBNull.Value)
             //        tripInfo.shiftStart = ((DateTime)drTripInfo["ShiftStart"]).ToString(dataFormat);

             //    if (drTripInfo["OdometerStart"] != DBNull.Value)
             //        tripInfo.odoStart = drTripInfo["OdometerStart"].ToString();

             //    if (drTripInfo["OdometerEnd"] != DBNull.Value)
             //        tripInfo.odoEnd = drTripInfo["OdometerEnd"].ToString();

             //    if (drTripInfo["CoDriverName"] != DBNull.Value)
             //        tripInfo.coDriver = drTripInfo["CoDriverName"].ToString();

             //    if (drTripInfo["HomeTerminalAddress"] != DBNull.Value)
             //        tripInfo.hometerm = drTripInfo["HomeTerminalAddress"].ToString();

             //    if (drTripInfo["EquipmentNo"] != DBNull.Value)
             //        tripInfo.equipmentNo = drTripInfo["EquipmentNo"].ToString();

             //    if (drTripInfo["ShippingDocument"] != DBNull.Value)
             //        tripInfo.shippingDoc = drTripInfo["ShippingDocument"].ToString();

             //    if (drTripInfo["TruckNo"] != DBNull.Value)
             //        tripInfo.truckNo = drTripInfo["TruckNo"].ToString();

             //    if (drTripInfo["TrailerNo"] != DBNull.Value)
             //        tripInfo.tailerNo = drTripInfo["TrailerNo"].ToString();

             //    if (drTripInfo["Cycle"] != DBNull.Value)
             //        tripInfo.cycle = drTripInfo["Cycle"].ToString();

             //    if (drTripInfo["Deffered"] != DBNull.Value)
             //        tripInfo.deferredHours = drTripInfo["Deffered"].ToString();

             //    if (drTripInfo["DeferedDay"] != DBNull.Value)
             //        tripInfo.deferredDay = drTripInfo["DeferedDay"].ToString();

             //    if (drTripInfo["Emergency"] != DBNull.Value)
             //        tripInfo.emergency = drTripInfo["Emergency"].ToString();

             //    if (drTripInfo["Remark"] != DBNull.Value)
             //        tripInfo.remark = drTripInfo["Remark"].ToString();

             //    if (drTripInfo["PersonalStartOdo"] != DBNull.Value)
             //        tripInfo.personalStart = drTripInfo["PersonalStartOdo"].ToString();

             //    if (drTripInfo["PersonalEndOdo"] != DBNull.Value)
             //        tripInfo.personalEnd = drTripInfo["PersonalEndOdo"].ToString();

             //    if (drTripInfo["AdverseDrivingCondition"] != DBNull.Value)
             //        tripInfo.adverseDriving = drTripInfo["AdverseDrivingCondition"].ToString();

             //    if (drTripInfo["Approved"] != DBNull.Value)
             //        tripInfo.approved = drTripInfo["Approved"].ToString().ToLower();
             //    //if (user.IsDriver) tripInfo.approved = "true";

             //    ViewBag.tripInfo = tripInfo;

             //    List<Duty> duties = new List<Models.Duty>();
             //    foreach (DataRow drDuty in ds.Tables[1].Rows)
             //    { 
             //        Duty duty = new Models.Duty();
             //        duty.date = tripInfo.date;
             //        duty.cycle = tripInfo.cycle;
             //        if (drDuty["time"] != DBNull.Value)
             //            duty.time = ((DateTime)drDuty["time"]).ToString(dataFormat);

             //        if (drDuty["duty"] != DBNull.Value)
             //            duty.duty = drDuty["duty"].ToString();

             //        if (drDuty["action"] != DBNull.Value)
             //            duty.action = drDuty["action"].ToString();

             //        if (drDuty["province"] != DBNull.Value)
             //            duty.province = drDuty["province"].ToString();

             //        if (drDuty["city"] != DBNull.Value)
             //            duty.city = drDuty["city"].ToString();

             //        if (drDuty["remark"] != DBNull.Value)
             //            duty.remark = drDuty["remark"].ToString();

             //        if (drDuty["odometer"] != DBNull.Value)
             //            duty.odometer = drDuty["odometer"].ToString();

             //        if (drDuty["actionTxt"] != DBNull.Value)
             //            duty.actionTxt = drDuty["actionTxt"].ToString();

             //        if (drDuty["cityTxt"] != DBNull.Value)
             //            duty.cityTxt = drDuty["cityTxt"].ToString();

             //        if (drDuty["provinceTxt"] != DBNull.Value)
             //            duty.provinceTxt = drDuty["provinceTxt"].ToString();

             //        if (drDuty["provinceTxt"] != DBNull.Value)
             //            duty.provinceTxt = drDuty["provinceTxt"].ToString();

             //        if (drDuty["TLID"] != DBNull.Value)
             //            duty.TLID = drDuty["TLID"].ToString();


             //        if (duty.duty == "101") duty.dutyTxt = Resources.HOSResources.OffDuty;
             //        if (duty.duty == "102") duty.dutyTxt = Resources.HOSResources.Sleep;
             //        if (duty.duty == "103") duty.dutyTxt = Resources.HOSResources.Driving;
             //        if (duty.duty == "104") duty.dutyTxt = Resources.HOSResources.OnDuty;
             //        duties.Add(duty);
             //    }

             //    ViewBag.duties = duties;

             //    ds = null;
             //    sql = "usp_hos_GetStates";
             //    sqlExec.ClearCommandParameters();
             //    ds = sqlExec.SPExecuteDataset(sql);

             //    if (ds.Tables[0].Rows.Count > 0)
             //    {
             //        for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
             //        {
             //            DataRow dr = ds.Tables[0].Rows[index];
             //            states.Add(new States(dr["FullName"].ToString(), dr["ID"].ToString()));
             //        }
             //    }
             //}
             //catch (Exception ex)
             //{
             //    //Need to log
             //    throw new Exception(ex.Message);
             //}
             //ViewBag.States = states;
             //user.RefId = refid;
             //return View("ModifyDriverLog");
         }

         [Authorize]
         public ActionResult AddDriverLog()
         {
             //user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
             if (string.IsNullOrEmpty(user.DriverId)) return RedirectToAction("DriverList","HOS");
             AddScript(); 
             Driver driver = new Models.Driver(user.DriverId, user.DriverName);
             ViewBag.Driver = driver;
             List<States> states = new List<States>();
             try
             {
                 DataSet ds = null;
                 string sql = "usp_hos_GetStates";
                 sqlExec.ClearCommandParameters();
                 ds = sqlExec.SPExecuteDataset(sql);

                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                     {
                         DataRow dr = ds.Tables[0].Rows[index];
                         states.Add(new States(dr["FullName"].ToString(), dr["ID"].ToString()));
                     }
                 }
             }
             catch (Exception ex)
             {
                 //Need to log
                 throw new Exception(ex.Message);
             }
             ViewBag.States = states;
             user.RefId = "-1";
             if (Session["Cycle"] != null)
                 ViewBag.Cycle = Session["Cycle"].ToString();
             return View();
         }

         [Authorize]
         public ActionResult DriverView(string driverId, string driverName)
         {
             //user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
             AddScript();
             Driver driver = new Models.Driver(driverId, driverName);
             ViewBag.Driver = driver;
             user.DriverId = driverId;
             user.DriverName = driverName;
             return View();
         }

         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult GetCities(string state)
         {
             ArrayList cities = new ArrayList();
             try
             {
                 DataSet ds = null;
                 string sql = "usp_hos_GetCities";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@State", SqlDbType.Int, int.Parse(state));
                 ds = sqlExec.SPExecuteDataset(sql);

                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                     {
                         DataRow dr = ds.Tables[0].Rows[index];
                         Dictionary<string, string> log = new Dictionary<string, string>();
                         log.Add("LocationID", dr["LocationID"].ToString());
                         log.Add("Name", dr["Name"].ToString());
                         cities.Add(log);
                     }
                 }
             }
             catch (Exception ex)
             {
                 //Need to log
                 throw new Exception(ex.Message);
             }
             return Json(cities, JsonRequestBehavior.AllowGet);
         }

         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult GetDutyAction(string duty)
         {
             ArrayList actions = new ArrayList();
             try
             {
                 DataSet ds = null;
                 string sql = "usp_hos_GetLogbookStop";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@type", SqlDbType.Int, int.Parse(duty));
                 ds = sqlExec.SPExecuteDataset(sql);

                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     for (int index = 0; index < ds.Tables[0].Rows.Count ; index++)
                     {
                         DataRow dr = ds.Tables[0].Rows[index];
                         Dictionary<string, string> log = new Dictionary<string, string>();
                         log.Add("LSID", dr["LSID"].ToString());
                         log.Add("Item", dr["Item"].ToString());
                         actions.Add(log);
                     }
                 }
             }
             catch (Exception ex)
             {
                 //Need to log
                 throw new Exception(ex.Message);
             }
             return Json(actions, JsonRequestBehavior.AllowGet);
         }


         private List<Duty> GetDutyListByrefId(Int64 refId)
         {
             List<States> states = new List<States>();
             string dataFormat = "M/d/yyyy";
             if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
             {
                 dataFormat = "d/M/yyyy";
             }
             dataFormat = "HH:mm";
             try
             {
                 DataSet ds = null;
                 string sql = "usp_hos_GetLogData_Reference_ManualDutyByRefId";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@refId", SqlDbType.BigInt, refId);
                 ds = sqlExec.SPExecuteDataset(sql);


                 List<Duty> duties = new List<Models.Duty>();
                 foreach (DataRow drDuty in ds.Tables[0].Rows)
                 {
                     Duty duty = new Models.Duty();

                     if (drDuty["time"] != DBNull.Value)
                         duty.time = ((DateTime)drDuty["time"]).ToString(dataFormat);

                     if (drDuty["duty"] != DBNull.Value)
                         duty.duty = drDuty["duty"].ToString();

                     if (drDuty["action"] != DBNull.Value)
                         duty.action = drDuty["action"].ToString();

                     if (drDuty["province"] != DBNull.Value)
                         duty.province = drDuty["province"].ToString();

                     if (drDuty["city"] != DBNull.Value)
                         duty.city = drDuty["city"].ToString();

                     if (drDuty["remark"] != DBNull.Value)
                         duty.remark = drDuty["remark"].ToString();

                     if (drDuty["odometer"] != DBNull.Value)
                         duty.odometer = drDuty["odometer"].ToString();

                     if (drDuty["actionTxt"] != DBNull.Value)
                         duty.actionTxt = drDuty["actionTxt"].ToString();

                     if (drDuty["cityTxt"] != DBNull.Value)
                         duty.cityTxt = drDuty["cityTxt"].ToString();

                     if (drDuty["provinceTxt"] != DBNull.Value)
                         duty.provinceTxt = drDuty["provinceTxt"].ToString();

                     if (duty.duty == "101") duty.dutyTxt = Resources.HOSResources.OffDuty;
                     if (duty.duty == "102") duty.dutyTxt = Resources.HOSResources.Sleep;
                     if (duty.duty == "103") duty.dutyTxt = Resources.HOSResources.Driving;
                     if (duty.duty == "104") duty.dutyTxt = Resources.HOSResources.OnDuty;
                     duties.Add(duty);
                 }

                 return duties;
             }
             catch (Exception ex) {
                 throw new Exception(ex.Message);
             }
         }

         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult Delete_LogData_TimeLog_Manual(string duty)
         {
             if (user.DriverId != null)
             {
                 try
                 {
                     string dataFormat = "M/d/yyyy";
                     if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                     {
                         dataFormat = "d/M/yyyy";
                     }

                     JavaScriptSerializer JSS = new JavaScriptSerializer();
                     Duty dutiesObj = JSS.Deserialize<Duty>(duty);

                     DateTime curDate = DateTime.ParseExact(dutiesObj.date,
                             dataFormat, new CultureInfo("en-US"));
                     int tlid = int.Parse(dutiesObj.TLID);

                     using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
                     {
                         using (SqlCommand cmd = new SqlCommand("usp_hos_Delete_LogData_TimeLog_Manual", conn))
                         {
                             cmd.Parameters.Add("@Driver", SqlDbType.VarChar).Value = user.DriverId;
                             cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = curDate;
                             cmd.Parameters.Add("@TLID", SqlDbType.Int).Value = tlid;
                             conn.Open();
                             cmd.CommandType = CommandType.StoredProcedure;
                             cmd.ExecuteNonQuery();
                             conn.Close();
                         }
                     }

                 }
                 catch (Exception ex)
                 {
                     return Json("-1", JsonRequestBehavior.AllowGet);
                 }
             }
             else return Json("-1", JsonRequestBehavior.AllowGet);
             return Json(1, JsonRequestBehavior.AllowGet);
         }


         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult Insert_LogData_TimeLog_Manual(string trip, string duties, Boolean isDelete)
         {
             string tlib = "";
             if (user.DriverId != null)
             {

                 try
                 {
                     string dataFormat = "M/d/yyyy";
                     if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                     {
                         dataFormat = "d/M/yyyy";
                     }

                     JavaScriptSerializer JSS = new JavaScriptSerializer();
                     TripInfo tripObj = JSS.Deserialize<TripInfo>(trip);
                     List<Duty> dutiesObj = JSS.Deserialize<List<Duty>>(duties);

                     DateTime curDate = DateTime.ParseExact(dutiesObj[0].date,
                             dataFormat, new CultureInfo("en-US"));

                     StringBuilder auditTimeSB = new StringBuilder();
                     dataFormat = dataFormat + " HH:mm";

                     DateTime shiftStart = DateTime.MinValue;
                     if (tripObj.shiftStart != string.Empty)
                         shiftStart = DateTime.ParseExact(dutiesObj[0].date + " " + tripObj.shiftStart,
                              dataFormat, new CultureInfo("en-US"));

                     DateTime dutyTime = DateTime.ParseExact(dutiesObj[0].date + " " + dutiesObj[0].time,
                             dataFormat, new CultureInfo("en-US"));

                     string sql = "usp_hos_Insert_LogData_TimeLog_Manual";
                     sqlExec.ClearCommandParameters();

                     int odoStart = 0;
                     int odoEnd = 0;
                     int.TryParse(tripObj.odoStart, out odoStart);
                     int.TryParse(tripObj.odoEnd, out odoEnd);
                     float defer = 0;
                     int personalStartOdo = 0;
                     int personalEndOdo = 0;
                     int.TryParse(tripObj.personalStart, out personalStartOdo);
                     int.TryParse(tripObj.personalEnd, out personalEndOdo);
                     float.TryParse(tripObj.deferredHours, out defer);
                     int defeyDay;
                     int.TryParse(tripObj.deferredDay, out defeyDay);
                     
                     using (SqlConnection conn = new SqlConnection(sqlExec.ConnectionString))
                     {
                         using (SqlCommand cmd = new SqlCommand(sql, conn))
                         {
                             cmd.Parameters.Add("@Driver", SqlDbType.VarChar).Value= user.DriverId;
                             cmd.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = tripObj.company;
                             cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value =  curDate;
                             if (shiftStart != DateTime.MinValue)
                                 cmd.Parameters.Add("@ShiftStart", SqlDbType.DateTime).Value  = shiftStart;
                             else cmd.Parameters.Add("@ShiftStart", SqlDbType.DateTime).Value = DBNull.Value;
                             cmd.Parameters.Add("@OdometerStart", SqlDbType.Int).Value = odoStart;
                             cmd.Parameters.Add("@OdometerEnd", SqlDbType.Int).Value  = odoEnd;
                             cmd.Parameters.Add("@CoDriverName", SqlDbType.VarChar).Value = tripObj.coDriver;
                             cmd.Parameters.Add("@DistanceDriven", SqlDbType.Int).Value = odoEnd - odoStart;
                             cmd.Parameters.Add("@HomeTerminalAddress", SqlDbType.VarChar).Value =  tripObj.hometerm;
                             cmd.Parameters.Add("@EquipmentNo", SqlDbType.VarChar).Value =  tripObj.equipmentNo;
                             cmd.Parameters.Add("@ShippingDocument", SqlDbType.VarChar).Value = tripObj.shippingDoc;
                             cmd.Parameters.Add("@TruckNo", SqlDbType.VarChar).Value =  tripObj.truckNo;
                             cmd.Parameters.Add("@TrailerNo", SqlDbType.VarChar).Value = tripObj.tailerNo;
                             cmd.Parameters.Add("@Cycle", SqlDbType.VarChar).Value = dutiesObj[0].cycle;
                             cmd.Parameters.Add("@Deffered", SqlDbType.Float).Value =  defer;
                             cmd.Parameters.Add("@DeferedDay", SqlDbType.SmallInt).Value =  defeyDay;
                             cmd.Parameters.Add("@Emergency", SqlDbType.VarChar).Value =  tripObj.emergency;
                             cmd.Parameters.Add("@Remark", SqlDbType.VarChar).Value =  tripObj.remark.Trim();
                             cmd.Parameters.Add("@PersonalStartOdo", SqlDbType.Int).Value =  personalStartOdo;
                             cmd.Parameters.Add("@PersonalEndOdo", SqlDbType.Int).Value =  personalEndOdo;
                             if (tripObj.adverseDriving == "1")
                                 cmd.Parameters.Add("@AdverseDrivingCondition", SqlDbType.Bit).Value =  true;
                             else cmd.Parameters.Add("@AdverseDrivingCondition", SqlDbType.Bit).Value =  false;

                             SqlParameter sqlp = new SqlParameter("@TLID", SqlDbType.Int);
                             sqlp.Direction =  ParameterDirection.InputOutput;
                             sqlp.Value = dutiesObj[0].TLID;
                             cmd.Parameters.Add(sqlp);
                             cmd.Parameters.Add("@Event", SqlDbType.Int).Value =  dutiesObj[0].duty;
                             cmd.Parameters.Add("@LogTime", SqlDbType.DateTime).Value =  dutyTime;
                             cmd.Parameters.Add("@LocationID", SqlDbType.Int).Value =  dutiesObj[0].city;
                             int odometer = 0;
                             int.TryParse(dutiesObj[0].odometer, out odometer);
                             cmd.Parameters.Add("@Odometer", SqlDbType.Int).Value =  odometer;
                             cmd.Parameters.Add("@Logbookstopid", SqlDbType.Int).Value =  dutiesObj[0].action;
                             cmd.Parameters.Add("@RemarkLog", SqlDbType.VarChar).Value =  dutiesObj[0].remark;
                             cmd.Parameters.Add("@IsDelete", SqlDbType.Bit).Value = isDelete;
                             conn.Open();
                             cmd.CommandType = CommandType.StoredProcedure;
                             cmd.CommandText = sql;
                             cmd.ExecuteNonQuery();
                             conn.Close();
                             tlib = cmd.Parameters["@TLID"].Value.ToString();
                         }
                     } 

                 }
                 catch (Exception ex)
                 {
                     return Json("-1", JsonRequestBehavior.AllowGet);
                 }
             }
             else return Json("-1", JsonRequestBehavior.AllowGet);
             return Json(tlib, JsonRequestBehavior.AllowGet);
         }
         
         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult SubmitManualLog(string trip, string duties)
         {
             if (user.DriverId != null)
             {
                 
                 try
                 {
                     string dataFormat = "M/d/yyyy";
                     if (System.Threading.Thread.CurrentThread.CurrentUICulture.ToString().ToLower().IndexOf("fr") >= 0)
                     {
                         dataFormat = "d/M/yyyy";
                     }

                     JavaScriptSerializer JSS = new JavaScriptSerializer();
                     TripInfo tripObj = JSS.Deserialize<TripInfo>(trip);
                     List<Duty> dutiesObj = JSS.Deserialize<List<Duty>>(duties);

                     Int64 iRefId = Int64.Parse(user.RefId);
                     Boolean hasChange = false;
                     if (iRefId >= 0)
                     {
                         List<Duty> oldDuties = GetDutyListByrefId(iRefId);
                         if (oldDuties.Count != dutiesObj.Count) hasChange = true;
                         else
                         {
                             for (int index = 0; index < oldDuties.Count; index++)
                             {
                                 if (
                                     oldDuties[index].action != dutiesObj[index].action ||
                                     oldDuties[index].time != dutiesObj[index].time ||
                                     oldDuties[index].city != dutiesObj[index].city ||
                                     oldDuties[index].duty != dutiesObj[index].duty ||
                                     oldDuties[index].odometer != dutiesObj[index].odometer ||
                                     oldDuties[index].action != dutiesObj[index].action ||
                                     oldDuties[index].remark.Trim() != dutiesObj[index].remark.Trim())
                                     hasChange = true;
                             }
                         }

                     }
                     else hasChange = true;

                     DateTime curDate = DateTime.ParseExact(tripObj.date ,
                             dataFormat, new CultureInfo("en-US") );

                     StringBuilder auditTimeSB = new StringBuilder();
                     auditTimeSB.Append("<ROOT>");
                     int TLID = 1;
                     dataFormat = dataFormat + " HH:mm";

                     DateTime shiftStart = DateTime.MinValue;
                     if (tripObj.shiftStart != string.Empty )
                        shiftStart = DateTime.ParseExact(tripObj.date + " " + tripObj.shiftStart,
                             dataFormat, new CultureInfo("en-US"));

                     foreach (Duty curDuty in dutiesObj)
                     {
                         DateTime dutyTime = DateTime.ParseExact(tripObj.date + " " + curDuty.time,
                             dataFormat, new CultureInfo("en-US") );

                         auditTimeSB.Append(
                            string.Format("<Duty><TLID>{0}</TLID>" +
                               "<Event>{1}</Event>" +
                               "<LogTime>{2}</LogTime>" +
                               "<LocationID>{3}</LocationID>" +
                               "<Odometer>{4}</Odometer>" +
                               "<Logbookstopid>{5}</Logbookstopid>" +
                               "<Remark>{6}</Remark>" +
                               "</Duty>",
                         TLID, curDuty.duty,
                          string.Format("{0:yyyy-MM-dd HH:mm:ss.000}", dutyTime),
                         curDuty.city, curDuty.odometer, curDuty.action, curDuty.remark.Trim()
                         ));

                         TLID = TLID + 1;
                     }
                     auditTimeSB.Append("</ROOT>");


                     string sql = "usp_hos_Insert_LogData_Reference_Manual";
                     sqlExec.ClearCommandParameters();

                     int odoStart = 0;
                     int odoEnd = 0;
                     int.TryParse(tripObj.odoStart, out odoStart);
                     int.TryParse(tripObj.odoEnd, out odoEnd);
                     float defer = 0;
                     int personalStartOdo = 0;
                         int personalEndOdo = 0;
                     int.TryParse(tripObj.personalStart, out personalStartOdo);
                     int.TryParse(tripObj.personalEnd, out personalEndOdo);
                     float.TryParse(tripObj.deferredHours, out defer);
                     int defeyDay;
                     int.TryParse(tripObj.deferredDay, out defeyDay);
                     sqlExec.AddCommandParam("@RefId", SqlDbType.BigInt, Int64.Parse(user.RefId));
                     sqlExec.AddCommandParam("@Driver", SqlDbType.VarChar, user.DriverId);
                     sqlExec.AddCommandParam("@CompanyName", SqlDbType.VarChar, tripObj.company);
                     sqlExec.AddCommandParam("@Date", SqlDbType.DateTime, curDate);
                     if (shiftStart != DateTime.MinValue)
                        sqlExec.AddCommandParam("@ShiftStart", SqlDbType.DateTime, shiftStart);
                     else sqlExec.AddCommandParam("@ShiftStart", SqlDbType.DateTime, DBNull.Value);
                     sqlExec.AddCommandParam("@OdometerStart", SqlDbType.Int, odoStart );
                     sqlExec.AddCommandParam("@OdometerEnd", SqlDbType.Int, odoEnd );
                     sqlExec.AddCommandParam("@CoDriverName", SqlDbType.VarChar, tripObj.coDriver );
                     sqlExec.AddCommandParam("@DistanceDriven", SqlDbType.Int, odoEnd -  odoStart);
                     sqlExec.AddCommandParam("@HomeTerminalAddress", SqlDbType.VarChar, tripObj.hometerm );
                     sqlExec.AddCommandParam("@EquipmentNo", SqlDbType.VarChar, tripObj.equipmentNo );
                     sqlExec.AddCommandParam("@ShippingDocument", SqlDbType.VarChar, tripObj.shippingDoc );
                     sqlExec.AddCommandParam("@TruckNo", SqlDbType.VarChar, tripObj.truckNo );
                     sqlExec.AddCommandParam("@TrailerNo", SqlDbType.VarChar, tripObj.tailerNo);
                     sqlExec.AddCommandParam("@Cycle", SqlDbType.VarChar, tripObj.cycle);
                     sqlExec.AddCommandParam("@Deffered", SqlDbType.Float, defer);
                     sqlExec.AddCommandParam("@DeferedDay", SqlDbType.SmallInt, defeyDay);
                     sqlExec.AddCommandParam("@Emergency", SqlDbType.VarChar, tripObj.emergency);
                     sqlExec.AddCommandParam("@Remark", SqlDbType.VarChar, tripObj.remark.Trim());
                     sqlExec.AddCommandParam("@PersonalStartOdo", SqlDbType.Int, personalStartOdo);
                     sqlExec.AddCommandParam("@PersonalEndOdo", SqlDbType.Int, personalEndOdo);
                     if (tripObj.adverseDriving == "1")
                       sqlExec.AddCommandParam("@AdverseDrivingCondition", SqlDbType.Bit, true);
                     else sqlExec.AddCommandParam("@AdverseDrivingCondition", SqlDbType.Bit, false);
                     if (tripObj.approved == "1")
                     {
                         sqlExec.AddCommandParam("@Approved", SqlDbType.Bit, true);
                         sqlExec.AddCommandParam("@ApprovedBy", SqlDbType.Int, user.UserId);
                         sqlExec.AddCommandParam("@ApprovedDate", SqlDbType.DateTime, DateTime.UtcNow);
                     }
                     else
                     {
                         sqlExec.AddCommandParam("@Approved", SqlDbType.Bit, false);
                         sqlExec.AddCommandParam("@ApprovedBy", SqlDbType.Int, null);
                         sqlExec.AddCommandParam("@ApprovedDate", SqlDbType.DateTime, null);
                     }

                     sqlExec.AddCommandParam("@HasChange", SqlDbType.Bit, hasChange);
                     sqlExec.AddCommandParam("@Duties", SqlDbType.VarChar,auditTimeSB.ToString() , -1);
                     sqlExec.SPExecuteNonQuery(sql);

                 }
                 catch (Exception ex)
                 {
                     return Json("0", JsonRequestBehavior.AllowGet);
                 }
             }
             return Json("1", JsonRequestBehavior.AllowGet);
         }

         [OutputCache(Duration = 0, VaryByParam = "None")]
         public ActionResult DriverLogs(string driverId)
         {
             //user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
             List<Dictionary<string, string>> logs = new List<Dictionary<string,string>>();
             if (driverId == "")
             {
                 driverId = user.DriverId;
             }
             //else user.DriverId = driverId;
             try
             {
                 DataSet dsManual = null;
                 string sql = "usp_hos_GetLogData_Reference_Manual";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@start", SqlDbType.DateTime, DateTime.Now.Date.AddDays(-14));
                 sqlExec.AddCommandParam("@stop", SqlDbType.DateTime, DateTime.Now.Date.AddDays(1));
                 sqlExec.AddCommandParam("@DriverId", SqlDbType.VarChar, driverId);
                 sqlExec.AddCommandParam("@OrganizationId", SqlDbType.Int, user.OrganizationId);
                 sqlExec.AddCommandParam("@UserId", SqlDbType.Int, user.UserId);
                 
                 sqlExec.AddCommandParam("@Cycle", SqlDbType.VarChar, ParameterDirection.Output,50,"");

                 dsManual = sqlExec.SPExecuteDataset(sql);
                 String cycle = "";
                 String cyclValue = "0";
                 try
                 {
                     cycle = sqlExec.ReadCommandParam("@Cycle").ToString();
                     if (cycle == "US-60/7") cyclValue = "10";
                     if (cycle == "US-70/8") cyclValue = "11";
                     if (cycle == "CA Cycle 1") cyclValue = "6";
                     if (cycle == "CA Cycle 2") cyclValue = "7";
                     if (cycle == "US Oil Field") cyclValue = "20";
                     if (cycle == "CA Oil Field") cyclValue = "18";
                 }
                 catch (Exception ex) { }
                 if (cyclValue != "") Session["Cycle"] = cyclValue;
                 sql = "VerigoManager_GetReportLogSheet_ByDriver";
                 sqlExec.ClearCommandParameters();
                 sqlExec.AddCommandParam("@companyid", SqlDbType.Int, user.OrganizationId);
                 sqlExec.AddCommandParam("@start", SqlDbType.DateTime, DateTime.Now.Date.AddDays(-14));
                 sqlExec.AddCommandParam("@stop", SqlDbType.DateTime, DateTime.Now.Date.AddDays(1));
                 sqlExec.AddCommandParam("@driverId", SqlDbType.Int, driverId);

                 DataSet ds = sqlExec.SPExecuteDataset(sql);
                 Dictionary<string, string>  log = null;
                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     List<DateTime> logsDate = new List<DateTime>();
                     for (int index = ds.Tables[0].Rows.Count - 1; index >= 0; index--)
                     {
                         log = new Dictionary<string, string>();
                         DataRow dr = ds.Tables[0].Rows[index];
                         DateTime dt = (DateTime)dr["date"];
                         log.Add("FileName", dr["filename"].ToString().Replace(@"\", @"\\") + Server.UrlEncode("@" + user.DriverName + "-" + dt.ToString("MM/dd/yyyy")));
                         log.Add("Type", "0");
                         log.Add("Desc", "Logsheet");
                         log.Add("Date", dt.ToString("MM/dd/yyyy"));
                         log.Add("DateSort", dt.ToString("MM/dd/yyyy") + "0");
                         logs.Add(log);
                         if (dr["inspection"] != DBNull.Value)
                         {
                             string inspection = dr["inspection"].ToString();
                             string[] instpections = inspection.Split(',');
                             foreach (string insp in instpections)
                             {
                                 if (!string.IsNullOrEmpty(insp))
                                 {
                                     try
                                     {
                                         string[] inspDetail = insp.Split('#');
                                         log = new Dictionary<string, string>();
                                         log.Add("FileName", inspDetail[2].Replace(@"\", @"\\") + Server.UrlEncode("@" + user.DriverName + "-insepction-" + dt.ToString("MM/dd/yyyy") + " " + inspDetail[1]));
                                         log.Add("Desc", inspDetail[0] + " Inspection");
                                         log.Add("Type", "1");
                                         log.Add("Date", dt.ToString("MM/dd/yyyy") + " " + inspDetail[1]);
                                         log.Add("DateSort", dt.ToString("MM/dd/yyyy") + "1");
                                         logs.Add(log);
                                     }
                                     catch (Exception ex) { }
                                 }
                             }
                         }
                         if (dsManual != null && dsManual.Tables.Count > 0 &&
                             dsManual.Tables[0].Rows.Count > 0)
                         {
                             for (int index_1 = 0; index_1 < dsManual.Tables[0].Rows.Count; index_1++)
                             {
                                 DataRow drManual = dsManual.Tables[0].Rows[index_1];
                                 if ((DateTime)drManual["Date"] == dt)
                                 {
                                     log = new Dictionary<string, string>();
                                     log.Add("FileName", drManual["FileName"].ToString().Replace(@"\", @"\\"));
                                     log.Add("Desc", Resources.HOSResources.ManualLogsheets);
                                     if (drManual["Approved"] != DBNull.Value &&
                                         (Boolean)drManual["Approved"])
                                     {
                                         if (drManual["UserName"] != DBNull.Value && drManual["UserName"].ToString() != "")
                                             log.Add("ApprovedBy", drManual["UserName"].ToString());
                                         log.Add("Type", "3");
                                     }
                                     else log.Add("Type", "2");
                                     log.Add("Date", "");
                                     log.Add("DateSort", dt.ToString("MM/dd/yyyy") + "2");
                                     logs.Add(log);
                                     drManual["FileName"] = "";
                                 }
                             }
                         }
                     }
                 }
                 for (int index_1 = 0; index_1 < dsManual.Tables[0].Rows.Count; index_1++)
                 {
                     DataRow drManual = dsManual.Tables[0].Rows[index_1];
                     if (drManual["FileName"] != "")
                     {
                         log = new Dictionary<string, string>();
                         log.Add("FileName", drManual["FileName"].ToString().Replace(@"\", @"\\"));
                         log.Add("Desc", Resources.HOSResources.ManualLogsheets);
                         if (drManual["Approved"] != DBNull.Value &&
                             (Boolean)drManual["Approved"])
                         {
                             log.Add("Type", "3");
                             if (drManual["UserName"] != DBNull.Value && drManual["UserName"].ToString() != "")
                                 log.Add("ApprovedBy", drManual["UserName"].ToString());
                         }
                         else log.Add("Type", "2");
                         log.Add("Date", ((DateTime)drManual["Date"]).ToString("MM/dd/yyyy"));

                         log.Add("DateSort", ((DateTime)drManual["Date"]).ToString("MM/dd/yyyy") + "2");
                         logs.Add(log);
                         drManual["FileName"] = "";
                     }
                 }

                 logs.Sort(
                     delegate(Dictionary<string, string> log1, Dictionary<string, string> log2){
                         try
                         {
                             DateTime t1 = DateTime.ParseExact(log1["DateSort"].ToString().Substring(0, 10), "MM/dd/yyyy", new CultureInfo("en-US"));
                             DateTime t2 = DateTime.ParseExact(log2["DateSort"].ToString().Substring(0, 10), "MM/dd/yyyy", new CultureInfo("en-US"));
                             if (t1 != t2)
                                 return (t2.CompareTo(t1));
                             else
                             {
                                 string s1 = log1["DateSort"].ToString().Substring(10, 1);
                                 string s2 = log2["DateSort"].ToString().Substring(10, 1);
                                 return (s1.CompareTo(s2));
                             }
                         }
                         catch {

                             return 0; 
                         }
                     }
                );
             }
             catch (Exception ex)
             {
                 //Need to log
                 throw new Exception(ex.Message);
             }
             return Json(logs, JsonRequestBehavior.AllowGet);
         } 
        
         public User LoginDriver(string driverID, string passWord, string domainName)
         {
             DataSet dsUser = null;
             User driver = null;
             string sql = "usp_mb_AuthenticateDriverForMobile";
             sqlExec.ClearCommandParameters();
             sqlExec.AddCommandParam("@DriverID", SqlDbType.VarChar, driverID);
             sqlExec.AddCommandParam("@Password", SqlDbType.VarChar, passWord);
             sqlExec.AddCommandParam("@DomainName", SqlDbType.VarChar, domainName);
             dsUser = sqlExec.SPExecuteDataset(sql);
             if (dsUser.Tables[0].Rows.Count > 0)
             {
                 driver = new Models.User(dsUser.Tables[0].Rows[0]["DriverName"].ToString(),
                     int.Parse(dsUser.Tables[0].Rows[0]["DriverID"].ToString()), "");                 
                 driver.OrganizationId = int.Parse(dsUser.Tables[0].Rows[0]["OrganizationId"].ToString());
                 driver.DefaultLanguage = "en-US";
                 driver.IsDriver = true;
             }           
             return driver;
         }

         private void AddScript()
         {
             ViewBag.LastUpdatedmobilebaseFile = System.IO.File.GetLastWriteTime(Server.MapPath("~/Scripts/mobile-base.js")).ToString("yyyyMMddHHmmss");
             ViewBag.LastUpdatedmobilejqFile = System.IO.File.GetLastWriteTime(Server.MapPath("~/Scripts/mobile-jq.js")).ToString("yyyyMMddHHmmss");
             ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");

         }

         private void DeletePDF()
         {
             string path = Server.MapPath("~/PDF");
             string[] fileEntries = Directory.GetFiles(path, "*.pdf");
             foreach (string fileName in fileEntries)
             {
                 FileInfo fInfo = new FileInfo(fileName);
                 if (System.DateTime.Now.Subtract(fInfo.CreationTime).TotalHours > 8)
                 {
                     try
                     {
                         fInfo.Delete();
                     }
                     catch (Exception ex) { }
                 }
             }
         }
    }
}
