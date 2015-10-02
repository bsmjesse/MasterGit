using System;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;
using VLF.DAS.Logic;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Configuration;

namespace VLF.ASI
{
	/// <summary>
	/// Provides login-related functionality.
	/// </summary>
	public class LoginManager:System.Web.HttpApplication     // : ASLBase
   {
       
		/// <summary>
		/// Constructor
		/// </summary>
        /// 

		private LoginManager()
		{
		}
		/// <summary>
		/// Log in user into the system.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="passKey">returns new passKey</param>
		/// <returns>UserId</returns>
		public int LoginUser( string userName, string password, ref string passKey )
		{
            
			int userId = GetUserID( userName, password );
			Trace.WriteLineIf(AppConfig.tsStat.Enabled|AppConfig.tsMain.TraceInfo,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,string.Format("LoginManager.Login( {0}, <pwd> ) UserID = {1}", userName, userId )));
            LoginManager.SetConnnectionString(userId, "SentinelFM");
			CheckUserExpiration(userId);
			passKey = SecurityKeyManager.GetInstance().CreatePassKey( userId ) ;
			return userId ; 
		}


      /// <summary>
      /// Log in user into the system.
      /// </summary>
      /// <param name="userName"></param>
      /// <param name="password"></param>
      /// <param name="passKey">returns new passKey</param>
      /// <returns>UserId</returns>
        public int LoginUserMD5(string userName, string password, string dbName, ref string passKey)
       {
         //string dbConnection = dbName == "" ? System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString : System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.Replace("SentinelFM", dbName);
           string dbConnection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
         int userId = GetUserIDMD5(userName, password, dbConnection);
         Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("LoginManager.Login( {0}, <pwd> ) UserID = {1}", userName, userId)));
         LoginManager.SetConnnectionString(userId, dbName);
         CheckUserExpiration(userId);
         passKey = SecurityKeyManager.GetInstance().CreatePassKey(userId);
         return userId;
      }

		/// <summary>
		/// Relogin user into the system.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="passKey">returns new passKey</param>
		/// <returns>UserId</returns>
		public void ReloginUser( string userName, string password, ref string passKey )
		{
            
			int userId = GetUserID( userName, password );
            LoginManager.SetConnnectionString(userId, "SentinelFM");
			Trace.WriteLineIf(AppConfig.tsStat.Enabled|AppConfig.tsMain.TraceInfo,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,string.Format("LoginManager.Relogin( {0}, <pwd> ) UserID = {1}", userName, userId )));
			CheckUserExpiration(userId);
			passKey = SecurityKeyManager.GetInstance().CreatePassKey( userId ) ;
		}



        /// <summary>
        /// Relogin user into the system.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="passKey">returns new passKey</param>
        /// <returns>UserId</returns>
        public void ReloginUserMD5(string userName, string password, string dbName, ref string passKey)
        {
            //string dbConnection = dbName == "" ? System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString : System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString.Replace("SentinelFM", dbName);
            string dbConnection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            int userId = GetUserIDMD5(userName, password, dbConnection);
            Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("LoginManager.ReloginUserMD5( {0}, <pwd> ) UserID = {1}", userName, userId)));
            CheckUserExpiration(userId);
            passKey = SecurityKeyManager.GetInstance().CreatePassKey(userId);
        }

		/// <summary>
		/// Logout user from the system.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="passKey">returns new passKey</param>
		/// <returns>UserId</returns>
		public void LogoutUser( int userId )
		{
			SecurityKeyManager.GetInstance().DeletePassKey( userId ) ;
			Trace.WriteLineIf(AppConfig.tsStat.Enabled|AppConfig.tsMain.TraceInfo,CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info,string.Format("LoginManager.Logout( UserID = {0} )", userId )));
		}	

		/// <summary>
		/// Validate passkey
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="passKey"></param>
		public void SecurityCheck( int userId, string passKey )
		{
			ValidationResult res =
			SecurityKeyManager.GetInstance().ValidatePasskey( userId, passKey ) ;
          
			switch(res)
			{
				case ValidationResult.Failed:
					throw new VLF.ERR.ASIAuthenticationFailedException("SecurityCheck::Authentication for user " + userId + " failed.") ;
				case ValidationResult.Expired:
					throw new VLF.ERR.ASIPassKeyExpiredException("Security key for user " + userId + " is expired.") ;
                case ValidationResult.CallFrequencyExceeded:
                    throw new VLF.ERR.ASICallFrequencyExceededException("CallFrequencyExceededException for user " + userId + ".");
				default:
					CheckUserExpiration(userId);
               break;
			}
		}
		/// <summary>
		/// Check user login expiration
		/// </summary>
		/// <param name="userId"></param>
		public void CheckUserExpiration(int userId)
		{
			User dbUser = null;
			try
			{
				//AppConfig appConfig = AppConfig.GetInstance();
				//dbUser = new User(appConfig.ConnectionString);

                string ConnStr = LoginManager.GetConnnectionString(userId);

                try
                {
                    dbUser = new User(ConnStr);
                    dbUser.CheckUserExpiration(userId);
                }
                catch
                {
                    Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error, string.Format("Error in CheckUserExpiration. ConnString={0} and  User  = {1}",ConnStr, userId)));
                }

			}
			finally
			{
				if(dbUser != null)
					dbUser.Dispose();
			}
		}
		/// <summary>
		/// Authorize user operation
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="operationType"></param>
		/// <param name="operationId"></param>
		public void AuthorizeOperation(int userId, VLF.CLS.Def.Enums.OperationType operationType, int operationId)
		{
			bool isValidOperation = false;
			UserGroup dbUserGroup = null;
			try
			{
				//AppConfig appConfig = AppConfig.GetInstance();
				//dbUserGroup = new dbUserGroup(appConfig.ConnectionString);
                dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId));
				isValidOperation = dbUserGroup.AuthorizeOperation(userId,operationType,operationId);
			}
			finally
			{
				if(dbUserGroup != null)
					dbUserGroup.Dispose();
			}
			if(isValidOperation == false)
				throw new VLF.ERR.ASIAuthorizationFailedException("SecurityCheck::AuthorizeOperation for user=" + userId + " operationType=" + operationType.ToString() + " operationId=" + operationId + " failed.") ;
		}

      /// <summary>
      /// Authorize user operation
      /// </summary>
      /// <param name="userId">User Id</param>
      /// <param name="methodName">Method name</param>
      /// <param name="className">Method class</param>
      public void AuthorizeWebMethod(int userId, string methodName, string className)
      {
         bool isValid = false;
         UserGroup dbUserGroup = null;
         try
         {
            //AppConfig appConfig = AppConfig.GetInstance();
            //dbUserGroup = new UserGroup(appConfig.ConnectionString);
             dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId));
            isValid = dbUserGroup.AuthorizeWebMethod(userId, methodName, className);
         }
         finally
         {
            if (dbUserGroup != null)
               dbUserGroup.Dispose();
         }
         if (!isValid)
            throw new VLF.ERR.ASIAuthorizationFailedException("SecurityCheck::AuthorizeWebMethod for user=" + userId + " WebMethod=" + methodName + " failed.");
      }

		/// <summary>
		/// Get user id by user name
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		protected int GetUserID(string userName, string password)
		{
			int userId = VLF.CLS.Def.Const.unassignedIntValue;
			User dbUser = null;
			try
			{
                dbUser = new User(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
				userId = dbUser.ValidateUser(userName,password);
			}
			finally
			{
				if(dbUser != null)
					dbUser.Dispose();
			}
			
			if(userId == VLF.CLS.Def.Const.unassignedIntValue)
				throw new VLF.ERR.ASIAuthenticationFailedException("GetUserID::Authentication for user " + userName + " failed.") ;

			return userId ;
		}


      /// <summary>
      /// Get user id by user name
      /// </summary>
      /// <param name="userName"></param>
      /// <param name="password"></param>
      /// <returns></returns>
        protected int GetUserIDMD5(string userName, string password, string dbConnection)
      {
         int userId = VLF.CLS.Def.Const.unassignedIntValue;
         User dbUser = null;
         try
         {
            //AppConfig appConfig = AppConfig.GetInstance();
            //dbUser = new User(appConfig.ConnectionString);

             dbUser = new User(dbConnection);
            userId = dbUser.ValidateUserMD5(userName, password);
         }
         finally
         {
            if (dbUser != null)
               dbUser.Dispose();
         }

         if (userId == VLF.CLS.Def.Const.unassignedIntValue)
             throw new VLF.ERR.ASIAuthenticationFailedException("GetUserIDMD5::Authentication for user " + userName + " failed.");

         return userId;
      }

        internal static string GetConnnectionString(int userId)
        {
            string ConnnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            //try
            //{

            //    MemcachedClient mc = new MemcachedClient();
            //    string Id = "DatabaseName_" + userId;
            //    string DatabaseName = "";
            //    if (mc.Get(Id) != null && Convert.ToString(mc.Get(Id)) != "")
            //    {
            //        DatabaseName = Convert.ToString(mc.Get(Id));
            //        ConnnectionString = ConnnectionString.Replace("SentinelFM", DatabaseName);
            //    }
            //    //else
            //    //{
            //    //    Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("Used Default Database: Null Memcached - database name for user UserID = {0}", userId)));
            //    //}
            //}
            //catch
            //{
            //    Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("Error in reading from Memcached: database name for user UserID = {0}", userId)));
            //}
             
             return ConnnectionString;
             


            //Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("GetConnnectionString-{0}:{1}",Id, ConnnectionString)));


        }

         internal static void SetConnnectionString(int userId, string dbConnectionString)
        {
            //MemcachedClient mc = new MemcachedClient();
            //string Id = "DatabaseName_" + userId;
            //mc.Store(StoreMode.Set, Id, dbConnectionString);
            ////Trace.WriteLineIf(AppConfig.tsStat.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, string.Format("SetConnnectionString-{0}:{1}",Id, dbConnectionString)));
        }


   
   

		#region Singleton functionality
		private static LoginManager instance = null ;
		public static LoginManager GetInstance()
		{
         lock (typeof(LoginManager))
         {
            if (instance == null)
               instance = new LoginManager();
            return instance;
         }
		}
		#endregion  Singleton functionality

	}
}
