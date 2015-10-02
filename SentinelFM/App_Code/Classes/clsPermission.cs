using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace SentinelFM
{

    /// <summary>
    /// Summary description for clsPermission
    /// </summary>
    public class clsPermission
    {
        public clsPermission()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static bool FeaturePermissionCheck(SentinelFMSession sn, string featureName)
        {
            bool _permission = false;

            String _c = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FeaturePermission.xml")))
                {
                    _c = sr.ReadToEnd();
                }

                if (_c != "")
                {
                    DataSet ds_c = new DataSet();
                    ds_c.ReadXml(new StringReader(_c));
                    char[] delimiters = new char[] { ',', ';' };
                    foreach (DataRow _dr in ds_c.Tables[0].Rows)
                    {
                        if (_dr["Name"].ToString().Trim().ToLower() == featureName.Trim().ToLower())
                        {
                            List<int> organizations = _dr["Organization"].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();
                            List<int> usergroups = _dr["UserGroup"].ToString().Trim().ToLower().Replace(" ", "").Trim(';').Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();
                            List<int> users = _dr["User"].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();

                            if (users.Contains(sn.UserID))
                            {
                                _permission = true;
                            }
                            else if (
                                (organizations.Count == 0 || organizations.Contains(sn.User.OrganizationId)) &&
                                (usergroups.Count == 0 || usergroups.Contains(sn.User.UserGroupId))
                            )
                            {
                                _permission = true;
                            }
                        }
                    }
                } 
            }
            catch { 
                return false; 
            }

            return _permission;
        }

        public static List<int> GetFeaturePermissionData(SentinelFMSession sn, string featureName)
        {
           
            List<int> organizationDays = new List<int>();
            
            String _c = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FeaturePermission.xml")))
                {
                    _c = sr.ReadToEnd();
                }

                if (_c != "")
                {
                    DataSet ds_c = new DataSet();
                    ds_c.ReadXml(new StringReader(_c));
                    char[] delimiters = new char[] { ',', ';' };
                    foreach (DataRow _dr in ds_c.Tables[0].Rows)
                    {
                        if (_dr["Name"].ToString().Trim().ToLower() == featureName.Trim().ToLower())
                        {

                            List<int> organizations = _dr["Organization"].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();

                            if (organizations.Count != 0 && organizations.Contains(sn.User.OrganizationId))
                            {
                                organizationDays = _dr["UserGroup"].ToString().Trim().ToLower().Replace(" ", "").Trim(';').Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();

                            }
                            else
                            {
                                organizationDays = _dr["User"].ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Where(x => Regex.IsMatch(x, @"\d")).Select(n => int.Parse(n)).ToList();
                            }
                            
                        }
                    }
                }
            }
            catch
            {
                
            }

            return organizationDays;
        }
    }

    
}