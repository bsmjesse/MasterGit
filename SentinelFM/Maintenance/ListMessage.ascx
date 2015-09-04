<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListMessage.ascx.cs" Inherits="Maintenance_ListMessage" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table>
   <tr>
      <td>
         <table id="tblGroup_ListMessage" >
            <tr>
               <td>
                  <asp:Label ID="lblGroup" runat="server" class="formtext"  ></asp:Label>
               </td>
            </tr>
            <tr>
               <td>
                  <select id="selectedGroup" size="10" style="width:350px; height:70px" class="formtext" ></select>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   <tr>
      <td>
         <table id="tblVehicles_ListMessage" >
            <tr>
               <td>
                  <asp:Label ID="lblVehicle" runat="server" class="formtext" ></asp:Label>
               </td>
            </tr>
            <tr>
               <td>
                   <telerik:RadGrid AutoGenerateColumns="false" ID="gdVehicle"  Height="310px"
                         AllowPaging="false" runat="server" Skin="Simple" GridLines="Both"
                          meta:resourcekey="gdVehicleResource1" 
                          Width="350px" AllowFilteringByColumn="false" >
                          <GroupingSettings CaseSensitive="false" /> 
                          <MasterTableView >
                             <Columns>
                                <telerik:GridBoundColumn HeaderText="Box ID" DataField="BoxId"
                                    UniqueName="BoxId" >
                                    <ItemStyle Width="60px"  />
                                    <HeaderStyle Width="60px" />
                                </telerik:GridBoundColumn>

                                <telerik:GridBoundColumn HeaderText="Vehicle" DataField="Description"
                                    UniqueName="Description" >

                                </telerik:GridBoundColumn>                                
                             </Columns>
                         </MasterTableView>
                         <ClientSettings>
                             <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true"  />
                          </ClientSettings>

                   </telerik:RadGrid>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   <tr>
      <td align="center">
         <asp:Button ID="btnYes" runat ="server" Text ="Yes" OnClientClick="return ListMessage_ClickYes()" />
         <asp:Button ID="btnNo" runat ="server" Text ="No" OnClientClick="return CLoseConfirm()" />
         <asp:HiddenField ID="hidButtonSaveCtl" runat ="server" />
      </td>
   </tr>
</table>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >

        function ListMessage_ClickYes() {
            CLoseConfirm();
            var ctl = $telerik.$("#<%= hidButtonSaveCtl.ClientID %>").val();
            $telerik.$("#" + ctl).click();
            return false;
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function ListMessage_SetGroupsVehicle(groups, vehicles, isDelete, ctl) {
            $telerik.$("#<%= hidButtonSaveCtl.ClientID %>").val(ctl);
            if (groups == null) $telerik.$("#tblGroup_ListMessage").hide();
            else
            {
                var vNames = "";
                if (groups.length > 1) {
                    vNames = "<%= affectGroupStr2 %>"
                    vNames = vNames.replace('(n)', '(' + groups.length + ')')
                }
                else vNames = "<%= affectGroupStr1 %>"

                $telerik.$("#<%= lblGroup.ClientID%>").text(vNames);
                $telerik.$('#selectedGroup option').remove();
                for (var index = 0; index < groups.length; index++) {
                    $telerik.$('#selectedGroup').append('<option>' + groups[index].MccName + '</option>');
                }
                $telerik.$("#tblGroup_ListMessage").show();

            }
            if (vehicles == null) $telerik.$("#tblVehicles_ListMessage").hide();
            else {
                var vNames = "";
                if (isDelete) {
                    if (vehicles.length > 1) {
                        vNames = "<%= assignStr2 %>"
                        vNames = vNames.replace('(n)', '(' + vehicles.length + ')')
                    }
                    else vNames = "<%= assignStr1 %>"
                }
                else {
                    if (vehicles.length > 1) {
                        vNames = "<%= overwriteStr2 %>"
                        vNames = vNames.replace('(n)', '(' + vehicles.length + ')')
                    }
                    else vNames = "<%= overwriteStr1 %>"

                }
                $telerik.$("#<%= lblVehicle.ClientID%>").text(vNames);


                var tableView = $find("<%= gdVehicle.ClientID %>").get_masterTableView();
                tableView.set_dataSource(vehicles);
                tableView.dataBind();
                $telerik.$($find("<%= gdVehicle.ClientID %>").get_element()).height(310);
                $telerik.$($find("<%= gdVehicle.ClientID %>").get_element()).find(".rgDataDiv").height(280);
                $telerik.$("#tblVehicles_ListMessage").show();
            }
        }

    </script>
</telerik:RadCodeBlock>