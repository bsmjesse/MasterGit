<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSHistory.aspx.cs" Inherits="HOS_frmHOSHistory"
    meta:resourcekey="PageResource1" Theme="TelerikControl" %>
<%@ Register Src="../UserControl/FleetOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="HOSViewTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js"></script>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:radscriptmanager runat="server" id="RadScriptManager1" asyncpostbacktimeout="300">
        <scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </scripts>
    </telerik:radscriptmanager>

    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" enableajax="false">
    </telerik:RadAjaxManager>
    <div style="text-align: left">
        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="1024px">
            <tr align="left">
                <td>
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" style="width: 800px;">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdHistory" />
                                </td>
                            </tr>
                            <tr valign="top">
                                <td valign="top">
                                    <table id="Table3" border="0" cellpadding="3" cellspacing="3" style="border-color: gray;
                                        border-style: outset; border-width: 2px; height:800px; width: 800px;"   >
                                        <tr style="height:50px" >
                                            <td align="center">

                                                    <table style="border-width: 1px; border-color: Gray; border-style: solid; width: 550px;"
                                                        cellspacing="0">
                                                        <tr>
                                                            <td align="left">
                                                                <asp:label id="lblFromTitle" runat="server" cssclass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                    text="From:"></asp:label>
                                                            </td>
                                                            <td align="left">
                                                                <telerik:raddatepicker id="txtFrom" runat="server" width="100px" height="17px" calendar-showrowheaders="false"
                                                                    calendar-width="170px" meta:resourcekey="txtFromResource2" maxdate="9998-12-31"
                                                                    mindate="1753-01-01" OnLoad="txtFrom_Load" >
                                                                    <calendar userowheadersasselectors="False" usecolumnheadersasselectors="False" showrowheaders="False"
                                                                        viewselectortext="x" width="170px"></calendar>
                                                                    <dateinput height="17px" labelcssclass=""></dateinput>
                                                                    <datepopupbutton imageurl="" hoverimageurl="" cssclass=""></datepopupbutton>
                                                                </telerik:raddatepicker>
                                                            </td>
                                                            <td align="left">
                                                                <asp:label id="lblToTitle" runat="server" cssclass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                    text="To:"></asp:label>
                                                            </td>
                                                            <td align="left">
                                                                <telerik:raddatepicker id="txtTo" runat="server" width="100px" height="17px" calendar-showrowheaders="false"
                                                                    calendar-width="170px" meta:resourcekey="txtFromResource2" maxdate="9998-12-31"
                                                                    mindate="1753-01-01">
                                                                    <calendar userowheadersasselectors="False" usecolumnheadersasselectors="False" showrowheaders="False"
                                                                        viewselectortext="x" width="170px"></calendar>
                                                                    <dateinput height="17px" labelcssclass=""></dateinput>
                                                                    <datepopupbutton imageurl="" hoverimageurl="" cssclass=""></datepopupbutton>
                                                                </telerik:raddatepicker>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td id="FleetColumn" runat="server" class="tableheading" style="width: 52px; height: 14px" align="left">
                                                                <asp:label id="lblFleet" runat="server" cssclass="tableheading" width="33px" meta:resourcekey="lblFleetResource1"
                                                                    text="Fleet:"></asp:label>
                                                            </td>
                                                            <td style="width: 312px; height: 14px" align="left">
                                                                <asp:dropdownlist id="cboFleet" runat="server" autopostback="True" cssclass="RegularText"
                                                                    width="258px" datatextfield="FleetName" datavaluefield="FleetId" onselectedindexchanged="cboFleet_SelectedIndexChanged"
                                                                    meta:resourcekey="cboFleetResource1">
                                                                </asp:dropdownlist>
                                                                <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                                                            </td>
                                                            <td class="formtext" style="width: 32px">
                                                                <asp:label id="lblVehicleName" runat="server" cssclass="tableheading" width="53px"
                                                                    meta:resourcekey="lblVehicleNameResource1" text="Vehicle:"></asp:label>
                                                            </td>
                                                            <td style="width: 300px" align="left">
                                                                <asp:dropdownlist id="cboVehicle" runat="server" cssclass="RegularText" width="258px"
                                                                    datatextfield="Description" datavaluefield="BoxId" meta:resourcekey="cboVehicleResource1">
                                                                </asp:dropdownlist>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tableheading" style="width: 52px; height: 14px" align="left">
                                                                <asp:label id="lblDriver" runat="server" cssclass="tableheading" width="33px" meta:resourcekey="lblDriverResource1"
                                                                    text="Driver:"></asp:label>
                                                            </td>
                                                            <td style="width: 312px; height: 14px" align="left">
                                                                <asp:dropdownlist id="cboDriver" runat="server" cssclass="RegularText" width="258px"
                                                                    datatextfield="drivername" datavaluefield="driverid" meta:resourcekey="cboDriverResource1">
                                                                </asp:dropdownlist>
                                                            </td>
                                                            <td class="formtext" align="left" colspan="2">
                                                                <asp:checkbox id="chkFlatIncludeName" runat="server" cssclass="tableheading" meta:resourcekey="chkFlatIncludeNameResource1"
                                                                    text="Including driver name in flat file">
                                                                </asp:checkbox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="4" align="center">
                                                                <asp:button id="cmdViewAllData" runat="server" text="View History" onclick="cmdViewAllData_Click"
                                                                    height="29px" style="margin-top: 2px" width="167px" meta:resourcekey="cmdViewAllDataResource1" />
                                                                &nbsp;&nbsp;&nbsp;
                                                                <asp:button id="cmdFlatFile" runat="server" text="Create Flat File" height="29px"
                                                                    style="margin-top: 2px" width="167px" meta:resourcekey="cmdFlatFileResource1"
                                                                    onclick="cmdFlatFile_Click" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="4" align="center">
                                                                <asp:label id="lblMessage" runat="server" cssclass="errortext" meta:resourcekey="lblMessageResource1">
                                                                </asp:label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                            </td>
                                        </tr>
                                        <tr valign="top" >
                                            <TD style="WIDTH: 800px;  " align="center" valign="top">
                                                <telerik:RadGrid  ID="dgHistory" runat="server" AllowPaging="True"  width="1024px"
                                                    AutoGenerateColumns="False" PageSize="12"
                                                          EnableTheming="True" 
                                                          AllowFilteringByColumn="true"
                                                          AllowSorting="True" 
                                                          FilterItemStyle-HorizontalAlign="Left" Skin="Hay" onneeddatasource="dgHistory_NeedDataSource" 
                                                         >
                                                    <groupingsettings casesensitive="False" />
                                                    
                                                    <mastertableview commanditemdisplay="Top">
                                                      <Columns>
                                                        <telerik:GridBoundColumn DataField="BoxId" HeaderText="Box ID" 
                                                            meta:resourcekey="BoxIdResource1" UniqueName="BoxId">
                                                            <ItemStyle Width="500px" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="VehicleName" HeaderText="Vehicle" 
                                                            meta:resourcekey="DescriptionResource1" UniqueName="VehicleName">
                                                            <ItemStyle Width="100px" />

                                                        </telerik:GridBoundColumn>

                                                        <telerik:GridBoundColumn DataField="EventDateTime" HeaderText="Original Time" 
                                                            meta:resourcekey="EventDateTimeResource1" UniqueName="EventDateTime">
                                                            <ItemStyle Width="150px" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn HeaderText="MDT Time"  DataField="MdtDateTime"
                                                            meta:resourcekey="MdtDateTimeResource2" SortExpression="MdtDateTime" 
                                                            UniqueName="MdtDateTime">
                                                            <ItemStyle Width="150px" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn DataField="DriverName" HeaderText="Driver"   
                                                            meta:resourcekey="DriverNameResource2" UniqueName="DriverName">
                                                            <ItemStyle Width="150px" />
                                                        </telerik:GridBoundColumn>
                                                        <telerik:GridBoundColumn HeaderText="Odometer"  DataField="Odometer" 
                                                            meta:resourcekey="OdometerResource2" SortExpression="Odometer" AllowFiltering="false"
                                                            UniqueName="Odometer">
                                                            <ItemStyle Width="70px" />
                                                        </telerik:GridBoundColumn>

                                                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" 
                                                            meta:resourcekey="DescriptionResource3" UniqueName="Description">
                                                            <ItemStyle Width="300px" />

                                                        </telerik:GridBoundColumn>

                                                    </Columns>
                                                    <CommandItemTemplate>
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="right">
                                                                <nobr>
                                                            <asp:ImageButton ID="imgFilter" runat="server" OnClientClick ="javascript:return showFilterItem();" ImageUrl="~/images/filter.gif" />
                                                            <asp:LinkButton ID="hplFilter" runat="server"  OnClientClick ="javascript:return showFilterItem();" Text="Show Filter" meta:resourcekey="hplFilterResource1" Font-Underline="true"></asp:LinkButton>
                                                            </nobr>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    </CommandItemTemplate> 
                                                    </mastertableview>
                                                    <filteritemstyle horizontalalign="Left" />
                                                    <filteritemstyle horizontalalign="Left"></filteritemstyle>
                                                    <filtermenu cssclass="FiltMenuCss" enabletheming="True">
                                                    </filtermenu>
                                                    <ClientSettings>
                                                    <ClientEvents OnGridCreated="GridCreated" />
                                                    </ClientSettings>
                                                </telerik:RadGrid>
                                                <asp:HiddenField ID="hidFilter" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                </td>
            </tr>
        </table>
    </div>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgHistory.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=dgHistory.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgHistory.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }

        </script>
    </telerik:RadCodeBlock>    </form>
</body>
</html>
