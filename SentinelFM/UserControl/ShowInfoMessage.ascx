<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShowInfoMessage.ascx.cs"
    Inherits="UserControl_ShowInfoMessage" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table width="100%">
    <tr>
        <td align="center">
            <span id="ShowInfoMessage_spanHeader" class="formtext"></span>
        </td>
    </tr>
    <tr>
        <td align="center">
            <select id="ShowInfoMessage_selectedVehicle" size="15" style="width: 300px" class="formtext">
            </select>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Button ID="btnYes" runat="server" 
                meta:resourcekey="btnYesResource1" Text="Yes" CssClass="combutton" />
            <asp:Button ID="btnNo" runat="server" meta:resourcekey="btnNoResource1" Text="No"
                 CssClass="combutton" />
        </td>
    </tr>
</table>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
  <script type="text/javascript" >
      function ShowInfoMessage_SetValue(title, data, yesEvent, noEvent, hideYes, hideClose, yesText, noText) {
          $telerik.$("#ShowInfoMessage_spanHeader").text(title);
          $telerik.$('#ShowInfoMessage_selectedVehicle').find("option").remove();
          if (data != null && data.length > 0) {
              for (var i = 0; i < data.length; i++)
                  $telerik.$('#ShowInfoMessage_selectedVehicle').append('<option>' + data[i] + '</option>');
          }

          if (yesEvent != null) {
              $telerik.$("#<%= btnYes.ClientID %>").unbind("click");
              $telerik.$("#<%= btnYes.ClientID %>").bind("click", yesEvent);
          }
          if (noEvent != null) {
              $telerik.$("#<%= btnNo.ClientID %>").unbind("click");
              $telerik.$("#<%= btnNo.ClientID %>").bind("click", noEvent);
          }

          if (hideYes == true) $telerik.$("#<%= btnYes.ClientID %>").css("display", "none");
          else $telerik.$("#<%= btnYes.ClientID %>").css("display", "inline");
          if (hideClose == true) $telerik.$("#<%= btnNo.ClientID %>").css("display", "none");
          else $telerik.$("#<%= btnNo.ClientID %>").css("display", "inline");

          if (yesText != null) $telerik.$("#<%= btnYes.ClientID %>").val(yesText);
          else $telerik.$("#<%= btnYes.ClientID %>").val("Yes");
          if (noText != null) $telerik.$("#<%= btnNo.ClientID %>").val(noText);
          else $telerik.$("#<%= btnNo.ClientID %>").val('No');

      }


  </script>
</telerik:RadCodeBlock>