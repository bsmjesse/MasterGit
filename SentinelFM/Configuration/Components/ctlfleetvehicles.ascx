<%@ Control Language="c#" Inherits="SentinelFM.Components.ctlFleetVehicles" CodeFile="ctlFleetVehicles.ascx.cs" %>

<script type="text/javascript">
    function Confirm(isAddAll) {
        var ConfirmText = "";
        if (isAddAll)
            ConfirmText = "<%= ConfirmAddAll %>";
        else
            ConfirmText = "<%= ConfirmRemoveAll %>";
        return confirm(ConfirmText);
    }
</script>
<table id="Table1" style="width: 679px; height: 444px" cellspacing="0" cellpadding="0"
    width="679" align="center" border="0">
    <tr>
        <td class="tableheading" style="border-bottom-width: 1px; border-bottom-color: black; height: 30px"
            align="center" valign="top">
            <table id="Table3" style="width: 257px; " cellspacing="0" cellpadding="0"
                width="257" align="center" border="0">
                <tbody>
                    <tr>
                        <td class="formtext" style="width: 100%" align="center" valign="top">&nbsp;
                            <table>
                            <tr>
                                <td  style="width: 100px">
                                    <asp:Label ID="lblFleet" runat="server" Text="Fleet:" meta:resourcekey="lblFleetResource1"
                                        CssClass="formtext"></asp:Label></td>
                                <td style="width: 100px">
                                    <asp:DropDownList ID="cboToFleet" AutoPostBack="True" DataValueField="FleetId" DataTextField="FleetName"
                                        CssClass="RegularText" Width="200px" runat="server" OnSelectedIndexChanged="cboToFleet_SelectedIndexChanged"
                                        meta:resourcekey="cboToFleetResource1">
                                    </asp:DropDownList></td>
                            </tr>
                                <tr><td>
                                   <asp:FileUpload ID="newFleetVehicleAssignment" runat="server" CssClass="RegularText" Width="200px" style="margin: 20px 0px 20px 0px;" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click"/>

                                    </td>
                                </tr>

                                <tr><td>
                                   <asp:FileUpload ID="newMultipleFleetVehicleAssignment" runat="server" CssClass="RegularText" Width="200px" style="margin: 20px 0px 20px 0px;" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnBatchUpload" runat="server" Text="Batch Upload" OnClick="btnBatchUpload_Click"/>

                                    </td>
                                </tr>
                                
                                    
                                
                        </table>
                        </td>
                        <td></td>
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
                                                    <td style="width: 138px" align="center"></td>
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
                                                                <td style="height: 20px"></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" CommandName="37"
                                                                        OnClick="cmdAddAll_Click" OnClientClick="if ( ! Confirm(true)) return false;" meta:resourcekey="cmdAddAllResource1"></asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td id="TD1" style="height: 20px" runat="server"></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove" CommandName="38"
                                                                        OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 20px"></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All"
                                                                        CommandName="38" OnClick="cmdRemoveAll_Click" OnClientClick="if ( ! Confirm(false)) return false;;" meta:resourcekey="cmdRemoveAllResource1"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:ListBox ID="lstAss" DataValueField="VehicleId" DataTextField="Description" CssClass="formtext"
                                                            Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstAssResource1"></asp:ListBox></td>
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

