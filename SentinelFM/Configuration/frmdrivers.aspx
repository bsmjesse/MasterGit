<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmdrivers.aspx.cs" Inherits="SentinelFM.Configuration_frmdrivers" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" EnableTheming="true" Theme="Default" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Drivers Info</title>
 	<link href="Configuration.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1
        {
            width: 194px;
        }
    </style>
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" >
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" >
        <ajaxsettings>

       </ajaxsettings>
    </telerik:RadAjaxManager>
    <asp:HiddenField ID="ImportState" runat="server" Value="0" />

    <div>
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdDriver"  />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="1100px">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width:1140px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
                                                                        width: 190px; position: relative; top: 0px; height: 22px">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="cmdDriversInfo" runat="server" CausesValidation="False" CommandName="4"
                                                                                    CssClass="selectedbutton" 
                                                                                    Text="Drivers Info" Width="112px" meta:resourcekey="cmdDriversInfoResource1" /></td>
                                                                            <td style="width: 250px">
                                                                                <asp:Button ID="cmdDriversVehicles" runat="server" CausesValidation="False" CommandName="5"
                                                                                    CssClass="confbutton"  Text="Vehicles-Drivers Assignment"
                                                                                    Width="186px" PostBackUrl="frmdriversvehicles.aspx" meta:resourcekey="cmdDriversVehiclesResource1" /></td>
                                                                        </tr>
                                                                    </table>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder" >
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" 
                                                                                                style="width: 100%; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%;" valign="top">
                                                                                                        <table id="tblViewDrivers" runat="server" style="margin: 12px 0px 12px 0px" 
                                                                                                            width="100%">
                                                                                                            <tr>
                                                                                                                <td style="text-align: left; vertical-align: top; padding-top: 20px;" class="auto-style1">
                                                                                                                    <ul style="list-style: none; line-height: 25px; text-align: left; padding-left:0 ">
                                                                                                                        <li>
                                                                                                                            <asp:LinkButton ID="cmdAddDriver" runat="server" CssClass="linkbutton" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                Text="Add Driver" Width="140px" OnClick="cmdAddDriver_Click" meta:resourcekey="cmdAddDriverResource1" />
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <asp:LinkButton ID="cmdUploadDriverList" runat="server" CssClass="linkbutton" Width="140px" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                Text="Import Drivers" meta:resourcekey="cmdUploadDriverListResource1" OnClick="cmdImportDrivers_Click" />
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <asp:LinkButton ID="cmdUploadAssignments" runat="server" CssClass="linkbutton" Width="140px" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                Text="Import Assignments" meta:resourcekey="cmdUploadAssignmentsResource1" OnClick="cmdImportAssignments_Click" />
                                                                                                                        </li>
                                                                                                                         <li>
                                                                                                                            <asp:LinkButton ID="cmdUpdateDriver" runat="server" CssClass="linkbutton" Width="140px" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                Text="Update Driver" meta:resourcekey="cmdUpdateDriversResource1" OnClick="cmdUpdateDrivers_Click" />
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <hr style="width: 123px;" />
                                                                                                                            <a href="frmExportDrivers.aspx" class="linkbutton"><asp:Label ID="Label2" 
                                                                                                                                runat="server" Text="Create Driver List" 
                                                                                                                                meta:resourcekey="cmdDownloadDriversResource1" Font-Names="Verdana" 
                                                                                                                                Font-Size="11px"></asp:Label></a>
                                                                                                                            
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <asp:HyperLink ID="linkDriverList" runat="server" CssClass="linkbutton" Visible="False" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                meta:resourcekey="linkDriverListResource1" Width="100px"></asp:HyperLink>
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <asp:HyperLink ID="linkDriverAssignmentList" runat="server" CssClass="linkbutton" Visible="False" Font-Names="Verdana" Font-Size="11px"
                                                                                                                                meta:resourcekey="linkDriverAssignmentListResource1" Width="100px"></asp:HyperLink>
                                                                                                                        </li>
                                                                                                                     </ul>
                                                                                                                </td>
                                                                                                                <td valign="top">
                                                                                                                    <telerik:RadGrid  ID="gdvDrivers" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                                                                                        GridLines="None" PageSize="8"
                                                                                                                         EnableTheming="True"
                                                                                                                        Width="555px" 
                                                                                                                        AllowFilteringByColumn="true"
                                                                                                                        AllowSorting="True" 
                                                                                                                        FilterItemStyle-HorizontalAlign="Left" Skin="Hay"
                                                                                                                        meta:resourcekey="gdvDriversResource2" 
                                                                                                                        onitemdatabound="gdvDrivers_ItemDataBound" 
                                                                                                                        onneeddatasource="gdvDrivers_NeedDataSource" 
                                                                                                                        onitemcommand="gdvDrivers_ItemCommand"  >
                                                                                                                        <GroupingSettings CaseSensitive="false" />   
                                                                                                                        <MasterTableView DataKeyNames="DriverId" ClientDataKeyNames="DriverId" CommandItemDisplay="Top"
                                                                                                                           GroupLoadMode="Server" NoMasterRecordsText=""  meta:resourcekey="MasterTableViewResource1" >                                                                                                                        
                                                                                                                           
                                                                                                                        <Columns>
                                                                                                                            <telerik:GridBoundColumn DataField="DriverId" HeaderText ="ID"  meta:resourcekey="BoundFieldResource1" />
                                                                                                                            <telerik:GridBoundColumn DataField="FirstName" HeaderText ="First Name" meta:resourcekey="BoundFieldResource2" />
                                                                                                                            <telerik:GridBoundColumn DataField="LastName" HeaderText = "Last Name" meta:resourcekey="BoundFieldResource3" />
                                                                                                                            <telerik:GridBoundColumn DataField="License" HeaderText ="License" meta:resourcekey="BoundFieldResource4" />
                                                                                                                            <telerik:GridBoundColumn DataField="Class" ItemStyle-Width ="56px" HeaderText ="Class" meta:resourcekey="BoundFieldClassResource" />
                                                                                                                            <telerik:GridBoundColumn DataField="VehicleDescription" HeaderText="Description"  meta:resourcekey="BoundFieldResource5" />
                                                                                                                            <telerik:GridTemplateColumn meta:resourcekey="CommandFieldResource3" AllowFiltering="false" HeaderText='<%$ Resources:gdvDrivers_Emergency %>'  >
                                                                                                                                <ItemTemplate>
                                                                                                                                    <asp:LinkButton ID="lnkEmergency"   runat="server" Text ="Phones"  meta:resourcekey="lnkEmergencyResource1" Visible="false" />
                                                                                                                                    &nbsp;
                                                                                                                                </ItemTemplate>
                                                                                                                            </telerik:GridTemplateColumn>

                                                                                                                            <telerik:GridBoundColumn DataField="KeyFobId" HeaderText ="KeyFob" ItemStyle-Width="0px" meta:resourcekey="BoundFieldResource6" />

                                                                                                                            <telerik:GridTemplateColumn meta:resourcekey="CommandFieldResource2" AllowFiltering="false" >
                                                                                                                                <ItemTemplate>
                                                                                                                                    <asp:ImageButton ID="btnEditDriver" CommandName="EditDriver"  runat="server" ToolTip="Edit Driver"  ImageUrl="~/images/Edit.gif"  meta:resourcekey="btnEditDriverResource1" />
                                                                                                                                </ItemTemplate>
                                                                                                                            </telerik:GridTemplateColumn>

                                                                                                                            <telerik:GridTemplateColumn meta:resourcekey="TemplateFieldResource1" AllowFiltering="false">
                                                                                                                                <ItemTemplate>
                                                                                                                                    <asp:ImageButton ID="btnDeleteDriver" CommandName="DeleteDriver"  runat="server" ToolTip="Delete Driver" ImageUrl="~/images/delete.gif"  meta:resourcekey="btnDeleteDriverResource1" />
                                                                                                                                </ItemTemplate>
                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                        </Columns>
                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                        <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                        <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader"  />
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
                                                                                                                        </MasterTableView>
                                                                                                                        <ClientSettings>
                                                                                                                           <ClientEvents OnGridCreated="GridCreated" />
                                                                                                                        </ClientSettings>

                                                                                                                    </telerik:RadGrid>
                                                                                                                    <asp:HiddenField ID="hidFilter" runat="server" />
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                     <table id="tblDriverDetails" runat="server" cellpadding="2" cellspacing="2" class="RegularText" 
                                                                                                            style="width: 674px; height: 78px; margin: 20px 0px 20px 0px;">
                                                                                                            <tr style="height: 21px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblFirstName" runat="server" CssClass="RegularText" meta:resourcekey="lblFirstNameResource1" Text="First name"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtFirstName" runat="server" Width="154px" meta:resourcekey="txtFirstNameResource1" CssClass="RegularText"></asp:TextBox>
 <span id="valTxtFirstName" style="color:Red;">

		*

	</span></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblLastName" runat="server" CssClass="RegularText" meta:resourcekey="lblLastNameResource1" Text="Last name"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtLastName" runat="server" Width="154px" meta:resourcekey="txtLastNameResource1" CssClass="RegularText"></asp:TextBox>
<span id="valTxtLastName" style="color:Red;">

		*

	</span>   
                                                                                                                    </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblLicense" runat="server" CssClass="RegularText" meta:resourcekey="lblLicenseResource1" Text="License"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtLicense" runat="server" Width="154px" meta:resourcekey="txtLicenseResource1" CssClass="RegularText"></asp:TextBox>
<span id="valTxtLicense" style="color:Red;">

		*

	</span>                         </td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblClass" runat="server" CssClass="RegularText" meta:resourcekey="lblClassResource1" Text="License Class"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtClass" runat="server" Width="154px" meta:resourcekey="txtClassResource1" CssClass="RegularText"></asp:TextBox>
<span id="valTxtClass" style="color:Red;">

		*

	</span>   
                                                                                                                    </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblLicenseIssued" runat="server" CssClass="RegularText" meta:resourcekey="lblLicenseIssuedResource1" Text="License issued"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left" valign="middle">
                                                                                                                    <asp:TextBox ID="txtLicenseIssued" runat="server" Width="126px" meta:resourcekey="txtLicenseIssuedResource1" CssClass="RegularText"></asp:TextBox>&nbsp;
                                                                                                                       <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtLicenseIssued','cal','width=220,height=200,left=270,top=380')">
                                                                                                                          <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-top-width: 0px;
                                                                                                                              border-left-width: 0px; border-bottom-width: 0px; border-right-width: 0px" /></a>
<span id="valTxtLicenseIssued" style="color:Red;">

		*

	</span>  
                                                                                                                </td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblLicenseExpired" runat="server" CssClass="RegularText" Width="102px" meta:resourcekey="lblLicenseExpiredResource1" Text="License expired"></asp:Label></td>
                                                                                                                <td align="left" valign="middle">
                                                                                                                    <asp:TextBox ID="txtLicenseExpired" runat="server" Width="125px" meta:resourcekey="txtLicenseExpiredResource1" CssClass="RegularText"></asp:TextBox>&nbsp;
                                                                                                                        <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtLicenseExpired','cal','width=220,height=200,left=270,top=380')">
                                                                                                                            <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-top-width: 0px;
                                                                                                                                border-left-width: 0px; border-bottom-width: 0px; border-right-width: 0px" /></a>
<span id="valTxtLicenseExpired" style="color:Red;">

		*

	</span>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblGender" runat="server" CssClass="RegularText" meta:resourcekey="lblGenderResource1" Text="Gender"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:DropDownList ID="ddlGender" runat="server" Width="58px" meta:resourcekey="ddlGenderResource1" CssClass="RegularText">
                                                                                                                        <asp:ListItem meta:resourcekey="ListItemResource1" Text="F"></asp:ListItem>
                                                                                                                        <asp:ListItem meta:resourcekey="ListItemResource2" Text="M"></asp:ListItem>
                                                                                                                    </asp:DropDownList></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblHeight" runat="server" CssClass="RegularText" meta:resourcekey="lblHeightResource1" Text="Height"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtHeight" runat="server" Width="154px" meta:resourcekey="txtHeightResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblHomePhone" runat="server" CssClass="RegularText" meta:resourcekey="lblHomePhoneResource1" Text="Home phone"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtHomePhone" runat="server" Width="154px" meta:resourcekey="txtHomePhoneResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblCellPhone" runat="server" CssClass="RegularText" meta:resourcekey="lblCellPhoneResource1" Text="Cell phone"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtCellPhone" runat="server" Width="154px" meta:resourcekey="txtCellPhoneResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblAdditionalPhone" runat="server" CssClass="RegularText" meta:resourcekey="lblAdditionalPhoneResource1" Text="Additional phone"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtAdditionalPhone" runat="server" Width="154px" meta:resourcekey="txtAdditionalPhoneResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                                <td align="left">
                                                                                                                        <asp:Label ID="lblEmail" runat="server" CssClass="RegularText" meta:resourcekey="lblEmailResource1" Text="E-mail"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtEmail" runat="server" Width="154px" meta:resourcekey="txtEmailResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblCity" runat="server" CssClass="RegularText" meta:resourcekey="lblCityResource1" Text="City"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtCity" runat="server" Width="154px" meta:resourcekey="txtCityResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblAddress" runat="server" CssClass="RegularText" meta:resourcekey="lblAddressResource1" Text="Address"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtAddress" runat="server" Width="154px" meta:resourcekey="txtAddressResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblState" runat="server" CssClass="RegularText" meta:resourcekey="lblStateResource1" Text="State/province"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:DropDownList ID="ddlState" runat="server" Width="160px" meta:resourcekey="ddlStateResource1" CssClass="RegularText">
                                                                                                                    </asp:DropDownList></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblZipCode" runat="server" CssClass="RegularText" meta:resourcekey="lblZipCodeResource1" Text="Zip code"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtZipcode" runat="server" Width="154px" meta:resourcekey="txtZipcodeResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblCountry" runat="server" CssClass="RegularText" meta:resourcekey="lblCountryResource1" Text="Country"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtCountry" runat="server" Width="154px" meta:resourcekey="txtCountryResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblDescription" runat="server" CssClass="RegularText" meta:resourcekey="lblDescriptionResource1" Text="Description"></asp:Label></td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtDescription" runat="server" Width="154px" meta:resourcekey="txtDescriptionResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblSMSID" runat="server" CssClass="RegularText" 
                                                                                                                        meta:resourcekey="lblSMSIDResource1" Text="Employee ID"></asp:Label></td>
                                                                                                                <td align="left" >
                                                                                                                    <asp:TextBox ID="txtSmsid" runat="server" Width="154px" meta:resourcekey="txtSmsidResource1" CssClass="RegularText"></asp:TextBox></td>
                                                                                                                     <td align="left" >
                                                                                                                     <asp:Label ID="Label1" runat="server" Text="Confirm Employee ID Password" 
                                                                                                                         meta:resourcekey="Label1Resource1"></asp:Label>
                                                                                                                </td>
                                                                                                                 <td align="left" >
                                                                                                                     <asp:TextBox ID="txtSMSPWDConfirm" runat="server" CssClass="RegularText" OnPreRender="txtSMSPWD_PreRender"
                                                                                                                         TextMode="Password" Width="154px" meta:resourcekey="txtSMSPWDConfirmResource1"></asp:TextBox>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblSMSPWD" runat="server" Text="Employee ID Password" 
                                                                                                                        meta:resourcekey="lblSMSPWDResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left" >
                                                                                                                    <asp:TextBox ID="txtSMSPWD" runat="server" CssClass="RegularText" OnPreRender="txtSMSPWD_PreRender"
                                                                                                                        TextMode="Password" Width="154px" meta:resourcekey="txtSMSPWDResource1"></asp:TextBox></td>
                                                                                                                 <td align="left" >
                                                                                                                     <asp:Label ID="lblTerminationDate" runat="server" Text="Termination Date" 
                                                                                                                         meta:resourcekey="lblTerminationDate"></asp:Label></td>
                                                                                                                 <td align="left" >
                                                                                                                    <asp:TextBox ID="txtTerminationDate" runat="server" Width="125px" 
                                                                                                                         meta:resourcekey="txtTerminationDate" CssClass="RegularText"></asp:TextBox>&nbsp;
                                                                                                                        <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtTerminationDate','cal','width=220,height=200,left=270,top=380')">
                                                                                                                            <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-top-width: 0px;
                                                                                                                                border-left-width: 0px; border-bottom-width: 0px; border-right-width: 0px" /></a></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left"><asp:Label ID="lblKeyFobID" runat="server" Text="Key Fob ID" 
                                                                                                                         meta:resourcekey="lblKeyFobID"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left" >
                                                                                                                    <asp:TextBox ID="txtKeyFobId" runat="server" CssClass="RegularText" 
                                                                                                                         Width="154px" meta:resourcekey="txtKeyFobIdResource1" 
                                                                                                                        ></asp:TextBox></td>
                                                                                                                 <td align="left" > 
                                                                                                                     

                                                                                                                     <asp:Label ID="lblPositionInfo" runat="server" Text="Position Info" 
                                                                                                                         meta:resourcekey="lblPositionInfo"></asp:Label> 
                                                                                                                     

                                                                                                                     </td>
                                                                                                                 <td align="left" >
                                                                                                                    <asp:TextBox ID="txtPositionInfo" runat="server" Width="154px" 
                                                                                                                         meta:resourcekey="txtPositionInfo" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px" runat="server" id="divCycle"> 
                                                                                                                <td align="left"><asp:Label ID="lblUsaCycle" runat="server" Text="US Cycle" 
                                                                                                                         meta:resourcekey="lblUsaCycleResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left" >
                                                                                                                    <asp:DropDownList ID="ddlUsaCycle" runat="server" CssClass="RegularText" 
                                                                                                                        AppendDataBoundItems="True"  Width="154px" 
                                                                                                                        meta:resourcekey="ddlUsaCycleResource1" onchange="javascript:return SelectUSACycle();" >
                                                                                                                        <asp:ListItem Text = "Select a USA Cycle" value="-1" meta:resourcekey="ddlUsaCycleResource1Item0Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US 60/7" value="10" meta:resourcekey="ddlUsaCycleResource1Item1Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US 70/8" value="11" meta:resourcekey="ddlUsaCycleResource1Item2Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Oil Field Rule" value="20" meta:resourcekey="ddlUsaCycleResource1Item3Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Texas" value="30" meta:resourcekey="ddlUsaCycleResource1Item4Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US California" value="31" meta:resourcekey="ddlUsaCycleResource1Item5Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Florida 70/7" value="32" meta:resourcekey="ddlUsaCycleResource1Item6Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Florida 80/8" value="33" meta:resourcekey="ddlUsaCycleResource1Item7Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Alaskan 70/7" value="34" meta:resourcekey="ddlUsaCycleResource1Item8Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "US Alaskan 80/8" value="35" meta:resourcekey="ddlUsaCycleResource1Item9Resource1" > </asp:ListItem>
                                                                                                                    </asp:DropDownList>

                                                                                                                    </td>
                                                                                                                <td align="left"><asp:Label ID="lblCaCycle" runat="server" Text="Canadian Cycle" 
                                                                                                                         meta:resourcekey="lblCaCycleResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left" >
                                                                                                                    <asp:DropDownList ID="ddlCaCycle" runat="server" CssClass="RegularText" 
                                                                                                                        AppendDataBoundItems="True"  Width="154px" 
                                                                                                                        meta:resourcekey="ddlCaCycleResource1" >
                                                                                                                        <asp:ListItem Text = "Select a Canadian Cycle" value="-1" meta:resourcekey="ddlCaCycleResource1Item0Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "Canadian Cycle 1" value="6" meta:resourcekey="ddlCaCycleResource1Item1Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "Canadian Cycle 2" value="7" meta:resourcekey="ddlCaCycleResource1Item2Resource1" > </asp:ListItem>
                                                                                                                        <asp:ListItem Text = "Canadian Oil Field Rule" value="18" meta:resourcekey="ddlCaCycleResource1Item3Resource1" > </asp:ListItem>
                                                                                                                    </asp:DropDownList>

                                                                                                                    </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td align="left" ><asp:Label ID="lblTimeZone" runat="server" Text="Time Zone" 
                                                                                                                         meta:resourcekey="lblTimeZone"></asp:Label>
                                                                                                                    </td>
                                                                                                                <td align="left">
                                                                                                                <asp:DropDownList ID="ddlTimeZone" runat="server" CssClass="RegularText" AppendDataBoundItems="True"  Width="308px" 

                                                                                                                        meta:resourcekey="ddlTimeZone" onchange="javascript:return SelectTimeZone();" onclick="ClickTimeZone() >
                                                                                                                         <asp:ListItem Value="0" meta:resourcekey="ddlTimeZoneItemResource2" Text="Please select a timezone" Selected="True" ></asp:ListItem>

                                                                                                                                                                                                                                                <asp:ListItem Value="-11" meta:resourcekey="ddlTimeZoneItemResource1" Text="GMT-11 Samoa Time"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-10" meta:resourcekey="ddlTimeZoneItemResource2" 

                                                                                                                        Text="GMT-10 Hawaii-Aleutian Time"></asp:ListItem>
                                                                                                                                                                                                                                                <asp:ListItem Value="-9" meta:resourcekey="ddlTimeZoneItemResource3" Text="GMT-9 Alaska"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-8" meta:resourcekey="ddlTimeZoneItemResource4" 

                                                                                                                        Text="GMT-8 Pacific Time (USA&amp;Canada)"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-7" meta:resourcekey="ddlTimeZoneItemResource5" 

                                                                                                                        Text="GMT-7 Mountain Time (USA&amp;Canada)"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-6" meta:resourcekey="ddlTimeZoneItemResource6" 

                                                                                                                        Text="GMT-6 Central Time (USA&amp;Canada)"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-5" meta:resourcekey="ddlTimeZoneItemResource7" 

                                                                                                                        Text="GMT-5 Eastern Time (USA&amp;Canada)"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-4" meta:resourcekey="ddlTimeZoneItemResource8" 

                                                                                                                        Text="GMT-4 Atlantic Time (USA&amp;Canada)"></asp:ListItem>
										                                                                                                                                                                                                        <asp:ListItem Value="-3.5" meta:resourcekey="ddlTimeZoneItemResource9" 

                                                                                                                        Text="GMT-3.5 NewFoundLand Time (Canada)"></asp:ListItem>										                                                                                
										                                                                                                                                                                                                        <asp:ListItem Value="+10" 

                                                                                                                        meta:resourcekey="ddlTimeZoneItemResource0" Text="GMT+10 Chamorro Time"></asp:ListItem>
                                                                                                                        										                                      

                                          
                                                                                                                    </asp:DropDownList>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td colspan="2" align="left">
                                                                                                                     <asp:CheckBox ID="chkAssignEmergency" runat="server" Text="Assign emergency plan" CssClass="RegularText" meta:resourcekey="chkAssignEmergencyResource1"  />
                                                                                                                </td>
                                                                                                                <td colspan="2" align="left">
                                                                                                                <asp:CheckBox ID="chkIsSupervisor" runat="server" Text="Is Supervisor" CssClass="RegularText" meta:resourcekey="chkIsSupervisorResource1"  />
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px; visibility:hidden" id="trEmergencyPlan" >
                                                                                                                <td align="left">
                                                                                                                    <nobr>
                                                                                                                    <asp:Label ID="lblEmergencytel" runat="server" Text="Emergency phone" CssClass="RegularText"
                                                                                                                        meta:resourcekey="lblEmergencytelResource1"></asp:Label>
                                                                                                                     </nobr>
                                                                                                                </td>
                                                                                                                <td align="left">
                                                                                                                    <asp:TextBox ID="txtEmergencytel" runat="server" CssClass="RegularText" 
                                                                                                                        MaxLength="20"  Width="154px" 
                                                                                                                        meta:resourcekey="txtEmergencytelResource1"></asp:TextBox>
                                                                                                                </td>
                                                                                                                <td align="left">
                                                                                                                    <asp:Label ID="lblEmergencyPlan" runat="server" Text="Emergency plan" CssClass="RegularText"
                                                                                                                        meta:resourcekey="lblEmergencyPlanResource1"></asp:Label>
                                                                                                                </td>
                                                                                                                <td align="left">
                                                                                                                    <asp:DropDownList ID="ddlEmergencyPlan" runat="server" CssClass="RegularText" 
                                                                                                                        AppendDataBoundItems="True"  Width="154px" 
                                                                                                                        meta:resourcekey="ddlEmergencyPlanResource1" 
                                                                                                                        DataTextField="ContactPlanName" DataValueField="ContactPlanId">
                                                                                                                        <asp:ListItem Text = "Select a Plan" value="-1" meta:resourcekey="ddlEmergencyPlanItem0Resource1" > </asp:ListItem>
                                                                                                                    </asp:DropDownList>
                                                                                                                </td>

                                                                                                            </tr>
                                                                                                            <tfoot>
                                                                                                              <tr style="height: 50px; vertical-align:bottom">
                                                                                                                <td align="center" colspan="4">
                                                                                                                    <ul style="list-style:none; width:230px">
                                                                                                                        <li style="float:left">
                                                                                                                            <asp:Button ID="cmdCancel" runat="server" CausesValidation="False" CssClass="combutton"
                                                                                                                                OnClick="cmdCancel_Click" Text="Cancel" meta:resourcekey="cmdCancelResource1" />
                                                                                                                        </li>
                                                                                                                        <li style="float:right">
                                                                                                                            <asp:Button ID="cmdSaveDriver" runat="server" CssClass="combutton" OnClick="cmdSaveDriver_Click"
                                                                                                                                Text="Save" meta:resourcekey="cmdSaveDriverResource1" />
                                                                                                                        </li>
                                                                                                                    </ul>
                                                                                                                </td>
                                                                                                              </tr>
                                                                                                            </tfoot>
                                                                                                        </table>
                                                                                                        <br />
                                                                                                        <fieldset id="tblUploadData" runat="server" style="width: 674px; text-align: center; border: double 4px gray" visible="false">
                                                                                                            <legend style="text-align:center">
                                                                                                                <asp:Label ID="LabelImportCaption" runat="server" CssClass="RegularText" Font-Bold="True" meta:resourcekey="LabelImportCaptionResource1"></asp:Label>
                                                                                                            </legend>
                                                                                                            <asp:FileUpload ID="filePath" runat="server" CssClass="RegularText" Width="500px" style="margin: 20px 0px 20px 0px;" meta:resourcekey="filePathResource1" />
                                                                                                            <br />
                                                                                                            <asp:CheckBox ID="chkHeaderRow" runat="server" CssClass="RegularText" Checked="True" Text="First row is a header" meta:resourcekey="chkHeaderRowResource1" />
                                                                                                            <ul style="list-style:none; width:230px">
                                                                                                                <li style="float:left">
                                                                                                                    <asp:Button ID="cmdCancelUpload" runat="server" CausesValidation="False" CssClass="combutton"
                                                                                                                        Text="Cancel" meta:resourcekey="cmdCancelResource1" OnClick="cmdCancelUpload_Click" />
                                                                                                                </li>
                                                                                                                <li style="float:right">
                                                                                                                    <asp:Button ID="cmdSaveUpload" runat="server" CssClass="combutton"
                                                                                                                        Text="Submit" meta:resourcekey="cmdSaveDriverResource1" OnClick="cmdSaveUpload_Click" />
                                                                                                                </li>
                                                                                                            </ul>
                                                                                                            <br />
                                                                                                            <asp:Label ID="LabelCustomerNote" runat="server" CssClass="RegularText" ForeColor="Gray" meta:resourcekey="LabelCustomerNoteResource1"></asp:Label>
                                                                                                        </fieldset>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="center" style="height: 15px">
                                                                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="615px" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
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
            var v_timezone = "0";
            function ClickTimeZone()
            {
                v_timezone  = $telerik.$("#<%=ddlTimeZone.ClientID %> option:selected").val();;
            }
            function SelectUSACycle()
            {
                var usacycle = $telerik.$("#<%=ddlUsaCycle.ClientID %> option:selected").val();
                if (usacycle >= '30')
                {
                    alert("This selection is for mobile app.");
                }
            }
            function SelectTimeZone()
            {
                var timezonetext = $telerik.$("#<%=ddlTimeZone.ClientID %> option:selected").text();
                var timezonevalue = $telerik.$("#<%=ddlTimeZone.ClientID %> option:selected").val();
                if(timezonevalue != 0)  
                {
                    var timezoneMsg = "Important: Are you sure you want to change to timezone " +  timezonetext;
                    if (!confirm(timezoneMsg))
                    {
                        $telerik.$("#<%=ddlTimeZone.ClientID %> option[value='" + v_timezone + "']").attr("selected", "selected");
                       return false;
                   }
               }
               return true;
           }
            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdvDrivers.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=gdvDrivers.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdvDrivers.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }

            function chkAssignEmergency_click() {
                if ($telerik.$("#<%= chkAssignEmergency.ClientID %>:checked").length == 1) {
                    $telerik.$("#trEmergencyPlan").css("visibility", "visible");
                }
                else {
                    $telerik.$("#trEmergencyPlan").css("visibility", "hidden");
                }
            }

            $telerik.$(document).ready(function () {
                chkAssignEmergency_click();
            });

            function OpenEmergency(cp) {
                var url = "Contact/frmEmergency.aspx?cp=" + cp;
                window.radopen(url, "Emergency");
                return false;
            }
        </script>
    </telerik:RadCodeBlock>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
        EnableAriaSupport="true">
        <Windows>
            <telerik:RadWindow ID="Emergency" runat="server" Title="Emergency Phones" Height="450px"
                Width="800px" ReloadOnShow="true" ShowContentDuringLoad="false" Skin="Hay" Modal="true"
                meta:resourcekey="UserListDialogResource1" VisibleStatusbar="false" VisibleTitlebar="false" />
        </Windows>
    </telerik:RadWindowManager>

    </form>
</body>
</html>
