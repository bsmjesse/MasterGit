using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using VLF.DAS.Logic;

public partial class UserControl_DriverSearchData : System.Web.UI.Page
{
    SentinelFMSession sn;
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    ContactManager contactMsg = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        int maxSearchResultReturned = 20;

        if (HttpContext.Current.Session["SentinelFMSession"] != null)
            sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else
        {
            noDriver();
            return;
        }

        if (sn.User.OrganizationId == 0)
        {
            noDriver();
            return;
        }

        contactMsg = new ContactManager(sConnectionString);

        DataSet dsDrivers = new DataSet();
        if (HttpContext.Current.Session["DriversDataSet"] == null)
        {
            dsDrivers = GetDrivers();
            HttpContext.Current.Session["DriversDataSet"] = dsDrivers;
        }
        else
        {
            dsDrivers = (DataSet)HttpContext.Current.Session["DriversDataSet"];
        }



        string input = Request.QueryString["input"];
        if (string.IsNullOrEmpty(input))
        {
            throw new Exception("Input cannot be empty");
        }
        string json = "{\"predictions\":[]}";
        try
        {
            bool hasMoreResult = false;

            DataTable searchedTable = dsDrivers.Tables[0];
            input = input.Replace(" ", "   ");
            searchedTable = searchedTable.Select(string.Format("FullName LIKE '%{0}%'", input)).CopyToDataTable();
            int totalFound = searchedTable.Rows.Count;
            searchedTable = searchedTable.AsEnumerable().Skip(0).Take(maxSearchResultReturned).CopyToDataTable();
            if (searchedTable.Rows.Count < totalFound)
                hasMoreResult = true;

            IList<Dictionary<string, string>> finaleResults = new List<Dictionary<string, string>>();
            foreach (DataRow dr in searchedTable.Rows)
            {
                Dictionary<string, string> aResult = new Dictionary<string, string>();
                aResult.Add("DriverName", dr["FullName"].ToString());
                aResult.Add("DriverDescription", dr["DriverId"].ToString() + " " + dr["FullName"].ToString() + " " + dr["KeyFobId"].ToString());
                aResult.Add("DriverId", dr["DriverId"].ToString());                

                finaleResults.Add(aResult);
            }

            Dictionary<string, IList<Dictionary<string, string>>> results = new Dictionary<string, IList<Dictionary<string, string>>>();
            results.Add("predictions", finaleResults);

            IList<Dictionary<string, string>> moreResults = new List<Dictionary<string, string>>();
            Dictionary<string, string> hasMoreResultList = new Dictionary<string, string>();
            if (hasMoreResult)
                hasMoreResultList.Add("hasMoreResults", "1");
            else
                hasMoreResultList.Add("hasMoreResults", "0");
            moreResults.Add(hasMoreResultList);
            results.Add("moreResult", moreResults);

            var jss = new JavaScriptSerializer();
            json = jss.Serialize(results);
        }
        catch { }

        //string json = "{\"predictions\":[{\"description\":\"75 International Boulevard, Etobicoke, ON, Canada\",\"reference\":\"CkQ1AAAAfSLXoZnE4X9Z9oytBw8MVQY4FQX9hwS8u63eEdDUTQh22KjOgYUxz0gn2Oo4iipvMVWVApvqeNs9zkCImZFdwhIQOpq1knbtzZUPND7RyENCthoUT4QFJbnuGxkCmc4t1B5GPmKSdH0\",\"value\":\"75 International Boulevard, Etobicoke, ON, Canada\"},{\"description\":\"75 International Blvd, Burlington, ON, Canada\",\"reference\":\"CkQxAAAA5ZSyyWzCli1LfuPuAP_86n6025XIaRogJ0mkIJTd5ovM5mnclZ3SOwmDX9uaL-x3VbjZEWNJ6DmSPp_6l9zKCBIQ4FSj5rLirelMFEkmCR2dsBoUSkj__ISorn3nX6IGD86uLjM5wAs\",\"value\":\"75 International Blvd, Burlington, ON, Canada\"},{\"description\":\"75 International Drive, Orlando, FL, United States\",\"reference\":\"CkQ2AAAAYgGFITxNAPrEsWGfpLEdU_lkkiSycwXDuPde8WFFpxW077MvHosq7NTEPw3uZ3RfFwr9k2HApC7SFW6kv03vMBIQMx54Hu7Vd8l47uYNPqf6NBoU3FeziNSLZll9dWVjAgR8ZyyXWtU\",\"value\":\"75 International Drive, Orlando, FL, United States\"},{\"description\":\"75 International Drive, Pembroke, ON, Canada\",\"reference\":\"CjQwAAAAcz12-74TJnknu2n4Y333E5y3iCG1x1FZwknBvV5Lczd6O1fdNNS3DoTShkTlAT0sEhDFE6QorLjyxtXjcftYCgNTGhSV_E6Ap-XyXBAO6KOV70QYv3uMNQ\",\"value\":\"75 International Drive, Pembroke, ON, Canada\"},{\"description\":\"75 International Parkway, Whitchurch-Stouffville, ON, Canada\",\"reference\":\"CkRAAAAATn7EgwBu31fgQ_BwAPIumfFNSYJroauY8zQB6H_A3NFKYFfTvDXyKfVXA8NMGUnVH4BkWSsot2hpHVL_z7lJBhIQOdmpv65sylc5dRVGd1srzhoU4pgOLEgkFRvTahc5Si5k12KtgA8\",\"value\":\"75 International Parkway, Whitchurch-Stouffville, ON, Canada\"}]}";
        Response.Clear();
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(json);
        Response.End();
    }

    private void noDriver()
    {
        string json = "{\"predictions\":[]}";
        Response.Clear();
        Response.ContentType = "application/json; charset=utf-8";
        Response.Write(json);
        Response.End();
    }

    private DataSet GetDrivers()
    {
        DataSet dsDrivers = new DataSet();

        try
        {
            dsDrivers = contactMsg.GetOrganizationDrivers(sn.User.OrganizationId);
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //lblMessage.Text = Ex.Message;
            
        }

        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

        }

        return dsDrivers;

    }
}