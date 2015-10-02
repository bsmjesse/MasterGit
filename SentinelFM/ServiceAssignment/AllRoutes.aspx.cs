using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using VLF.DAS.Logic;

public partial class ServiceAssignment_AllRoutes : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    protected void Page_Load(object sender, EventArgs e)
    {
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }

        Dictionary<string, string> conditions = new Dictionary<string, string>();
        foreach (string queryKey in Request.QueryString.Keys)
        {
            conditions.Add(queryKey, Request[queryKey]);
        }

        if (conditions.ContainsKey("jsondata"))
        {
            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            IList<Dictionary<string, string>> recordData = new List<Dictionary<string, string>>();
            int totalCount = 0;
            if (conditions.ContainsKey("bInvolveDeleted"))
            {
                if (Convert.ToInt32(conditions["bServiceConfigId"]) > 0)
                {
                    recordData = ServiceAssignment.GetFilteredAssignmentHistory(conditions, sn.User.OrganizationId, 4, Convert.ToInt32(conditions["bServiceConfigId"]), sn.UserID,
                                ref totalCount);    
                }
                
                
            }
            else
            {
                recordData = PostGisLandmark.GetFilteredRoutesWithService(conditions, sn.User.OrganizationId, sn.UserID,
                ref totalCount);
            }
            
            jsonData.Add("sEcho", (conditions.ContainsKey("sEcho") ? conditions["sEcho"] : null));
            jsonData.Add("iTotalDisplayRecords", Convert.ToString(totalCount));
            jsonData.Add("iTotalRecords", Convert.ToString(recordData.Count));
            jsonData.Add("aaData", recordData);
            var oSerializer = new JavaScriptSerializer();
            string json = oSerializer.Serialize(jsonData);
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }        
}