<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewReport.ascx.cs" Inherits="SentinelFM.Reports_UserControl_ViewReport" %>
<script type="text/javascript">
    RefreshPage_ViewReport();
    //refresh this user control
    function RefreshPage_ViewReport() {
        if ($telerik.$('#ViewRepor_iframe_Report').attr("src") == "") {

            var viewRepor_Hidden_Msg = $telerik.$('#<%= ViewRepor_Hidden_Msg.ClientID %>').attr("value");
            if (viewRepor_Hidden_Msg != null && viewRepor_Hidden_Msg != "")
                $telerik.$('#ViewRepor_iframe_Report').attr("src", viewRepor_Hidden_Msg)
        }
        try {
            if ($telerik.$('#ViewRepor_iframe_Report').attr("src") == "") {//$telerik.$('#ViewRepor_Span_Msg').text("<%= SelectReportMsg %>");
                var tabStrip = $find("<%= RadTabStripClientID %>");
                tabStrip.findTabByValue("2").disable();
            }
            else {
                //$telerik.$('#ViewRepor_Span_Msg').text("");
                var tabStrip = $find("<%= RadTabStripClientID %>");
                tabStrip.findTabByValue("2").enable();
            }
        }
        catch (err) { }
        var height = $telerik.$(document).height();
        var width = $telerik.$(document).width();
        $telerik.$('#ViewRepor_iframe_Report').attr("height", "700px");
        $telerik.$('#ViewRepor_iframe_Report').attr("width", "90%");
    }

    function stateChangeIE(_frame) {
        if (_frame.readyState == "complete")//state: loading ,interactive,   complete
        {
            $telerik.$("#ViewReport_loading").fadeOut();
        }
    }
    function stateChangeFirefox(_frame) {
        $telerik.$("#ViewReport_loading").fadeOut();
    }
</script>
<table style="width :100%; height:100%">
<tr align ="center" style ="height:20px"> 
  <td>
     <span    id="ViewReport_loading" style=" background-color:Green; color:White; font-weight:bold; width :120px; height:15px; font-size:14px; display:none; "     ><%= LoadingResource %> </span>
  </td>
</tr>
<tr align ="center" style ="height:0px"> 
  <td>
     <span id="ViewRepor_Span_Msg" class="errortext"  ></span>
     <asp:HiddenField  runat="server" id="ViewRepor_Hidden_Msg" />
  </td>
</tr>
<tr align ="center" valign="top">
<td>
    <iframe id="ViewRepor_iframe_Report" src="" onreadystatechange='stateChangeIE(this)' onload='stateChangeFirefox(this)'      ></iframe>
</td>
</tr>
</table>


