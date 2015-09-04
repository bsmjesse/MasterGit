<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ListMessageAssign_Dt.ascx.cs" Inherits="Maintenance_ListMessageAssign_Dt" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
       <table>
   <tr>
      <td>
         <table id="tblVehicles_ListMessage_Dt" >
            <tr>
               <td>
                  <asp:Label ID="lblVehicle_Dt" runat="server" class="formtext" ></asp:Label>
               </td>
            </tr>
            <tr>
               <td>
                   <telerik:RadGrid AutoGenerateColumns="false" ID="gdVehicle_Dt"  Height="310px"
                         AllowPaging="false" runat="server" Skin="Simple" GridLines="Both"
                          meta:resourcekey="gdVehicle_DtResource1" 
                          Width="330px" AllowFilteringByColumn="false" 
                       onitemdatabound="gdVehicle_Dt_ItemDataBound" >
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


                            <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="ColcalValue" HeaderText="Start Date"  >
                               <ItemTemplate>
                                    <telerik:RadDatePicker ID="calValue" runat="server"  DateInput-EmptyMessage="" Width="100px"
                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US"  > 
                                    <Calendar ID="Calendar1"  runat="server" >
                                        <SpecialDays>
                                           <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>

                                    </Calendar>
                                    <DateInput ID="DateInput1"  Culture="en-US"  runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" >
                                      <ClientEvents  OnValueChanging="SaveInitalInputCalValue_Dt" />
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
                          </ClientSettings>
                   </telerik:RadGrid>
               </td>
            </tr>
         </table>
      </td>
   </tr>
   <tr>
      <td align="center">
         <asp:Button ID="btnYes_Dt" runat ="server" Text ="Yes" OnClientClick="return ListMessage_Dt_ClickYes()" />
         <asp:Button ID="btnNo_Dt" runat ="server" Text ="No" OnClientClick="return CLoseConfirm_Dt()" />
         <asp:HiddenField ID="hidButtonSaveCtl_Dt" runat ="server" />

      </td>
   </tr>
</table>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" >

        function ListMessage_Dt_ClickYes() {
            CLoseConfirm_Dt();
            var ctl = $telerik.$("#<%= hidButtonSaveCtl_Dt.ClientID %>").val();
            $telerik.$("#" + ctl).click();
            return false;
        }
        function ListMessage_SetGroupsVehicle_Dt(vehicles, isDelete, ctl) {
            $telerik.$("#<%= hidButtonSaveCtl_Dt.ClientID %>").val(ctl);
            if (vehicles == null) $telerik.$("#tblVehicles_ListMessage_Dt").hide();
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
                $telerik.$("#<%= lblVehicle_Dt.ClientID%>").text(vNames);

                $telerik.$("#tblVehicles_ListMessage_Dt").show();
            }
        }

        function SaveInitalInputCalValue_Dt(sender, eventArgs) {
            if ($telerik.$.trim(eventArgs.get_newValue()) == '') {
                eventArgs.set_cancel(true);
            }
        }


    </script>
</telerik:RadCodeBlock>