<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEmailAddress.aspx.cs" Inherits="HOS_frmEmailAddress" meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="../Configuration/Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>

<%@ Register src="HosTabs.ascx" tagname="HosTabs" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body topmargin="5px" leftmargin="3px">
    <script language="javascript">
	<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
    var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
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

    function OrganizationHierarchyNodeSelected(nodecode, fleetId)
    {            
        $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
        $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
        $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
    }
            
    //-->
</script>
    <form id="form1" runat="server">

        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
            style="display:none;" AutoPostBack="True"
                    OnClick="hidOrganizationHierarchyPostBack_Click" 
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

    <div style="text-align:center">

        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300" >
            <tr align="left">
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" SelectedControl="btnHOS"  />
                </td>
            </tr>
            <tr align="left">
                <td  >
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td><uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdEmailAddress" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width:990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder" >
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0" 
                                                                                                style="width: 950px; height: 495px">
                                                                                                <tr valign="top">
                                                                                                    <td>
                                                                                                    
																		<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table9" class=table WIDTH="960px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=top  >
																								<TABLE id="Table10" style="WIDTH: 900px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
																									width="900px" align="center" border="0">
																									<TR>
																										<TD class="tableheading" align="left" height="5" style="width: 900px"></TD>
																									</TR>
                                                                                                    <tr id="trFleetSelectOption" visible="false" runat="server">
                                                                                                        <td align="center" class="configTabBackground">
                                                                                                            <table class="formtext" runat="server" id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                                                                                <tr>
                                                                                                                    <td> 
                                                                                                                        <asp:Label ID="Label11" runat="server" Text="Based On:" meta:resourcekey="Label11Resource1"></asp:Label> 
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:RadioButtonList ID="optAssignBased" name="AssignBased" runat="server"  class="formtext" 
                                                                                                                                    RepeatDirection="Horizontal" AutoPostBack="True"
                                                                                                                                    onselectedindexchanged="optAssignBased_SelectedIndexChanged" 
                                                                                                                                    meta:resourcekey="optAssignBasedResource1" >
                                                                                                                            <asp:ListItem id="Radio2" type="radio" name="raFleetSelectOption" value="0" checked
                                                                                                                                runat="server" Selected="True" meta:resourcekey="ListItemResource1" >Fleet</asp:ListItem> 
                                                                                                                            <asp:ListItem id="Radio1" name="raFleetSelectOption" value="1" runat="server" 
                                                                                                                                        meta:resourcekey="ListItemResource2" >Organization Hierarchy Fleet</asp:ListItem>
                                                                                                                        </asp:RadioButtonList>                                                         
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
																									<TR id="trBasedOnNormalFleet" runat="server">
																										<TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 900px;"
																											align="center" colSpan="1">
																											<TABLE id="Table11" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<TR>
																													<TD><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label></TD>
																													<TD><asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" AutoPostBack="True" DataValueField="FleetId"
																															DataTextField="FleetName" onselectedindexchanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1"></asp:dropdownlist></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
                                                                                                    <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                                                                                                        <TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 669px;"
																											align="center" colSpan="1">
																											<TABLE id="Table16" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<tr>
                                                                                                                      <td class="style1">
                                                                                                                            <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                                                                                                                Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                                                                                                                     <td>
                                                                                                                            <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                                                                                                                CssClass="combutton" Width="200px" 
                                                                                                                                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                                                                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                     </td>
                                                                                                                  </tr>
																											</TABLE>
																										</TD>
                                                                                                    </tr>
																									<TR>
																										<TD style="WIDTH: 900px; HEIGHT: 217px" align="center">
																											<TABLE id="Table12" style="HEIGHT: 210px"  cellSpacing="0" cellPadding="0" border="0">
																												<TR>
																													<TD><asp:datagrid id="dgEmails" width="900px" runat="server" Visible="False" PageSize="7" AllowPaging="True" DataKeyField="RowID"
																															AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px"
																															BackColor="White" GridLines="None" CellSpacing="1" meta:resourcekey="dgEmailsResource1" OnDeleteCommand="dgEmails_DeleteCommand" OnCancelCommand="dgEmails_CancelCommand" OnEditCommand="dgEmails_EditCommand" OnUpdateCommand="dgEmails_UpdateCommand" OnPageIndexChanged="dgEmails_PageIndexChanged" OnItemCreated="dgEmails_ItemCreated">
																															<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
																															<AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
																															<ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
																															<HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
																															<FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
																															<Columns>
																																<asp:TemplateColumn Visible="False" HeaderText="ID">
																																	<ItemTemplate>
																																		<asp:Label Visible=False ID="lblRowID1" text='<%# DataBinder.Eval(Container.DataItem,"RowID") %>' Runat=server Width=75px meta:resourcekey="lblRowID1Resource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox Visible=False ID="lblRowID" Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"RowID") %>' Width=75px meta:resourcekey="txtRowIDResource1"></asp:TextBox>
																																	</EditItemTemplate>

																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Email Address'>
																																	<ItemTemplate>
																																		<asp:Label ID="LabelEmail" text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Runat=server Width="200px" style="display:inline-block;width:200px;word-wrap: normal; word-break: break-all;" meta:resourcekey="LabelEmailResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:TextBox ID="txtEmail"  Runat=server Text='<%# DataBinder.Eval(Container.DataItem,"Email") %>' Width="200px" MaxLength="300" meta:resourcekey="txtEmailResource1" ></asp:TextBox>
                                                                                                                                        <asp:CustomValidator id="valEmail1" runat="server" ClientValidationFunction="CustomValidateEmail" 
																															ErrorMessage="Please remove invalid or duplicate email addresses." ControlToValidate="txtEmail" meta:resourcekey="valEmailResource1" Text="*" CssClass="errortext"></asp:CustomValidator><asp:requiredfieldvalidator id="valReqEmail1" runat="server" Width="1px" ErrorMessage="Please enter an email address"
																															ControlToValidate="txtEmail" meta:resourcekey="valReqEmailResource1" Text="*" CssClass="errortext" ></asp:requiredfieldvalidator>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Time Zone'>
																																	<ItemStyle Wrap="False"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:Label ID="lblTimeZone" text='<%# DataBinder.Eval(Container.DataItem,"TimeZoneName") %>' Runat=server meta:resourcekey="lblTimeZoneResource1"></asp:Label>
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:DropDownList ID="cboTimeZone" DataSource='<%# dsTimeZone %>' DataValueField="TimeZoneId" DataTextField="TimeZoneName" SelectedIndex='<%# GetTimeZone(Convert.ToInt16(DataBinder.Eval(Container.DataItem,"TimeZone"))) %>' Runat=server meta:resourcekey="cboTimeZoneResource1">
																																		</asp:DropDownList>
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Auto Adjust Daylight Savings' Visible="false" >
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDaylight1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AutoAdjustDayLightSaving")) %>' runat="server" meta:resourcekey="chkDaylight1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDaylight" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "AutoAdjustDayLightSaving")) %>' runat="server" meta:resourcekey="chkDaylightResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Hours Violation'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkHoursViolation1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "HoursViolation")) %>' runat="server" meta:resourcekey="chkHoursViolation1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkHoursViolation" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "HoursViolation")) %>' runat="server" meta:resourcekey="chkHoursViolationResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Driving Without Sign In'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkUnsignInDriving1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "UnsignInDriving")) %>' runat="server" meta:resourcekey="chkUnsignInDriving1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkUnsignInDriving" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "UnsignInDriving")) %>' runat="server" meta:resourcekey="chkUnsignInDrivingResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																<asp:TemplateColumn HeaderText='Pre Trip Inspection Not Done'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkPreTripInspection1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "PreTripInspection")) %>' runat="server" meta:resourcekey="chkPreTripInspection1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkPreTripInspection" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "PreTripInspection")) %>' runat="server" meta:resourcekey="chkPreTripInspectionResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>


                                                                                                                                <asp:TemplateColumn HeaderText='Post Trip Inspection Not Done'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkPostTripInspection1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "PostTripInspection")) %>' runat="server" meta:resourcekey="chkPostTripInspection1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkPostTripInspection" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "PostTripInspection")) %>' runat="server" meta:resourcekey="chkPostTripInspectionResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>

                                                                                                                                <asp:TemplateColumn HeaderText='Driving With Major Defect'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDrivingWithMajorDefect1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DrivingWithMajorDefect")) %>' runat="server" meta:resourcekey="chkDrivingWithMajorDefect1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDrivingWithMajorDefect" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DrivingWithMajorDefect")) %>' runat="server"  meta:resourcekey="chkDrivingWithMajorDefectResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>

                                                                                                                                 <asp:TemplateColumn HeaderText='Driver Pending List'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDriverPendingList1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DPLEmailNotif")) %>' runat="server" meta:resourcekey="chkDriverPendingList1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDriverPendingList" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DPLEmailNotif")) %>' runat="server"  meta:resourcekey="chkDriverPendingListResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																
																																<asp:TemplateColumn HeaderText='Driver logs not received in 3 days'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDriverLogNotReceived3Days1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverLogNotReceived3Days")) %>' runat="server" meta:resourcekey="chkDriverLogNotReceived3Days1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDriverLogNotReceived3Days" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverLogNotReceived3Days")) %>' runat="server"  meta:resourcekey="chkDriverLogNotReceived3DaysResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>

                                                                                                                                <asp:TemplateColumn HeaderText='Driver logs not received in 2 1/2 days'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkDriverLogNotReceived2Days1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverLogNotReceived2Days")) %>' runat="server" meta:resourcekey="chkDriverLogNotReceived2Days1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkDriverLogNotReceived2Days" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "DriverLogNotReceived2Days")) %>' runat="server"  meta:resourcekey="chkDriverLogNotReceived2DaysResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>
																																
																																 <asp:TemplateColumn HeaderText='Email Log sheet'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkLogSheet1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "LogSheet")) %>' runat="server" meta:resourcekey="chkLogSheet1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkLogSheet" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "LogSheet")) %>' runat="server"  meta:resourcekey="chkLogSheetResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>

                                                                                                                                 <asp:TemplateColumn HeaderText='Email Inspection Log'>
																																	<HeaderStyle Width="20px"></HeaderStyle>
																																	<ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
																																	<ItemTemplate>
																																		<asp:CheckBox ID="chkInspection1" Enabled="False" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Inspection")) %>' runat="server" meta:resourcekey="chkInspection1Resource1" />
																																	</ItemTemplate>
																																	<EditItemTemplate>
																																		<asp:CheckBox ID="chkInspection" Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Inspection")) %>' runat="server"  meta:resourcekey="chkInspectionResource1" />
																																	</EditItemTemplate>
																																</asp:TemplateColumn>


																																<asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;" 
 EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource1"></asp:EditCommandColumn>
																																<asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
																															</Columns>
																															<PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
																														</asp:datagrid></TD>
																												</TR>
																												<TR>
																													<TD align="center" height="20"></TD>
																												</TR>
																												<TR>
																													<TD align="center"><asp:button id="cmdAddEmail" runat="server" Width="127px" CssClass="combutton" Text="Add Email Address"
																															CommandName="23" onclick="cmdAddEmail_Click" meta:resourcekey="cmdAddEmailResource1"></asp:button></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																									<TR>
																										<TD style="WIDTH: 100%;" align="center">
																											<TABLE id="tblEmailAdd" cellSpacing="0" cellPadding="0" border="0" runat="server">
																												<TR>
																													<TD class="formtext" valign="top">
                                                                                                                        <asp:Label ID="lblEmailAddr" runat="server" Text="Email Address:" meta:resourcekey="lblEmailAddrResource1"></asp:Label><asp:CustomValidator id="valEmail" runat="server" ClientValidationFunction="CustomValidateEmail"
																															ErrorMessage="Please remove invalid or duplicate email." ControlToValidate="txtNewEmail" meta:resourcekey="valEmailResource1" Text="*" CssClass="errortext"></asp:CustomValidator><asp:requiredfieldvalidator id="valReqEmail" runat="server" Width="1px" ErrorMessage="Please enter an email address"
																															ControlToValidate="txtNewEmail" meta:resourcekey="valReqEmailResource1" Text="*" CssClass="errortext"></asp:requiredfieldvalidator></TD>
																													<TD valign="top" align="left">
                                                                                                                        <asp:textbox id="txtNewEmail" runat="server" 
                                                                                                                            Width="500px" CssClass="formtext" meta:resourcekey="txtNewEmailResource1" 
                                                                                                                            MaxLength="300"></asp:textbox><br />(Separate multiple emails with semi colon)</TD>
																													<TD style="WIDTH: 35px"></TD>
																													<TD valign="top"><asp:button id="cmdSaveEmail" runat="server" CssClass="combutton" Text="Save" onclick="cmdSaveEmail_Click" meta:resourcekey="cmdSaveEmailResource1"></asp:button></TD>
																												</TR>
																												<TR>
																													<TD class="formtext" valign="top">
                                                                                                                        <asp:Label ID="lblPhone" runat="server" Text="Phone:" Visible="False" 
                                                                                                                            meta:resourcekey="lblPhoneResource1"></asp:Label>
                                                                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                                                                                            ControlToValidate="txtPhone" CssClass="formtext" 
                                                                                                                            ErrorMessage="Invalid Phone Number:" 
                                                                                                                            
                                                                                                                            
                                                                                                                            ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$" 
                                                                                                                            Enabled="False" meta:resourcekey="RegularExpressionValidator1Resource1">*</asp:RegularExpressionValidator>
                                                                                                                    </TD>
																													<TD valign="top">
                                                                                                                        <asp:textbox id="txtPhone" runat="server" Width="261px" 
                                                                                                                            CssClass="formtext" Enabled="False" Visible="False" 
                                                                                                                            meta:resourcekey="txtPhoneResource1"></asp:textbox></TD>
																													<TD style="WIDTH: 35px">&nbsp;</TD>
																													<TD valign="top">&nbsp;</TD>
																												</TR>
																												<TR>
																													<TD class="formtext" align="left" valign="top">
                                                                                                                        <asp:Label ID="lblTimeZone" runat="server" Text="Time Zone:" meta:resourcekey="lblTimeZoneResource2"></asp:Label></TD>
																													<TD align="left" valign="top"><asp:dropdownlist id="cboTimeZoneAdd" runat="server" Width="261px" CssClass="RegularText" DataValueField="TimeZoneId"
																															DataTextField="TimeZoneName" meta:resourcekey="cboTimeZoneAddResource1"></asp:dropdownlist></TD>
																													<TD style="WIDTH: 35px"></TD>
																													<TD valign="top"><asp:button id="cmdCancelAddEmal" runat="server" CausesValidation="False" CssClass="combutton"
																															Text="Cancel" onclick="cmdCancelAddEmal_Click" meta:resourcekey="cmdCancelAddEmalResource1"></asp:button></TD>
																												</TR>
																												<TR>
																													<TD class="formtext" align="center"></TD>
																													<TD colspan="3" align="left" valign="top">
																														<TABLE id="Table13" cellSpacing="0" cellPadding="0" border="0">
																															<TR>
																																<TD></TD>
																															</TR>
																															<TR style="visibility:hidden">
																																<TD><asp:checkbox id="chkAddDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time" meta:resourcekey="chkAddDayLightResource1"></asp:checkbox></TD>
																															</TR>
																															<TR>
																																<TD class="formtext" style="height: 10px;"></TD>
																															</TR>
																															<TR>
																																<TD class="formtext">&nbsp;<asp:Label ID="lblEmailNotification" runat="server" meta:resourcekey="lblEmailNotificationResource1"
                                                                                                                                        Text="Email notification in case of:"></asp:Label></TD>
																															</TR>
																															<TR>
																																<TD>
																																	<TABLE class="formtext" id="Table14" cellSpacing="0" cellPadding="0" border="0">
																																		<TR>
																																			<TD><asp:checkbox id="chkAddHoursViolation" runat="server" Text="Hours Violation" meta:resourcekey="chkAddHoursViolationResource1"></asp:checkbox> &nbsp;&nbsp;</TD>
                                                                                                                                        </tr>
                                                                                                                                        <tr>
																																			<TD><asp:checkbox id="chkAddUnsignInDriving" runat="server" Text="Driving Without Sign In" meta:resourcekey="chkAddUnsignInDrivingResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
                                                                                                                                        </tr> 
                                                                                                                                        <tr>
																																			<TD><asp:checkbox id="chkAddPreTripInspection" runat="server" Text="Pre Trip Inspection Not Done" meta:resourcekey="chkAddPreTripInspectionResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
                                                                                                                                        </tr>
                                                                                                                                        <tr>
                                                                                                                                            <TD><asp:checkbox id="chkAddPostTripInspection" runat="server" Text="Post Trip Inspection Not Done" meta:resourcekey="chkAddPostTripInspectionResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
                                                                                                                                        </tr>
                                                                                                                                        <tr>
                                                                                                                                            <TD><asp:checkbox id="chkAddDrivingWithMajorDefect" runat="server" Text="Driving With Major Defect" meta:resourcekey="chkAddDrivingWithMajorDefectResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
                                                                                                                                         <tr>
                                                                                                                                            <TD><asp:checkbox id="chkDriverPendingList" runat="server" Text="Driver Pending List" meta:resourcekey="chkAddDriverPendingListResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
																																		 <tr>
                                                                                                                                            <TD><asp:checkbox id="chkDriverLogNotReceived3Days" runat="server" Text="Driver logs not received in 3 days" meta:resourcekey="chkDriverLogNotReceived3DaysResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
                                                                                                                                         <tr>
                                                                                                                                            <TD><asp:checkbox id="chkDriverLogNotReceived2Days" runat="server" Text="Driver logs not received in 2 1/2 days" meta:resourcekey="chkDriverLogNotReceived2DaysResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
																																		<tr>
                                                                                                                                            <TD><asp:checkbox id="chkLogSheet" runat="server" Text="Email Log sheet" meta:resourcekey="chkLogSheetResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
                                                                                                                                        <tr>
                                                                                                                                            <TD><asp:checkbox id="chkInspection" runat="server" Text="Email Inspection Log" meta:resourcekey="chkInspectionResource1"></asp:checkbox>&nbsp;&nbsp;</TD>
																																		</TR>
																																		
																																	</TABLE>
																																</TD>
																															</TR>
																														</TABLE>
																													</TD>
                                                                                                                    
																													<%--<TD style="WIDTH: 35px"></TD>
																													<TD></TD>--%>
																												</TR>
																											</TABLE>
																											<asp:label id="lblMessage" runat="server" Width="270px" CssClass="errortext" Visible="False"
																												Height="8px" meta:resourcekey="lblMessageResource1"></asp:label><asp:validationsummary id="ValidationSummary1" runat="server" Width="332px" Height="23px" meta:resourcekey="ValidationSummary1Resource1" CssClass="errortext"></asp:validationsummary>
																											<TABLE id="Table15" style="WIDTH: 100%; HEIGHT: 64px" cellSpacing="1" cellPadding="1" width="900"
																												border="0">
																												<TR>
																													<TD class="formtext" align="left"><U>&nbsp;<asp:Label ID="lblNote" runat="server" Font-Bold="True" meta:resourcekey="lblNoteResource1"
                                                                                                                            Text="Note:"></asp:Label></U><BR>
                                                                                                                        <asp:Label ID="lblNoteDesc" runat="server" meta:resourcekey="lblNoteDescResource1" 
                                                                                                                            
                                                                                                                            Text="In order to receive email notifications on alarms, you must enter valid email addresses. In addition, you have to select the type of alert."></asp:Label></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																								</TABLE>
																							</TD>
																						</TR>
																					</TABLE>
																				</TD>
																			</TR>
																		</TABLE>
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




        <script type="text/javascript" >
            function CustomValidateEmail(sender, args) {
                args.IsValid = true
                var splitEmail = args.Value.split(";");
                
                for (var inci = 0; inci < splitEmail.length; inci++) {
                    for (var incj = inci + 1; incj < splitEmail.length; incj++) {
                        var inputTexti = splitEmail[inci];
                        var inputTextj = splitEmail[incj];
                        inputTexti = inputTexti.replace(/^\s+|\s+$/g, "")
                        inputTextj = inputTextj.replace(/^\s+|\s+$/g, "")
                        if (inputTexti == inputTextj) {
                            args.IsValid = false;
                            return;
                        }
                    }
                }
                for( var inc=0;inc<splitEmail.length;inc++) {
                    var inputText = splitEmail[inc];
                    inputText = inputText.replace(/^\s+|\s+$/g, "")
                    if (inputText != '') {
                        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
                        if (reg.test(inputText) == false) {
                            args.IsValid = false;
                            return;
                        }
                    }
                }
            }
        </script>

     
    </div>
    </form>
</body>
</html>

