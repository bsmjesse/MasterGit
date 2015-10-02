<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/MasterPage/MasterPage.master" CodeFile="frmMaintenanceReport.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceReport" Theme="TelerikControl" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  
         EnableAJAX="true">
        <AjaxSettings>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server"  >
    <table style="background-color:white">
        <tr>
            <td>
                <table>
                   <tr>
                      <td>
                         <asp:DropDownList ID="ddlDateTime" runat = "server" class="formText" AutoPostBack="true"
                              onselectedindexchanged="ddlDateTime_SelectedIndexChanged" >
                            <asp:ListItem Text="One Week"     Value = "1" Selected="True" ></asp:ListItem>
                            <asp:ListItem Text="Two Weeks"  Value = "2" ></asp:ListItem>
                            <asp:ListItem Text="One Month"    Value = "3" ></asp:ListItem>
                            <asp:ListItem Text="Two Months" Value = "4" ></asp:ListItem>
                            <asp:ListItem Text="Three Months"  Value = "5" ></asp:ListItem>
                            <asp:ListItem Text="Six Months" Value = "6" ></asp:ListItem>
                            <asp:ListItem Text="One Year"      Value = "7" ></asp:ListItem>
                            <asp:ListItem Text="Two Years"   Value = "8" ></asp:ListItem>
                            <asp:ListItem Text="All"           Value = "9" ></asp:ListItem>
                         </asp:DropDownList>
                      </td>
                   </tr>
                </table>
            </td>
        </tr>
        <tr>
           <td style="margin-left: 40px">
               
            <Sentinel:Grid ID="dgMaintenance" runat="server" Visible="true" FilterItemStyle-HorizontalAlign="Left"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1"
                AllowFilteringByColumn="True" ExportText="Export" 
                AllowPaging="true"  PageSize="20"  Width="100%" Height="100%" 
                onneeddatasource="dgMaintenance_NeedDataSource" IsAutoResize="true" 
                   Skin="Simple" 
                >
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView  CommandItemDisplay="Top"  >
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Table" DataField="target"
                            UniqueName="Table" >
                            <ItemStyle Width="60px"  />
                            <HeaderStyle Width="60px" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Name" DataField="Name"
                            UniqueName="Name" >
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Table" DataField="target"
                            UniqueName="Table" >
                            <ItemStyle Width="60px"  />
                            <HeaderStyle Width="60px" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="operation" DataField="operation"
                            UniqueName="operation" >
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Date" DataField="date"
                            UniqueName="date" >
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="User" DataField="UserName"
                            UniqueName="UserName" >
                        </telerik:GridBoundColumn>


                    </Columns>
                    <CommandItemTemplate>
                        <table id="tblCustomerCommand" runat="server" width="100%">
                            <tr>
                                <td align="left"> 
                                </td>
                            </tr>
                        </table>
                    </CommandItemTemplate>
                </MasterTableView>
                <ClientSettings>
                    <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="false">
                    </Resizing>
                </ClientSettings>
            </Sentinel:Grid>
            </td>
        </tr>
    </table>
    </asp:Panel>     
</asp:Content>
