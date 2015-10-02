<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ctlOrganizationSubMenuTabs.ascx.cs" Inherits="SentinelFM.Components.Configuration_Components_ctlOrganizationSubMenuTabs" %>
<TABLE id="Table5" style="Z-INDEX: 101; WIDTH: 190px; POSITION: relative; TOP: 0px; HEIGHT: 22px"
	cellSpacing="0" cellPadding="0" border="0">
	<TR>
		<TD><asp:button id="cmdSettings" runat="server" Width="117px" 
                CausesValidation="False" CssClass="confbutton"
				Text="Settings" onclick="cmdSettings_Click" meta:resourcekey="cmdSettingsResource1" ></asp:button></TD>
		<TD >
			<asp:button id="cmdPushSettings" runat="server" 
                Width="117px" CausesValidation="False" CssClass="confbutton"
				Text="Push Settings" onclick="cmdPushSettings_Click" CommandName="51" meta:resourcekey="cmdPushSettingsResource1" ></asp:button>
			</TD>
		<TD>
			<asp:button id="cmdFuel" runat="server" Width="117px" CausesValidation="False" CssClass="confbutton"
				Text="Fuel Transactions" OnClick="cmdFuel_Click" CommandName="50" meta:resourcekey="cmdFuelResource1" />
																						
			</TD>
                <TD>
			<asp:button id="cmdPanicMangement" runat="server" 
                Width="117px" CausesValidation="False" CssClass="confbutton"
				Text="PanicManagement" CommandName="54" onclick="cmdPanicMangement_Click" Visible="False" meta:resourcekey="cmdPanicMangementResource1" ></asp:button></TD>
        <TD>
			<asp:button id="cmdMapSubscription" runat="server"
                Width="117px" CausesValidation="False" CssClass="confbutton"
				Text="Map Subscription" CommandName="54" onclick="cmdMapSubscription_Click" meta:resourcekey="cmdMapSubscriptionResource1" ></asp:button>
        </TD>
        <TD>
			<asp:button id="cmdOverlaySubscription" runat="server"
                Width="127px" CausesValidation="False" CssClass="confbutton"
				Text="Overlay Subscription" CommandName="54" onclick="cmdOverlaySubscription_Click" meta:resourcekey="cmdOverlaySubscriptionResource1" ></asp:button>
        </TD>

        <TD>
			<asp:button id="cmdHierarchy" runat="server"
                Width="97px" CausesValidation="False" CssClass="confbutton"
				Text="Hierarchy" CommandName="54" onclick="cmdHierarchy_Click" meta:resourcekey="cmdHierarchyResource1" ></asp:button>
        </TD>
        <TD>
			<asp:button id="cmdHierarchyImport" runat="server"
                Width="117px" CausesValidation="False" CssClass="confbutton"
				Text="Hierarchy Import" CommandName="54" onclick="cmdHierarchyImport_Click" meta:resourcekey="cmdHierarchyImportResource1" ></asp:button>
        </TD>
        <TD>
			<asp:button id="cmdHierarchyAssignmentImport" runat="server"
                Width="177px" CausesValidation="False" CssClass="confbutton" ToolTip="Hierarchy Assignment Import"
				Text="Hierarchy Assignment Import" CommandName="54" onclick="cmdHierarchyAssignmentImport_Click" 
                meta:resourcekey="cmdHierarchyAssignmentImportResource1" ></asp:button>
        </TD>
    <td>
        <asp:Button ID="btnFuelCategory" runat="server" Width="168px" Visible= "False" 
            CausesValidation="False" CssClass="confbutton"
                Text="Fuel Category" PostBackUrl="~/Configuration/FuelCategory/frmFuelCategory.aspx"
            meta:resourcekey="btnFuelCategoryResource1"></asp:Button>

    </td>

    <TD>
			<asp:button id="btnQueryWindows" runat="server"
                Width="177px" CausesValidation="False" CssClass="confbutton" ToolTip="Query Windows"
				Text="Query Windows" CommandName="54" onclick="cmdQueryWindows_Click" 
                meta:resourcekey="cmdQueryWindowsResource1" ></asp:button>
        </TD>


	</TR>
</TABLE>