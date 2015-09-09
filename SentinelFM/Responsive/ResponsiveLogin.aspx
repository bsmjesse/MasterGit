<%@ Page Title="" Language="C#" MasterPageFile="~/Responsive/ResponsiveMaster.master" AutoEventWireup="true" CodeFile="ResponsiveLogin.aspx.cs" Inherits="SentinelFM.Responsive_ResponsiveLogin" meta:resourcekey="PageResource1" %>

<%@ MasterType VirtualPath="~/Responsive/ResponsiveMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        var AuthenticationFailed = <%=AuthenticationFailed %>;
    </script>
    <script type="text/javascript" src="js/login.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" >
    <div class="row parent" runat="server" id="parentConDiv" style="padding-top:6%;">
        <div class="col m5 offset-m2" style="width:35% ;">
            <div class="card" >
                <img src="images/card.png" width="100%" alt="" style="width:100%; min-width:100px" />
                <div class="row" style="background: #3D5B43; margin-bottom: 0px !important; font-weight: 900 !important;">

                    <div class="col s7 text-center" style="font-family: Calibri; font-size: xx-large !important; color:#fff;">GET EARLY ACCESS</div>
                    <div class="col s5 text-center">
                        <p><a href="http://www.bsmwireless.com/" class="btn btn-block waves-light green" style="background-color: #575954 !important; margin-bottom:11px;" target="_blank">Sign up</a></p>
                    </div>
                </div>
            </div>
        </div>
        <div class="col m3 offset-s1" style="height:100%;">
            <!-- Start of Form -->
            <div class="card" style="opacity: 0.7;">
                <div>
                     <div class="col s12" style="height:4% ;background-color:lightgreen;">
                       
                    </div>
                    <div class="col s12 loginHeader">
                        <img src="images/sentinel_logo.png" width="180" height="51" alt="" />
                    </div>
                     <div class="col s12" style="height:2% ;">
                         </div>
                    <div class="col s12">
                        <div class="input-field">
                            <i class="material-icons prefix">account_circle</i>

                            <asp:TextBox ID="txtUserName" runat="server" class="" ClientIDMode="Static" meta:resourcekey="txtUserNameResource1"></asp:TextBox>

                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:PHUsername %>" AssociatedControlID="txtUserName"></asp:Label>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Style="display: none" ValidationGroup="grp" ControlToValidate="txtUserName" meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col s12">
                        <div class="input-field">
                            <i class="material-icons prefix">https</i>

                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" class="borderBx " ClientIDMode="Static" meta:resourcekey="txtPasswordResource1"></asp:TextBox>

                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:PHPassword %>" AssociatedControlID="txtPassword"></asp:Label>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Style="display: none" ValidationGroup="grp" ControlToValidate="txtPassword" meta:resourcekey="RequiredFieldValidator1Resource1"></asp:RequiredFieldValidator>
                    </div>
                    <!-- Captcha Panel -->
                    <asp:Panel ID="CaptchaPanel" runat="Server" meta:resourcekey="CaptchaPanelResource1" Visible="False">
                        <div class="row mt10 captcha borderBx" id="Div2" runat="server">


                            <div class="col s12 mt10 text-center">
                                <asp:Image ID="imgCaptcha" runat="server" Height="50px" Width="100%" meta:resourceKey="imgCaptchaResource1" />
                            </div>
                            <div class="col-xs-9 mt10 text-center">
                                <asp:TextBox ID="txtCaptcha" runat="server" Width="150px" meta:resourceKey="txtCaptchaResource1"></asp:TextBox>
                            </div>
                            <div class="col-xs-3 mt10 text-right">
                                <%--<i class="material-icons prefix">loop</i>--%>
                                <asp:Button ID="btnRegen" runat="server" Text="Reload" OnClick="btnRegen_Click" CssClass="btn-primary" meta:resourceKey="btnRegenResource1" />
                                <%--<asp:Button ID="btnRegen" runat="server" Text="Next" OnClick="btnRegen_Click" CssClass="btn-primary" meta:resourceKey="btnRegenResource1" />--%>
                            </div>


                        </div>
                    </asp:Panel>
                    <!-- End of Captcha Panel -->
                    <div class="col s12" style="height:2% ;">
                         </div>
                    <div class="col s12">

                        <asp:CheckBox ID="rememberme" runat="server" />
                        <%--<label for="rememberme">Remember Me</label>--%>
                        <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Remember1Resource1 %>" AssociatedControlID="rememberme"></asp:Label>

                    </div>

                    <div class="col s12">

                        <asp:DropDownList ID="cboDataBaseName" Width="180px" runat="server"
                            Visible="False" meta:resourcekey="cboDataBaseNameResource1">
                            <asp:ListItem Selected="True" Text="SentinelFM" Value="SentinelFM" meta:resourcekey="ListItemResource1"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:HiddenField ID="HdDestinationUrl" runat="server" />
                        <asp:HiddenField ID="SNuserid" runat="server" />
                        <asp:HiddenField ID="SNsecId" runat="server" />
                        <asp:HiddenField ID="HDCulture" runat="server" />
                        <div class="col s12" style="height:2% ;">
                         </div>
                        <asp:Button ID="cmdLogin" runat="server" Text="Login" class="btn btn-block waves-light green" OnClientClick="javascript:prepareLogin();"
                            meta:resourcekey="cmdLoginResource2" ValidationGroup="grp" UseSubmitBehavior="false" style="width:100%"/>
                        <asp:RadioButtonList ID="rblSFM" runat="server" RepeatDirection="Horizontal" meta:resourcekey="rblSFMResource1" Visible="False">
                            <asp:ListItem Text="Standard" Selected="True" Value="0" meta:resourcekey="ListItemResource2"></asp:ListItem>
                            <asp:ListItem Text="Lite [BETA]" Value="1" meta:resourcekey="ListItemResource3"></asp:ListItem>
                        </asp:RadioButtonList>
                        <input id="txtHash" type="hidden" name="txtHash">
                        <input id="txtRnd" type="hidden" value='<%=ViewState["auth_seed"]%>' name="txtRnd">
                    </div>
                    <div class="col s12 error">
                        <asp:Label ID="lblMessage" runat="server" Visible="False" meta:resourcekey="lblMessageResource1" ForeColor="Red"></asp:Label>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="grp" meta:resourcekey="ValidationSummary1Resource1" />
                    </div>

                </div>
              
                <div class="row culturelinks" id="Div1" runat="server">
                    <div class="col s6 center-align" >

                        <asp:LinkButton ID="lnkEnglish" runat="server" CausesValidation="False" OnClick="lnkEnglish_Click"
                            Text="ENGLISH" meta:resourcekey="lnkEnglishResource1"></asp:LinkButton>
                    </div>
                    <div class="col s6 center-align">

                        <asp:LinkButton ID="lnkFrench" runat="server" CausesValidation="False"
                            OnClick="lnkFrench_Click" Text="FRANÇAIS" meta:resourcekey="lnkFrenchResource1"></asp:LinkButton>
                    </div>
                </div>

                <div class="row loginlinks" id="loginlinksDiv" runat="server" style="display: none">
                    <div class="col s12">

                        <asp:LinkButton ID="LinkButton1" runat="server" meta:resourcekey="LinkButton1Resource1" Text="Forgot your password?"></asp:LinkButton>
                    </div>

                    <div class="col s12">

                        <asp:LinkButton ID="LinkButton3" runat="server" meta:resourcekey="LinkButton3Resource1" Text="Login into customer domain"></asp:LinkButton>
                    </div>
                </div>
                <div class="row" style="margin-bottom:0px;">
                    <div class="s12 login_footer" >
                        CUSTOMER SUPPORT<br />
                        TOLL FREE DIRECT LINE: 1-866-567-098
                        EMAIL: CUSTOMERCARE@BSMWIRELESS.COM
                    </div>
                    </div>
            </div>
        <!-- End of Form -->
            <div class="col s7 hide">

                <ul class="bxslider">
                    <li>
                        <img src="images/1.png" /><asp:Button ID="btnBanner1" runat="server" Text="Learn More" class="learnMore  borderBx" OnClick="btnBanner1_Click" CausesValidation="false" meta:resourcekey="LearnMoreBtnResource1" /></li>
                    <li>
                        <img src="images/2.png" /><asp:Button ID="btnBanner2" runat="server" Text="Learn More" class="learnMore  borderBx" meta:resourcekey="LearnMoreBtnResource1" OnClick="btnBanner2_Click" /></li>
                    <li>
                        <img src="images/3.png" /><asp:Button ID="btnBanner3" runat="server" Text="Learn More" class="learnMore  borderBx" meta:resourcekey="LearnMoreBtnResource1" OnClick="btnBanner3_Click" /></li>

                </ul>

            </div>
            <div class="clearfix"></div>
        </div>
    </div>
</asp:Content>

