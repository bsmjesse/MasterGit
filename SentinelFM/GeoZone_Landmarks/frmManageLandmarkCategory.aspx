<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmManageLandmarkCategory.aspx.cs" Inherits="SentinelFM.frmManageLandmarkCategory" Culture="en-US" UICulture="auto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Landmark Category</title>
	<script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
	<script type="text/javascript">
        function ConfirmDelete(message) {
			var selectedValue = $('#lbxCategoryList option:selected').val();
			var isYes = true;
			
			if(selectedValue){
				isYes = confirm(message);
			}
			
			return isYes;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <div>
            <asp:Label ID="lblSectionTitle" runat="server" Text="Manage Landmark Category" 
                        CssClass="formtext" Font-Bold="true" meta:resourcekey="lblSectionTitleResource1"></asp:Label>
            <br /><asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px" ></asp:Label><br />
            <table>
                 <tr>
                    <td colspan="2">
                        <asp:Label ID="lblCategoryName" runat="server" Text="Category Name" 
                                    CssClass="formtext" meta:resourcekey="lblCategoryNameResource1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="360px">
                        <asp:TextBox ID="txtCategoryName" runat="server" Width="340px" CssClass="formtext" MaxLength="40"></asp:TextBox>
                        <asp:TextBox ID="txtCategoryId" runat="server" Visible="false"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnAddCategory" runat="server" Text="Add" CssClass="combutton" 
                            Width="100px" OnClick="btnAddCategory_Click" meta:resourcekey="btnAddCategoryResource1" />

                        <asp:Button ID="btnUpdateCategory" runat="server" Text="Update" CssClass="combutton" 
                            Width="100px" OnClick="btnUpdateCategory_Click" meta:resourcekey="btnUpdateCategoryResource1" />
                        <asp:Button ID="btnCancelUpdateCategory" runat="server" Text="Cancel" CssClass="combutton" 
                            Width="100px" OnClick="btnCancelUpdateCategory_Click" meta:resourcekey="btnCancelUpdateCategoryResource1" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <asp:Label ID="lblLandmarkCategoryList" runat="server" Text="Category List" 
                            CssClass="formtext" meta:resourcekey="lblLandmarkCategoryListResource1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:ListBox ID="lbxCategoryList" runat="server" Width="340px" Height="300px" CssClass="formtext"></asp:ListBox>
                    </td>
                    <td valign="top">
                        <asp:Button ID="btnEditCategory" runat="server" Text="Edit" CssClass="combutton" 
                                    Width="100px" OnClick="btnEdit_Click" meta:resourcekey="btnEditCategoryResource1" />
                        <asp:Button ID="btnDeleteCategory" runat="server" Text="Delete" CssClass="combutton" 
                                    Width="100px" OnClick="btnDeleteCategory_Click" meta:resourcekey="btnDeleteCategoryResource1"
                              OnClientClick="" />
                    </td>
                </tr>
                <tr>
                    <td>

                    </td>
                    <td>

                    </td>
                </tr>
            </table>
            
            
        </div>

    </div>
    </form>
</body>
</html>
