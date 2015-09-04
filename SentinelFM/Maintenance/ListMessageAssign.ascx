<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListMessageAssign.ascx.cs" Inherits="Maintenance_ListMessageAssign" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
       <table>
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
                          Width="330px" AllowFilteringByColumn="false" >
                          <GroupingSettings CaseSensitive="false" /> 
                          <MasterTableView DataKeyNames="VehicleId" ClientDataKeyNames="VehicleId" >
                             <Columns>
                                <telerik:GridBoundColumn HeaderText="Box ID" DataField="BoxId"
                                    UniqueName="BoxId" >
                                    <ItemStyle Width="60px"  />
                                    <HeaderStyle Width="60px" />
                                </telerik:GridBoundColumn>

                                <telerik:GridBoundColumn HeaderText="Vehicle" DataField="Description"
                                    UniqueName="Description" >

                                </telerik:GridBoundColumn>                                


                            <telerik:GridTemplateColumn AllowFiltering="false" Visible="false" UniqueName="ColcalValue" HeaderText="Start Date"  >
                               <ItemTemplate>
                                    <telerik:RadDatePicker ID="calValue" runat="server"  DateInput-EmptyMessage="" Width="100px"
                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US"  > 
                                    <Calendar ID="Calendar1"  runat="server" >
                                        <SpecialDays>
                                           <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>

                                    </Calendar>
                                    <DateInput ID="DateInput1"  Culture="en-US"  runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" >
                                      <ClientEvents  OnValueChanging="SaveInitalInputCalValue" />
                                    </DateInput>
                                    
                                </telerik:RadDatePicker>
                               </ItemTemplate>
                               <ItemStyle Width="110px" />
                               <HeaderStyle  Width="110px"  />
                            </telerik:GridTemplateColumn>
                             </Columns>
                          </MasterTableView>
                          <ClientSettings>
                             <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true"  />
                             <ClientEvents OnRowDataBound="gridRowBound"/>
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

        function gridRowBound(sender, args) 
        {             
           //var link = args.get_item().findElement("taskName"); 
            //link.innerHTML = args.get_dataItem().Text;

       }

        function ListMessage_ClickYes() {
            CLoseConfirm();
            var ctl = $telerik.$("#<%= hidButtonSaveCtl.ClientID %>").val();
            $telerik.$("#" + ctl).click();
            return false;
        }

        function ListMessage_Clean() {
            var tableView = $find("<%= gdVehicle.ClientID %>").get_masterTableView();
            var emptyData = eval(<%= emptyData %>);
            tableView.set_dataSource(emptyData);
            tableView.dataBind();
        }

        function ListMessage_SetGroupsVehicle(vehicles, isDelete, ctl) {
            $telerik.$("#<%= hidButtonSaveCtl.ClientID %>").val(ctl);
            if (vehicles == null) $telerik.$("#tblVehicles_ListMessage").hide();
            else {
                var vNames = "";
                if (isDelete) {
                    if (vehicles.length > 1) {
                        vNames = "<%= deleteStr2 %>"
                        vNames = vNames.replace('(n)', '(' + vehicles.length + ')')
                    }
                    else vNames = "<%= deleteStr1 %>"
                }
                else {
                    if (vehicles.length > 1) {
                        vNames = "<%= assignStr2 %>"
                        vNames = vNames.replace('(n)', '(' + vehicles.length + ')')
                    }
                    else vNames = "<%= assignStr1 %>"

                }
                $telerik.$("#<%= lblVehicle.ClientID%>").text(vNames);

                var tableView = $find("<%= gdVehicle.ClientID %>").get_masterTableView();
                tableView.set_dataSource(vehicles);
                tableView.dataBind();
                $telerik.$("#tblVehicles_ListMessage").show();
            }
        }

        function SaveInitalInputCalValue(sender, eventArgs) {
            if ($telerik.$.trim(eventArgs.get_newValue()) == '') {
                eventArgs.set_cancel(true);
            }
        }


    </script>
</telerik:RadCodeBlock>