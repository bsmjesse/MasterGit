<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmTaskScheduler.aspx.cs" Inherits="SentinelFM.Configuration_frmTaskScheduler" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>
<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmEmails</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		


    
  </HEAD>
	<body>
    <script language="javascript">
    <!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
    var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
    var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

    var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
        if (MutipleUserHierarchyAssignment) {
            mypage = mypage + "&m=1&f=0&loadVehicle=0";
        }
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
        var myVal = document.getElementById('<%=cboVehicle.ClientID %>');
        ValidatorEnable(myVal, false); 

        $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
        $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
        $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
    }
            
		//-->
</script>
		<form id="Form1" method="post" runat="server" enctype="multipart/form-data">
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
            style="display:none;" AutoPostBack="True"
                    OnClick="hidOrganizationHierarchyPostBack_Click" 
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdScheduledTasks" HasOrgCommandName ="false" />
				    </TD>
				</TR>
				<TR>
					<TD>
						<TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table HEIGHT="550px"
										width="990" border="0">
										<TR>
											<TD class="configTabBackground">
											
											      
                                            <fieldset>
                <table class="formtext" style="width: 820px" >
                                <tr id="trFleetSelectOption" visible="false" runat="server">
                                    <td align="center" class="configTabBackground" colspan="4">
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
                                                            runat="server" Selected="True" meta:resourcekey="ListItemAssignBasedFleet" >Fleet</asp:ListItem> 
                                                        <asp:ListItem id="Radio1" name="raFleetSelectOption" value="1" runat="server" 
                                                                    meta:resourcekey="ListItemAssignBasedHierarchy" >Organization Hierarchy Fleet</asp:ListItem>
                                                    </asp:RadioButtonList>                                                         
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                   <td style="width: 100px" align="left">
                                
                                <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext"   meta:resourcekey="lblFromTitleResource1"
                                    Text="From:"></asp:Label></td>
                                   <td align="left">
                                   
                                      <table  border=0px cellpadding=0 cellspacing=0   >
                                         <tr>
                                            <td style="height: 21px" Width="181px">
                                            <ISWebInput:WebInput ID="txtFrom" runat="server" Width="171px"  Height="17px" 
                                                    Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtFromResource2" 
                                                    MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>

                                                  <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

					                                    <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                              </DateTimeEditor>   
                            
                                             </ISWebInput:WebInput>
                                             </td>
                                            <td >
                                              <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px"
                                    meta:resourcekey="cboHoursFromResource1" Height="21px">
                                </asp:DropDownList></td>
                                         </tr>
                                      </table>
                                   </td>
                                   <td align="left">
                                <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                    Text="To:"></asp:Label>
                                     
                                   </td>
                                   <td align="left" >
                                    
                                      <table border=0px cellpadding=0 cellspacing=0 >
                                         <tr>
                                            <td style="height: 21px" Width="181px">
                                            <ISWebInput:WebInput ID="txtTo" runat="server" Width="171px"  Height="17px" 
                                                    Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtToResource2" 
                                                    MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>

                                                        <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

					                                      <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                                 </DateTimeEditor>   
                        </ISWebInput:WebInput>
                                </td>
                                            <td style="height: 21px" >
                                               <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px" meta:resourcekey="cboHoursToResource1" Height="21px">
                                </asp:DropDownList></td>
                                         </tr>
                                      </table>
                                   </td>
                                </tr>
                                <tr>
                                   <td align="left" >
                                
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                                    Text="Fleet:"></asp:Label>
                                <asp:RangeValidator ID="valFleet" runat="server" MaximumValue="999999999999999" MinimumValue="1"
                                    ErrorMessage="Please select a Fleet" ControlToValidate="cboFleet" meta:resourcekey="valFleetResource1"
                                    Text="*"></asp:RangeValidator></td>
                                   <td style="width: 100px">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="257px"
                                    DataValueField="FleetId" DataTextField="FleetName" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList>
                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Visible="False"
                                        CssClass="combutton" Width="200px" 
                                        OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                        meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                </td>
                                   <td style="width: 100px" align="left">
                            
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Width="30px" 
                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:Label>
                                <asp:RequiredFieldValidator ID="valVehicle" runat="server" ControlToValidate="cboVehicle"
                                    ErrorMessage="Please select a Vehicle" meta:resourcekey="RequiredFieldValidator1Resource1"
                                    Text="*"></asp:RequiredFieldValidator></td>
                                   <td style="width: 100px">
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="256px"
                                    DataValueField="BoxId" DataTextField="Description" Visible="False"
                                     meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList></td>
                                </tr>
                                <tr>
                                   <td align="left" colspan="4" >
                                
                                        <table ID="tblSchButtons" runat="server" cellpadding="2" cellspacing="2">
                                       <tr id="Tr1" runat="server">
                                           <td id="Td1" align="left" runat="server">
                                               <asp:Button ID="cmdViewScheduledTasks" runat="server" CommandName="25" 
                                                   CssClass="combutton" meta:resourcekey="cmdScheduledTasksResource1" 
                                                   OnClick="cmdScheduledTasks_Click" Text="View Scheduled Tasks" Width="170px" />
                                               <asp:Button ID="cmdDeleteScheduledTasks" runat="server" CssClass="combutton" 
                                                   onclick="cmdDeleteScheduledTasks_Click" Text="Delete Scheduled Tasks"  meta:resourcekey="cmdDeleteScheduledTasksResource1" 
                                                   Width="170px" />
                                               <asp:Label ID="lblScheduledMessageFilter" runat="server" CssClass="formtext" 
                                                   meta:resourcekey="lblScheduledMessageFilterResource1" 
                                                   Text="Scheduled Messages:" Visible="False"></asp:Label>
                                               <asp:DropDownList ID="cboScheduledMessageFilter" runat="server" 
                                                   CssClass="formtext" meta:resourcekey="cboFormsResource1" Visible="False" 
                                                   Width="250px">
                                                   <asp:ListItem meta:resourcekey="ListItemResource9" Text="All" Value="1"></asp:ListItem>
                                                   <asp:ListItem meta:resourcekey="ListItemResource10" Text="Sent" Value="2"></asp:ListItem>
                                               </asp:DropDownList>
                                               <asp:Label ID="lblDeleteScheduleTaskMsg" runat="server" CssClass="formtext" 
                                                   Text="* Please note: only &quot;Pending&quot; tasks can be deleted" meta:resourcekey="lblDeleteScheduleTaskMsgResource1" ></asp:Label>
                                           </td>
                                       </tr>
                                   </table>
                                      
                                    </td>
                                </tr>
                                <tr>
                                   <td align="left" valign=top colspan="4" >
                             
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="formtext" 
                                           meta:resourcekey="valSummaryResource1">
                                </asp:ValidationSummary>
                              
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="138px" 
                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                </tr>
                             </table>
                    </fieldset> 
                    
                    
											
											    
                                   
                                  
                                  <ISWebGrid:WebGrid ID="dgSched" runat="server" UseDefaultStyle="True" 
                    Width="100%" Height="450px" ViewStateStorage="Session" 
                    OnInitializeDataSource="dgSched_InitializeDataSource" 
                    OnInitializeLayout="dgSched_InitializeLayout" 
                    meta:resourcekey="dgSchedResource1"><RootTable DataKeyField="TaskId"><Columns><ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                               Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" EditType="NoEdit" IsRowChecker="True"
                               Name="Select Row" ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Date %>' Name="RequestDateTime" DataMember="RequestDateTime" DataType="System.DateTime"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Vehicle %>' Name="Description" DataMember="Description" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Command %>' Name="BoxCmdOutTypeName" DataMember="BoxCmdOutTypeName" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_LastTryDate %>' Name="LastDateTimeSent" DataMember="LastDateTimeSent" DataType="System.DateTime"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Sent %>' DataMember="MsgOutDateTime" Name="MsgOutDateTime" DataType="System.DateTime"
                               ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Status %>' DataMember="CommandStatus" Name="CommandStatus" DataType="System.String"
                               ></ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Data" DataMember="CustomProp" Name="CustomProp" DataType="System.String"></ISWebGrid:WebGridColumn>

                                                                                         </Columns></RootTable><LayoutSettings  AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes"   RowHeightDefault=25px      
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True" PersistRowChecker="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  AllowExport="Yes" DisplayDetailsOnUnhandledError="False" ><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>
                      
                      
												</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		
           
		
		</form>
	</body>
</HTML>
