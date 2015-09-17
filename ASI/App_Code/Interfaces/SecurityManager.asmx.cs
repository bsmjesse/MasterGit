using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Security.Cryptography;
using VLF.ERRSecurity ;
using VLF.DAS.Logic;
using System.Configuration; 


namespace VLF.ASI.Interfaces
{



	/// <summary>
	/// Summary description for SecurityManager.
	/// </summary>
	/// 
	[WebService(Namespace="http://www.sentinelfm.com")]
	public class SecurityManager : System.Web.Services.WebService
	{
		public SecurityManager()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

      #region refactored functions 
      /// <summary>
      ///      by replacing the log calls we can add a UDP sender for logs
      ///      or dynamic filtering 
      /// </summary>
      /// <param name="strFormat"></param>
      /// <param name="objects"></param>
      private void Log(string strFormat, params object[] objects)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                 CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                   string.Format(strFormat, objects)));
         }
         catch (Exception exc)
         {

         }
      }


      /// <summary>
      ///      the exception should be saved in a separate file or in the Event log of the computer
      /// </summary>
      /// <param name="strFormat"></param>
      /// <param name="objects"></param>
      private void LogException(string strFormat, params object[] objects)
      {
         try
         {
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                 CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                   string.Format(strFormat, objects)));
         }
         catch (Exception exc)
         {

         }
      }

      private int AddUserLogin(string userName, string userIp, ref int userId)
      {
         // Retrieve user id					
         if (userId == VLF.CLS.Def.Const.unassignedIntValue)
         {
            // unsucessful login
            Log("<< AddUserLogin : Unsuccessful (userName={0}, userIp={1})", userName, userIp);
            return (int)InterfaceError.AuthenticationFailed;
         }
         else
         {
            Log(">> AddUserLogin (userName={0}, userIp={1})", userName, userIp);

            // store user info into vlfUserLogin table
            if (string.IsNullOrEmpty(userIp))
               userIp = "N/A";


           using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               // it return an int, but is not used
               dbUser.AddUserLogin(userId, DateTime.Now.AddHours(-Convert.ToInt16(AppConfig.GetInstance().ServerTimeZone)), userIp);
            }

         }

         return (int)InterfaceError.NoError;
      }

      private int AddUserLoginExtended(string UserName, string UserIP, int UserId, int LoginUseId)
      {
          int LoginID = 0;
          				
          if (UserId == VLF.CLS.Def.Const.unassignedIntValue)
          {
              // unsucessful login
              Log("<< AddUserLoginExtended : Unsuccessful (UserName={0}, UserIP={1}, LoginUserId={2})", UserName, UserIP, LoginUseId.ToString());
              return (int)InterfaceError.AuthenticationFailed;
          }
          else
          {
              Log(">> AddUserLoginExtended (UserName={0}, UserIP={1}, LoginUserId={2})", UserName, UserIP, LoginUseId.ToString());

              using (User dbUser = new User(LoginManager.GetConnnectionString(UserId)))
              {
                  // it returns inserted record id
                  LoginID = dbUser.AddUserLoginExtended(UserId, UserIP, LoginUseId);
              }

          }

          return LoginID;
      }

      #endregion refactored functions

      [WebMethod]
		public int Login( string userName, string password,string userIp, ref int userId, ref string SID )
		{
			try
			{
            Log(">> Login (username={0}, userIp={1})", userName, userIp);

				userId = LoginManager.GetInstance().LoginUser( userName, password, ref SID ) ;
            
            return AddUserLogin(userName, userIp, ref userId);
			}
			catch( Exception Ex )
			{
            LogException("<< Login : username={0} EXC={1}", userName, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}




        [WebMethod]
      public int LoginUserMD5(string userName, string password,  ref string passKey)
      {
          //string dbConnection = dbName == "" ? System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString : System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.Replace("SentinelFM", dbName);
          string dbConnection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
          int userId =LoginManager.GetInstance().GetUserIDMD5(userName, password, dbConnection);
          Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("LoginManager.Login( {0}, <pwd> ) UserID = {1}", userName, userId)));
          passKey = SecurityKeyManager.GetInstance().CreatePassKey(userId);
          return userId;
      }





		[WebMethod]
		public int LoginMD5(string key, string userName, string password,string userIp, ref int userId, ref string SID )
		{
         Log(">> LoginMD5 : username={0} key={1} ip={2}", userName, key, userIp);

			string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
            string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
			try
			{

                using (User user = new User(connection))
				{
					// Retrieves user password from DB
					DataSet dsInfo = user.GetUserInfoByUserName(userName);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
						srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
				}
				
				// Generate Hash Number with Existing Password
				MD5CryptoServiceProvider oMD5=new MD5CryptoServiceProvider();
				byte []bServerPassword=System.Text.Encoding.ASCII.GetBytes(srvPassword + key );    
				oMD5.ComputeHash(bServerPassword) ;
				bServerPassword=oMD5.Hash; 
		
				//Get Entered Hashed Password
				byte[] bUserPassword = VLF.CLS.Util.StringArrayAsHexDumpToChar(password,password.Length/2);
				if(bServerPassword.Length == bUserPassword.Length)
				{
					for(int i=0;i<bUserPassword.Length&&i<bServerPassword.Length;i++)
					{
						if(bUserPassword[i] != bServerPassword[i])
						{
							// unsucessful login
							Log("<< LoginMD5 : unmatched password (userName={0}, userIp={1})", userName,userIp);									
							return (int)InterfaceError.AuthenticationFailed;
						}
					}
				}


                userId = LoginManager.GetInstance().LoginUserMD5(userName, srvPassword, "", ref SID);
                

            return AddUserLogin(userName, userIp, ref userId);
			}
			catch(Exception Ex )
			{
            LogException("<< LoginMD5 : username={0} EXC={1}", userName, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}



        [WebMethod]
        public int LoginMD5Extended(string key, string userName, string password,
                                    string userIp, ref int userId, ref string SID, ref int SuperOrganizationId, ref string Email, ref bool isDisclaimer)
        {
            Log(">> LoginMD5Extended (userName={0}, userIp={1})", userName, userIp);

            string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
            string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            try
            {
                using (User user = new User(connection))
                {
                    //user = new User(Application["ConnectionString"].ToString());                    
                    //Retrieves user password from DB
                    DataSet dsInfo = user.GetUserInfoByUserName(userName);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    {
                        srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
                        SuperOrganizationId = Convert.ToInt32(dsInfo.Tables[0].Rows[0]["SuperOrganizationId"].ToString().TrimEnd());
                        Email = Convert.ToString(dsInfo.Tables[0].Rows[0]["Email"]).TrimEnd();
                        isDisclaimer = Convert.ToBoolean(dsInfo.Tables[0].Rows[0]["isDisclaimer"]);
                    }
                    else
                    {
                        return (int)InterfaceError.AuthenticationFailed;
                    }
                }

                // Generate Hash Number with Existing Password
                MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
                byte[] bServerPassword = null;
                if (key != "")
                {
                    bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword + key);

                    oMD5.ComputeHash(bServerPassword);
                    bServerPassword = oMD5.Hash;

                    //Get Entered Hashed Password
                    byte[] bUserPassword = VLF.CLS.Util.StringArrayAsHexDumpToChar(password, password.Length / 2);
                    if (bServerPassword.Length == bUserPassword.Length)
                    {
                        for (int i = 0; i < bUserPassword.Length && i < bServerPassword.Length; i++)
                        {
                            if (bUserPassword[i] != bServerPassword[i])
                            {
                                // unsucessful login
                                Log("<< LoginMD5Extended : unmatched password (userName={0},userIp={1})", userName, userIp);
                                return (int)InterfaceError.AuthenticationFailed;
                            }
                        }
                    }
                }
                else
                {
                    if (password != srvPassword)
                    {
                        // unscessful login
                        Log("<< LoginMD5Extended : unmatched password (II) (userName={0},userIp={1})", userName, userIp);
                        return (int)InterfaceError.AuthenticationFailed;
                    }
                }

                userId = LoginManager.GetInstance().LoginUserMD5(userName, srvPassword, "", ref SID);

                return AddUserLogin(userName, userIp, ref userId);
            }
            catch (Exception Ex)
            {
                LogException("<< LoginMD5Extended : username={0} EXC={1}", userName, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int LoginMD5ExtendedSuperUser(string userName, string password, string userIp, int LoginUserId,
                                     ref int userId, ref string SID, ref int SuperOrganizationId, ref string Email, ref bool isDisclaimer)
        {
            Log(">> LoginMD5Extended (userName={0}, userIp={1})", userName, userIp);

            string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
            string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            try
            {
                using (User user = new User(connection))
                {                  
                    //Retrieves user password from DB
                    DataSet dsInfo = user.GetUserInfoByUserName(userName);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    {
                        srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
                        SuperOrganizationId = Convert.ToInt32(dsInfo.Tables[0].Rows[0]["SuperOrganizationId"].ToString().TrimEnd());
                        Email = Convert.ToString(dsInfo.Tables[0].Rows[0]["Email"]).TrimEnd();
                        isDisclaimer = Convert.ToBoolean(dsInfo.Tables[0].Rows[0]["isDisclaimer"]);
                    }
                    else
                    {
                        return (int)InterfaceError.AuthenticationFailed;
                    }
                }
                
                if (password != srvPassword)
                {
                    // unscessful login
                    Log("<< LoginMD5ExtendedSwitchUser : unmatched password (II) (userName={0}, userIp={1})", userName, userIp);
                    return (int)InterfaceError.AuthenticationFailed;
                }
         
                userId = LoginManager.GetInstance().LoginUserMD5(userName, srvPassword, "", ref SID);

                return AddUserLoginExtended(userName, userIp, userId, LoginUserId);
            }
            catch (Exception Ex)
            {
                LogException("<< LoginMD5ExtendedSwitchUser : username={0} EXC={1}", userName, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod]
        public int LoginMD5ByDBName(string key, string userName, string password,
                                    string userIp, string dbName, ref int userId, ref string SID, ref int SuperOrganizationId)
        {
            Log(">> LoginMD5ByDBName (userName={0}, userIp={1},dbName={2})", userName, userIp, dbName);

            string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
            try
            {

                string dbConnection = dbName == "" ? System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString : System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.Replace("SentinelFM", dbName);

                using (User user = new User(dbConnection))
                {
                    // user = new User(Application["ConnectionString"].ToString());                    
                    // Retrieves user password from DB
                    DataSet dsInfo = user.GetUserInfoByUserName(userName);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    {
                        srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
                        try
                        {
                            SuperOrganizationId = Convert.ToInt32(dsInfo.Tables[0].Rows[0]["SuperOrganizationId"].ToString().TrimEnd());
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        return (int)InterfaceError.AuthenticationFailed;
                    }
                }

                // Generate Hash Number with Existing Password
                MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
                byte[] bServerPassword = null;
                if (key != "")
                {
                    bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword + key);

                    oMD5.ComputeHash(bServerPassword);
                    bServerPassword = oMD5.Hash;

                    //Get Entered Hashed Password
                    byte[] bUserPassword = VLF.CLS.Util.StringArrayAsHexDumpToChar(password, password.Length / 2);
                    if (bServerPassword.Length == bUserPassword.Length)
                    {
                        for (int i = 0; i < bUserPassword.Length && i < bServerPassword.Length; i++)
                        {
                            if (bUserPassword[i] != bServerPassword[i])
                            {
                                // unsucessful login
                                Log("<< LoginMD5Extended : unmatched password (userName={0},userIp={1})", userName, userIp);
                                return (int)InterfaceError.AuthenticationFailed;
                            }
                        }
                    }
                }
                else
                {
                    if (password != srvPassword)
                    {
                        // unscessful login
                        Log("<< LoginMD5ByDBName : unmatched password (II) (userName={0},userIp={1})", userName, userIp);
                        return (int)InterfaceError.AuthenticationFailed;
                    }
                }


                userId = LoginManager.GetInstance().LoginUserMD5(userName, srvPassword, dbName, ref SID);
                return AddUserLogin(userName, userIp, ref userId);

            }
            catch (Exception Ex)
            {
                LogException("<< LoginMD5ByDBName : username={0} EXC={1}", userName, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



      [WebMethod]
      public int LoginSHA(string userName, string password, string userIp, ref int userId, ref string SID)
      {
         Log(">> LoginSHA (userName={0}, userIp={1})", userName, userIp);

         string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
         try
         {            
            using(User user = new User(Application["ConnectionString"].ToString()))
            {
               // Retrieves user password from DB
               DataSet dsInfo = user.GetUserInfoByUserName(userName);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  srvPassword = dsInfo.Tables[0].Rows[0]["Password"].ToString().TrimEnd();
            }

            // Generate Hash Number with Existing Password
            int key = Convert.ToInt32( (DateTime.Now.ToUniversalTime().Minute + DateTime.Now.ToUniversalTime().Minute));

            if (SHAHash(srvPassword, key.ToString()) != password)
            {
               if (SHAHash(srvPassword, (key - 1).ToString()) != password)
               {
                  // unsucessful login
                  Log("<< LoginSHA : unmatched password (userName={0}, userIp={1})", userName, userIp);
                  return (int)InterfaceError.AuthenticationFailed;
               }
            }
            
            // Retrieve user id					
            userId = LoginManager.GetInstance().LoginUser(userName, srvPassword, ref SID);
            
            return AddUserLogin(userName, userIp, ref userId);

         }
         catch (Exception Ex)
         {
            LogException("<< LoginSHA : username={0} EXC={1}", userName, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

 

		[WebMethod]
		public int Logout( int userId, string SID )
		{
			try
			{
				Log(">> Logout (uId={0})", userId );

				LoginManager.GetInstance().SecurityCheck( userId, SID ) ;
				LoginManager.GetInstance().LogoutUser( userId ) ;
				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< Logout: uId={0}, EXC={1}", userId, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
		}

		[WebMethod]
		public int Relogin( string userName, string password,string userIp,ref string SID )
		{
			try
			{
				Log(">> Relogin (userName={0}, userIp={1})", userName, userIp);

				LoginManager.GetInstance().ReloginUser( userName, password, ref SID ) ;
				// TODO: store user info (UserId,UserName,Password,UserUP) into vlfUserLogin table
				return (int)InterfaceError.NoError ;
			}
			catch( Exception Ex )
			{
            LogException("<< Relogin: username={0}, EXC={1}", userName, Ex.Message);
				return (int)ASIErrorCheck.CheckError( Ex ) ;
			}
      }



     [WebMethod]
     public int ReloginMD5(string key, string userName, string password, string userIp, ref string SID)
     {
         try
         {
              Log(">> ReloginMD5 (userName={0}, userIp={1})", userName, userIp);

              string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;
              string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

              using (User user = new User(connection))
              {
                  // Retrieves user password from DB
                  DataSet dsInfo = user.GetUserInfoByUserName(userName);
                  if (ASIErrorCheck.IsAnyRecord(dsInfo))
                      srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
              }

              // Generate Hash Number with Existing Password
              MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
              byte[] bServerPassword=null;
              if (key != "")
              {
                  bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword + key);
                  oMD5.ComputeHash(bServerPassword);
                  bServerPassword = oMD5.Hash;
              }
              else
              {
                  bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword);
              }

             
              //Get Entered Hashed Password
              byte[] bUserPassword=null;
              try
              {
                  bUserPassword = VLF.CLS.Util.StringArrayAsHexDumpToChar(password, password.Length / 2);
              }
              catch (Exception exc)
              {
                  LogException("<< ReloginMD5 : unmatched password (I) ( userName={0}, pwd={1} userIp={2} )", userName,password, userIp);
                  return (int)InterfaceError.AuthenticationFailed;
              }

              if (bServerPassword.Length == bUserPassword.Length)
              {
                  for (int i = 0; i < bUserPassword.Length && i < bServerPassword.Length; i++)
                  {
                      if (bUserPassword[i] != bServerPassword[i])
                      {
                          // unsucessful login
                          LogException("<< ReloginMD5 : unmatched password (II) ( userName={0}, userIp={1} )", userName, userIp);
                          return (int)InterfaceError.AuthenticationFailed;
                      }
                  }
              }


              LoginManager.GetInstance().ReloginUserMD5(userName, srvPassword,connection, ref SID);
              // TODO: store user info (UserId,UserName,Password,UserUP) into vlfUserLogin table
              return (int)InterfaceError.NoError;
             
         }
         catch (Exception Ex)
         {
            LogException("<< ReloginMD5 : userName={0} EXC={1}", userName, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
     }

        [WebMethod]
        public int ReloginMD5ByDBName(int userId,string key, string userName, string password, string userIp, ref string SID)
        {
            try
            {
                Log(">> ReloginMD5 (userName={0}, userIp={1})", userName, userIp);

                string srvPassword = VLF.CLS.Def.Const.unassignedStrValue;

                using (User user = new User(LoginManager.GetConnnectionString(userId)))
                {
                    // Retrieves user password from DB
                    DataSet dsInfo = user.GetUserInfoByUserName(userName);
                    if (ASIErrorCheck.IsAnyRecord(dsInfo))
                        srvPassword = dsInfo.Tables[0].Rows[0]["HashPassword"].ToString().TrimEnd();
                }

                // Generate Hash Number with Existing Password
                MD5CryptoServiceProvider oMD5 = new MD5CryptoServiceProvider();
                byte[] bServerPassword = null;
                if (key != "")
                {
                    bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword + key);
                    oMD5.ComputeHash(bServerPassword);
                    bServerPassword = oMD5.Hash;
                }
                else
                {
                    bServerPassword = System.Text.Encoding.ASCII.GetBytes(srvPassword);
                }


                //Get Entered Hashed Password
                byte[] bUserPassword = null;
                try
                {
                    bUserPassword = VLF.CLS.Util.StringArrayAsHexDumpToChar(password, password.Length / 2);
                }
                catch (Exception exc)
                {
                    LogException("<< ReloginMD5 : unmatched password (I) ( userName={0}, pwd={1} userIp={2} )", userName, password, userIp);
                    return (int)InterfaceError.AuthenticationFailed;
                }

                if (bServerPassword.Length == bUserPassword.Length)
                {
                    for (int i = 0; i < bUserPassword.Length && i < bServerPassword.Length; i++)
                    {
                        if (bUserPassword[i] != bServerPassword[i])
                        {
                            // unsucessful login
                            LogException("<< ReloginMD5 : unmatched password (II) ( userName={0}, userIp={1} )", userName, userIp);
                            return (int)InterfaceError.AuthenticationFailed;
                        }
                    }
                }


                LoginManager.GetInstance().ReloginUserMD5(userName, srvPassword,LoginManager.GetConnnectionString(userId), ref SID);
                // TODO: store user info (UserId,UserName,Password,UserUP) into vlfUserLogin table
                return (int)InterfaceError.NoError;

            }
            catch (Exception Ex)
            {
                LogException("<< ReloginMD5 : userName={0} EXC={1}", userName, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        
        //[WebMethod]
        //public int SetConnnectionString(string userName, string SID, string dbConnectionString)
        //{
        //    try
        //    {
        //        Log(">> SetConnnectionString (userName={0}, dbConnectionString={1})", userName, dbConnectionString);
        //        // Authenticate 
        //        //LoginManager.GetInstance().SecurityCheck(userId, SID);

        //        LoginManager.SetConnnectionString(userName, dbConnectionString);

        //        return (int)InterfaceError.NoError;

        //    }
        //    catch (Exception Ex)
        //    {
        //        LogException("<< SetConnnectionString : userName={0} EXC={1}", userName, Ex.Message);
        //        return (int)ASIErrorCheck.CheckError(Ex);
        //    }
        //}


         

      private string SHAHash(string psw, string key)
      {
         SHA256 oSHA = new SHA256Managed();
         byte[] bServerPassword = System.Text.Encoding.ASCII.GetBytes(psw + key);
         oSHA.ComputeHash(bServerPassword);
         bServerPassword = oSHA.Hash;
         return Convert.ToBase64String(bServerPassword);

      }

      public string base64Encode(string data)
      {
         try
         {
            byte[] encData_byte = new byte[data.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
         }
         catch (Exception e)
         {
            throw new Exception("Error in base64Encode" + e.Message);
         }
      }


      public string base64Decode(string data)
      {
         try
         {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(data);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
         }
         catch (Exception e)
         {
            throw new Exception("Error in base64Decode" + e.Message);
         }
      }

        #region (OBSOLETE) Failed Logins Tracking
      /*
        [WebMethod]
        public  void AddFailedEntry(string UserName, string IPAddress)
        {
            DataTable dtLoginFailedList = new DataTable(); 
            dtLoginFailedList = (DataTable)Application["dtLoginFailedList"];
 
            
            DataRow[] drCollections=null;
            string strSQL = "IPAddress='" + IPAddress.Trim() + "' or UserName='" + UserName.Trim() + "'";
            if (dtLoginFailedList.Rows.Count>0)
                drCollections = dtLoginFailedList.Select(strSQL);
         
            if ((drCollections !=null) && (drCollections.Length >0))
              {
                  foreach (DataRow dr in dtLoginFailedList.Rows)
                  {
                      if (dr["IPAddress"].ToString() == IPAddress.Trim() || 
                          dr["UserName"].ToString() == UserName.Trim())
                      {
                          dr["NumTrials"] = Convert.ToInt16(dr["NumTrials"]) + 1;

                          TimeSpan currDuration = new TimeSpan(System.DateTime.Now.Ticks - Convert.ToDateTime(dr["LoginDate"]).Ticks);
                          if ((currDuration.TotalSeconds < Convert.ToInt16(@ConfigurationSettings.AppSettings["TrialsDurationInSeconds"]) && Convert.ToInt16(dr["NumTrials"]) == Convert.ToInt16(@ConfigurationSettings.AppSettings["NumberTrials"])))
                          {


                              Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Account Locked for IP:"+IPAddress+", UserName:"+UserName));

                              //dr["Cycle"] = Convert.ToInt16(dr["Cycle"]) + 1;
                              //if (Convert.ToInt16(dr["Cycle"]) == 2)
                              //{
                                      try
                                      {
                                          MailLib.EMailMessageAuth eMail = new MailLib.EMailMessageAuth(
                                            ConfigurationSettings.AppSettings["UserName"],
                                            ConfigurationSettings.AppSettings["Password"],
                                            ConfigurationSettings.AppSettings["SMTPServer"]);

                                          // eMail.SendMail("mvaksman@bsmwireless.com;gbardas@bsmwireless.com;prodsupport@bsmwireless.com", "SentinelFM locked for IP:","IP:"+IPAddress+", UserName:"+UserName);
                                          eMail.SendMail("mvaksman@bsmwireless.com;gbardas@bsmwireless.com;", "SentinelFM locked for IP:", "IP:"+IPAddress+", UserName:"+UserName);
                                          
                                          Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Email has been sent to '" + "prodsupport@bsmwireless.com" + "' successfully."));
                                      }
                                      catch (Exception ex)
                                      {
                                          Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Error in sending Email 'Account Locked'."));
                                      }

                           //   }

                              dr["Status"] = "Locked";
                          }
                          else if (currDuration.TotalSeconds > Convert.ToInt16(@ConfigurationSettings.AppSettings["TrialsDurationInSeconds"]))
                          {
                              dr["NumTrials"] = 1;
                              dr["LoginDate"] = System.DateTime.Now.ToString();
                          }
                          break;
                      }
                  }
               
            }
            else
            {
                DataRow drNew = dtLoginFailedList.NewRow();
                drNew["IPAddress"] = IPAddress.Trim();
                drNew["NumTrials"] = 1;
                drNew["Cycle"] = 0;
                drNew["Status"] = "Open";
                drNew["LoginDate"] = System.DateTime.Now.ToString() ;
                drNew["UserName"] = UserName.Trim();
                dtLoginFailedList.Rows.Add(drNew);
                 
            }

            
            Application.Lock();
            Application["dtLoginFailedList"] = dtLoginFailedList;
            Application.UnLock();
            
        }


        [WebMethod]
        public void GetFailedLogins(ref string xml)
        {
           DataTable dt=new DataTable();

           
           dt = (DataTable)Application["dtLoginFailedList"];
           if (dt != null && dt.Rows.Count > 0)
           {
               DataSet ds = new DataSet();
               ds.Tables.Add(dt)   ;
               
               xml = ds.GetXml();
               ds.Tables.Clear();  
               dt = null;
               ds=null; 
           }
           else
           {
               xml = "";
           }
        }

        [WebMethod]
        public void GetIpStatus(string IPAddress,ref bool IsLocked)
        {
            IsLocked = false; 
            DataTable dtLoginFailedList = new DataTable(); 
            dtLoginFailedList = (DataTable)Application["dtLoginFailedList"];
 
            DataRow[] drCollections=null;
            string strSQL = "IPAddress='" + IPAddress+"'";
            if (dtLoginFailedList.Rows.Count>0)
                drCollections = dtLoginFailedList.Select(strSQL);

            if ((drCollections != null) && (drCollections.Length > 0) )
            {    
                DataRow dRow= drCollections[0];
                if (dRow["Status"] == "Locked")
                {
                    TimeSpan currDuration = new TimeSpan(System.DateTime.Now.Ticks - Convert.ToDateTime(dRow["LoginDate"]).Ticks);
                    if (currDuration.TotalMinutes < Convert.ToInt16(@ConfigurationSettings.AppSettings["LockedAccessInMinutes"]))
                         IsLocked = true;
                     else
                     {
                         //foreach (DataRow dr in dtLoginFailedList.Rows)
                         //{
                         //    if (dr["IPAddress"].ToString() == IPAddress)
                         //    {
                         //           dr["LoginDate"] = System.DateTime.Now.ToString();
                         //           dr["NumTrials"] = 1;
                         //           dr["Status"] = "Open";
                         //           IsLocked = false;

                                    
                         //           Application.Lock();
                         //           Application["dtLoginFailedList"] = dtLoginFailedList;
                         //           Application.UnLock();
                         //           break;
                         //    }  
                         //}


                         dRow["LoginDate"] = System.DateTime.Now.ToString();
                         dRow["NumTrials"] = 1;
                         dRow["Status"] = "Open";
                         IsLocked = false;



                         Application.Lock();
                         Application["dtLoginFailedList"] = dtLoginFailedList;
                         Application.UnLock();
                         
                     }
                }           
            }
        }


        [WebMethod]
        public void GetIpUserNameStatus(string IPAddress,string UserName, ref bool IsLocked)
        {
            IsLocked = false;
            DataTable dtLoginFailedList = new DataTable();
            dtLoginFailedList = (DataTable)Application["dtLoginFailedList"];

            DataRow[] drCollections = null;
            string strSQL = "IPAddress='" + IPAddress + "' or UserName='" + UserName+"'";
            if (dtLoginFailedList.Rows.Count > 0)
                drCollections = dtLoginFailedList.Select(strSQL);

            if ((drCollections != null) && (drCollections.Length > 0))
            {
                DataRow dRow = drCollections[0];
                if (dRow["Status"] == "Locked")
                {
                    TimeSpan currDuration = new TimeSpan(System.DateTime.Now.Ticks - Convert.ToDateTime(dRow["LoginDate"]).Ticks);
                    if (currDuration.TotalMinutes < Convert.ToInt16(@ConfigurationSettings.AppSettings["LockedAccessInMinutes"]))
                        IsLocked = true;
                    else
                    {
                        //foreach (DataRow dr in dtLoginFailedList.Rows)
                        //{
                        //    if (dr["IPAddress"].ToString() == IPAddress)
                        //    {
                        //           dr["LoginDate"] = System.DateTime.Now.ToString();
                        //           dr["NumTrials"] = 1;
                        //           dr["Status"] = "Open";
                        //           IsLocked = false;


                        //           Application.Lock();
                        //           Application["dtLoginFailedList"] = dtLoginFailedList;
                        //           Application.UnLock();
                        //           break;
                        //    }  
                        //}


                        dRow["LoginDate"] = System.DateTime.Now.ToString();
                        dRow["NumTrials"] = 1;
                        dRow["Status"] = "Open";
                        IsLocked = false;



                        Application.Lock();
                        Application["dtLoginFailedList"] = dtLoginFailedList;
                        Application.UnLock();

                    }
                }
            }
        }

        [WebMethod]
        public void ClearFailedLoginsByIP(string IPAddress)
        {
            DataTable dtLoginFailedList = new DataTable();
            dtLoginFailedList = (DataTable)Application["dtLoginFailedList"];
            DataRow[] drCollections=null;
            string strSQL = "IPAddress='" + IPAddress + "'";
            drCollections = dtLoginFailedList.Select(strSQL); 

            if (dtLoginFailedList.Rows.Count>0)
                drCollections = dtLoginFailedList.Select(strSQL);

            if ((drCollections != null) && (drCollections.Length > 0))
            {
                dtLoginFailedList.Rows.Remove(drCollections[0]);

                Application.Lock();
                Application["dtLoginFailedList"] = dtLoginFailedList;
                Application.UnLock();
            }
        }


        [WebMethod]
        public void ClearFailedLoginsByIPUserName(string IPAddress, string UserName)
        {
            DataTable dtLoginFailedList = new DataTable();
            dtLoginFailedList = (DataTable)Application["dtLoginFailedList"];
            DataRow[] drCollections = null;
            string strSQL = "IPAddress='" + IPAddress + "' and UserName='" + UserName + "'";
            drCollections = dtLoginFailedList.Select(strSQL);

            if (dtLoginFailedList.Rows.Count > 0)
                drCollections = dtLoginFailedList.Select(strSQL);

            if ((drCollections != null) && (drCollections.Length > 0))
            {
                dtLoginFailedList.Rows.Remove(drCollections[0]);

                Application.Lock();
                Application["dtLoginFailedList"] = dtLoginFailedList;
                Application.UnLock();
            }
        }      
 */
        #endregion Failed Logins Tracking
   }    
}
