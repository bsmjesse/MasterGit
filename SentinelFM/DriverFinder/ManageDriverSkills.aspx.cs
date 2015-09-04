using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using VLF.DAS.Logic;
using System.Web.Script.Serialization;


namespace SentinelFM
{
    public partial class DriverFinder_ManageDriverSkills : System.Web.UI.Page
    {
        protected SentinelFMSession sn;
        
        protected void Page_Load(object sender, EventArgs e)
        {            
            sn = (SentinelFMSession)Session["SentinelFMSession"];            
            int organizationId = sn.User.OrganizationId;
            Dictionary<string, string> myresult = new Dictionary<string, string>();
            string json = null;
           if (organizationId == 0)
            {
                myresult.Add("result", "NeedLogin");
                json = ParseResultJson(myresult);
            }
            else
            {
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                Organization organization = new Organization(sConnectionString);
                DriverManager driver = new DriverManager(sConnectionString);
                string action = Request.QueryString["action"];
                int result = 0;
                string driverResult = null;
                DataSet results = null;
                string skillName = null;
                string description = null;
                string driverVehicle = null;
                string[] drivervehicleInfo = null;
                int skillId = 0;
                int driverId = 0;
                int vehicleId = 0;
                int oldSkillId = 0;
                int delete = 0;
                switch (action)
                {
                    case "sos":
                        skillName = Request.Form["skillname"];
                        description = Request.Form["description"];
                        result = organization.SaveOrganizationSkill(organizationId, skillName, description, skillId, delete);
                        break;
                    case "uos":
                        skillName = Request.Form["skillname"];
                        description = Request.Form["description"];
                        skillId = Convert.ToInt32(Request.Form["skillid"]);
                        result = organization.SaveOrganizationSkill(organizationId, skillName, description, skillId, delete);
                        break;
                    case "dos":
                        skillName = Request.Form["skillname"];
                        description = Request.Form["description"];
                        skillId = Convert.ToInt32(Request.Form["skillid"]);
                        delete = 1;
                        result = organization.SaveOrganizationSkill(organizationId, skillName, description, skillId, delete);
                        break;
                    case "sds":
                        driverId = Convert.ToInt32(Request.Form["driverid"]);
                        skillId = Convert.ToInt32(Request.Form["skillid"]);
                        description = Request.Form["description"];
                        driverResult = driver.SaveDriverSkill(driverId, organizationId, skillId, 0, description, delete);
                        break;
                    case "uds":
                        driverId = Convert.ToInt32(Request.Form["driverid"]);
                        skillId = Convert.ToInt32(Request.Form["skillid"]);
                        oldSkillId = Convert.ToInt32(Request.Form["oldskillid"]);
                        description = Request.Form["description"];
                        driverResult = driver.SaveDriverSkill(driverId, organizationId, skillId, oldSkillId, description, delete);
                        break;
                    case "dds":
                        driverId = Convert.ToInt32(Request.Form["driverid"]);
                        skillId = Convert.ToInt32(Request.Form["skillid"]);
                        oldSkillId = Convert.ToInt32(Request.Form["oldskillid"]);
                        description = Request.Form["description"];
                        delete = 1;
                        driverResult = driver.SaveDriverSkill(driverId, organizationId, skillId, oldSkillId, description, delete);
                        break;
                    case "ros":
                        results = organization.GetOrganizationSkills(organizationId);
                        break;
                    case "rds":
                        results = driver.GetDriverSkills(organizationId);
                        break;
                }                

                if (results != null)
                {
                    json = ParseResultsJson(results.Tables[0]);
                }
                else if (result > 0)
                {
                    myresult.Add("result", Convert.ToString(result));
                    json = ParseResultJson(myresult);
                }
                else
                {
                    myresult.Add("result", driverResult);
                    json = ParseResultJson(myresult);
                }
            }

            Response.AddHeader("Content-type", "text/json");
            Response.Write(json);
            Response.End();
        }


        private string ParseResultJson(Dictionary<string, string> result)
        {
            try
            {
                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(result);
                return json;
            }
            catch (Exception exception)
            {
            }

            return null;
        }

        private string ParseResultsJson(DataTable dt)
        {
            try
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();                                
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> mylist = new Dictionary<string, string>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        string columnName = column.ColumnName;
                        mylist.Add(columnName, Convert.ToString(row[columnName]));
                    }
                    results.Add(mylist);
                    i++;
                }


                var oSerializer = new JavaScriptSerializer();
                string json = oSerializer.Serialize(results);
                return json;
            }
            catch (Exception exception)
            {

            }
            return null;
        }
    }
        
}
