using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;
using System.Configuration;


public partial class ViewDynamicForm : SentinelFMBasePage
{

    String hosConnectionString =
            ConfigurationManager.ConnectionStrings["SentinelHOSConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtFrom.SelectedDate = System.DateTime.Now.Date.AddDays(-7);
            txtTo.SelectedDate = System.DateTime.Now.Date;
        }
     }


    protected void cmdViewAllData_Click(object sender, EventArgs e)
    {
        using (SqlConnection conn = new SqlConnection(hosConnectionString))
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter("usp_hos_GetLogData_Forms", conn))
            {
                adapter.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter para = new SqlParameter("@start", System.Data.SqlDbType.DateTime);
                para.Value = txtFrom.SelectedDate.Value;
                adapter.SelectCommand.Parameters.Add(para);


                para = new SqlParameter("@end", System.Data.SqlDbType.DateTime);
                para.Value = txtTo.SelectedDate.Value.AddDays(1);
                adapter.SelectCommand.Parameters.Add(para);

                para = new SqlParameter("@organizationId", System.Data.SqlDbType.Int);
                para.Value = sn.User.OrganizationId;
                adapter.SelectCommand.Parameters.Add(para);

                adapter.Fill(dt);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<table border='1' >");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    String content = dr["Form"].ToString();
                    String title = dr["FormName"].ToString();
                    String driverName = dr["DriverName"].ToString();
                    String date = "";
                    if (dr["Date"] != DBNull.Value)
                        date = dr["Date"].ToString();
                    sb.Append(String.Format("<tr valign='top' ><td >{0}</td><td >{1}</td><td>{2}</td><td>{3}</td></tr>", date, driverName, title, ParseFormContent(content).Replace(Environment.NewLine, "</BR>")));
                }
            }
            else
            {
                sb = new StringBuilder();
                sb.Append("<table><tr><td>No data to display.</td></tr>");
            }
            sb.Append("</table>");
            Literal literal = new Literal();
            literal.Text = sb.ToString();
            PlaceHolder1.Controls.Add(literal);
        }
    }

    private String ParseFormContent(String currentFormContent)
    {
                    StringBuilder sb = new StringBuilder();
        using (XmlReader reader = XmlReader.Create(new StringReader(currentFormContent)))
        {
            Boolean startData = false;
            string id = string.Empty;
            string label = string.Empty;
            string value = string.Empty;
            sb.Append("<table >");
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "data")
                        {
                            startData = true;
                        }
                        if (reader.Name == "id")
                        {
                            id = reader.ReadString();
                        }
                        if (reader.Name == "label")
                        {
                            label = XmlConvert.DecodeName(reader.ReadString());
                        }
                        if (reader.Name == "value")
                        {
                            value = XmlConvert.DecodeName(reader.ReadString());
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "data")
                        {

                            sb.Append(String.Format("<tr  ><td  style='border-color: black; border-style: outset; border-width: 1px;'>{0}</td><td  style='border-color: black; border-style: outset; border-width: 1px;'>{1}</td></tr>", label, value));

                            startData = false;
                            id = string.Empty;
                            label = string.Empty;
                            value = string.Empty;
                        }
                        break;
                }
            }
            sb.Append("</table>");

        }
        return sb.ToString();
    }

    protected void txtFrom_Load(object sender, EventArgs e)
    {
        txtFrom.DateInput.DateFormat = sn.User.DateFormat;
        txtTo.DateInput.DateFormat = sn.User.DateFormat;
    }
}