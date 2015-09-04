using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SentinelFM.Resolver;
using SentinelFM.ServerDBFleet;
using SentinelFM.ServerDBSystem;

namespace SentinelFM
{
    public partial class DriverFinder_SearchForm : System.Web.UI.Page
    {
        public string MyLayout;
        protected SentinelFMSession sn;
        protected void Page_Load(object sender, EventArgs e)
        {
            MyLayout = Request.QueryString["layout"];

            DropDownList skillList = new DropDownList();
            DropDownList vehicleTypes = new DropDownList();
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            int organizationId = sn.User.OrganizationId;
            int userId = sn.UserID;


            if (MyLayout == "h")
            {
                skillList.ID = "skillList_h";
                vehicleTypes.ID = "vehicleTypes_h";
                skillListPlaceHolder_h.Controls.Add(skillList);
                vehicleTypesPlaceHolder_h.Controls.Add(vehicleTypes);
            }
            else
            {
                skillList.ID = "skillList";
                vehicleTypes.ID = "vehicleTypes";
                skillListPlaceHolder_v.Controls.Add(skillList);
                vehicleTypesPlaceHolder_v.Controls.Add(vehicleTypes);
            }


            DBFleet fleet = new DBFleet();
            string xml = null;
            int result = fleet.GetOrganizationSkills(userId, organizationId, ref xml);
            IList<Dictionary<string, string>> skills = GetSkills(xml);
            try
            {
                ListItem defaultSkill = new ListItem();
                defaultSkill.Text = "All";
                defaultSkill.Value = "0";
                skillList.Items.Add(defaultSkill);

                foreach (var skill in skills)
                {
                    ListItem item = new ListItem();
                    item.Text = skill["SkillName"];
                    item.Value = skill["SkillId"];
                    skillList.Items.Add(item);
                }
            }
            catch (Exception exception)
            {

            }
            DBSystem dbSystem = new DBSystem();
            result = dbSystem.GetAllVehicleTypes(userId, sn.SecId, ref xml);

            IList<Dictionary<string, string>> types = GetVehicleTypes(xml);
            try
            {
                ListItem defaultType = new ListItem();
                defaultType.Text = "All";
                defaultType.Value = "-1";
                vehicleTypes.Items.Add(defaultType);
                foreach (var type in types)
                {
                    ListItem item = new ListItem();
                    item.Text = type["VehicleTypeName"];
                    item.Value = type["VehicleTypeId"];
                    vehicleTypes.Items.Add(item);
                }
            }
            catch (Exception exception)
            {

            }
        }



        private IList<Dictionary<string, string>> GetSkills(string myXmlString)
        {
            List<Dictionary<string, string>> skills = new List<Dictionary<string, string>>();
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.GetElementsByTagName("OrganizationSkills");
                foreach (XmlNode sNode in xnList)
                {
                    try
                    {
                        Dictionary<string, string> skill = new Dictionary<string, string>();
                        skill.Add("SkillId", (sNode["SkillId"] != null ? sNode["SkillId"].InnerText.Trim() : null));
                        skill.Add("SkillName", (sNode["SkillName"] != null ? sNode["SkillName"].InnerText.Trim() : null));
                        skills.Add(skill);
                    }
                    catch (Exception exception)
                    {
                        Dictionary<string, string> skill = new Dictionary<string, string>();
                        skill.Add("ERROR", exception.Message);
                        skills.Add(skill);
                    }

                }
            }
            catch (Exception exception)
            {
                Dictionary<string, string> skill = new Dictionary<string, string>();
                skill.Add("ERROR", exception.Message);
                skills.Add(skill);
            }
            return skills;
        }


        private IList<Dictionary<string, string>> GetVehicleTypes(string myXmlString)
        {
            List<Dictionary<string, string>> types = new List<Dictionary<string, string>>();
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
                XmlNodeList xnList = xml.GetElementsByTagName("AllVehicleTypes");
                foreach (XmlNode sNode in xnList)
                {
                    try
                    {
                        Dictionary<string, string> type = new Dictionary<string, string>();
                        type.Add("VehicleTypeId", (sNode["VehicleTypeId"] != null ? sNode["VehicleTypeId"].InnerText.Trim() : null));
                        type.Add("VehicleTypeName", (sNode["VehicleTypeName"] != null ? sNode["VehicleTypeName"].InnerText.Trim() : null));
                        types.Add(type);
                    }
                    catch (Exception exception)
                    {
                        Dictionary<string, string> type = new Dictionary<string, string>();
                        type.Add("ERROR", exception.Message);
                        types.Add(type);
                    }

                }
            }
            catch (Exception exception)
            {
                Dictionary<string, string> type = new Dictionary<string, string>();
                type.Add("ERROR", exception.Message);
                types.Add(type);
            }
            return types;
        }
    }
}