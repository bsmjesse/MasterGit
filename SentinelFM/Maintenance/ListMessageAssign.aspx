<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListMessageAssign.aspx.cs" Inherits="SentinelFM.Maintenance_ListMessageAssign" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   <telerik:RadScriptManager runat="server" ID="RadScriptManager1" AsyncPostBackTimeout="300">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" 
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnYes" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicle" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager> 
    <div>
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
                          Width="470px" AllowFilteringByColumn="false" >
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

                                <telerik:GridBoundColumn HeaderText="Vehicle" DataField="Description"
                                    UniqueName="Description" >

                                </telerik:GridBoundColumn>                                

                            <telerik:GridTemplateColumn AllowFiltering="false" Visible="false" UniqueName="ColcalValue" HeaderText="Start Date"  >
                               <ItemTemplate>
                               <telerik:RadDatePicker ID="calValue" runat="server"  DateInput-EmptyMessage="" Width="100px"
                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US" >
                                    <Calendar ID="Calendar1" Width="120px" runat="server" >
                                        <SpecialDays>
                                           <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>

                                    </Calendar>
                                    <DateInput ID="DateInput1"  Culture="en-US"  runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" LabelCssClass="" >
                                      <ClientEvents  OnValueChanged="InputCalValueChanged"  OnError = "onErrorInputValue" OnValueChanging="SaveInitalInputCalValue"  />
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
         <asp:Button ID="btnYes" runat ="server" Text ="Yes" OnClientClick="return ListMessage_ClickYes()" />
         <asp:Button ID="btnNo" runat ="server" Text ="No" OnClientClick="return CLoseConfirm()" />
         <asp:HiddenField ID="hidButtonSaveCtl" runat ="server" />
      </td>
   </tr>
</table>
    </div>
    </form>
</body>
</html>
