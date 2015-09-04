<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="frmLandmarkImport.aspx.cs" Inherits="SentinelFM.frmLandmarkImport" Debug="true" Theme="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Landmark Data Import</title>
    <link rel="stylesheet" type="text/css" href="GlobalStyle.css" />
</head>
<body>
    <form id="form1" runat="server" enableviewstate="true">
    <div>
        <table cellpadding="5" style="font-size: small; width: 703px; font-family: Arial; border-width: medium; border-color:Gray">
            <tr>
                <td style="width: 10px">
                </td>
                <td colspan="2">
                    <asp:Label ID="lblSectionTitle" runat="server" Text="Landmark Data Import" 
                        CssClass="formtext" Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                    <span>This tool will help you import landmarks in batches</span>
                    <ul>
                          <li>
                            Don't worry we won't override a landmark if the name already exists
                          </li>
                          <li>
                              Size and data type of fields matters 
                              <img border="0" src="../images/Info.png" width="16px" height="16px" alt="Info" 
                                  title="LandmarkName: text (64) &#013;CategoryName: text (40) &#013;Latitude: float &#013;Longitude: float &#013;Radius: int &#013;Address: text (256) &#013;Description: text (100)  &#013;ContactPersonName: text (100) &#013;ContactPhoneNumber: text (20) &#013;Email: text (120) &#013;Phone: text (55) &#013;TimeZone: int &#013;DayLightSaving: boolean ([true/false] or [yes/no] or [1/0]) &#013;AutoAdjustDayLightSaving: boolean ([true/false] or [yes/no] or [1/0]) &#013;Public: boolean ([true/false] or [yes/no] or [1/0])" />
                          </li>
                          <li>
                              Address resolution will automatically happen 
                              <img border="0" src="../images/Info.png" width="16px" height="16px" alt="Info" 
                                  title="If the address resolution fails, then the landmark will not be imported. Always resolved address will be saved in the system" />
                          </li>
                          <li>
                              Mandatory fields must be provided 
                              <img border="0" src="../images/Info.png" width="16px" height="16px" alt="Info"  
                                  title="LandmarkName &#013;CategoryName &#013;Radius&#013;Latitude and Longitude or Address" />
                          </li>
                          <li>
                              When optional fields are blank, we will assign default values 
                              <img border="0" src="../images/Info.png" width="16px" height="16px" alt="Info" 
                                  title="Description, default: NULL &#013;ContactPersonName, default: NULL &#013;ContactPhoneNumber, default: NULL &#013;Email, default: NULL &#013;Phone, default: NULL &#013;TimeZone, default: 0 &#013;DayLightSaving, default: false &#013;AutoAdjustDayLightSaving, default: false &#013;Public, default: false" />
                          </li>
                          <li>
                              Acceptable import file columns are 
                              <span style="color:red">
                                  LandmarkName, CategoryName, Latitude, Longitude, Radius, Address
                              </span>, Description, ContactPersonName, ContactPhoneNumber, Email, Phone, TimeZone, DayLightSaving, AutoAdjustDayLightSaving, and Public.
                          </li>
                    </ul> 
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                    <asp:Label ID="Label2" runat="server" Text="Upload Landmarks XLS file" CssClass="heading" Width="264px" Font-Names="Arial" />
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                    <asp:FileUpload ID="FileUpload1" runat="server" Width="528px" ToolTip="Landmarks XLS file" />
                    <asp:RequiredFieldValidator ID="rvFileUpload" runat ="server" ValidationGroup ="gpUpload" ErrorMessage ="Please select a file to upload." ControlToValidate ="FileUpload1"  ></asp:RequiredFieldValidator>
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                    <asp:Button id="Submit2" runat="server" Text="Submit" CssClass="combutton" OnClick="Submit2_Click" ValidationGroup ="gpUpload" />
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
                <td style="width: 409px">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="Small" SkinID ="None"
                        ForeColor="Red" Height="18px" Width="536px" CssClass="errortext" Font-Bold="True"></asp:Label></td>
                <td style="width: 65px">
                </td>
            </tr>
            
        </table>
        <table cellpadding="5" style="font-size: small; width: 90%; font-family: Arial; border-width: medium; border-color:Gray">
            <tr>
                <td style="width: 10px">
                </td>
            
                <td >
                    <asp:Label ID="lblGridCaption" runat="server" Height="18px" Font-Bold="True"
                         Text="Import Summary:"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 10px">
                </td>
            
                <td >
                   <asp:GridView ID="gvErrors" runat="server"  Width="100%" AutoGenerateColumns ="false">
                      <HeaderStyle  HorizontalAlign="left" />
                      <Columns >
                         <asp:BoundField DataField ="RowNumber" HeaderText = "Row No"  />
                         <asp:BoundField DataField ="LandmarkName" HeaderText = "Landmark Name"  />
                         <asp:BoundField DataField ="CategoryName"  HeaderText = "Category" />
                         <asp:BoundField DataField ="ResolvedAddress"  HeaderText = "Resolved Address" />
                         <asp:BoundField DataField ="SelectedLatitude"  HeaderText = "Latitude" />
                         <asp:BoundField DataField ="SelectedLongitude"  HeaderText = "Longitude" />
                         <asp:TemplateField HeaderText="Result" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Literal id="Literal1" runat="server" text='<%# Eval ("ActionResultHtml") %>'></asp:Literal>
                            </ItemTemplate>
                         </asp:TemplateField>
                         <asp:BoundField DataField ="ErrorMessage" HeaderText ="Error" />
                      </Columns>
                   </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
