<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceOperation.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceOperation" Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1">
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server" meta:resourcekey="pnlResource1"  >
    <table style="background-color:white">
        <tr>
            <td>
                <table>
                   <tr>
                      <td>
                         <asp:DropDownList ID="ddlDateTime" runat = "server" class="formText" AutoPostBack="True"
                              onselectedindexchanged="ddlDateTime_SelectedIndexChanged" 
                              meta:resourcekey="ddlDateTimeResource1" >
                            <asp:ListItem Text="One Week"     Value = "1" Selected="True" 
                                 meta:resourcekey="ListItemResource1" ></asp:ListItem>
                            <asp:ListItem Text="Two Weeks"  Value = "2" 
                                 meta:resourcekey="ListItemResource2" ></asp:ListItem>
                            <asp:ListItem Text="One Month"    Value = "3" 
                                 meta:resourcekey="ListItemResource3" ></asp:ListItem>
                            <asp:ListItem Text="Two Months" Value = "4" 
                                 meta:resourcekey="ListItemResource4" ></asp:ListItem>
                            <asp:ListItem Text="Three Months"  Value = "5" 
                                 meta:resourcekey="ListItemResource5" ></asp:ListItem>
                            <asp:ListItem Text="Six Months" Value = "6" 
                                 meta:resourcekey="ListItemResource6" ></asp:ListItem>
                            <asp:ListItem Text="One Year"      Value = "7" 
                                 meta:resourcekey="ListItemResource7" ></asp:ListItem>
                            <asp:ListItem Text="Two Years"   Value = "8" 
                                 meta:resourcekey="ListItemResource8" ></asp:ListItem>
                            <asp:ListItem Text="All"           Value = "9" 
                                 meta:resourcekey="ListItemResource9" ></asp:ListItem>
                         </asp:DropDownList>
                      </td>
                   </tr>
                </table>
            </td>
        </tr>
        <tr>
           <td style="margin-left: 40px">
               
            <Sentinel:Grid ID="dgMaintenance" runat="server"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1"
                AllowFilteringByColumn="True" ExportText="Export" 
                AllowPaging="True"  PageSize="20"  Width="100%" Height="100%" OnDataBinding="dgMaintenance_DataBinding" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="True"
                   Skin="Simple" allText="All" ClearAllFiltersText="Clear All Filters" 
                   GridLines="None" IsShowExportIcon="True" IsShowFilterIcon="True" meta:resourcekey="dgMaintenanceResource1" 
                >
                <GroupingSettings CaseSensitive="False" />
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" 
                        SaveScrollPosition="False"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True">
                    </Resizing>
                </ClientSettings>
                <MasterTableView CommandItemDisplay="Top">
                    <CommandItemSettings ExportToPdfText="Export to Pdf" />
                    <RowIndicatorColumn>
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn>
                        <HeaderStyle Width="20px" />
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="target" HeaderText="Table" 
                            meta:resourcekey="GridBoundColumnResource1" UniqueName="Table">
                            <HeaderStyle Width="110px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Name" 
                            meta:resourcekey="GridBoundColumnResource2" UniqueName="Name">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="operation" HeaderText="Operation" 
                            meta:resourcekey="GridBoundColumnResource3" UniqueName="operation">
                            <HeaderStyle Width="110px" />
                            <ItemStyle Width="110px" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="date" HeaderText="Date" 
                            meta:resourcekey="GridBoundColumnResource4" UniqueName="date">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="UserName" HeaderText="User" 
                            meta:resourcekey="GridBoundColumnResource5" UniqueName="UserName">
                        </telerik:GridBoundColumn>
                    </Columns>
                    <CommandItemTemplate>
                        <table ID="tblCustomerCommand" runat="server" width="100%">
                            <tr id="Tr1" runat="server">
                                <td id="Td1" runat="server" align="left">
                                </td>
                            </tr>
                        </table>
                    </CommandItemTemplate>
                </MasterTableView>
                <FilterItemStyle HorizontalAlign="Left" />
                <FilterMenu CssClass="FiltMenuCss" EnableTheming="True">
                </FilterMenu>
            </Sentinel:Grid>
            </td>
        </tr>
    </table>
    </asp:Panel>    
</asp:Content>

