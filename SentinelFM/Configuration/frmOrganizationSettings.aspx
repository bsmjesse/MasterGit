<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmOrganizationSettings.aspx.cs" Inherits="SentinelFM.Configuration_frmOrganizationSettings" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Assembly="obout_ColorPicker" Namespace="OboutInc.ColorPicker" TagPrefix="obout" %>

<%@ Register Src="../UserControl/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc1" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <head runat="server">
		<title>frmEmails</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
		
		<script type="text/JavaScript">

function OnColorPicked(sender){
  if(typeof ob_post == "object"){
    ob_post.AddParam("mColor", sender.getColor());     
    ob_post.post(null, "OnColorSelect", function(){});
  }
}

</script>

        <style type="text/css">
            .style2
            {
                width: 247px;
            }
            .style3
            {
            }
            .formtext
            {
                margin-left: 0px;
            }
            .style5
            {
                width: 176px;
            }
            .style6
            {
                width: 160px;
            }
            .style7
            {
                width: 100%;
            }
        </style>

  </HEAD>
	<body>
		<form id="Form1" method="post" runat="server" enctype="multipart/form-data">
             <asp:scriptmanager runat="server"></asp:scriptmanager>
			<table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<tr>
					<td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdOrganization" HasOrgCommandName="false" />
					</td>
				</tr>
				<tr>
					<td>
						<table id="tblBody" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<tr>
								<td>
									<table id="tblForm" class=table HEIGHT="550px"
										width="1060" border="0">
										<tr>
											<td class="configTabBackground">
												<table id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<tr>
														<td>
															<table id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<tr>
																	<td>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="cmdSettings" />
																	</td>
																</tr>
																<tr>
																	<td  >
																		<table id="Table6" cellSpacing="0" cellPadding="0" width="957" align="center" border="0">
																			<tr>
																				<td>
																					<table id="Table7" class=table WIDTH="1030px" HEIGHT="500px"
																						border="0">
																						<tr>
																							<td class="configTabBackground" valign=middle align=center    >
                                                                                                <table class="formtext" width="500">
                                                                                                    
                                                                                                    
                                                                                                    <tr>
                                                                                            <td  colspan="2" align="center">
                                                                                                  <fieldset style="solid 1px #6666; width:450px" >
                                                                      <!--Driver message forward to email switch, HL-->
                                                                <table id="panelDriverMessageForwardToEmail" width="250" align="center">
                                                                  <tr>
                                                                    <td>
                                                                                                    
                                                                        <asp:Label ID="lblDriverMessageForwardToEmail" runat="server" 
                                                                            CssClass="formtext" Text="Driver Message Forward To Email"
                                                                            meta:resourcekey="lblDriverMessageForwardToEmailResource1" style="text-align: left;" ></asp:Label>    
                                                                                <asp:CheckBox ID="chkDriverMessageForward" runat="server" 
                                                                            EnableTheming="False" />

                                                                       
                                                                                                    
                                                                    </td>
                                                                                                
                                                                </tr>
                                                                </table>
                                                                 </fieldset>
                                                                                            </td>
                                                                                        </tr>    
                                                                                                    <tr>
                                                                                                        <td align="left" class="style3" >
                                                                                                            <asp:Label ID="lblHeader_Menu1" runat="server" CssClass="formtext" 
                                                                                                                Text="Picture on Home Page" Visible="False" 
                                                                                                                meta:resourcekey="lblHeader_Menu1Resource1"></asp:Label>
                                                                                                        
                                                                                                            </td>
                                                                                                        <td align="left" class="style2">
                                                                                                            <asp:TextBox ID="txtFileName" runat="server" Visible="False" 
                                                                                                                meta:resourcekey="txtFileNameResource1" Text="Default"></asp:TextBox>
                                                                                                        
                                                                                                            </td>
                                                                                                    </tr>
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="left" class="style3" >
                                                                                                            <asp:Label ID="lblNewPicture" runat="server" Text="Upload New Picture" 
                                                                                                                Visible="False" meta:resourcekey="lblNewPictureResource1"></asp:Label>
                                                                                                        
                                                                                                            </td>
                                                                                                        <td align="left" class="style2">
                                                                                                            <asp:FileUpload ID="fileUpEx" runat="server" Visible="False" 
                                                                                                                meta:resourcekey="fileUpExResource1" />
                                                                                                        
                                                                                                            </td>
                                                                                                    </tr>
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="left" class="style3" >
                                                                                                            &nbsp;</td>
                                                                                                        <td align="left" class="style2">
                                                                                                            &nbsp;</td>
                                                                                                    </tr>
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="center" class="style3" >
                                                                                                            <asp:Label ID="lblHeader_Menu" runat="server" CssClass="formtext" 
                                                                                                                Text="Color of Menus and Titles" 
                                                                                                                meta:resourcekey="lblHeader_MenuResource1" Width="450px" style="text-align:right" ></asp:Label>
                                                                                                        
                                                                                                            </td>
                                                                                                        <td align="left" class="style2" >
                                                                                                        <obout:ColorPicker runat="server" ID="pickerMenus"
             TargetId="pickerMenusColor" TargetProperty="style.backgroundColor" 
             OnColorPostBack="pickerMenus_ColorPostBack" PickButton="False" AutoPostBack="True" CloseButtonImage="" CloseButtonOverImage="" CloseButtonPressedImage="" 
                                                                                                                PickerCallerImage="" SaveButtonImage="" SaveButtonOverImage="" 
                                                                                                                SaveButtonPressedImage="" SetColorImage="" SetColorOverImage="" 
                                                                                                                SetColorPressedImage="" StyleFile="" >
             <asp:TextBox readOnly="true" id="pickerMenusColor" style="cursor: pointer;" runat="server" meta:resourcekey="pickerMenusColorResource1"/>
      </obout:ColorPicker>
                                                                                                        
                                                                                                            </td>
                                                                                                    </tr>
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="center" class="style3" >
                                                                                                            <asp:Label ID="lblHeader_Menu0" runat="server" CssClass="formtext" 
                                                                                                                Text="Background color of Tabs" 
                                                                                                                meta:resourcekey="lblHeader_Menu0Resource1" Width="450px" style="text-align:right"></asp:Label>
                                                                                                        
                                                                                                            </td>
                                                                                                        <td align="left" class="style2" >
                                                                                                        <obout:ColorPicker runat="server" ID="pickerTabBackground"
             TargetId="pickerTabBackgroundColor" TargetProperty="style.backgroundColor" 
             OnColorPostBack="pickerTabBackground_ColorPostBack" PickButton="False" AutoPostBack="True" CloseButtonImage="" CloseButtonOverImage="" CloseButtonPressedImage="" 
                                                                                                                PickerCallerImage="" SaveButtonImage="" SaveButtonOverImage="" 
                                                                                                                SaveButtonPressedImage="" SetColorImage="" SetColorOverImage="" 
                                                                                                                SetColorPressedImage="" StyleFile="" >
             <asp:TextBox readOnly="true" id="pickerTabBackgroundColor" style="cursor: pointer;" runat="server" meta:resourcekey="pickerTabBackgroundColorResource1"/>
      </obout:ColorPicker>
                                                                                                        
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                     <tr>
                                                                                                        <td align="center" class="style3"  >
                                                                                                            <asp:Label ID="lblHeader_Menu2" runat="server" CssClass="formtext" 
                                                                                                                Text="Supported Zones(s) for sending command" Width="450px" 
                                                                                                                meta:resourcekey="lblHeader_Menu2Resource1" style="text-align:right"></asp:Label>
                                                                                                        
                                                                                                            </td>
                                                                                                        <td align="left" class="style2" >
                                                                                                       
                                                                                                            <asp:TextBox  id="txtZone"  runat="server" Text="1" />
                                                                                                            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Temperature should a whole number between be 1-99" ControlToValidate="txtZone" type="Integer" MinimumValue="1" MaximumValue="99" meta:resourcekey="TemperaturerRangeResource1"></asp:RangeValidator>
                                                                                                        
                                                                                                        </td>
                                                                                                    </tr>              
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="left" colspan=2 >
                                                                                                            <table id="DriversReport" runat=server visible=false    cellpadding=2 cellspacing=2  >
                                                                                                                <tr>
                                                                                                                    <td class="style5">
                                                                                                            <asp:Label ID="lblDriverViolationReport" runat="server" CssClass="formtext" 
                                                                                                                Text="Idling Summary Report. Iding:" meta:resourcekey="lblDriverViolationReportResource1" 
                                                                                                                ></asp:Label>
                                                                                                        
                                                                                                                    </td>
                                                                                                                    <td align=left class="style6"  >
 <asp:DropDownList ID="cboIdlingThreshold" runat="server" CssClass="formtext" 
                                                                     Width="140px" meta:resourcekey="cboIdlingThresholdResource1">
                                                                            <asp:ListItem Value="-1" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">More than 1 Hour</asp:ListItem>
                                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">More than 2 Hours</asp:ListItem>
                                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource4">More than 3 Hours</asp:ListItem>
                                                                        </asp:DropDownList>

                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="2" align=left  >
                                                                                                                                
                                                <fieldset runat=server id="tblPoints" style="width: 300px"   class="formtext" >
                                                <legend class="formtext"> Driver Violation Report. Scores Points:</legend> 
                                                <table id="Table1" runat="server" class="formtext">
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed120" runat="server" CssClass="formtext" Text="Speed >" meta:resourcekey="lblSpeed120Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed120" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed120Resource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="Label9" runat="server" CssClass="formtext" Text="Acc. Harsh" meta:resourcekey="Label9Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccHarshResource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingExtreme" runat="server" CssClass="formtext" Text="Braking Extreme"
                                                                            meta:resourcekey="lblBrakingExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingExtreme" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingExtremeResource1" Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed130" runat="server" Text="Speed >" meta:resourcekey="lblSpeed130Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed130" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed130Resource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblAccExtreme" runat="server" Text="Acc. Extreme" meta:resourcekey="lblAccExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccExtremeResource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSeatBelt" runat="server" Text="Seat Belt" meta:resourcekey="lblSeatBeltResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSeatBelt" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSeatBeltResource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed140" runat="server" Text="Speed >" meta:resourcekey="lblSpeed140Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed140" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed140Resource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingHarsh" runat="server" CssClass="formtext" Text="Braking Harsh"
                                                                            meta:resourcekey="lblBrakingHarshResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingHarsh" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingHarshResource1" Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                                </fieldset>
                                

                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="2"  >
                                                                                                                                
                                                                                                                        <table class="fromtext" width="100%" cellpadding=0 cellspacing=0 border=0    >
                                                                                                                            <tr>
                                                                                                                                <td valign=top >
                                                                                                                                    <asp:Label ID="lblReportingFrequency" runat="server" CssClass="formtext" 
                                                                                                                                        Text="Reporting Frequency for Idling and driver violation report:" 
                                                                                                                                        meta:resourcekey="lblReportingFrequencyResource1"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <table class="style7">
                                                                                                                                        <tr>
                                                                                                                                            <td>
                                                                                                                                                <asp:CheckBox ID="chkWeeklyReportActive" runat="server" CssClass="formtext" 
                                                                                                                                                    Text="Weekly Report Active" 
                                                                                                                                                    meta:resourcekey="chkWeeklyReportActiveResource1" />
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                        <tr>
                                                                                                                                            <td>
                                                                                                                                                <asp:CheckBox ID="chkMonthlyReportActive" runat="server" CssClass="formtext" 
                                                                                                                                                    Text="Monthly Report Active" 
                                                                                                                                                    meta:resourcekey="chkMonthlyReportActiveResource1" />
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </table>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                

                                                                                                                    </td>
                                                                                                                </tr>

                                                                                                                <tr>
                                                                                                                    <td  align=left  >
                                                                                                                                
                                                                                                                        <asp:Label ID="lblFuelConsumptionSpeeding" runat="server" CssClass="formtext" 
                                                                                                                            Text="Fuel Consumption while Speeding threshold" 
                                                                                                                            meta:resourcekey="lblFuelConsumptionSpeedingResource1"></asp:Label>
                                                                                                                                
                                                                                                                    </td> 
                                                                                                                        <td  align=left class="style6"  >
                                                                                                                                
                                                                                                                            <asp:TextBox ID="txtFuelConsumptionSpeeding" runat="server" CssClass="formtext" 
                                                                                                                                Width="40px" meta:resourcekey="txtFuelConsumptionSpeedingResource1"></asp:TextBox>
                                                                                                                                
                                                                                                                    </td> 
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                            </td>
                                                                                                    </tr>
                                                                                                                  
                                                                                                                  
                                                                                                                  
                                                                                                                  
                                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align=left>
                                                                                                            <asp:Label ID="lblCustomMsg" runat="server" CssClass="formtext" 
                                                                                                                Text="Message on Home Page:" Visible=false meta:resourcekey="lblCustomMsgResource"> </asp:Label>
                                                                                                        </td>
                                                                                                        <td align="left" >
                                                                                                              <asp:TextBox ID="txtCustomMsg" runat="server" CssClass="formtext"  
                                                                                                                MaxLength="500" Rows=14 TextMode="MultiLine" Width="358px" Visible=false  ></asp:TextBox>
                                                                                                           </td>
                                                                                                        
                                                                                                    </tr>
                                                                                                                   
                                                                                                                   
                                                                                                                    
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="left" class="style3" >
                                                                                                            &nbsp;</td>
                                                                                                        <td align="left" class="style2" >

                                                                                                            &nbsp;</td>
                                                                                                    </tr>       
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td align="center" colspan="2" >
                                                                                                        
                                                                                                            <asp:Button ID="cmdRestoreDefaults" runat="server" CssClass="combutton" 
                                                                                                                OnClick="cmdRestoreDefaults_Click" Text="Restore Defaults" 
                                                                                                                meta:resourcekey="cmdRestoreDefaultsResource1" Width="200px" />
                                                                                                        &nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                            <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" 
                                                                                                                OnClick="cmdUpdate_Click" Text="Update Settings" 
                                                                                                                meta:resourcekey="cmdUpdateResource1" Width="200px" CausesValidation="true"/> 
                                                                                                        
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td colspan="2" align="center">
                                                                                                            <table id="Table8" style="WIDTH: 586px; HEIGHT: 284px" cellSpacing="0" cellPadding="0" width="586" border="0">
			                                                                                                    <tr>
				                                                                                                    <td style="HEIGHT: 270px" align="center">
					                                                                                                    <table id="tblUsers" style="WIDTH: 548px; HEIGHT: 265px" cellSpacing="0" cellPadding="0"
						                                                                                                    width="548" border="0" runat="server">
						                                                                                                    <tr>
							                                                                                                    <td colspan="2" class="formtext" style="text-align:left">
                                                                                                                             <asp:Label style="text-align:left" ID="lblUnUsers" runat="server" Text="Available Sensors"  meta:resourcekey="lblAvailableSensors"></asp:Label></td>
							                                                                                                    
							                                                                                                    <td class="formtext">
                                                                                                                             <asp:Label  ID="lblAssUsers" runat="server" Text="Show sensors on Map Pop-Up"  meta:resourcekey="lblShowSensorsOnMap"></asp:Label></td>
						                                                                                                    </tr>
						                                                                                                    <tr>
							                                                                                                    <td style="WIDTH: 110px" valign=top>
								                                                                                                    <asp:listbox id="lstUnAss" DataValueField="SensorName" DataTextField="SensorName" CssClass="formtext"
									                                                                                                    Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1"></asp:listbox></td>
							                                                                                                    <td style="WIDTH: 130px" align="center" valign=top>
								                                                                                                    <table id="tblAddRemoveBtns" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									                                                                                                    width="75" border="0" runat="server">
									                                                                                                    <tr>
										                                                                                                    <td valign=middle>
											                                                                                                    <asp:button id="cmdAdd" CssClass="combutton" runat="server" Text="Add->"  CommandName="39" onclick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" CommandName="39" onclick="cmdAddAll_Click" meta:resourcekey="cmdAddAllResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td id="TD1" style="HEIGHT: 20px" runat="server"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" CommandName="40" onclick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All" CommandName="40" onclick="cmdRemoveAll_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:button></td>
									                                                                                                    </tr>
								                                                                                                    </table>
							                                                                                                    </td>
							                                                                                                    <td valign=top>
								                                                                                                    <asp:listbox id="lstAss" DataValueField="SensorName" DataTextField="SensorName" CssClass="formtext"
									                                                                                                    Width="200px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" meta:resourcekey="lstAssResource1"></asp:listbox></td>
						                                                                                                    </tr>
					                                                                                                    </table>
				                                                                                                    </td>
			                                                                                                    </tr>
		                                                                                                    </table>
                                                                                                        </td>

                                                                                                    </tr>
                                                                                                  
                                                                                                    <tr>
                                                                                                        
                                                                                                        <td >
                                                                                                              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                                                <ContentTemplate> 
                                                                                                            <table id="Table12" style="WIDTH: 500px; HEIGHT: 284px" cellSpacing="0" cellPadding="0" width="500" border="0">
			                                                                                                    <tr>
				                                                                                                    <td style="HEIGHT: 270px" align="center">
					                                                                                                    <table id="Table3" style="WIDTH: 480px; HEIGHT: 265px" cellSpacing="0" cellPadding="0"
						                                                                                                    width="490px" border="0" runat="server">
						                                                                                                    <tr>
							                                                                                                    <td colspan="2" class="formtext" style="text-align:left">
                                                                                                                             <asp:Label style="text-align:left" ID="lblAvailableEvents" runat="server" Text="Available Events"  meta:resourcekey="lblAvailableEvents"></asp:Label></td>
							                                                                                                    
							                                                                                                    <td class="formtext">
                                                                                                                             <asp:Label  ID="lblShowEvents" runat="server" Text="Show Events on EV"  meta:resourcekey="lblShowEventsOnEV"></asp:Label></td>
						                                                                                                    </tr>
						                                                                                                    <tr>
							                                                                                                    <td style="WIDTH: 110px" valign=top>
								                                                                                                    <asp:listbox id="lstUnAssEvents" DataValueField="EventTypeID" DataTextField="Description" CssClass="formtext"
									                                                                                                    Width="170px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssEventsResource1"></asp:listbox></td>
							                                                                                                    <td style="WIDTH: 130px" align="center" valign=top>
								                                                                                                    <table id="Table4" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									                                                                                                    width="75" border="0" runat="server">
									                                                                                                    <tr>
										                                                                                                    <td valign=middle>
											                                                                                                    <asp:button id="Button1" CssClass="combutton" runat="server" Text="Add->"  CommandName="39" onclick="cmdAddEvent_Click" meta:resourcekey="cmdAddResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button2" CssClass="combutton" runat="server" Text="Add All->" CommandName="39" onclick="cmdAddAllEvent_Click" meta:resourcekey="cmdAddAllResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td id="TD2" style="HEIGHT: 20px" runat="server"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button3" CssClass="combutton" runat="server" Text="<-Remove" CommandName="40" onclick="cmdRemoveEvent_Click" meta:resourcekey="cmdRemoveResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button4" CssClass="combutton" runat="server" Text="<-Remove All" CommandName="40" onclick="cmdRemoveAllEvent_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:button></td>
									                                                                                                    </tr>
								                                                                                                    </table>
							                                                                                                    </td>
							                                                                                                    <td valign=top>
								                                                                                                    <asp:listbox id="lstAssEvents" DataValueField="EventTypeID" DataTextField="EventName" CssClass="formtext"
									                                                                                                    Width="170px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" meta:resourcekey="lstAssEventsResource1"></asp:listbox></td>
						                                                                                                    </tr>
					                                                                                                    </table>
				                                                                                                    </td>
			                                                                                                    </tr>
		                                                                                                    </table>
                                                                                                     </ContentTemplate>
                                                                                                </asp:UpdatePanel>
                                                                                                        </td>
                                                                                                         <td >
                                                                                                               <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                                                <ContentTemplate> 
                                                                                                            <table id="Table13" style="WIDTH: 500px; HEIGHT: 284px" cellSpacing="0" cellPadding="0" width="500" border="0">
			                                                                                                    <tr>
				                                                                                                    <td style="HEIGHT: 270px" align="center">
					                                                                                                    <table id="Table5" style="WIDTH: 480px; HEIGHT: 265px" cellSpacing="0" cellPadding="0"
						                                                                                                    width="480" border="0" runat="server">
						                                                                                                    <tr>
							                                                                                                    <td colspan="2" class="formtext" style="text-align:left">
                                                                                                                             <asp:Label style="text-align:left" ID="lblAvailableViolations" runat="server" Text="Available Violations"  meta:resourcekey="lblAvailableViolations"></asp:Label></td>
							                                                                                                    
							                                                                                                    <td class="formtext">
                                                                                                                             <asp:Label  ID="lblShowViolations" runat="server" Text="Show Violations on EV "  meta:resourcekey="lblShowViolationsOnEV"></asp:Label></td>
						                                                                                                    </tr>
						                                                                                                    <tr>
							                                                                                                    <td style="WIDTH: 110px" valign=top>
								                                                                                                    <asp:listbox id="lstUnAssViolations" DataValueField="EventTypeID" DataTextField="Description" CssClass="formtext"
									                                                                                                    Width="170px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssViolationsResource1"></asp:listbox></td>
							                                                                                                    <td style="WIDTH: 130px" align="center" valign=top>
								                                                                                                    <table id="Table9" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									                                                                                                    width="75" border="0" runat="server">
									                                                                                                    <tr>
										                                                                                                    <td valign=middle>
											                                                                                                    <asp:button id="Button5" CssClass="combutton" runat="server" Text="Add->" CommandName="39" onclick="cmdAddViolation_Click" meta:resourcekey="cmdAddResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button6" CssClass="combutton" runat="server" Text="Add All->" CommandName="39" onclick="cmdAddAllViolation_Click" meta:resourcekey="cmdAddAllResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td id="TD3" style="HEIGHT: 20px" runat="server"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button7" CssClass="combutton" runat="server" Text="<-Remove" CommandName="40" onclick="cmdRemoveViolation_Click" meta:resourcekey="cmdRemoveResource1"></asp:button></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td style="HEIGHT: 20px"></td>
									                                                                                                    </tr>
									                                                                                                    <tr>
										                                                                                                    <td>
											                                                                                                    <asp:button id="Button8" CssClass="combutton" runat="server" Text="<-Remove All" CommandName="40" onclick="cmdRemoveAllViolation_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:button></td>
									                                                                                                    </tr>
								                                                                                                    </table>
							                                                                                                    </td>
							                                                                                                    <td valign=top>
								                                                                                                    <asp:listbox id="lstAssViolations" DataValueField="EventTypeID" DataTextField="ViolationName" CssClass="formtext"
									                                                                                                    Width="170px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" meta:resourcekey="lstAssViolationsResource1"></asp:listbox></td>
						                                                                                                    </tr>
					                                                                                                    </table>
				                                                                                                    </td>
			                                                                                                    </tr>
		                                                                                                    </table>
                                                                                                       </ContentTemplate>
                                                                                                </asp:UpdatePanel>
                                                                                                        </td>
                                                                                                        </tr>
                                                                                                   
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td class="style3" >
                                                                                                            &nbsp;</td>
                                                                                                        <td>
                                                                                                            &nbsp;</td>
                                                                                                    </tr>
                                                                                                                   
                                                                                                    
                                                                                                    <tr>
                                                                                                        <td class="style3" colspan="2" align=center   >
                                                                                                            <asp:Label ID="lblMessage" runat="server" CssClass="formtext" 
                                                                                                                meta:resourcekey="lblMessageResource1"></asp:Label>
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
		
           
		
		</form>
	</body>
</HTML>
