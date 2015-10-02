using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using SentinelFM;
using VLF.DAS.Logic;

public partial class Routing_GetSegments : System.Web.UI.Page
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
        Dictionary<string, object> jsonData = new Dictionary<string, object>();
        int totalCount = 0;
        IList<Dictionary<string, string>> recordData = PostGisLandmark.GetSegmentsForGrid(conditions, sn.User.OrganizationId, ref totalCount);        
        jsonData.Add("sEcho", (conditions.ContainsKey("sEcho") ? conditions["sEcho"] : null));
        jsonData.Add("iTotalDisplayRecords", Convert.ToString(totalCount));
        jsonData.Add("iTotalRecords", Convert.ToString(recordData.Count));
        jsonData.Add("aaData", recordData);
        var oSerializer = new JavaScriptSerializer();
        string json = oSerializer.Serialize(jsonData);
        Response.ContentType = "application/json";
        Response.Write(json);
    }
}