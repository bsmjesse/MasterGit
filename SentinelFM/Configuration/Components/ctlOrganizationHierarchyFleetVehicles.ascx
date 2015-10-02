<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlOrganizationHierarchyFleetVehicles.ascx.cs" Inherits="SentinelFM.Components.Configuration_Components_ctlOrganizationHierarchyFleetVehicles" %>
<script language="javascript">
	<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
    var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
    var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

    var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
    function onOrganizationHierarchyNodeCodeClick()
    {
        var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val() + '&loadVehicle=0';
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
<table id="Table1" style="width: 679px; height: 444px" cellspacing="0" cellpadding="0"
    width="679" align="center" border="0">
    <tr>
        <td class="tableheading" style="border-bottom-width: 1px; border-bottom-color: black;
            height: 30px" align="center" valign="top">
            <table id="Table3" style="width: 257px; height: 20px" cellspacing="0" cellpadding="0"
                width="257" align="center" border="0">
                <tbody>
                    <tr>
                        <td class="formtext" style="width: 100%" align="center" valign="top">
                            &nbsp;<table>
                                <tr>
                                        <td class="style1">
                                        <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                            Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                                 <td>
                                        <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                            CssClass="combutton" Width="220px"
                                            OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                            meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                 </td>
                                </tr>                               
                            </table>
                        </td>

                        <tr>
                        <td class="formtext" style="width: 100%" align="center" valign="top">
                            &nbsp;<table>
                                <tr>
                                        <td  >
                                        <asp:Label ID="lblManagerName" runat="server" CssClass="formtext" 
                                            Text=" Manager Name:" meta:resourcekey="lblManagerNameResource1" Visible="false"  /></td>
                                 <td>
                                      

                                     <asp:TextBox ID="txtboxManagerName" runat="server" 
                                             Width="220px" Visible="false"                                         
                                            />
                                 </td>
                                    <td></td>

                                     <td>
                                        <asp:Button ID="btnSave" runat="server" 
                                            CssClass="combutton" Width="88px"
                                            onclick="cmdSave_Click" Text="Edit"                                           
                                            meta:resourcekey="btnSaveResource1" Visible="false" CauseValidation=false  />
                                 </td>

                                     <td></td>

                                     <td>
                                        <asp:Button ID="btnCancel" runat="server" 
                                            CssClass="combutton" Width="88px"
                                            onclick="cmdCancel_Click" Text="Cancel"                                           
                                            meta:resourcekey="btnCancelResource1" Visible="false" CauseValidation=false   />
                                 </td>
                                </tr>                               
                            </table>
                        </td>
                        
                        <tr>
                            <td style="width: 959px; height: 217px" align="center">
                                <table id="Table8" style="width: 568px; height: 303px" cellspacing="0" cellpadding="0"
                                    width="568" border="0">
                                    <tr>
                                        <td style="height: 270px" align="center">
                                            <table id="tblVehicles" style="height: 284px" cellspacing="0" cellpadding="0" width="100%"
                                                border="0" runat="server">
                                                <tr>
                                                    <td class="formtext" style="width: 213px">
                                                        <asp:Label ID="lblUnAssVehicles" runat="server" Text="Unassigned vehicles" meta:resourcekey="lblUnAssVehiclesResource1"></asp:Label></td>
                                                    <td style="width: 138px" align="center">
                                                    </td>
                                                    <td class="formtext">
                                                        <asp:Label ID="lblAssVehicles" runat="server" Text="Assigned vehicles" meta:resourcekey="lblAssVehiclesResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 213px" valign="top">
                                                        <asp:ListBox ID="lstUnAss" DataValueField="VehicleId" DataTextField="Description"
                                                            CssClass="formtext" Width="200px" runat="server" SelectionMode="Multiple" Rows="15"
                                                            meta:resourcekey="lstUnAssResource1"></asp:ListBox></td>
                                                    <td style="width: 138px" align="center" valign="top">
                                                        <table id="tblAddRemoveBtns" style="width: 75px; height: 99px" cellspacing="0" cellpadding="0"
                                                            width="75" border="0" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdAdd" CssClass="combutton" runat="server" Text="Add->" CommandName="37"
                                                                        OnClick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 20px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" CommandName="37"
                                                                        OnClick="cmdAddAll_Click" OnClientClick="return confirm('Are you sure you want to Add All?');" meta:resourcekey="cmdAddAllResource1"></asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td id="TD1" style="height: 20px" runat="server">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" CommandName="38"
                                                                        OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 20px">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All"
                                                                        CommandName="38" OnClick="cmdRemoveAll_Click" OnClientClick="return confirm('Are you sure you want to Remove All?');" meta:resourcekey="cmdRemoveAllResource1">
                                                                    </asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:ListBox ID="lstAss" DataValueField="VehicleId" DataTextField="Description" CssClass="formtext"
                                                            Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstAssResource1">
                                                        </asp:ListBox></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblMessage" CssClass="errortext" Width="270px" runat="server" Height="8px"
                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                        </tr>
        </td>
    </tr>
</table>
