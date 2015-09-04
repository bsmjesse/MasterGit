<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchForm.aspx.cs" Inherits="SentinelFM.DriverFinder_SearchForm" %>

<form id="SearchForm" runat="server" defaultbutton="btnDisableEnter">
    <div>
        <% if (MyLayout != "h")
           {
           %>
           
         <table cellpadding="2" cellspacing="2">
            <tr>
                <td align="left">
                    <b>Skill:</b>
                </td>
                <td>                   
                    <asp:placeholder ID="skillListPlaceHolder_v" runat="server"></asp:placeholder>
                </td>
                <td>
                    
                </td>
            </tr>
            
            <tr>
                <td align="left">
                    <b>Vehicle Type:</b>
                </td>
                <td valign="bottom">
                    <asp:placeholder ID="vehicleTypesPlaceHolder_v" runat="server"></asp:placeholder>
                             
                </td>
                <td>
                    <img id="searchButton" src="Content/img/searchbutton.png" border="0" style="cursor: pointer;" onclick="RunSearch('popup')" />
                </td>
            </tr>
        </table>  
       <% }
           else
           {
        %>
                 <table cellpadding="2" cellspacing="2">
            <tr>
                <td>
                    <b>Address: </b>
                </td>
                <td>
<input type="text" name="txtAddress" id="txtAddress" runat="server" width="45"/>
                </td>
                <td align="left">
                    <b>Skill:</b>
                </td>
                <td>                   
                    <asp:placeholder ID="skillListPlaceHolder_h" runat="server"></asp:placeholder>
                </td>                         
                <td align="left">
                    <b>Vehicle Type:</b>
                </td>
                <td valign="bottom">                   
                    <asp:placeholder ID="vehicleTypesPlaceHolder_h" runat="server"></asp:placeholder>
                             
                </td>
                <td>
                    <img id="Img1" src="Content/img/searchbutton.png" border="0" style="cursor: pointer;" onclick="RunSearch('page')" />
                </td>
            </tr>
        </table> 
        <%
           }
          %>  
    </div>
    <asp:Button ID="btnDisableEnter" runat="server" Text="" OnClientClick="return false;" style="display:none;"/>
    </form>
