using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity ;
using System.IO;
using VLF.DAS.Logic;
using VLF.CLS;

namespace VLF.ASI.Interfaces
{
   [WebService(Namespace = "http://www.sentinelfm.com")]

   /// <summary>
   /// Summary description for DBSystem.
   /// </summary>
   public class DBSystem : System.Web.Services.WebService
   {
      public DBSystem()
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
      protected override void Dispose(bool disposing)
      {
         if (disposing && components != null)
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
      private bool ValidateUserBox(int userId, string SID, int boxId)
      {
         // Authenticate & Authorize
         LoginManager.GetInstance().SecurityCheck(userId, SID);

         //Authorization
         using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
         {
            return dbUser.ValidateUserBox(userId, boxId);
         }
      }

      #endregion refactored functions

      [WebMethod]
      public int GetAllStateProvinces(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetAllStateProvinces( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetAllStateProvinces();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetAllStateProvinces : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }


      [WebMethod]
      public int GetAllVehicleTypes(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">> GetAllVehicleTypes( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetAllVehicleTypes();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
           }
            
           return (int)InterfaceError.NoError;

         }
         catch (Exception Ex)
         {
            LogException("<< GetAllVehicleTypes : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }


       [WebMethod]
       public int GetBoxMsgInTypes(int userId, string SID, ref string xml)
       {
           try
           {
               Log(">> GetBoxMsgInTypes( uid={0} )", userId);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);

               using( Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
               {
                   DataSet dsInfo = dbBox.GetBoxMsgInTypes();
                   if (ASIErrorCheck.IsAnyRecord(dsInfo))
                       xml = dsInfo.GetXml();
               }

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
              LogException("<< GetBoxMsgInTypes : uId={0}, EXC={1})", userId, Ex.Message);
              return (int)ASIErrorCheck.CheckError(Ex);
           }
       }


       [WebMethod]
       public int GetBoxMsgInTypesByLang(int userId, string SID, string lang, ref string xml)
       {
           try
           {
               Log(">> GetBoxMsgInTypesByLang( uid={0} )", userId);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);

               using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
               {
                   DataSet dsInfo = dbBox.GetBoxMsgInTypes();
                   if (Util.IsDataSetValid(dsInfo))
                   {
                       if (ASIErrorCheck.IsLangSupported(lang))
                       {
                           LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                           dbl.LocalizationData(lang, "BoxMsgInTypeId", "BoxMsgInTypeName", "MessageType", ref dsInfo);
                       }

                       xml = dsInfo.GetXml();
                   }

               }

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< GetBoxMsgInTypesByLang : uId={0}, EXC={1})", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
      [WebMethod]
      public int GetIconsInfo(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetIconsInfo( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetIconsInfo();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetIconsInfo : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      [WebMethod]
      public int GetAllMakesInfo(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetAllMakesInfo( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetAllMakesInfo();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetAllMakesInfo : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      [WebMethod]
      public int GetModelsInfoByMakeId(int userId, string SID, int makeId, ref string xml)
      {
         try
         {
            Log(">>GetModelsInfoByMakeId( uid={0}, makeId = {1} )", userId, makeId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetModelsInfoByMakeId(makeId);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetModelsInfoByMakeId : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }



      public int GetDclInfo(int userId, string SID, string dclName, ref string xml)
      {
         try
         {
            Log(">>GetDclInfo( uid={0}, dclName = {1} )", userId, dclName);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using( Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbBox.GetDclInfo(dclName);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetDclInfo : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      [WebMethod]
      public int GetSheduledTasksHistory(int userId, string SID, string fromDate, string toDate, Int32 fleetId, Int32 boxId, ref string xml)
      {
         try
         {
            Log(">>GetSheduledTasksHistory(uid={4} dtFrom={0}, dtTo={1}, fleetId={2}, vehicleId={3})", 
                           fromDate, toDate, fleetId, boxId, userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsSched = dbSystem.GetSheduledTasksHistory(Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), fleetId, boxId);
               if (Util.IsDataSetValid(dsSched))
                  xml = dsSched.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetSheduledTasksHistory : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }


      #region System Updates
      [WebMethod]
      public int AddSystemUpdate(int userId, string SID, string msg, string msgFr, 
                                 short systemUpdateType, short severity,string FontColor,Int16 FontBold)
      {
         try
         {
            Log(">>AddSystemUpdate(uid={0}, msg={1} msgFrench={4}, systemUpdateType={2}, severity={3})", 
                     userId, msg, systemUpdateType, severity, msgFr);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);
           
        
           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               dbSystem.AddSystemUpdate(DateTime.Now, msg,msgFr, 
                                        (VLF.CLS.Def.Enums.SystemUpdateType)systemUpdateType, 
                                        (VLF.CLS.Def.Enums.AlarmSeverity)severity, FontColor,FontBold);
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< AddSystemUpdate : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }


       [WebMethod]
       public int UpdateSystemUpdateTable(int userId, string SID,int msgid, string msg,string msgFr, short systemUpdateType, short severity, string FontColor, Int16 FontBold)
       {
           try
           {
               Log(">>AddSystemUpdate(uid={0}, msg={1}, msgFrench={4}, systemUpdateType={2}, severity={3})",
                     userId, msg, systemUpdateType, severity, msgFr);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);

               using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
               {
                   dbSystem.UpdateSystemUpdateTable(msgid,DateTime.Now, msg,msgFr, 
                                                   (VLF.CLS.Def.Enums.SystemUpdateType)systemUpdateType, 
                                                   (VLF.CLS.Def.Enums.AlarmSeverity)severity, FontColor, FontBold);
               }
              
               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
              LogException("<< AddSystemUpdate : uId={0}, EXC={1})", userId, Ex.Message);
              return (int)ASIErrorCheck.CheckError(Ex);
           }
       }

      [WebMethod]
      public int GetSystemUpdates(int userId, string SID, string from, string to, 
                                  short systemUpdateType, ref string xml)
      {
         try
         {
            Log(">> GetSystemUpdates(uid={0}, dtFrom={1}, dtTo={2}, systemUpdateType={3})", 
                              userId, from, to, systemUpdateType);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);


            DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;
            if (!string.IsNullOrEmpty(from))
               fromDT = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to))
               toDT = Convert.ToDateTime(to);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetSystemUpdates(userId, fromDT, toDT, 
                                                          (VLF.CLS.Def.Enums.SystemUpdateType)systemUpdateType);
               if (ASIErrorCheck.IsAnyRecord(dsInfo)) 
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetSystemUpdates : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      [WebMethod]
       public int GetFullInfoSystemUpdates(int userId, string SID, string from, string to, 
                                           short systemUpdateType, ref string xml)
       {
           try
           {
              Log(">>GetFullInfoSystemUpdates(uid={0}, dtFrom={1}, dtTo={2}, systemUpdateType={3})", 
                                 userId, from, to, systemUpdateType);

              // Authenticate & Authorize
              LoginManager.GetInstance().SecurityCheck(userId, SID);

              DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
              DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;
              if (!string.IsNullOrEmpty(from))
                  fromDT = Convert.ToDateTime(from);
              if (!string.IsNullOrEmpty(to))
                  toDT = Convert.ToDateTime(to);


              using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
              {
                   DataSet dsInfo = dbSystem.GetFullInfoSystemUpdates(userId, fromDT, toDT, 
                                                               (VLF.CLS.Def.Enums.SystemUpdateType)systemUpdateType);
                   if (ASIErrorCheck.IsAnyRecord(dsInfo))
                       xml = dsInfo.GetXml();
              }
               
              return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
              LogException("<< GetFullInfoSystemUpdates : uId={0}, EXC={1})", userId, Ex.Message);
              return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
      [WebMethod]
      public int GetSystemUpdatesByLang(int userId, string SID, string from, string to, 
                                        short systemUpdateType, string lang, ref string xml)
      {
         try
         {
            Log(">>GetSystemUpdatesByLang(uid={0}, dtFrom={1}, dtTo={2}, systemUpdateType={3}, lang={4})",
                        userId, from, to, systemUpdateType, lang);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);



            DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
            DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;
            if (!string.IsNullOrEmpty(from))
               fromDT = Convert.ToDateTime(from);
            if (!string.IsNullOrEmpty(to))
               toDT = Convert.ToDateTime(to);

          using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetSystemUpdatesByLang(userId, fromDT, toDT, 
                                                               (VLF.CLS.Def.Enums.SystemUpdateType)systemUpdateType, 
                                                               lang);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetSystemUpdatesByLang : uId={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int DeleteSystemUpdate(int userId, string SID, int msgId)
      {
         try
         {
            Log(">> DeleteSystemUpdate(uid={0}, msgId={1})", userId, msgId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dbSystem.DeleteSystemUpdate(msgId);
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< DeleteSystemUpdate(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      #endregion

      #region Configuration
      [WebMethod]
      public int GetConfigurationModuleTypesInfo(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">> GetConfigurationModuleTypesInfo(uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetConfigurationModuleTypesInfo();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetConfigurationModuleTypesInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetConfigurationModuleTypeId(int userId, string SID, string moduleTypeName, ref short moduleTypeId)
      {
         try
         {
            Log(">>GetConfigurationModuleTypeId( uid={0}, moduleTypeName = {1} )", userId, moduleTypeName);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }


            using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               moduleTypeId = dbSystem.GetConfigurationModuleTypeId(moduleTypeName);
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetConfigurationModuleTypeId(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetConfigurationValue(int userId, string SID, short moduleTypeId, short cfgGroupId, string keyName, ref string keyValue)
      {
         try
         {
           Log(">>GetConfigurationValue( uid={0}, moduleTypeId={1}, cfgGroupId={2}, keyName={3})", 
                        userId, moduleTypeId, cfgGroupId, keyName);

           // Authenticate & Authorize
           LoginManager.GetInstance().SecurityCheck(userId, SID);

           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               keyValue = dbSystem.GetConfigurationValue(moduleTypeId, cfgGroupId, keyName);
           }
            
           return (int)InterfaceError.NoError;

         }
         catch (Exception Ex)
         {
            LogException("<< GetConfigurationValue(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetComputerModulesInfo(int userId, string SID, short moduleTypeId, string computerIp, ref string xml)
      {
         try
         {
            Log(">>GetComputerModulesInfo(uid={0}, moduleTypeId={1}, computerIp={2})", 
                        userId, moduleTypeId, computerIp);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }


           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsInfo = dbSystem.GetComputerModulesInfo(moduleTypeId, computerIp);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
           
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetComputerModulesInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int UpdateModuleStatus(int userId, string SID, short moduleTypeId, bool enable)
      {
         try
         {
            Log(">>UpdateModuleStatus( uid={0}, moduleTypeId = {1}, enable = {2} )", userId, moduleTypeId, enable);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }


           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               dbSystem.UpdateModuleStatus(moduleTypeId, enable);
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< UpdateModuleStatus(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int DeleteModule(int userId, string SID, short moduleId)
      {
         try
         {
            Log(">>DeleteModule( uid={0}, moduleId = {1} )", userId, moduleId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }
            
            using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dbSystem.DeleteModule(moduleId);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< DeleteModule(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetCommModesInfo(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">> GetCommModesInfo( uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            //				VLF.DAS.Logic.User   dbUser=new User (LoginManager.GetConnnectionString(userId)); 
            //				if (!dbUser.ValidateHGISuperUser(userId))
            //					return Convert.ToInt32(InterfaceError.AuthorizationFailed);


           using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {

               DataSet dsInfo = dbSystem.GetCommModesInfo();

               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetCommModesInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetAllFirmwareInfo(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">> GetAllFirmwareInfo(uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using ( SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
           {
               DataSet dsResult = new DataSet();
               dsResult = dbSystem.GetAllFirmwareInfo((short)VLF.CLS.Def.Enums.FirmwareType.SentinelFM);
               DataSet dsInfo = dbSystem.GetAllFirmwareInfo((short)VLF.CLS.Def.Enums.FirmwareType.Bantek);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  dsResult.Merge(dsInfo);
               if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                  xml = dsResult.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetAllFirmwareInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      #endregion

      #region Groups Security Information
      [WebMethod]
      public int GetUserControls(int userId, string SID, ref string xml)
      {
         try
         {
            xml = "";
            Log(">> GetUserControls(uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbUserGroup.GetUserControls(userId);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }
            
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetUserControls(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetUserReports(int userId, string SID, ref string xml)
      {
         try
         {
            xml = "";
            Log(">> GetUserReports( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbUserGroup.GetUserReports(userId);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetUserReports(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetUserReportsByLang(int userId, string SID, string lang, ref string xml)
      {
         try
         {
            xml = "";

            Log(">> GetUserReportsByLang( uid={0}, lang={1} )", userId, lang);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            DataSet dsInfo = null;
            
            using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
            {
               dsInfo = dbUserGroup.GetUserReports(userId);
            }


            if (ASIErrorCheck.IsAnyRecord(dsInfo))
            {
               if (ASIErrorCheck.IsLangSupported(lang))
               {
                  LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                  dbl.LocalizationData(lang, "ReportTypesId", "GuiName", "Reports", ref dsInfo);
               }

               xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetUserReportsByLang(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

       [WebMethod]
       public int GetUserReportsByCategory(int userId, string SID, string lang,int category, ref string xml)
       {
           try
           {
               xml = "";
               Log(">> GetUserReportsByCategory( uid={0} )", userId);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);

              DataSet dsInfo = null;
              using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
              {
                   dsInfo = dbUserGroup.GetUserReportsByCategory(userId,category);
              }

              // if (lang == "fr" && lang != null)
              if(ASIErrorCheck.IsLangSupported(lang))
              {
                  LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                  dbl.LocalizationData(lang, "ReportTypesId", "GuiName", "Reports", ref dsInfo);
              }

            if (ASIErrorCheck.IsAnyRecord(dsInfo))
                   xml = dsInfo.GetXml();

             return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
              LogException("<< GetUserReportsByCategory(uid={0}, EXC={1})", userId, Ex.Message);
              return (int)ASIErrorCheck.CheckError(Ex);
           }
       }
      [WebMethod]
      public int GetGroupSecurityFullInfo(int userId, string SID, short userGroupId, ref string xml)
      {
         try
         {
            xml = "";
            Log(">> GetGroupSecurityFullInfo( uid={0}, userGroupId = {1})", userId, userGroupId);

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);
            // Authorize operation "cmdUserGroups"

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateUserGroupSecurity(userId, userGroupId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }

            LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

            using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbUserGroup.GetGroupSecurityFullInfo(userGroupId);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetGroupSecurityFullInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetAllGroupSecurityFullInfo(int userId, string SID, ref string xml)
      {
         try
         {
            xml = "";
            Log(">> GetAllGroupSecurityFullInfo( uid={0})", userId);

            // Authenticate 
            LoginManager.GetInstance().SecurityCheck(userId, SID);
            // Authorize operation "cmdUserGroups"
            LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 21);

            using( UserGroup dbUserGroup = new UserGroup(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbUserGroup.GetAllGroupSecurityFullInfo();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetAllGroupSecurityFullInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      /// <summary>
      /// Get all audit org. groups
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="SID"></param>
      /// <param name="xml"></param>
      /// <returns></returns>
      [WebMethod(Description = "Get All Audit Groups")]
      public int GetAuditGroupInfo(ref string xml)
      {
         try
         {
            Log(">> GetAuditGroupInfo ...");

            // Authenticate 
            //LoginManager.GetInstance().SecurityCheck(userId, SID);
            // Authorize operation
            //LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.System, );

            using (Organization org = new Organization(Application["ConnectionString"].ToString()))
            {
               DataSet dsInfo = org.GetAuditGroupInfo();
               if (Util.IsDataSetValid(dsInfo))
                  xml = dsInfo.GetXml();
            }
            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetAuditGroupInfo: EXC={0}", Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      #endregion

      #region Map Usage
      [WebMethod]
      public int AddMapUserUsage(int userId, string SID, short mapTypeId, short mapId)
      {
         try
         {
            Log(">> AddMapUserUsage(uid={0}, mapTypeId={1}, mapId={2})", userId, mapTypeId, mapId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using( User dbUser = new User(LoginManager.GetConnnectionString(userId)))            
            {
               DateTime dtNow = DateTime.Now.AddHours(-AppConfig.GetInstance().ServerTimeZone);               
               dbUser.AddMapUserUsage(userId, mapTypeId, Convert.ToInt16(dtNow.Year), Convert.ToInt16(dtNow.Month), mapId);
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< AddMapUserUsage(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetOrganizationMapUsageInfo(int userId, string SID, int organizationId, 
                                             int mapId, short usageYear, short usageMonth, ref string xml)
      {
         try
         {
            Log(">>GetOrganizationMapUsageInfo( uid={0}, orgId={1}, mapId={2}, usageYear={3}, usageMonth={4})", 
                           userId, organizationId, mapId, usageYear, usageMonth);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);


            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
               if (!dbUser.ValidateHGISuperUser(userId))
                  return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            }
            
            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbSystemConfig.GetOrganizationMapUsageInfo(organizationId, mapId, 
                                                                           usageYear, usageMonth);
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetOrganizationMapUsageInfo(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod]
      public int GetMapTypes(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetMapTypes( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsInfo = dbSystemConfig.GetMapTypes();
               if (ASIErrorCheck.IsAnyRecord(dsInfo))
                  xml = dsInfo.GetXml();
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetMapTypes(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      [WebMethod(Description = "Retrieves box geocode engine info. XML File format :[GeoCodeId],[Path]")]
      public int GetBoxGeoCodeEngineInfoXML(int userId, string SID, int boxId, ref string xml)
      {
         try
         {
            Log(">> GetBoxGeoCodeEngineInfoXML( uid={0}, boxId = {1} )", userId, boxId);

            if (!ValidateUserBox(userId, SID, boxId))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);


            DataSet dsMapEnginesInfo = null;
            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dsMapEnginesInfo = dbSystemConfig.GetBoxGeoCodeEngineInfo(boxId);
            }

            if (Util.IsDataSetValid(dsMapEnginesInfo))
               xml = dsMapEnginesInfo.GetXml();

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetBoxGeoCodeEngineInfoXML(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod(Description = "Retrieves box map engine info. XML File format :[MapId],[Path],[ExternalPath]")]
      public int GetBoxMapEngineInfoXML(int userId, string SID, int boxId, ref string xml)
      {
         try
         {
            Log(">>GetBoxMapEngineInfoXML(uid={0}, boxId={1})", userId, boxId);

            if (!ValidateUserBox(userId, SID, boxId))
               return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            DataSet dsMapEnginesInfo = null;
            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dsMapEnginesInfo = dbSystemConfig.GetBoxMapEngineInfo(boxId);
            }

            if (Util.IsDataSetValid(dsMapEnginesInfo))
               xml = dsMapEnginesInfo.GetXml();

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetBoxMapEngineInfoXML(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod(Description = "Retrieves user geocode engine info. XML File format :[GeoCodeId],[Path]")]
      public int GetUserGeoCodeEngineInfoXML(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetUserGeoCodeInfoXML( uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);


            ////Authorization
            //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
            //if (!dbUser.ValidateUserBox(userId, boxId))
            //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
            DataSet dsGeoCodeEnginesInfo = null;
            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dsGeoCodeEnginesInfo = dbSystemConfig.GetUserGeoCodeEngineInfo(userId);
            }

            if (Util.IsDataSetValid(dsGeoCodeEnginesInfo))
               xml = dsGeoCodeEnginesInfo.GetXml();

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetUserGeoCodeEngineInfoXML(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }
      [WebMethod(Description = "Retrieves user map engine info. XML File format :[MapId],[Path],[ExternalPath]")]
      public int GetUserMapEngineInfoXML(int userId, string SID, ref string xml)
      {
         try
         {
            Log(">>GetUserMapEngineInfoXML( uid={0} )", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);


            //Authorization
            //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
            //if (!dbUser.ValidateUserBox(userId, boxId))
            //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

            DataSet dsMapEnginesInfo = null;
            
            using( SystemConfig dbSystemConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               dsMapEnginesInfo = dbSystemConfig.GetUserMapEngineInfo(userId);
            }

            if (Util.IsDataSetValid(dsMapEnginesInfo))
               xml = dsMapEnginesInfo.GetXml();

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< GetUserMapEngineInfoXML(uid={0}, EXC={1})", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

       [WebMethod(Description = "Get Lat/Long for LSD maps. The specialAddress should be in the format meridian-township-range-section-quarter in the format above ")]
       public int GetLatitudeLongitudeByLSD(int userId, string SID, string specialAddress, ref double latitude, ref  double longitude)
       {
           Log(">>GetLatitudeLongitudeByLSD( uid={0},specialAddress={1} )", userId, specialAddress);


           // Authenticate & Authorize
           LoginManager.GetInstance().SecurityCheck(userId, SID);

           DataTable dt = null;
           try
           {
               dt = UtilSql.DBG.RunQueryGetDataReader(LoginManager.GetConnnectionString(userId),
                                                        string.Format("SELECT * from vlfLSD where name ='{0}'", specialAddress));
               {
                   if (dt != null && dt.Rows != null && dt.Rows.Count>0)
                   {
                       latitude = Convert.ToDouble(dt.Rows[0]["latitude"]);
                       longitude = Convert.ToDouble(dt.Rows[0]["longitude"]);
                   }
                   else
                   {
                       latitude = longitude = .0;
                       return (int)InterfaceError.NotFound;
                   }
               }
           }
           catch (Exception Ex)
           {
               LogException("<< GetLatitudeLongitude(uid={0}, EXC={1})", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }

           return (int)InterfaceError.NoError;
       }



       [WebMethod(Description = "Acknowledge Notification")]
       public int AcknowledgeNotification(int userId, string SID,int notificationId, Int16  typeId, DateTime dtTime)
       {
           try
           {
               Log(">>AcknowledgeNotification( uid={0})", userId);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);


               ////Authorization
               //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
               //if (!dbUser.ValidateUserBox(userId, boxId))
               //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);
               using (Notification  dbNotification = new Notification(LoginManager.GetConnnectionString(userId),false ))
               {
                   dbNotification.AckNotificationMaintenance (notificationId, typeId, userId, dtTime, "");
               }


               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< AcknowledgeNotification(uid={0}, EXC={1})", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }




       [WebMethod(Description = "LocalizationData")]
       public int LocalizationData(int userId, string SID, string lang, string KeyField, string FieldName, string FieldGroup, ref string xml)
       {
        try
        {
           DataSet ds = new DataSet();
           ds.ReadXml(new StringReader(xml)); 
           if (ASIErrorCheck.IsLangSupported(lang))
           {
               LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
               dbl.LocalizationData(lang, KeyField, FieldName, FieldGroup, ref ds);
           }

           xml = ds.GetXml();
           return (int)InterfaceError.NoError;
       }
       catch (Exception Ex)
       {
           LogException("<< LocalizationData(uid={0}, EXC={1})", userId, Ex.Message);
           return (int)ASIErrorCheck.CheckError(Ex);
       }

       }
      #endregion

      # region Administration, installation jobs

      [WebMethod]
      public int InstallJob_Add(int userId, string SID, string xmlData, string description, string status, DateTime dtModified)
      {
         try
         {
            Log(">> InstallJob_Add(uid={0}, description = {1})", userId, description);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);
            int result = 0;
            using (SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               result = sysConfig.InstallJob_Add(xmlData, description, status, dtModified);
            }
            if (result != 1)
               return (int)InterfaceError.DatabaseError;
         }
         catch (Exception Ex)
         {
            LogException("<< InstallJob_Add : uid={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         return (int)InterfaceError.NoError;
      }

      [WebMethod]
      public int InstallJob_Update(int userId, string SID, string xmlData, string description)
      {
        
        Log(">> InstallJob_Update(uid={0}, description = {1})", userId, description);
        return (int)InterfaceError.NotImplemented;
      }

      [WebMethod]
      public int InstallJob_Delete(int userId, string SID, int jobId)
      {
         Log(">> InstallJob_Update(uid={0}, Job Id = {1})", userId, jobId);
         return (int)InterfaceError.NotImplemented; ;
      }

      [WebMethod]
      public int InstallJobs_GetAll(int userId, string SID, ref string xml)
      {
         try
         {
           Log(">> InstallJobs_GetAll(uid={0})", userId);

            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);
            int result = 0;
            using (SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsResult = sysConfig.InstallJob_GetAll();
               if (Util.IsDataSetValid(dsResult))
                  xml = dsResult.GetXml();
               else
                  return (int)InterfaceError.DatabaseError;
            }
         }
         catch (Exception Ex)
         {
            LogException("<< InstallJobs_GetAll : uid={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
         return (int)InterfaceError.NoError;
      }

      [WebMethod]
      public int InstallJob_Get(int userId, string SID, int jobId, ref string xml)
      {
         try
         {
            Log(">> InstallJob_Get(uid={0}, jobId={1})", userId, jobId);


            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            int result = 0;
            using (SystemConfig sysConfig = new SystemConfig(LoginManager.GetConnnectionString(userId)))
            {
               DataSet dsResult = sysConfig.InstallJob_Get(jobId);
               if (Util.IsDataSetValid(dsResult))
                  xml = dsResult.Tables[0].Rows[0]["XMLData"].ToString();
               else
                  return (int)InterfaceError.DatabaseError;
            }

            return (int)InterfaceError.NoError;
         }
         catch (Exception Ex)
         {
            LogException("<< InstallJob_Get : uid={0}, EXC={1}", userId, Ex.Message);
            return (int)ASIErrorCheck.CheckError(Ex);
         }
      }

      # endregion

      #region LogViewer

       [WebMethod(Description = "Get Log Data from MySQL")]
       public int GetLogData(int userId, string SID, DateTime from, DateTime to, string ModuleIds, string msgFilter,string topMsgs,ref string xml)
       {
           try
           {
               Log(">>GetLogData( uid={0})", userId);

               // Authenticate & Authorize
               LoginManager.GetInstance().SecurityCheck(userId, SID);
               DataSet ds = null;
               logViewer dbLog = new logViewer(System.Configuration.ConfigurationSettings.AppSettings["LogDBConnectionString"]);
               ds = dbLog.GetLogData(from, to, ModuleIds, msgFilter, topMsgs);
               

               if (Util.IsDataSetValid(ds))
                   xml = ds.GetXml();

               return (int)InterfaceError.NoError;
           }
           catch (Exception Ex)
           {
               LogException("<< GetLogData(uid={0}, EXC={1})", userId, Ex.Message);
               return (int)ASIErrorCheck.CheckError(Ex);
           }
       }


      #endregion
  }
}
