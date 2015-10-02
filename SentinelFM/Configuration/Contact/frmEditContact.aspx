<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEditContact.aspx.cs" Inherits="SentinelFM.Configuration_Contact_frmEditContact" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlContactMenu.ascx" TagName="ctlContactMenu" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="cboVehicle" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="margin-right: auto; margin-left: auto; text-align: center; width: 900px">
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
            left: 8px; position: absolute; top: 4px" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="cmdContact" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 550px;
                                    width: 990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <uc2:ctlContactMenu ID="ctlContactMenu" runat="server" selectedcontrol="cmdNewContact" />
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 5px; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%;" valign="top">
                                                                                                        <asp:Panel ID="pnl" runat="server">
                                                                                                            <table>
                                                                                                                <tr valign="top">
                                                                                                                    <td align="left" style="width: 80px">
                                                                                                                        <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="70px" meta:resourcekey="lblFleetResource1"
                                                                                                                            Text="Fleet:"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td style="width: 312px;" align="left">
                                                                                                                        <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                                                                                                                            AutoPostBack="true" DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                                                                                                                            Skin="Hay" MaxHeight="200px" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged">
                                                                                                                        </telerik:RadComboBox>
                                                                                                                    </td>
                                                                                                                    <td class="formtext" style="width: 80px" align="left">
                                                                                                                        <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1"
                                                                                                                            Text="Vehicle:"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td style="width: 300px" align="left">
                                                                                                                        <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                                                                                                            meta:resourcekey="cboVehicleResource1" MaxHeight="200px" DataTextField="Description"
                                                                                                                            DataValueField="VehicleId" Skin="Hay">
                                                                                                                        </telerik:RadComboBox>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td align="left" style="width: 80px">
                                                                                                                        <asp:Label ID="lblFirstName" runat="server" CssClass="RegularText" meta:resourcekey="lblFirstNameResource1"
                                                                                                                            Text="First name:"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td align="left">
                                                                                                                        <asp:TextBox ID="txtFirstName" runat="server" Width="254px" meta:resourcekey="txtFirstNameResource1"
                                                                                                                            CssClass="RegularText"></asp:TextBox>
                                                                                                                        <span style="color: Red">*</span><br />
                                                                                                                        <asp:RequiredFieldValidator ID="valReqtxtFirstName" runat="server" ControlToValidate="txtFirstName"
                                                                                                                            meta:resourcekey="valReqtxtFirstNameResource1" ValidationGroup="vgAdd" Text="Please enter first name"
                                                                                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                                                                                    </td>
                                                                                                                    <td style="width: 80px" align="left">
                                                                                                                        <asp:Label ID="lblMiddleName" runat="server" CssClass="RegularText" meta:resourcekey="lblMiddleNameResource1"
                                                                                                                            Text="Middle name:"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td align="left">
                                                                                                                        <asp:TextBox ID="txtMiddleName" runat="server" Width="254px" meta:resourcekey="txtMiddleNameResource1"
                                                                                                                            CssClass="RegularText"></asp:TextBox>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td align="left" style="width: 80px">
                                                                                                                        <asp:Label ID="lblLastName" runat="server" CssClass="RegularText" meta:resourcekey="lblLastNameResource1"
                                                                                                                            Text="Last name:"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td align="left">
                                                                                                                        <asp:TextBox ID="txtLastName" runat="server" Width="254px" meta:resourcekey="txtLastNameResource1"
                                                                                                                            CssClass="RegularText"></asp:TextBox>
                                                                                                                        <span style="color: Red">*</span><br />
                                                                                                                        <asp:RequiredFieldValidator ID="valReqtxtLastName" runat="server" ControlToValidate="txtLastName"
                                                                                                                            meta:resourcekey="valReqtxtLastNameResource1" ValidationGroup="vgAdd" Text="Please enter last name"
                                                                                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                                                                                    </td>
                                                                                                                    <td class="formtext" style="width: 80px" align="left">
                                                                                                                        <asp:Label ID="lblTimeZone" runat="server" Text=" Time Zone:" meta:resourcekey="lblTimeZoneResource1"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td class="formtext" style="width: 224px; height: 16px">
                                                                                                                        <telerik:RadComboBox ID="cboTimeZone" runat="server" Width="225px" Skin="Hay" meta:resourcekey="cboTimeZoneResource1">
                                                                                                                            <Items>
                                                                                                                                <telerik:RadComboBoxItem Value="-12" meta:resourcekey="ListItemResource1" Text="GMT-12 Eniwetok,Kwajalein">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-11" meta:resourcekey="ListItemResource2" Text="GMT-11 Midway Island">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-10" meta:resourcekey="ListItemResource3" Text="GMT-10 Hawaii">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-9" meta:resourcekey="ListItemResource4" Text="GMT-9 Alaska">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-8" meta:resourcekey="ListItemResource5" Text="GMT-8 Pacific Time (USA&amp;Canada)">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-7" meta:resourcekey="ListItemResource6" Text="GMT-7 Mountain Time (USA&amp;Canada)">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-6" meta:resourcekey="ListItemResource7" Text="GMT-6 Central Time (USA&amp;Canada)">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-5" meta:resourcekey="ListItemResource8" Text="GMT-5 Eastern Time (USA&amp;Canada)">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-4" meta:resourcekey="ListItemResource9" Text="GMT-4 Atlantic Time (Canada)">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-3" meta:resourcekey="ListItemResource10" Text="GMT-3 Brasilia,Greenland">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-2" meta:resourcekey="ListItemResource11" Text="GMT-2 Mid-Atlantic">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="-1" meta:resourcekey="ListItemResource12" Text="GMT-1 Azores,Cape Verde Is.">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="0" Selected="True" meta:resourcekey="ListItemResource13"
                                                                                                                                    Text="GMT Dublin,London"></telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="1" meta:resourcekey="ListItemResource14" Text="GMT+1 Amsterdam,Berlin,Bern,Rome">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource15" Text="GMT+2 Jerusalem,Riga,Tallinn">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource16" Text="GMT+3 Moscow,St. Petersburg">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="4" meta:resourcekey="ListItemResource17" Text="GMT+4 Abu Dhabi,Baku,Tbilisi">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="5" meta:resourcekey="ListItemResource18" Text="GMT+5 Islamabad,Karachi">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="6" meta:resourcekey="ListItemResource19" Text="GMT+6 Astana,Dhaka">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="7" meta:resourcekey="ListItemResource20" Text="GMT+7 Bangkok,Hanoi,Jakarta">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="8" meta:resourcekey="ListItemResource21" Text="GMT+8 Beijing,Hong Kong">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="9" meta:resourcekey="ListItemResource22" Text="GMT+9 Osaka,Tokyo,Seoul">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="10" meta:resourcekey="ListItemResource23" Text="GMT+10 Sydney,Melbourne">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="11" meta:resourcekey="ListItemResource24" Text="GMT+11 Magadan">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="12" meta:resourcekey="ListItemResource25" Text="GMT+12 Wellington,Fiji">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                                <telerik:RadComboBoxItem Value="13" meta:resourcekey="ListItemResource26" Text="GMT+13 Nuku'alofa">
                                                                                                                                </telerik:RadComboBoxItem>
                                                                                                                            </Items>
                                                                                                                        </telerik:RadComboBox>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        &nbsp;
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="4">
                                                                                                                        <fieldset>
                                                                                                                            <legend style="color: green" runat="server" id="legendID"></legend>
                                                                                                                            <telerik:RadGrid ID="gdContact" runat="server" AutoGenerateColumns="False" GridLines="Both"
                                                                                                                                ShowHeader="false" AllowPaging="false" BorderStyle="None" OnItemDataBound="gdContact_ItemDataBound">
                                                                                                                                <MasterTableView BorderStyle="None">
                                                                                                                                    <Columns>
                                                                                                                                        <telerik:GridTemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <table width="20px" cellpadding="0" cellspacing="0">
                                                                                                                                                    <tr>
                                                                                                                                                        <td>
                                                                                                                                                            <nobr>
                                                    <asp:ImageButton ID="btnUpArrow" runat="server" ImageUrl="~/images/UpArrow.gif" Width="12px"
                                                        Height="15px" />
                                                    <asp:ImageButton ID="btnDownArrow" runat="server" ImageUrl="~/images/DownArrow.gif"
                                                        Height="15px" Width="12px"  />
                                                </nobr>
                                                                                                                                                        </td>
                                                                                                                                                    </tr>
                                                                                                                                                </table>
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <ItemStyle Width="30px" HorizontalAlign="Center" />
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                        <telerik:GridTemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:DropDownList ID="ddlTypeName" runat="server" Width="150px" DataTextField="CommunicationTypeName"
                                                                                                                                                    DataValueField="CommunicationTypeId" Font-Bold="true">
                                                                                                                                                </asp:DropDownList>
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <ItemStyle Width="150px" HorizontalAlign="Left" />
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                        <telerik:GridTemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:TextBox ID="txtTypedata" runat="server" MaxLength="100" Width="500px" BackColor="#dee0c8"></asp:TextBox>
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <ItemStyle HorizontalAlign="Left" />
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                        <telerik:GridTemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:ImageButton ID="btnDel" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                                                                                                                                    meta:resourcekey="btnDelResource1" Width="12px" Height="12px" />
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <ItemStyle HorizontalAlign="Left" />
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                    </Columns>
                                                                                                                                    <ItemStyle HorizontalAlign="Left" BackColor="White" />
                                                                                                                                    <AlternatingItemStyle HorizontalAlign="Left" BackColor="White" />
                                                                                                                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                </MasterTableView>
                                                                                                                            </telerik:RadGrid>
                                                                                                                        </fieldset>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="4" align="center">
                                                                                                                        <asp:Button ID="btnSave" runat="server" Text="Save" meta:resourcekey="btnDelResource1"
                                                                                                                            ValidationGroup="vgAdd" OnClientClick="javascript:return Save_Click('vgAdd');" CssClass="combutton" /><br />
                                                                                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="true" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                                                        <br />
                                                                                                                        <asp:CustomValidator ID="cvContact" runat="server" ClientValidationFunction="CustomValidateContact"
                                                                                                                            ValidationGroup="vgAdd" Text="" meta:resourcekey="cvContact" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </asp:Panel>
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
            function CustomValidateContact(sender, args) {
                args.IsValid = true;
                $telerik.$("span[title='errorEmailSpan']").remove();
                $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr").each(function () {
                    if (parseInt($telerik.$(this).find("[name$='ddlTypeName']").val()) > 0) {
                        var selectedText = $telerik.$(this).find("[name$='ddlTypeName']").find('option').filter(':selected').text();
                        var inputText = $telerik.$.trim($telerik.$(this).find("[name$='txtTypedata']").val());
                        if (selectedText.toLowerCase() == "email" && inputText != '') { //validate email address
                            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/; 
                            if (reg.test(inputText) == false) {
                                $telerik.$(this).find("[name$='txtTypedata']").parent().append("<span title='errorEmailSpan'> <font color='red'> <br /><%= errorEmail %></font></span>");
                                args.IsValid = false;
                            }
                        }
                    }
                });
            }
            function SelectedIndxChanged() { //Add new row if it is last row
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                if (trs.length > 0) {
                    $telerik.$("#<%=gdContact.ClientID%>>table>tbody:last").append($telerik.$(trs[trs.length - 1]).clone(false));
                    trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                    SetupEvents();
                    $telerik.$(trs[trs.length - 1]).find("span[title='errorEmailSpan']").remove();
                    $telerik.$(trs[trs.length - 1]).find("[name$='ddlTypeName']")[0].selectedIndex = 0;
                    $telerik.$($telerik.$(trs[trs.length - 1]).find("[name$='txtTypedata']")[0]).val("");
                }
            }

            function DeleRow(event) {
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                if (trs.length - 1 > event.data.rowIndex) {
                    $telerik.$(trs[event.data.rowIndex]).remove();
                    SetupEvents();
                }
                return false;
            }

            function UpRow(event) {
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                if (event.data.rowIndex != 0) {
                    var tmp = $telerik.$(trs[event.data.rowIndex - 1]).html();
                    $telerik.$(trs[event.data.rowIndex - 1]).html($telerik.$(trs[event.data.rowIndex]).html());
                    $telerik.$(trs[event.data.rowIndex]).html(tmp)
                    if (event.data.rowIndex == trs.length - 1) { //Add new row if it is last row
                        SelectedIndxChanged();
                        return;
                    }
                    SetupEvents();
                }
                return false;
            }

            function DownRow(event) {
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                if (event.data.rowIndex != trs.length - 1) {
                    var tmp = $telerik.$(trs[event.data.rowIndex + 1]).html();
                    $telerik.$(trs[event.data.rowIndex + 1]).html($telerik.$(trs[event.data.rowIndex]).html());
                    $telerik.$(trs[event.data.rowIndex]).html(tmp)
                    if (event.data.rowIndex + 1 == trs.length - 1) {//Add new row if next row is last row
                        SelectedIndxChanged();
                        return;
                    }
                    SetupEvents();
                }
                return false;
            }

            function SetupEvents() {
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");
                if (trs.length > 0) {
                    trs.each(function (index) {
                        $telerik.$(this).find("[name$='ddlTypeName']").unbind('change');
                        $telerik.$(this).find("[name$='txtTypedata']").unbind('change');
                        $telerik.$(this).find("[name$='btnUpArrow']").unbind('click');
                        $telerik.$(this).find("[name$='btnDownArrow']").unbind('click');
                        $telerik.$(this).find("[name$='btnDel']").unbind("click");
                        if (index == 0)
                            $telerik.$(this).find("[name$='btnUpArrow']").hide();
                        else $telerik.$(this).find("[name$='btnUpArrow']").show();
                        if (index == trs.length - 1) {
                            $telerik.$(this).find("[name$='btnDownArrow']").hide();
                            $telerik.$(this).find("[name$='ddlTypeName']").change(SelectedIndxChanged);
                            $telerik.$(this).find("[name$='txtTypedata']").change(SelectedIndxChanged);
                            $telerik.$(this).find("[name$='btnDel']").hide();
                        }
                        else {
                            $telerik.$(this).find("[name$='btnDownArrow']").show();
                            $telerik.$(this).find("[name$='btnDel']").show();
                        }
                        $telerik.$(this).find("[name$='btnDel']").bind("click", { rowIndex: index }, DeleRow);
                        $telerik.$(this).find("[name$='btnUpArrow']").bind("click", { rowIndex: index }, UpRow)
                        $telerik.$(this).find("[name$='btnDownArrow']").bind("click", { rowIndex: index }, DownRow)
                    })
                }

            }
            $telerik.$(document).ready(function () {
                SetupEvents();
            });

            function Save_Click(validationGroup) {
                if (!Page_ClientValidate(validationGroup)) {
                    Page_BlockSubmit = false;
                    return false; //not valid return false
                }
                CreateSaveParameters();
                return false;
            }

            function ResetContact() {
                var trs = $telerik.$("#<%=gdContact.ClientID%>>table>tbody>tr");

                if (trs.length > 1) {
                    for (var index = trs.length - 1; index >= 1; index--) {
                        $telerik.$(trs[index]).remove();
                    }
                }

                if (trs.length >= 1) {
                    $telerik.$(trs[0]).find("[name$='ddlTypeName']").val("-1");
                    $telerik.$(trs[0]).find("[name$='txtTypedata']").val("");
                }
                $telerik.$("#<%= txtFirstName.ClientID %>").val("");
                $telerik.$("#<%= txtLastName.ClientID %>").val("");
                $telerik.$("#<%= txtMiddleName.ClientID %>").val("");

                SetupEvents();
            }

            function CreateSaveParameters() {
                var vehicleID = "";
                var firstName = "";
                var middleName = "";
                var lastName = "";
                var timeZone = "";
                var contacts = "";

                var combo = $find("<%= cboVehicle.ClientID %>");
                if (combo.get_selectedIndex() >= 0) {
                    if (combo.get_selectedItem().get_value()) {
                        vehicleID = combo.get_selectedItem().get_value();
                    }
                }

                combo = $find("<%= cboTimeZone.ClientID %>");
                if (combo.get_selectedIndex() >= 0) {
                    if (combo.get_selectedItem().get_value()) {
                        timeZone = combo.get_selectedItem().get_value();
                    }
                }
                firstName = $telerik.$("#<%= txtFirstName.ClientID %>").val();
                lastName = $telerik.$("#<%= txtLastName.ClientID %>").val();
                middleName = $telerik.$("#<%= txtMiddleName.ClientID %>").val();
                var trs = $telerik.$("#<%= gdContact.ClientID%>>table>tbody>tr");
                var priority = 1;
                var c = "\\" + "'";
                if (trs.length > 0) {
                    trs.each(function (index) {
                        var typeCtl = $telerik.$(this).find("[name$='ddlTypeName']");
                        var txtCtl = $telerik.$(this).find("[name$='txtTypedata']");
                        if (parseInt(typeCtl.val()) > 0 && $telerik.$.trim(txtCtl.val()) != '') {
                            if (contacts == '') {
                                contacts = "{'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";
                            }
                            else {
                                contacts = contacts + ",{'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";

                            }
                        }
                    });
                }
                
                contacts = "[" + contacts + "]";
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnl.ClientID %>");
                var postData = "{'VehicleId':'" + vehicleID +
                              "', 'FirstName':'" + firstName.replace("'", c) +
                              "', 'MiddleName':'" + middleName.replace("'", c) +
                              "', 'LastName':'" + lastName.replace("'", c) +
                              "', 'TimeZone':'" + timeZone +
                              "', 'Contacts':" + contacts + "}";
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmEditContact.aspx/Contact_Add",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '1') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").text("<%= saveSucceed %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").css("color", "green");
                            ResetContact();
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").text("<%= saveError %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").css("color", "red");
                        }

                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                        $telerik.$("#<%= lblMessage.ClientID%>").text("<%= saveError %>");
                        $telerik.$("#<%= lblMessage.ClientID%>").css("color", "red");
                        //alert(request.responseText);
                    }

                });
            }

        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
