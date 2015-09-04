using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected  void Page_Init(object sender, EventArgs e)
    {
        //string html = "<table><tr><td>Name</td></tr><tr><td><asp:TextBox id=\"name\" runat=\"server\" /><asp:DropDownList ID=\"DropDownList1\" runat=\"server\"><asp:ListItem Value=\"0\">A</asp:ListItem><asp:ListItem Value=\"1\">B</asp:ListItem></asp:DropDownList></td></tr></table>";
        //Control ctrl = Page.ParseControl(html);


        //foreach (Control c in ctrl.Controls)
        //{
        //    if (c.ID == "DropDownList1")
        //    {
        //        DropDownList t = (DropDownList)c;
        //        t.SelectedIndex = t.Items.IndexOf(t.Items.FindByValue("1"));
                
        //    }


        //}


        //this.PlaceHolder.Controls.Add(ctrl);


    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            Label1.ForeColor = System.Drawing.Color.Red;
            Label1.Text = Page.Request["name"] +" - "+ Page.Request["DropDownList1"];
            //int cnt = PlaceHolder.Controls.Count;
            //Label1.Text += "-" + cnt.ToString();
            Button.Visible = false;
            DropDownList t = (DropDownList) Page.FindControl("DropDownList1");
            Label1.Text = t.SelectedItem.Value;  
        }
        else
        {

            string html = "<table><tr><td>Name</td></tr><tr><td><asp:TextBox id=\"name\" runat=\"server\" /><asp:DropDownList ID=\"DropDownList1\" runat=\"server\"><asp:ListItem Value=\"0\">A</asp:ListItem><asp:ListItem Value=\"1\">B</asp:ListItem></asp:DropDownList></td></tr></table>";
            Control ctrl = Page.ParseControl(html);


            foreach (Control c in ctrl.Controls)
            {
                if (c.ID == "DropDownList1")
                {
                    DropDownList t = (DropDownList)c;
                    t.SelectedIndex = t.Items.IndexOf(t.Items.FindByValue("1"));
                    
                }


            }


            this.PlaceHolder.Controls.Add(ctrl);



             

        }

    }


    
}
