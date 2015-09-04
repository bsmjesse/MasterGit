<%@ Application Language="C#" %>

<script runat="server">

   
  
   


    void Application_Start(object sender, EventArgs e) 
    {
      //SentinelFM.AppConfig.GetInstance();  
       System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info , "Application_Start"));

       System.Data.DataTable dtUserCounter = new System.Data.DataTable();
       System.Data.DataColumn colUserId = new System.Data.DataColumn("UserId", Type.GetType("System.Int32"));
       dtUserCounter.Columns.Add(colUserId);
       System.Data.DataColumn colPage = new System.Data.DataColumn("Page", Type.GetType("System.String"));
       dtUserCounter.Columns.Add(colPage);
       System.Data.DataColumn colActionDateTime = new System.Data.DataColumn("ActionDateTime", Type.GetType("System.DateTime"));
       dtUserCounter.Columns.Add(colActionDateTime);
       Application.Lock(); 
       Application.Add("dtUserCounter", dtUserCounter);   
       Application.UnLock() ;    

        
    }
    
    void Application_End(object sender, EventArgs e) 
    {  
        System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Application_End"));    
    }
        
    void Application_Error(object sender, EventArgs e)   
    {
    
    
        int UserId = 0;
        string frmName = "";   

        //Session Check
        try
        {
            SentinelFM.SentinelFMSession snMain = (SentinelFM.SentinelFMSession)Session["SentinelFMSession"];
            if (snMain != null && snMain.UserID != null)
                UserId = snMain.UserID;
        }
        catch  
        {
        }

        //Form Name
        try
        {
            frmName = Request.Form.ToString();
        }
        catch
        {
        }

        //Get Last Error if exists
        try
        {
           
            
            if (Server.GetLastError() != null)
            {
                Exception ex = Server.GetLastError().GetBaseException();

                string Excp = "SentinelFM " +
                    "MESSAGE: " + ex.Message +
                    "\nSOURCE: " + ex.Source +
                    "\nFORM: " + Request.Form.ToString()   +
                    "\nQUERYSTRING: " + Request.QueryString.ToString() +
                    "\nTARGETSITE: " + ex.TargetSite +
                    "\nSTACKTRACE: " + ex.StackTrace;
                
                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UNHANDLED ERROR: Application_Error - UserId:" + UserId.ToString() + ", Session ID:"+ Session.SessionID +" , Form:" + frmName + " , Error :" + Excp));
                HttpContext.Current.Server.ClearError();
            }
        }
        catch
        {
            Exception ex = Server.GetLastError().GetBaseException();

            string Excp = "SentinelFM " +
                "MESSAGE: " + ex.Message +
                "\nSOURCE: " + ex.Source +
                "\nFORM: " + Request.Form.ToString() +
                "\nQUERYSTRING: " + Request.QueryString.ToString() +
                "\nTARGETSITE: " + ex.TargetSite +
                "\nSTACKTRACE: " + ex.StackTrace;

            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UNHANDLED ERROR: Application_Error - Form:" + Excp));
           /// System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "UNHANDLED ERROR: Application_Error - Message:" + Server.GetLastError().GetBaseException().Message+", StackTrace:" + Server.GetLastError().GetBaseException().StackTrace));
        }

    }

    void Session_Start(object sender, EventArgs e) 
    {
        SentinelFM.SentinelFMSession sn = new SentinelFM.SentinelFMSession();
        Session.Add("SentinelFMSession", sn);

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

        try
        {

            if (Server.GetLastError() != null)
            {
                Exception ex = Server.GetLastError().GetBaseException();

                string Excp = "SentinelFM " +
                    "MESSAGE: " + ex.Message +
                    "\nSOURCE: " + ex.Source +
                    "\nFORM: " + Request.Form.ToString() +
                    "\nQUERYSTRING: " + Request.QueryString.ToString() +
                    "\nTARGETSITE: " + ex.TargetSite +
                    "\nSTACKTRACE: " + ex.StackTrace;

                System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "End Session:" + Session.SessionID.ToString() + "-" + Session.GetHashCode() + " , Error :" + Excp));
                HttpContext.Current.Server.ClearError();
            }
            //else
            //    System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "End Session:" + Session.SessionID.ToString() + "-" + Session.GetHashCode()));


            
        }
        catch  (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(SentinelFM.AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "End Session"));    
        }
    }       
</script>
