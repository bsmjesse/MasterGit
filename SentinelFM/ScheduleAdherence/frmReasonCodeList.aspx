<%@ Page Title="" Language="C#" MasterPageFile="~/ScheduleAdherence/SAMasterPage.master" AutoEventWireup="true" CodeFile="frmReasonCodeList.aspx.cs" Inherits="ScheduleAdherence_frmReasonCodeList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        var IDHead = "ctl00_ContentPlaceHolder1_";
        function EditReasonCode(reasonId) {
            document.getElementById(IDHead + "hdReasonCode").value = reasonId;
        }

        function DeleteReasonCode(reasonId, name) {
            if (!confirm("Are you sure you want to delete " + name + "?"))
                return false;
            document.getElementById(IDHead + "hdReasonCode").value = reasonId;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdReasonCode" runat="server" />
<table width="100%" class="formtext">
    <tr><td><asp:Label ID="lbError" runat="server" CssClass="errortext"/></td></tr>
    <tr>
        <td>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" ></asp:ValidationSummary>
        </td>
    </tr>
</table>
<table id="Table_options" runat="server" width="100%" class="formtext">
    <tr><td>
<fieldset>
    <legend>Calculator Options</legend>
<table class="formtext">
    <tr>
        <td>
            <asp:Label ID="lb_OptionsError" Text="" ForeColor="Red" runat="server" />
        </td>
    </tr>
    <tr style="height:20px">
        <td>
            Before Windows(minutes):
            <asp:RequiredFieldValidator ID="valReasonCode" runat="server" ControlToValidate="txtWindowsbefore"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtWindowsbefore" runat="server" CssClass="formtext" ></asp:textbox></td>
        <td >
            After Windows(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtWindowsAfter"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtWindowsAfter" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>
            Early for Depot departure(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtRSCDepartEarly"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtRSCDepartEarly" runat="server" CssClass="formtext" ></asp:textbox></td>
        <td>
            Late for Depot departure(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtRSCDepartLate"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtRSCDepartLate" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>
            Early for Depot arrival(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtRSCArrivalEarly"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtRSCArrivalEarly" runat="server" CssClass="formtext" ></asp:textbox></td>
        <td>
            Late for Depot arrival(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtRSCDepartLate"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtRSCArrivalLate" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>
            Early for Station depature(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtStopDepartEarly"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtStopDepartEarly" runat="server" CssClass="formtext" ></asp:textbox></td>
        <td>
            Late for Station depature(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txtStopDepartLate"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtStopDepartLate" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr style="height:20px">
        <td>
            Early for Station arrival(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtStopArrivalEarly"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtStopArrivalEarly" runat="server" CssClass="formtext" ></asp:textbox></td>
        <td>
            Late for Station arrival(minutes):
            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtStopArrivalLate"
                ErrorMessage="Please enter an Reason Code" Text="*"></asp:RequiredFieldValidator>
        </td>
        <td><asp:textbox id="txtStopArrivalLate" runat="server" CssClass="formtext" ></asp:textbox></td>
    </tr>
    <tr><td height="20"></td></tr>
    <tr><td colspan="4" align="center">
    <asp:button id="cmdSave" runat="server" Width="50px" CssClass="combutton" Text="Save" onclick="cmdSave_Click" />
    </td></tr>
</table>
</fieldset>
    </td></tr>
    <tr><td>
        <fieldset>
        <legend>Reason Code</legend>
        <table>
        <tr><td>
        <asp:GridView ID="gvReasonCode" runat="server" Width="900" AutoGenerateColumns="false"
            onrowcommand="gvReasonCode_RowCommand" onrowdatabound="gvReasonCode_RowDataBound"
            onpageindexchanging="gvReasonCode_PageIndexChanging"
            GridLines="None" CellPadding="3" BackColor="White" BorderWidth="2px" BorderStyle="Ridge"
            CellSpacing="1" BorderColor="White" PageSize="13" AllowPaging="true">
            <EmptyDataTemplate>No Data.</EmptyDataTemplate>
            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
            <SelectedRowStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedRowStyle>
            <AlternatingRowStyle CssClass="gridtext" BackColor="Beige"></AlternatingRowStyle>
            <RowStyle CssClass="gridtext" BackColor="White"></RowStyle>
            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
            <PagerSettings Mode="NumericFirstLast" />
            <PagerStyle CssClass="cssPager" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
            <Columns>
                <asp:BoundField DataField="ReasonCode" HeaderText="Reason Code" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:ButtonField ItemStyle-Width="20px"  ButtonType="Image"  ImageUrl="../images/edit.gif" CommandName="cmdEdit" />
                <asp:ButtonField ItemStyle-Width="20px" ButtonType="Image" ImageUrl="../images/delete.gif" CommandName="cmdDelete" />
            </Columns>
        </asp:GridView>
         </td></tr>
        <tr><td align="center">
        <br />       
        <asp:button id="cmdAddReasonCode" runat="server" Width="127px" CssClass="combutton" Text="Add Reason Code" onclick="cmdAddReasonCode_Click" />
        </td></tr>
        </table>
        </fieldset>
   </td></tr>
   <tr><td>
    <fieldset>
        <legend>Import Format</legend>
        <table>
        <tr><td>
        <asp:DropDownList ID="DDL_FileFormat" runat="server">
            <asp:ListItem Text="" Value="" />
            <asp:ListItem Text="General Format" Value="General" />
            <asp:ListItem Text="Sobeys Format" Value="Sobeys" />
        </asp:DropDownList>
        </td></tr>
        <tr><td align="center">
        <asp:button id="cmdSaveFormat" runat="server" Width="50px" CssClass="combutton" Text="Save" onclick="cmdSaveFormat_Click" />
        </td></tr>
        </table>
    </fieldset>
   </td></tr>
</table>
</asp:Content>

