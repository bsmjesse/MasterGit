using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
 
public partial class auth : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


        string fct = string.Empty;
        string username = string.Empty;


        if (!string.IsNullOrEmpty((string)Page.Request.Params["fct"]))
            fct = (string)Page.Request.Params["fct"];
        if (!string.IsNullOrEmpty((string)Page.Request.Params["username"]))
            username = (string)Page.Request.Params["username"];
        switch (fct)
        {
            case "getUserOptions":
                Response.ContentType = "text/javascript";
                if (username.StartsWith("hgi_"))
                    Response.Write("optionsSet = true; enableOptions = true;selectRadio($('rblSFM_0'));");
                else
                {
                    switch (checkOrganizationAttributes(username))
                    {
                        case "1":
                            Response.Write("optionsSet = true; enableOptions = false;selectRadio($('rblSFM_0'));");
                            break;
                        case "2":
                            Response.Write("optionsSet = true; enableOptions = false;selectRadio($('rblSFM_1'));");
                            break;
                        case "3":
                            Response.Write("optionsSet = true; enableOptions = true;selectRadio($('rblSFM_0'));");
                            break;
                        default:
                            Response.Write("optionsSet = true; enableOptions = false;");
                            break;
                    }
                }
                break;
            default:
                Response.Write("alert('ok');");
                break;

        }
        Response.End();
    }


    private string  checkOrganizationAttributes(string username)
    {
        object result = null;
        string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
        string sql = "SELECT  Flag FROM vlfOrganization INNER JOIN " +
                     " vlfUser ON vlfOrganization.OrganizationId = vlfUser.OrganizationId"+
                     " where vlfUser.UserName='" + username+"'";

        using (SqlConnection con = new SqlConnection(constr))
        {
            con.Open(); 
            SqlCommand com = new SqlCommand(sql, con);
            result = com.ExecuteScalar();
            con.Close();
            if (result != null)
                return result.ToString(); 
        }
        return "";
    }
}
