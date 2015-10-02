<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmWorkingHrs.aspx.cs" Inherits="SentinelFM.Configuration_WorkingHour_frmWorkingHrs"
    meta:resourcekey="PageResource1" Theme="TelerikControl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register src="ctlWorkingHrsl.ascx" tagname="ctlWorkingHrsl" tagprefix="uc2" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" >
         .NoTDBorder
         {
             border-left-width:0px !important;
         }
    </style>
</head>
<body>
<%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
			var myname='OrganizationHierarchy';
			var w=740;
			var h=440;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2; 
			winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
			win = window.open(mypage, myname, winprops) 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName)
        {            
            var myVal = document.getElementById('<%=rvcboFleet.ClientID %>');
            ValidatorEnable(myVal, false); 
            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
			//-->
    </script>
<%} %>


    <form id="form1" runat="server">
    <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        Style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnNewWorkinghr">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddEmail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddEmail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdWorkingHr">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdEmails">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
    <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300" >
        <tr>
            <td>
                <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="btnWorkinghrs" />
            </td>
        </tr>
        <tr>
            <td>
                <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                    <tr>
                        <td>
                            <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 500px;
                                    width: 990px;">
                                <tr>
                                    <td class="configTabBackground" >
                                        <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px" >
                                            <tr align="center">
                                                <td valign="top" >
                                                                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <uc2:ctlWorkingHrsl ID="ctlWorkingHrsl1" runat="server" selectedcontrol="cmdWorkingHrs" />
                                                                                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">

                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 2px; height: 480px">
                                                                                                <tr valign="top">
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                    <table runat="server" id="pnlAll" width="600px">
                                                        <tr>
                                                            <td>
                                                                <table>
                                                                    <tr valign="top">
                                                                        <td align="left">
                                                                            <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="40px" meta:resourcekey="lblFleetResource1"
                                                                                Text="Fleet:"></asp:Label>
                                                                            <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text="Hierarchy Node:" Width="100" meta:resourcekey="lblOhTitle" Visible="false"  />
                                                                        </td>
                                                                        <td style="width: 512px;" align="left">
                                                                            <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px" Filter="Contains" MarkFirstMatch="True" 
                                                                                DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                                                                                Skin="Hay" AutoPostBack="true" MaxHeight="200px" 
                                                                                onselectedindexchanged="cboFleet_SelectedIndexChanged">
                                                                            </telerik:RadComboBox>
                                                                            <asp:RequiredFieldValidator ID="rvcboFleet" runat="server" ControlToValidate="cboFleet"
                                                                                CssClass="errortext" ErrorMessage="" Display="Dynamic"
                                                                                Text="<br/>Please Select a Fleet." ValidationGroup="Add" meta:resourcekey="rvcboFleetResource1"></asp:RequiredFieldValidator>
                                                                            <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Text="" CssClass="combutton" style="width:205px;" Visible="false" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" />
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td class="formtext">
                                                                            <asp:Label ID="lblTimeZoneTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeZoneTitleResource1"
                                                                                Text="Time Zone:" Width="80px"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <telerik:RadComboBox ID="cboTimeZone" runat="server" Width="400px" Skin="Hay" 
                                                                                meta:resourcekey="cboTimeZoneResource1" Filter="Contains" MarkFirstMatch="true" 
                                                                                Height="200px">
                                                                            </telerik:RadComboBox>

                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                           <td>
                                                              <asp:Panel id="pnlInfo" runat="server" Visible="false" >
                                                                 <table>
                                                        <tr>
                                                            <td>
                                                                <fieldset>
                                                                    <legend style="color: green" runat="server" id="legendID">
                                                                        <asp:Button ID="btnNewWorkinghr" runat="server" Text="New After Hour" meta:resourcekey="btnNewResource1"
                                                                            OnClick="btnNewWorkinghr_Click" CssClass="combutton" Width="150px" /></legend>
                                                                    <telerik:RadGrid ID="gdWorkingHr" runat="server" AutoGenerateColumns="false" AllowSorting="false"
                                                                        AllowPaging="false" GridLines="Both" Style="margin-top: 5px" Skin="Simple" OnItemDataBound="gdWorkingHr_ItemDataBound"
                                                                        OnItemCommand="gdWorkingHr_ItemCommand">
                                                                        <GroupingSettings CaseSensitive="false" />
                                                                        <MasterTableView GroupLoadMode="Server">
                                                                            <Columns>
                                                                                <telerik:GridTemplateColumn HeaderText="From" UniqueName="From">
                                                                                    <ItemTemplate>
                                                                                        <table cellpadding="0" cellspacing="0">
                                                                                        <tr>
                                                                                        <td class="NoTDBorder" >
                                                                                        <telerik:RadTimePicker ID="cboFrom" runat="server" ZIndex="50001" meta:resourcekey="cboFromResource1"
                                                                                            ShowPopupOnFocus="true" ToolTip="HH:MM" Width="100px" />
                                                                                        </td>
                                                                                        <td class="NoTDBorder">
                                                                                        <asp:CustomValidator ID="cvscboFrom" runat="server" ControlToValidate="cboFrom" Display="Dynamic"
                                                                                            Text="*" ErrorMessage=""
                                                                                            ClientValidationFunction="CustomValidateTimeFrom" ValidationGroup="Add" CssClass="errortext"></asp:CustomValidator>

                                                                                        </td>
                                                                                        </tr>
                                                                                        </table>
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="90px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="To" UniqueName="To">
                                                                                    <ItemTemplate>
                                                                                        <table cellpadding="0" cellspacing="0">
                                                                                        <tr>
                                                                                        <td class="NoTDBorder">
                                                                                        <telerik:RadTimePicker ID="cboTo" runat="server" ZIndex="50001" meta:resourcekey="cboToResource1"
                                                                                            ShowPopupOnFocus="true" ToolTip="HH:MM" Width="100px" />
                                                                                        </td>
                                                                                        <td class="NoTDBorder">
                                                                                        <asp:CustomValidator ID="cvscboTo" runat="server" ControlToValidate="cboTo" Display="Dynamic"
                                                                                            Text="*" ErrorMessage=""
                                                                                            ClientValidationFunction="CustomValidateTimeTo" ValidationGroup="Add" CssClass="errortext"></asp:CustomValidator>
                                                                                        </td>
                                                                                        </tr>
                                                                                        </table>
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="90px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Sun">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkSun" runat="server" meta:resourcekey="chkSunResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Mon">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkMon" runat="server" meta:resourcekey="chkMonResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Tue">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkTue" runat="server" meta:resourcekey="chkTueResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Wed">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkWed" runat="server" meta:resourcekey="chkWedResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Thu">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkThu" runat="server" meta:resourcekey="chkThuResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Fri">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkFri" runat="server" meta:resourcekey="chkFriResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Sat">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkSat" runat="server" meta:resourcekey="chkSatResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="Holiday">
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkHoliday" runat="server" meta:resourcekey="chkHolidayResource1" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnDel" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                                                                            CommandName="Delete" meta:resourcekey="btnDelResource1" Width="12px" Height="12px" />
                                                                                        <asp:HiddenField ID="hidID" runat="server" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                            </Columns>
                                                                        </MasterTableView>
                                                                    </telerik:RadGrid>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <fieldset>
                                                                    <legend runat="server" id="legend1"><span class='RegularText' style="color: green">Delivery
                                                                        Exception </span></legend>
                                                                    <table >
                                                                    <tr valign="top">
                                                                       <td>
                                                                            <telerik:RadComboBox ID="cboException" runat="server" CssClass="RegularText" Width="258px" Filter="Contains" MarkFirstMatch="True" 
                                                                                 DataTextField="Description" DataValueField="VehicleId" 
                                                                                meta:resourcekey="cboExceptionResource1" Skin="Hay" MaxHeight="150px" EmptyMessage="Select Exception(s)"
                                                                                AllowCustomText="true">
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox runat="server" ID="chkException" Checked="false" onclick="CheckException(this);" />
                                                                                    <asp:Label runat="server" ID="lblExceptionDescription" Text='<%# Eval("Description") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                            </telerik:RadComboBox>
                                                                       </td>
                                                                       <td>
                                                                            <asp:Button ID="lnkClearException" runat="server" Text ="Clear Selection" CssClass="combutton"  OnClientClick="return Clear_Exception();" />
                                                                       </td>
                                                                       <td align="right">
                                                                          <asp:Label ID="lblException" runat="server" Text="Exceptional Vehicles:" meta:resourcekey="lblExceptionResource1" class='RegularText'></asp:Label>
                                                                       </td>
                                                                       <td>

                                                                            <asp:ListBox ID="lstException" runat="server" Width ="280px" Height="100px" class='RegularText' >
                                                                            </asp:ListBox>
                                                                       </td>
                                                                    </tr>
                                                                    </table>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <fieldset>
                                                                    <legend style="color: green" runat="server" id="legend2">
                                                                        <asp:Button ID="btnAddEmail" runat="server" Text="Add Alert Message Address" meta:resourcekey="btnAddEmailResource1"
                                                                            OnClick="btnAddEmail_Click" CssClass="combutton" Width="200px" />
                                                                        <asp:Label id="lblAlert" runat="server" Text="Example: 5555551234@messaging.sprintpcs.com or 5555551234@txt.att.net"  CssClass="formtext" ></asp:Label>
                                                                    </legend>
                                                                    <telerik:RadGrid ID="gdEmails" runat="server" AutoGenerateColumns="false" AllowSorting="false"
                                                                        AllowPaging="false" GridLines="Both" Style="margin-top: 5px" Skin="Simple" OnItemDataBound="gdEmails_ItemDataBound"
                                                                        OnItemCommand="gdEmails_ItemCommand">
                                                                        <GroupingSettings CaseSensitive="false" />
                                                                        <MasterTableView GroupLoadMode="Server" >
                                                                            <Columns>
                                                                                <telerik:GridTemplateColumn HeaderText="Alert Message Address" UniqueName="email">
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox ID="txtEmail" runat="server" Width="100%"></asp:TextBox>
                                                                                        <asp:CustomValidator ID="cvstxtEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic"
                                                                                            Text="Please enter a correct email address" ErrorMessage=""
                                                                                            ClientValidationFunction="CustomValidateEmail" ValidationGroup="Add" CssClass="errortext"></asp:CustomValidator>
                                                                                    </ItemTemplate>
                                                                                </telerik:GridTemplateColumn>
                                                                                <telerik:GridTemplateColumn HeaderText="">
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="btnDel" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                                                                            CommandName="Delete" meta:resourcekey="btnDelResource1" Width="12px" Height="12px" />
                                                                                        <asp:HiddenField ID="hidID" runat="server" />
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                                                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                                                                </telerik:GridTemplateColumn>
                                                                            </Columns>
                                                                        </MasterTableView>
                                                                    </telerik:RadGrid>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:Button ID="btnSave" Text="Save" runat="server" ValidationGroup="Add" 
                                                                    CssClass="combutton" onclick="btnSave_Click" />
                                                                                    <asp:CustomValidator ID="cvEmail" runat="server" ClientValidationFunction="CustomValidateHasEmail"
                                                                                        EnableClientScript="true" ValidationGroup="Add" Display="None" ErrorMessage="Please input email."   />
                                                                                    <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="CustomValidateHasWorkingHour"
                                                                                        EnableClientScript="true" ValidationGroup="Add" Display="None" ErrorMessage="Please input after hour."   />


                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <table>
                                                                   <tr>
                                                                     <td align="left" > <!--for chrome browser -->
                                                                   <asp:ValidationSummary ID="valSum"  runat="server" ValidationGroup="Add" DisplayMode="BulletList"
                                                                    />

                                                                     </td>
                                                                    </tr>
                                                                </table>
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
            </td>
        </tr>
    </table>
    </div>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function CustomValidateEmail(sender, args) {
                args.IsValid = true;
                var inputText = $telerik.$.trim(args.Value);
                if (inputText != '') {
                    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
                    if (reg.test(inputText) == false) args.IsValid = false;
                }
            }

            function CustomValidateTimeFrom(sender, args) {
                args.IsValid = true;
                var dp1 = $find(sender.title);
                var dp2 = $find(sender.controltovalidate);

                if (dp1.get_selectedDate().getHours() == 0 &&
                    dp1.get_selectedDate().getMinutes() == 0 &&
                    dp2.get_selectedDate().getHours() == 0 &&
                    dp2.get_selectedDate().getMinutes() == 0) return;

                if (dp2.get_selectedDate() >= dp1.get_selectedDate()) {
                    args.IsValid = false;
                }
                if (args.IsValid) {
                    $telerik.$(dp1.get_element()).parents("tr:first").find("span.errortext").hide();
                }
            }

            function CustomValidateTimeTo(sender, args) {
                args.IsValid = true;
                var dp1 = $find(sender.title);
                var dp2 = $find(sender.controltovalidate);

                if (dp1.get_selectedDate().getHours() == 0 &&
                    dp1.get_selectedDate().getMinutes() == 0 &&
                    dp2.get_selectedDate().getHours() == 0 &&
                    dp2.get_selectedDate().getMinutes() == 0) return;

                if (dp2.get_selectedDate() <= dp1.get_selectedDate()) {
                    args.IsValid = false;
                }
                if (args.IsValid) {
                    $telerik.$(dp1.get_element()).parents("tr:first").find("span.errortext").hide();
                }

            }

            function CustomValidateHasEmail(sender, args) {
                args.IsValid = true;
                return;
                args.IsValid = false;
                var tableView = $find("<%= gdEmails.ClientID %>").get_masterTableView();
                var count = tableView.get_dataItems().length;
                for (var i = 0; i < count; i++) {
                    var item = tableView.get_dataItems()[i];
                    var selectCell = tableView.getCellByColumnUniqueName(item, "email")
                    if ($telerik.$(selectCell).find("input:first").val() != '') {
                        args.IsValid = true;
                        break;
                    }
                }

            }

            function CustomValidateHasWorkingHour(sender, args) {
                args.IsValid = false;
                var tableView = $find("<%= gdWorkingHr.ClientID %>").get_masterTableView();
                var count = tableView.get_dataItems().length;
                for (var i = 0; i < count; i++) {
                    var item = tableView.get_dataItems()[i];
                    var from = tableView.getCellByColumnUniqueName(item, "From")
                    var to = tableView.getCellByColumnUniqueName(item, "To")
                    try {
                        var fromId = $telerik.$(from).find("[id$=_wrapper]:first").attr("id");
                        var toId = $telerik.$(to).find("[id$=_wrapper]:first").attr("id");
                        fromId = fromId.substring(0, fromId.length - 8);
                        toId = toId.substring(0, toId.length - 8);

                        var dp_t1 = $find(toId).get_selectedDate();
                        var dp_t2 = $find(fromId).get_selectedDate();

                        if (dp_t1 > dp_t2 ||
                           (dp_t1.getHours() == 0 && dp_t1.getMinutes() == 0 &&
                            dp_t2.getHours() == 0 && dp_t2.getMinutes() == 0
                            )) {
                            args.IsValid = true;
                            break;
                        }
                    }
                    catch (eer) { }
                }
            }

            function Clear_Exception() {
                var combo = $find("<%= cboException.ClientID %>");
                var items = combo.get_items();
                for (i = 0; i < items.get_count(); i++) {
                    $telerik.$(items.getItem(i).get_element()).find("input:checkbox").attr("checked", false)
                }
                $telerik.$("#<%= lstException.ClientID %>").find("Option").remove();
                return false;
            }

            function ValidateAllData() {
                if (!Page_ClientValidate("Add")) {
                    Page_BlockSubmit = false;
                    return false; //not valid return false
                }

                return true;
            }

            function CheckException(ctl) {
                if ($telerik.$(ctl).attr("checked") == false) {
                    var vehicleName = $telerik.$(ctl).parent().find("[id$='lblExceptionDescription']").text();
                    $telerik.$("#<%= lstException.ClientID %>").find("Option").each(function (){
                       if ($telerik.$(this).text() == vehicleName) $telerik.$(this).remove();
                    });
                }
                else
                {
                    var vehicleName = $telerik.$(ctl).parent().find("[id$='lblExceptionDescription']").text();
                    var isFound = false;
                    $telerik.$("#<%= lstException.ClientID %>").find("Option").each(function (){
                       if ($telerik.$(this).text() == vehicleName) isFound = true;
                    });

                    if (!isFound) 
                       $telerik.$("#<%= lstException.ClientID %>").append('<Option>' + vehicleName + '</Option>');
                }
            }
        </script>

            <%if (LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" />
        <%} %>
    </telerik:RadCodeBlock>


    </form>
</body>
</html>
