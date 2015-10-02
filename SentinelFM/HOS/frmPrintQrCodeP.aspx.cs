using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HOS_frmPrintQrCodeP : SentinelFMBasePage
{
    public String barcodeString = "";
    public String msg_nobarcodefound = "No QR code defined.";
    protected void Page_Load(object sender, EventArgs e)
    {
        String num = Request.QueryString["num"];
        StringBuilder sb = new StringBuilder();
                    int index = 0;
                    string previoustxt = "";
        if (num != null)
        {
            barcodeString = num; ;
            String[] barcodes = num.Split(',');

            foreach (String barcode in barcodes)
            {
                if (!String.IsNullOrEmpty(barcode.Trim()))
                {
                    if (index % 2 == 0) sb.Append("<tr>");
                    sb.Append(String.Format("<td><div id='barcode_{0}' style='margin-top:10px;margin-bottom:10px;margin-right:100px;margin-left:100px' ></div>", barcode));
                    sb.Append("</td>");

                    if (index % 2 == 1)
                    {
                        sb.Append("</tr>");
                        sb.Append("<tr style='height:80px'>");
                        sb.Append("<td align ='center' valign ='top' >");
                        sb.Append("<span style='text-align: center;font-weight:bold;' >" + previoustxt + "</span>");
                        sb.Append("</td>");
                        sb.Append("<td align ='center' valign ='top'>");
                        sb.Append("<span style='text-align: center;font-weight:bold;' >" + barcode + "</span>");
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }


                    previoustxt = barcode;
                    index++;
                }
            }
        }
        if (sb.Length > 0)
        {
            if (index % 2 == 1)
            {
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append("<td align ='center'>");
                    sb.Append("<span style='text-align: center;font-weight:bold;margin-bottom:20px' >" + previoustxt + "</span>");
                    sb.Append("</td>");
                    sb.Append("<td align ='center'>");
                    //sb.Append("<span style='text-align: center;font-weight:bold;margin-bottom:20px' >" + barcode + "</span>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
            }

            sb.Append("</table>");
            sb.Insert(0, "<table>");
            phlBarcode.Controls.Add(new LiteralControl(sb.ToString()));
        }
        else phlBarcode.Controls.Add(new LiteralControl(msg_nobarcodefound));

    }
}