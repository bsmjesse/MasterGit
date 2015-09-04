<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlOrganizationHierarchyFleetUsers.ascx.cs" Inherits="SentinelFM.Components.ctlOrganizationHierarchyFleetUsers" %>
<script language="javascript">
	<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
    var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
    var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
    var RootOrganizationHierarchyNodeCode = '<%=RootOrganizationHierarchyNodeCode %>';
    var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
        if(MutipleUserHierarchyAssignment)
            mypage = mypage + '&loadVehicle=0&rootNodecode=' + RootOrganizationHierarchyNodeCode;        
        if(MutipleUserHierarchyAssignment && $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val().indexOf(",") < 0)
        {
            mypage = mypage + "&sl=1";
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
        $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
        $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
        $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
    }
            
		//-->
</script>
<asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
<asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
<asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

<asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
    style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" 
    meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

<TABLE id="Table1" style="WIDTH: 679px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
	width="679" align="center" border="0">
	<TR>
		<TD class="tableheading" style="BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: black; HEIGHT: 30px"
			align="center" valign="top">
			<TABLE id="Table3" style="WIDTH: 257px; HEIGHT: 20px" cellSpacing="0" cellPadding="0" width="257"
				align="center" border="0">
				<TBODY>
					<TR>
						<TD class="formtext" style="WIDTH: 100%" align="center" valign="top">
                      &nbsp;
                      <table>
                          <tr>
                              <td class="style1">
                                    <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                        Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                             <td colspan="3" >
                                    <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                        CssClass="combutton" Width="220px" 
                                        OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                        meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                             </td>
                          </tr>
                      </table>
                  </TD>
						<TD>
							</TD>
<TR>
	<TD style="WIDTH: 959px; HEIGHT: 217px" align="center">
		<TABLE id="Table8" style="WIDTH: 586px; HEIGHT: 284px" cellSpacing="0" cellPadding="0" width="586" border="0">
			<TR>
				<TD style="HEIGHT: 270px" align="center">
					<TABLE id="tblUsers" style="WIDTH: 548px; HEIGHT: 265px" cellSpacing="0" cellPadding="0"
						width="548" border="0" runat="server">
						<TR>
							<TD colspan="2" class="formtext" style="WIDTH: 240px">
                         <asp:Label ID="lblUnUsers" runat="server" meta:resourcekey="lblUnUsersResource1"
                             Text="Unassigned users"></asp:Label></TD>
							<%--<TD style="WIDTH: 130px" align="center"></TD>--%>
							<TD class="formtext">
                         <asp:Label ID="lblAssUsers" runat="server" meta:resourcekey="lblAssUsersResource1"
                             Text="Assigned users"></asp:Label></TD>
						</TR>
						<TR>
							<TD style="WIDTH: 110px" vAlign=top>
								<asp:listbox id="lstUnAss" DataValueField="UserId" DataTextField="UserFullName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1"></asp:listbox></TD>
							<TD style="WIDTH: 130px" align="center" vAlign=top>
								<TABLE id="tblAddRemoveBtns" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									width="75" border="0" runat="server">
									<TR>
										<TD vAlign=middle>
											<asp:button id="cmdAdd" CssClass="combutton" runat="server" Text="Add->" CommandName="39" onclick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" CommandName="39" onclick="cmdAddAll_Click" meta:resourcekey="cmdAddAllResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD id="TD1" style="HEIGHT: 20px" runat="server"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" CommandName="40" onclick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All" CommandName="40" onclick="cmdRemoveAll_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:button></TD>
									</TR>
								</TABLE>
							</TD>
							<TD vAlign=top>
								<asp:listbox id="lstAss" DataValueField="UserId" DataTextField="UserFullName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" meta:resourcekey="lstAssResource1"></asp:listbox></TD>
						</TR>
					</TABLE>
				</TD>
			</TR>
		</TABLE>
		<asp:label id="lblMessage" CssClass="errortext" Width="270px" runat="server" Height="8px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:label></TD>
</TR>
		</TD>
	</TR>
</TABLE>

