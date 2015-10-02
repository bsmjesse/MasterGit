<%@ Page Title="" Language="C#" MasterPageFile="~/Responsive/ResponsiveMaster.master" AutoEventWireup="true" CodeFile="ResponsiveLogin2.aspx.cs" Inherits="SentinelFM.Responsive_ResponsiveLogin2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="parentCon" runat="server" id="parentConDiv">
  <div class="conArea borderBx">
     <div class="col-xs-5 loginPnl borderBx">
    <div class="row">
        <div class="col-xs-12 loginHeader">
            <img src="images/sentinellogo.gif" width="180" height="51" alt=""/></div>
        <div class="col-xs-12 mt10 text-center" >
         
            <asp:TextBox ID="TextBox2" runat="server" class="borderBx" placeholder="Username"></asp:TextBox>
        </div>
        <div class="col-xs-12 text-center">
          
            <asp:TextBox ID="TextBox1" runat="server" TextMode="Password" class="borderBx" placeholder="Password"></asp:TextBox>
        </div>
        <div class="col-xs-12 mt10 text-center">
          
            <asp:Button ID="btnLogin" runat="server" Text="Login" class="btn-primary  borderBx"/>
        </div>
        <div class="col-xs-12 mt10 " >
          <input type="checkbox" name="group" id="remember">
          <label for="remember">Remember Username</label>
        </div>
    </div>
        <div class="row mt10 loginlinks borderBx" id="loginlinksDiv" runat="server" >
        <div class="col-xs-12" >
            
            <asp:LinkButton ID="LinkButton1" runat="server">Forgot your password?</asp:LinkButton>
        </div>
        <%--<div class="col-xs-12 text-right">
            
            <asp:LinkButton ID="LinkButton2" runat="server">Signup for free</asp:LinkButton>
        </div>--%>
        <div class="col-xs-12 mt10">
           
            <asp:LinkButton ID="LinkButton3" runat="server">Login into customer domain</asp:LinkButton>
        </div>
      </div>   


     </div>
    <div class="col-xs-7 slider borderBx">
        
        <ul class="bxslider">
  <li><img src="images/1.png" /></li>
  <li><img src="images/2.png" /></li>
  <li><img src="images/3.png" /></li>
  
</ul>
    </div>
    <div class="clearfix"></div>
  </div>
 </div>
</asp:Content>

