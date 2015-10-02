<%@ Page language="c#" Inherits="SentinelFM.frmdriverskill" CodeFile="frmdriverskill.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="ctlFleetVehicles" Src="Components/ctlFleetVehicles.ascx" %>
<!DOCTYPE HTML >
<HTML>
  <head id="Head1" runat="server">
		<title>frmFleetVehicles</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">                
	    <link rel="stylesheet" href="../DriverFinder/Content/DataTables-1.9.2/media/css/demo_table.css" type="text/css" />           

        <link rel="stylesheet" href="../DriverFinder/Content/base/jquery.ui.all.css">   
        <link rel="stylesheet" href="../DriverFinder/Content/sentinelpage.css">     
	    <script src="../DriverFinder/Scripts/jquery-1.8.0.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.bgiframe-2.1.2.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.core.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.widget.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.mouse.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.button.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.draggable.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.position.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.resizable.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.ui.dialog.js"></script>
	    <script src="../DriverFinder/Scripts/ui/jquery.effects.core.js"></script>

        <script src="../DriverFinder/Scripts/OpenLayers.js" type="text/javascript"></script>    
        <script src="../DriverFinder/Scripts/DataTables-1.9.2/media/js/jquery.dataTables.js" type="text/javascript"></script>           
        <style>
		body { font-size: 62.5%; }
		label, input { display:block; }
		input.text { margin-bottom:12px; width:95%; padding: .4em; }
		fieldset { padding:0; border:0; margin-top:5px; }
		h1 { font-size: 1.2em; margin: .6em 0; }
		div#users-contain { width: 350px; margin: 20px 0; }
		div#users-contain table { margin: 1em 0; border-collapse: collapse; width: 100%; }
		div#users-contain table td, div#users-contain table th { border: 1px solid #eee; padding: .6em 10px; text-align: left; }
		.ui-dialog .ui-state-error { padding: .3em; }
		.validateTips { border: 1px solid transparent; padding: 0.3em; }
		table#DriverSkill 
		{
		    font-family:Verdana,Arial; 
            font-size:12px;
		}
	</style>
        <script src="../DriverFinder/Scripts/form.js" type="text/javascript"></script>
		<!--<LINK href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
  </HEAD>
	<body>
	            

		<FORM id="frmEmails" method="post" runat="server">
			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="300" border="0">
				<TR>
					<TD>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdFleets"  />
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
												<TABLE id="Table2" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<TABLE id="Table5" style="Z-INDEX: 101; WIDTH: 190px; POSITION: relative; TOP: 0px; HEIGHT: 22px"
																			cellSpacing="0" cellPadding="0" border="0">
																			<TR>
																				<TD>
																					<asp:button id="cmdEmails" runat="server" Text="Email Addresses" CssClass="confbutton" CausesValidation="False"
																						Width="117px" CommandName="3" onclick="cmdEmails_Click" meta:resourcekey="cmdEmailsResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetMng" runat="server" Text="Fleet Management" CssClass="confbutton" CausesValidation="False"
																						Width="141px" CommandName="9" onclick="cmdFleetMng_Click" meta:resourcekey="cmdFleetMngResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetVehicle" runat="server" Text="Fleet-Vehicle Assignment" CssClass="confbutton"
																						CausesValidation="False" Width="168px" CommandName="7" meta:resourcekey="cmdFleetVehicleResource1"></asp:button></TD>
																				<TD>
																					<asp:button id="cmdFleetUsers" runat="server" CommandName="15" CausesValidation="False" CssClass="confbutton"
																						Text="Fleet-User Assignment" Width="160px" onclick="cmdFleetUsers_Click" meta:resourcekey="cmdFleetUsersResource1"></asp:button></TD>
                                                                                <TD>
																					<asp:button id="cmdDriverSkill" runat="server" CommandName="16" Text="Driver-Skill Assignment" CssClass="selectedbutton"
																						CausesValidation="False" Width="160px" onclick="cmdDriverSkill_Click" meta:resourcekey="cmdDriverSkillResource1"></asp:button></TD>
																			</TR>
																		</TABLE>
																	</TD>
																</TR>
																<TR>
																	<TD>
																		<TABLE id="Table6" cellSpacing="0" cellPadding="0" width="617" align="center" border="0">
																			<TR>
																				<TD>                                      
                                                                                <br />
                                                                                                                        
                                                                                    <asp:Button ID="btnManageSkills" runat="server" Text="Manage Skills" CssClass="confbutton" 
                                                                                        onclick="btnManageSkills_Click" />
                                                                                        <hr />
																					<TABLE id="Table7" class=table HEIGHT="500px" WIDTH="960px" border="0">
																						<TR>
																							<TD valign=top>
                                                                                                <table id="DriverSkill" cellpadding="0" cellspacing="0" border="0" class="display">
                                                                                                    <thead>
                                                                                                            <tr>
                                                                                                                <th>Driver's name</th>
                                                                                                                <th>Skill</th> 
                                                                                                                <th>Description</th>
                                                                                                                <th>Modify</th>                                                                                                                                                                                                                             
                                                                                                            </tr>
                                                                                                     </thead>
                                                                                                     <tbody>            
                                                                                                     </tbody>           
                                                                                            </table>
                                                                                            
                                                                                            </TD>
																						</TR>
                                                                                        <tr>
                                                                                        <td align=center>
                                                                                            <asp:Button ID="AddNewDriverSkill" runat="server" Text="Add new driver's skill"  CssClass="confbutton" Width="160px" OnClientClick="return false;" />
                                                                                        </td>
                                                                                        </tr>
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
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
			
            <div class="demo">

<div id="dialog-form" title="Create new driver's skill">
	<fieldset>        
        <div id="DriverName" style="display: none;"></div>
        <div id="listDrivers">                
        <label for="name">Driver's name</label>
        <asp:DropDownList ID="DriversList" runat="server">
        </asp:DropDownList>
        </div>
		<label for="name">SkillList</label>
        <asp:DropDownList ID="skillsList" runat="server">
        </asp:DropDownList>	
        <label for="name">Description</label>
        <textarea name="txtSkillDescription" id="txtSkillDescription" rows="2" cols="46" class="textarea ui-widget-content ui-corner-all">
        </textarea>				
	</fieldset>	
</div>
</div><!-- End demo -->

		</FORM>
	</body>
</HTML>
