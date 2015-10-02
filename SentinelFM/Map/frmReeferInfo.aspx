<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReeferInfo.aspx.cs" Inherits="frmReeferInfo" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body id="body" runat="server" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
        <div>
           <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
              left: 15px; width: 423px; position: absolute; top: 11px; height: 137px" width="423">
              <tr>
                 <td style="height: 11px">
                    <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                       <tr>
                          <td>
                          </td>
                          <td>
                          </td>
                          <td>
                          </td>
                       </tr>
                    </table>
                 </td>
              </tr>
              <tr>
                 <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 413px;
                       height: 284px" width="413">
                       <tr>
                          <td style="height: 281px">
                             <table id="tblForm" border="0" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                border-left: gray 2px solid; width: 421px; border-bottom: gray 2px solid;
                                height: 450px" width="421">
                                <tr>
                                   <td align="center" class="configTabBackground">
                                      <table id="Table1" border="0" cellpadding="0" cellspacing="0" class="formtext" style="border-top-width: thin;
                                         border-left-width: thin; border-left-color: black; border-bottom-width: thin;
                                         border-bottom-color: black; width: 340px; border-top-color: black; height: 312px;
                                         border-right-width: thin; border-right-color: black" width="340">
                                         <tr>
                                            <td class="BigFormText" style="width: 9px; height: 5px">
                                            </td>
                                            <td colspan=2  class="BigFormText" style="height: 5px">
                                               <asp:Label ID="lblTemperatureTitle" runat="server" CssClass="formtext" Text="Temperature Thresholds (Degrees C)" Font-Bold="True" ></asp:Label></td>
                                            
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px; height: 27px">
                                            </td>
                                            <td class="formtext" style="width: 111px; height: 27px">
                                               &nbsp;&nbsp;</td>
                                            <td style="height: 27px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px; height: 25px">
                                            </td>
                                            <td class="formtext" style="width: 111px; height: 25px">
                                               &nbsp;<table class="formtext">
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:Label ID="Label1" runat="server" CssClass="formtext" Text="Zone"></asp:Label></td>
                                                     <td style="width: 77px">
                                                        <asp:Label ID="Label2" runat="server" CssClass="formtext" Text="Upper"></asp:Label></td>
                                                     <td style="width: 100px">
                                                        <asp:Label ID="Label3" runat="server" CssClass="formtext" Text="Lower"></asp:Label></td>
                                                     <td style="width: 100px">
                                                        <asp:Label ID="Label4" runat="server" CssClass="formtext" Text="Alarm Status"></asp:Label></td>
                                                  </tr>
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="1" /></td>
                                                     <td style="width: 77px">
                                                        <asp:TextBox ID="TextBox1" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                        <asp:TextBox ID="TextBox6" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                     </td>
                                                  </tr>
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:CheckBox ID="CheckBox2" runat="server" Text="2" /></td>
                                                     <td style="width: 77px">
                                                        <asp:TextBox ID="TextBox2" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                        <asp:TextBox ID="TextBox7" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                     </td>
                                                  </tr>
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:CheckBox ID="CheckBox3" runat="server" Text="3" /></td>
                                                     <td style="width: 77px">
                                                        <asp:TextBox ID="TextBox3" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                        <asp:TextBox ID="TextBox8" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                     </td>
                                                  </tr>
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:CheckBox ID="CheckBox4" runat="server" Text="4" /></td>
                                                     <td style="width: 77px">
                                                        <asp:TextBox ID="TextBox4" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                        <asp:TextBox ID="TextBox9" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                     </td>
                                                  </tr>
                                                  <tr>
                                                     <td style="width: 100px">
                                                        <asp:CheckBox ID="CheckBox5" runat="server" Text="5" /></td>
                                                     <td style="width: 77px">
                                                        <asp:TextBox ID="TextBox5" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                        <asp:TextBox ID="TextBox10" runat="server" Width="30px"></asp:TextBox></td>
                                                     <td style="width: 100px">
                                                     </td>
                                                  </tr>
                                               </table>
                                            </td>
                                            <td style="height: 25px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px">
                                            </td>
                                            <td class="formtext" style="width: 111px">
                                               &nbsp; &nbsp;</td>
                                            <td>
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px">
                                            </td>
                                            <td class="formtext" style="width: 111px">
                                               &nbsp;<asp:Label ID="Label5" runat="server" Text="Fuel Threshold (Percent)"></asp:Label></td>
                                            <td>
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px">
                                            </td>
                                            <td class="formtext" style="width: 111px">
                                               &nbsp;</td>
                                            <td>
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px">
                                            </td>
                                            <td class="formtext" style="width: 111px">
                                               &nbsp;</td>
                                            <td>
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px; height: 25px">
                                            </td>
                                            <td class="formtext" style="width: 111px; height: 25px">
                                               &nbsp;</td>
                                            <td style="height: 25px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px">
                                            </td>
                                            <td class="formtext" style="width: 111px">
                                               &nbsp;</td>
                                            <td>
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="BigFormText" style="width: 9px; height: 22px">
                                            </td>
                                            <td class="formtext" style="width: 111px; height: 22px">
                                               &nbsp; &nbsp; &nbsp;</td>
                                            <td style="height: 22px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="RegularText" style="width: 9px; height: 20px">
                                            </td>
                                            <td class="RegularText" style="width: 111px; height: 20px">
                                               &nbsp;</td>
                                            <td style="height: 22px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="RegularText" height="20" style="width: 9px">
                                            </td>
                                            <td class="RegularText" height="20" style="width: 111px">
                                               &nbsp;</td>
                                            <td style="height: 22px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="RegularText" style="width: 9px; height: 20px">
                                            </td>
                                            <td class="RegularText" style="width: 111px; height: 20px">
                                               &nbsp;</td>
                                            <td style="height: 22px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="RegularText" style="width: 9px; height: 20px">
                                            </td>
                                            <td class="RegularText" style="width: 111px; height: 20px">
                                               &nbsp;</td>
                                            <td style="height: 22px">
                                            </td>
                                         </tr>
                                         <tr height="25">
                                            <td class="RegularText" style="width: 9px">
                                            </td>
                                            <td class="RegularText" style="width: 111px">
                                            </td>
                                            <td align="right">
                                               <%--<input class="combutton" id="cmdClose" onclick="top.close()" type="button"
                                                                value="Close">--%>
                                            </td>
                                         </tr>
                                         <tr>
                                            <td class="RegularText" height="5" style="width: 9px">
                                            </td>
                                            <td class="RegularText" height="5" style="width: 111px">
                                            </td>
                                            <td align="center" height="5">
                                            </td>
                                         </tr>
                                      </table>
                                   </td>
                                </tr>
                             </table>
                          </td>
                       </tr>
                    </table>
                 </td>
              </tr>
           </table>
        </div>
    </form>
</body>
</html>
