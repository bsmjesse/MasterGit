using System;
using System.Web.UI;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Web;

namespace VLF3.Messaging
{
    public partial class FormService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int fid = 0;
            string isInbox;

            int.TryParse(Page.Request.QueryString["fid"], out fid);
            isInbox = Request.QueryString["isInbox"];

            string body = string.Empty;
            switch (fid)
            {
                case 0:
                    if (isInbox.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        body = "<div style=\"position:relative;height :100%\"><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:0px;\">*********** Elite Swine Inc. ***********</span> " + 
                               "<input id=\"input_1\" name=\"input_1\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:56px;\" size=\"1\" value=\"\" title=\"YesNo: required:false\" onblur=\"validate(1,201,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:32px;top:56px;\">Landmark, MB</span><input id=\"input_2\" name=\"input_2\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:384px;top:56px;\" size=\"1\" value=\"\" title=\"YesNo: required:false\" onblur=\"validate(2,201,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:416px;top:56px;\">Strathmore, AB</span><input id=\"input_3\" name=\"input_3\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:84px;\" size=\"1\" value=\"\" title=\"YesNo: required:false\" onblur=\"validate(3,201,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:32px;top:84px;\">Brandon, MB</span><input id=\"input_4\" name=\"input_4\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:384px;top:84px;\" size=\"1\" value=\"\" title=\"YesNo: required:false\" onblur=\"validate(4,201,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:416px;top:84px;\">Moorefield, ON</span><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:169px;\">DELIVERY RECORD</span><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:464px;top:169px;\">REF #</span><input id=\"input_5\" name=\"input_5\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:560px;top:169px;\" size=\"5\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(5,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:226px;\">DELIVERY DATE:</span><input id=\"input_6\" name=\"input_6\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:240px;top:226px;\" size=\"8\" value=\"\" title=\"Date: required:false\" onblur=\"validate(6,305,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:400px;top:226px;\">TIME:</span><input id=\"input_7\" name=\"input_7\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:496px;top:226px;\" size=\"6\" value=\"\" title=\"Time12h: required:false\" onblur=\"validate(7,101,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:254px;\">NAME OF FEEDER:</span><input id=\"input_8\" name=\"input_8\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:256px;top:254px;\" size=\"24\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(8,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:283px;\">BARN:</span><input id=\"input_9\" name=\"input_9\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:96px;top:283px;\" size=\"10\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(9,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:311px;\">TOTAL FEEDERS DELIVERED:</span><input id=\"input_10\" name=\"input_10\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:400px;top:311px;\" size=\"6\" value=\"\" title=\"UnsignedInt: required:false\" onblur=\"validate(10,301,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:339px;\">GROSS:</span><input id=\"input_11\" name=\"input_11\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:112px;top:339px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(11,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:240px;top:339px;\">TARE:</span><input id=\"input_12\" name=\"input_12\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:336px;top:339px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(12,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:464px;top:339px;\">NET:</span><input id=\"input_13\" name=\"input_13\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:544px;top:339px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(13,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:368px;\">TRUCK DRIVER:</span><input id=\"input_14\" name=\"input_14\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:224px;top:368px;\" size=\"26\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(14,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:424px;\">----------------------------------------</span><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:481px;\">BILL OF LADING FOR FEEDERS</span><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:464px;top:481px;\">REF #</span><input id=\"input_15\" name=\"input_15\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:560px;top:481px;\" size=\"5\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(15,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:538px;\">PICKUP DATE:</span><input id=\"input_16\" name=\"input_16\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:208px;top:538px;\" size=\"8\" value=\"\" title=\"Date: required:false\" onblur=\"validate(16,305,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:368px;top:538px;\">TIME:</span><input id=\"input_17\" name=\"input_17\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:464px;top:538px;\" size=\"6\" value=\"\" title=\"Time12h: required:false\" onblur=\"validate(17,101,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:566px;\">FEEDER PRODUCER:</span><input id=\"input_18\" name=\"input_18\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:272px;top:566px;\" size=\"23\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(18,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:594px;\">BARN:</span><input id=\"input_19\" name=\"input_19\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:96px;top:594px;\" size=\"10\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(19,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:623px;\">NUMBER OF FEEDERS:</span><input id=\"input_20\" name=\"input_20\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:304px;top:623px;\" size=\"6\" value=\"\" title=\"UnsignedInt: required:false\" onblur=\"validate(20,301,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:651px;\">GROSS:</span><input id=\"input_21\" name=\"input_21\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:112px;top:651px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(21,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:240px;top:651px;\">TARE:</span><input id=\"input_22\" name=\"input_22\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:336px;top:651px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(22,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:464px;top:651px;\">NET:</span><input id=\"input_23\" name=\"input_23\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:544px;top:651px;\" size=\"6\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(23,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:679px;\">TRUCK NO.</span><input id=\"input_24\" name=\"input_24\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:160px;top:679px;\" size=\"8\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(24,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:320px;top:679px;\">TRAILER NO.</span><input id=\"input_25\" name=\"input_25\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:512px;top:679px;\" size=\"8\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(25,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:708px;\">TRUCK DRIVER:</span><input id=\"input_26\" name=\"input_26\" type=\"text\" style=\"font-family:monospace;font-size:12pt;position:absolute;left:224px;top:708px;\" size=\"26\" value=\"\" title=\"AlphanumericFreeForm: required:false\" onblur=\"validate(26,100,false,this.value)\"/><span style=\"font-family:monospace;font-size:12pt;position:absolute;left:0px;top:792px;\"></span></div>";

                        //body = \"<div id='Inbox_MessagePane'>" +
                        //       "<input type='text' id='inbox_0' class='stdin' maxlength='40' value=''/><br />" +
                        //       "<input type='text' id='inbox_1' class='stdin' maxlength='40' value=''/><br />" +
                        //       "<input type='text' id='inbox_2' class='stdin' maxlength='40' value=''/><br />" +
                        //       "<input type='text' id='inbox_3' class='stdin' maxlength='40' value=''/></div>";
                    }
                    else
                    {
                        body = "<div id='Outbox_MessagePane'>" +
                               "<input type='text' id='outbox_0' class='stdin' maxlength='40' value=''/><br />" +
                               "<input type='text' id='outbox_1' class='stdin' maxlength='40' value=''/><br />" +
                               "<input type='text' id='outbox_2' class='stdin' maxlength='40' value=''/><br />" +
                               "<input type='text' id='outbox_3' class='stdin' maxlength='40' value=''/></div>";
                    }
                    break;
                case -1: //Reply
                    body = "<table><tr><td>REPLY</td></tr></table><table><tr><td>Input from</td><td>"+
                            "<input readonly type =\"text\" runat=\"server\" id=\"inpFrom\" value=''></td></tr><tr><td>Input to</td><td>" +
                            "<input readonly type =\"text\" id=\"inpTo\" value=''></td></tr><tr><td valign ='top'>" +
                            "Message Text</td><td><textarea id='msgBody' name='limitedtextarea'"+ 
                            "onKeyDown='limitText(this.form.limitedtextarea,this.form.countdown,100);'"+
                            "onKeyUp='limitText(this.form.limitedtextarea,this.form.countdown,100);'>"+
                            "</textarea><br><font size='1'>(Maximum characters: 100)<br>You have "+
                            "<input readonly type='text' name='countdown' size='3' value='100'> characters left.</font>"+
                            "</td></tr></table>";
                    break;
                case -5: //Send Response
                    body = (SendMessage()) ? "1" : "0";
                    break;
                case -3: //New Form Message
                    body = "<table><tr><td>New Form Message</td></tr></table><table><tr><td>Input from</td><td><input type ='text'></td></tr><tr><td>Input to</td><td><input type ='text'></td></tr></table>";
                    break;
                case 1:
                    body = "<div id=\"formContainer\"><textarea id=\"input_0\"  rows=\"10\" style =\"width :99%\" >&nbsp;</textarea></div>";
                    break;
                case 2:
                    body = "<table style =\"width :99%\" rules ='all' style ='background-color:Aqua'><tr><td>Test1</td></tr><tr><td>Test2</td></tr><tr><td>Test3</td></tr><tr><td><textarea id=\"input_0\" rows=\"10\" style =\"width :99%\">&nbsp;</textarea></td></tr></table>";
                    break;
                case 3:
                    body = "<div id=\"formContainer\"><textarea id=\"input_0\" rows=\"10\" style =\"width :99%\">&nbsp;</textarea></div>";
                    break;
                case 4:
                    body = "<table style =\"width :99%\" rules ='all' style ='background-color:Aqua'><tr><td>Test1</td></tr><tr><td>Test2</td></tr><tr><td>Test3</td></tr><tr><td><textarea id=\"input_0\" rows=\"10\" style =\"width :99%\">&nbsp;</textarea></td></tr></table>";
                    break;
            }
            Response.Write(body);
            Response.End();
        }
        
        private bool SendMessage()
        {
            try
            {
                MailMessage message = new MailMessage(Request.Params["sender"], Request.Params["recipient"], "Reply", Request.Params["body"]);

                Configuration config= WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                SmtpClient client = new SmtpClient();
                NetworkCredential cred = new NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
                client.Credentials = cred;
                //Send the message.
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                String error = ex.Message; 
            }
            return false;
        }
    }
}