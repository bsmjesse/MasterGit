using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HOS_frmViewImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            string url = Request.Url.ToString().Substring(0,Request.Url.ToString().LastIndexOf("/"));
            clsHOSManager hosManager = new clsHOSManager();
            DataTable dt = hosManager.GetInspectionDefects(Int64.Parse(Request.QueryString["r"]), Int64.Parse(Request.QueryString["i"]));

            if (dt != null && dt.Rows.Count > 0)
            {
                Table tbl = new Table();
                foreach (DataRow dr in dt.Rows)
                {

                    string item = "";
                    if (dr["item"] != DBNull.Value)
                        item = dr["item"].ToString();
                    string comment = "";
                    if (dr["comments"] != DBNull.Value)
                        comment = dr["comments"].ToString();
                    string medias = "";
                    if (dr["Medias"] != DBNull.Value)
                        medias = dr["Medias"].ToString();

                    if (medias != string.Empty)
                    {
                        TableRow tr = new TableRow();
                        Label lblItem = new Label();
                        lblItem.Font.Bold = true;
                        lblItem.Text = item;
                        lblItem.Style.Add("display", "block");
                        lblItem.Style.Add("width", "100%");
                        TableCell tCell = new TableCell();
                        tCell.Controls.Add(lblItem);
                        tr.Cells.Add(tCell);
                        tbl.Rows.Add(tr);
                        if (comment != string.Empty)
                        {
                            tr = new TableRow();
                            Label lblComment = new Label();
                            lblComment.Text = "Comment:" + comment;
                            lblComment.Style.Add("display", "block");
                            lblComment.Style.Add("width", "100%");
                            tCell = new TableCell();
                            tCell.Controls.Add(lblComment);
                            tr.Cells.Add(tCell);
                            tbl.Rows.Add(tr);

                        }


                        foreach (string media in medias.Split(','))
                        {
                            if (media.ToLower().EndsWith(".jpg") ||
                                media.ToLower().EndsWith(".png"))
                            {
                                tr = new TableRow();
                                Image image = new Image();
                                image.ImageUrl = "frmImage.aspx?p=" + media.Replace(@"\", @"/");
                                //image.ImageUrl = Server.UrlDecode(url + @"/Images/" + media.Replace(@"\", @"/"));
                                image.Style.Add("width", "600px");
                                image.Style.Add("height", "600px");
                                tCell = new TableCell();
                                tCell.Controls.Add(image);
                                tr.Cells.Add(tCell);
                                tbl.Rows.Add(tr);
                            }
                            else
                            {
                                tr = new TableRow();
                                Literal lit = new Literal();
                                url = "frmImage.aspx?p=" + media.Replace(@"\", @"/"); ;
                                lit.Text = "<OBJECT codeBase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=4,0,2,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' width='600'" +
                                           "height='600'>   " +
                                           "<PARAM NAME='movie' VALUE='" + url + "'>" +
                                           "<PARAM NAME='play' VALUE='false'> " +
                                           "<PARAM NAME='quality' VALUE='high'> " +
                                            "<embed src='" + url + @"\Images\" + media + "' quality='high' pluginspage='http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash' type='application/x-shockwave-flash' width='600' height='600'></embed>   " +
                                            "</OBJECT> ";
                                tCell = new TableCell();
                                tCell.Controls.Add(lit);
                                tr.Cells.Add(tCell);
                                tbl.Rows.Add(tr);
                            }
                        }
                    }
                }
                pnl.Controls.Add(tbl);
            }
            else msg = "No records to display.";
        }
        catch(Exception ex)
        {
            msg = "Error:" + ex.Message;
        }
        if (msg != "")
        {
            Label lblComment = new Label();
            lblComment.Text = msg;
            lblComment.Style.Add("display", "block");
            lblComment.Style.Add("width", "100%");
            pnl.Controls.Add(lblComment);

        }
    }
}