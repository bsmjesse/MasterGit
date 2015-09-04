<%@ Application Inherits="VLF.ASI.Global" Language="C#" %>

<script runat="server">

      
   
   
    void Application_Start(object sender, EventArgs e) 
    {
   
        
        ////Failed Logins Logger
        //System.Data.DataTable dtLoginFailedList = new System.Data.DataTable();

        //System.Data.DataColumn colUserName = new System.Data.DataColumn("UserName", Type.GetType("System.String"));
        //dtLoginFailedList.Columns.Add(colUserName);
        //System.Data.DataColumn colIPAddress = new System.Data.DataColumn("IPAddress", Type.GetType("System.String"));
        //dtLoginFailedList.Columns.Add(colIPAddress);
        //System.Data.DataColumn colNumTrials = new System.Data.DataColumn("NumTrials", Type.GetType("System.Int16"));
        //dtLoginFailedList.Columns.Add(colNumTrials);
        //System.Data.DataColumn colCycle = new System.Data.DataColumn("Cycle", Type.GetType("System.Int16"));
        //dtLoginFailedList.Columns.Add(colCycle);
        //System.Data.DataColumn colStatus = new System.Data.DataColumn("Status", Type.GetType("System.String"));
        //dtLoginFailedList.Columns.Add(colStatus);
        //System.Data.DataColumn colLoginDate = new System.Data.DataColumn("LoginDate", Type.GetType("System.DateTime"));
        //dtLoginFailedList.Columns.Add(colLoginDate);

        ////Application["ConnectionString"] = @ConfigurationSettings.AppSettings["DBConnectionString"];
        Application["ConnectionString"]=System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        //Application.Add("dtLoginFailedList", dtLoginFailedList);
        //System.Diagnostics.Trace.WriteLine(VLF.CLS.Util.TraceFormat("ASI started"));
       
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

       //Exception ex = Context.Error.GetBaseException();
       Exception ex = Server.GetLastError().GetBaseException();

       string Excp = "ASI " +
           "MESSAGE: " + ex.Message +
           "\nSOURCE: " + ex.Source +
           "\nFORM: " + Request.Form.ToString() +
           "\nQUERYSTRING: " + Request.QueryString.ToString() +
           "\nTARGETSITE: " + ex.TargetSite +
           "\nSTACKTRACE: " + ex.StackTrace;


          System.Diagnostics.Trace.WriteLine(VLF.CLS.Util.TraceFormat("Unhandled error:" + Excp));

       
    }

    void Session_Start(object sender, EventArgs e) 
    {
       

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

       
    }
       
</script>
