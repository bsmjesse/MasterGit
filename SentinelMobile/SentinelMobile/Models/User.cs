using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SentinelMobile.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public bool IsSuperUser { get; set; }
        public string SecId { get; set; }
        public string UserName { get; set; }
        public int DefaultFleet { get; set; }
        public string DefaultLanguage { get; set; }
        public int SuperOrganizationId { get; set; }
        public string FleetType { get; set; }
        public string DefaultNodeCode { get; set; }
        public short TimeZone { get; set; }
        public short DayLightSaving { get; set; }
        public double UnitOfMes { get; set; }
        //for HOS
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        [System.ComponentModel.DefaultValue(-1)]
        public string RefId { get; set; }
        [System.ComponentModel.DefaultValue(false)]
        public Boolean IsDriver { get; set; }

        public int UserGroupId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Boolean HosEnabled { get; set; }

        public DataSet DsGUIControls { get; set;}

        public User(string userName, int userId, string secId)
        {
            UserId = userId;
            SecId = secId;
            UserName = userName;
            IsSuperUser = false;
        }

        public void ExistingPreference()
        {

            try
            {
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                //objUtil = new clsUtility(sn);
                string xml = "";
                StringReader strrXML = null;
                DataSet dsUser = new DataSet();
                //ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                dbu.GetUserInfoByUserId(UserId, SecId, ref xml);
                //if (objUtil.ErrCheck(dbu.GetUserInfoByUserId(sn.UserID, sn.SecId, ref xml), false))
                //    if (objUtil.ErrCheck(dbu.GetUserInfoByUserId(sn.UserID, sn.SecId, ref xml), true))
                //    {
                //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetUserInfoByUserId. User:" + sn.UserID.ToString() + " Form:clsUser "));
                //    }
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    dsUser.ReadXml(strrXML);

                    //sn.User.LastName = dsUser.Tables[0].Rows[0]["LastName"].ToString();
                    //sn.User.FirstName = dsUser.Tables[0].Rows[0]["FirstName"].ToString();
                    //sn.User.OrganizationName = dsUser.Tables[0].Rows[0]["OrganizationName"].ToString();
                    //sn.User.OrganizationId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["OrganizationId"]);
                    //sn.User.FleetPulseURL = dsUser.Tables[0].Rows[0]["FleetPulseURL"].ToString();
                    //sn.User.UserGroupId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserGroupId"].ToString());
                    //sn.User.HosEnabled = Convert.ToBoolean(dsUser.Tables[0].Rows[0]["HOSenabled"].ToString());

                    OrganizationId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["OrganizationId"]);
                    OrganizationName = dsUser.Tables[0].Rows[0]["OrganizationName"].ToString();
                    UserGroupId = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserGroupId"].ToString());
                    LastName = dsUser.Tables[0].Rows[0]["LastName"].ToString();
                    FirstName = dsUser.Tables[0].Rows[0]["FirstName"].ToString();
                    //for HOS
                    HosEnabled = Convert.ToBoolean(dsUser.Tables[0].Rows[0]["HOSenabled"].ToString());

                    
                }

                DefaultFleet = -1;
                DefaultLanguage = "en-US";
                FleetType = "flat";

                dbu.GetUserPreferencesXML(UserId, SecId, ref xml);
                if (xml != "")
                {
                    strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);

                    Int16 PreferenceId = 0;

                    UnitOfMes = 1;

                    foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                    {
                        PreferenceId = Convert.ToInt16(rowItem["PreferenceId"]);
                        switch (PreferenceId)
                        {
                            case 0:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    UnitOfMes = Convert.ToDouble(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case 1:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    TimeZone = Convert.ToInt16(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case 2:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                    DefaultFleet = Convert.ToInt32(rowItem["PreferenceValue"].ToString().TrimEnd());
                                break;
                            case 7:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "1")
                                    DayLightSaving = 1;
                                else
                                    DayLightSaving = 0;
                                break;
                            case 35:
                                DefaultNodeCode = rowItem["PreferenceValue"].ToString().TrimEnd();
                                break;
                            case 36:
                                if (rowItem["PreferenceValue"].ToString().TrimEnd() == "hierarchy")
                                    FleetType = "hierarchy";
                                break;

                            case 37:    // default language

                                if (rowItem["PreferenceValue"].ToString().TrimEnd() != "")
                                {
                                    DefaultLanguage = rowItem["PreferenceValue"].ToString().TrimEnd();                                    
                                }

                                break;
                        }
                    }
                }

                GetGuiControlsInfo();
                
            }
            catch { }
        }

        public void GetGuiControlsInfo()
        {
            string xml = "";

            ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
            dbs.GetUserControls(UserId, SecId, ref xml);

            //objUtil = new clsUtility(sn);
            //if (objUtil.ErrCheck(dbs.GetUserControls(sn.UserID, sn.SecId, ref xml), false))
            //    if (objUtil.ErrCheck(dbs.GetUserControls(sn.UserID, sn.SecId, ref xml), true))
            //    {
            //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:clsUser "));
            //        return;
            //    }

            if (xml == "")
            {
                //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:clsUser "));
                return;
            }

            StringReader strrXML = new StringReader(xml);
            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);
            DsGUIControls = ds;

        }

        public bool ControlEnable(int ControlId)
        {
            try
            {
                bool ControlStatus = false;

                if (DsGUIControls != null)
                {
                    foreach (DataRow rowItem in DsGUIControls.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(rowItem["ControlId"]) == Convert.ToInt32(ControlId))
                        {
                            ControlStatus = true;
                            break;
                        }
                    }
                }

                return ControlStatus;
            }
            catch
            {
                return false;
            }
        }
    }

    public class Cmd
    {
        public short CommandId { get; set; }
        public string CommandList { get; set; }
        public string LicensePlate { get; set; }
    }
}