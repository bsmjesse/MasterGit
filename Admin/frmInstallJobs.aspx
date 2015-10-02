<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmInstallJobs.aspx.cs" Inherits="frmInstallJobs" Theme="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Installation Reports</title>
</head>
<body style="font-family: Arial, Verdana; font-size:small">
    <form id="form1" runat="server">
    <div style="padding: 20px 20px 20px 20px">
        <table cellpadding="2" cellspacing="2" style="padding: 20px 20px 20px 20px; width: 500px; border: double 4px Gray; text-align: left;">
            <caption><b>Upload Install Job XML File</b></caption>
            <tr>
                <td>
                    <asp:FileUpload ID="FileUploadJobXml" runat="server" style="width:420px; text-align: left" />
                    <asp:RequiredFieldValidator runat="server" ID="FileValidator" ControlToValidate="FileUploadJobXml" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="LabelDescription" Text="Description"></asp:Label>
                    <asp:TextBox runat="server" ID="TextDescription" Width="350"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="DescriptionValidator" ControlToValidate="TextDescription" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnSubmitFile" Text="Upload" OnClick="btnSubmitFile_Click" />
                </td>
            </tr>
        </table>
        <br /><br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" DataKeyNames="JobID" Width="500px"
            OnPageIndexChanging="GridView1_PageIndexChanging"
            OnSelectedIndexChanged="GridView1_SelectedIndexChanged" Caption="<b>Install Jobs</b>" CaptionAlign="Top" 
            OnSorting="GridView1_Sorting">
            <Columns>
                <asp:BoundField DataField="JobID" HeaderText="JobID" InsertVisible="False" ReadOnly="True" Visible="False" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                <asp:BoundField DataField="BoxId" HeaderText="BoxId" SortExpression="BoxId" />
                <asp:BoundField DataField="LastModified" HeaderText="LastModified" HtmlEncode="true" DataFormatString="{0:g}" SortExpression="LastModified" />
                <asp:BoundField DataField="Installer" HeaderText="Installer" SortExpression="Installer" />
                <asp:CommandField ShowSelectButton="True" />
            </Columns>
        </asp:GridView>
        <br />
        <asp:Label ID="LabelMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
    </form>
</body>
</html>
