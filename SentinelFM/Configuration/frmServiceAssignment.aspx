<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmServiceAssignment.aspx.cs" Inherits="Configuration_frmServiceAssignment" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="ctlFleetVehicles" Src="Components/ctlFleetVehicles.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Service Assignment Configuration</title>
    <link href="Configuration.css" type="text/css" rel="stylesheet" />
    <meta name="CODE_LANGUAGE" Content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">     
    <style type="text/css">
    html, body, div, iframe { margin:0; padding:0; height:100%; }
   iframe { display:block; width:100%; border:none; }
</style>
</head>
<body>
    
            <form id="Form1" method="post" runat="server">
			<TABLE id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				cellPadding="0" width="1300" height="1000" border="0">
				<TR>
					<TD valign="top">
					    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="btnServiceAssigment"  />
					</TD>
				</TR>
               
																	
				<TR>
					<TD valign="top">
						<TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="1100" align="left" border="0" height="1000">
							<TR>
								<TD class="configTabBackground"  height="100%">
									<TABLE id="tblForm" class=table width="1300" height="100%" border="0">
										
																<TR>
																	<TD width="100%" height="100%" valign="top">
																		<TABLE id="Table6" width="100%" height="100%"  cellSpacing="0" cellPadding="0" height="100%" align="center" border="0">
																			<TR>
																				<TD valign="top" height="100%">
																				
                                                                                     <!--Content begin-->
                                                                                    
                                                                                    
                                                                                    <iframe id="frameServiceAssignment" src="../ServiceAssignment/Main.aspx" height="100%" frameBorder="0" style="overflow:hidden;height:100%;width:100%" width="100%">
                                                                                        
                                                                                    </iframe>
                                                                                    
                                                                                    

                                                                                    <!--Content end-->	
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
					</TD>
				</TR>
			</TABLE>
		
		</form>
</body>
</html>
