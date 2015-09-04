#define STANDALONE

using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Data;
using VLF.DAS.Logic;

namespace VLF.ASI
{

    public delegate void KeyRemovalHandler(int key);

    public sealed class OrganizationAudit
    {
        const int LOCKTIMEOUT = 30000;

        int _OrganizationId;

        int _AuditGroupId;
        public int AuditGroupId { get { return _AuditGroupId; } }

        string _Name;
        public string Name { get { return _Name; } }

        int _PeriodInMinutes;
        public int PeriodInMinutes { get { return _PeriodInMinutes; } }

        int _Frequency;
        public int Frequency { get { return _Frequency; } }

        int _RequestCount;
        public int RequestCount { get { return _RequestCount; } }

        DateTime _BaselineTimestamp;
        public DateTime BaselineTimestamp { get { return _BaselineTimestamp; } }

        List<string> _WebMethodNames;
        public List<string> WebMethodNames
        {
            get { return _WebMethodNames; }

        }

        public bool ContainsMethod(string methodName)
        {
            try
            {
                return _WebMethodNames.Contains(methodName);
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("ContainsMethod::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("ContainsMethod::Error:{0}", exc.Message)));
#endif
            }
            return false;

        }

        public void AddMethod(string methodName)
        {
            try
            {
                if (!WebMethodNames.Contains(methodName))
                    _WebMethodNames.Add(methodName);
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("AddMethod::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("AddMethod::Error:{0}", exc.Message)));
#endif
            }
        }


        public bool Validate()
        {
            DateTime now = DateTime.UtcNow;
            try
            {
                if (_BaselineTimestamp.AddMinutes(_PeriodInMinutes) < now)
                {
                    _BaselineTimestamp = now;
#if STANDALONE
                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                         string.Format("Validate::Resetting baseline timestamp for group [{0}][{1}] to {2}", _OrganizationId, _Name, now)));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                         string.Format("Validate::Resetting baseline timestamp for group [{0}][{1}] to {2}", _OrganizationId, _Name, now)));
#endif
                    _RequestCount = 0;
                }
                _RequestCount++;

#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Incrementing call counter to {3} for group [{0}][{1}] @ {2}", _OrganizationId, _Name, now, _RequestCount)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Incrementing call counter to {3} for group [{0}][{1}] @ {2}", _OrganizationId, _Name, now, _RequestCount)));
#endif


                if (_RequestCount <= _Frequency)
                {

#if STANDALONE
                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Audit passed; Count {3} of {4} for group [{0}][{1}] @  {2}", _OrganizationId, _Name, now, _RequestCount, _Frequency)));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Audit passed; Count {3} of {4} for group [{0}][{1}] @  {2}", _OrganizationId, _Name, now, _RequestCount, _Frequency)));
#endif

                    return true;
                }
                else
                {

#if STANDALONE
                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                        string.Format("Validate::Audit failed; Request count {3} exceeds maximum of {4} calls for group [{0}][{1}] @  {2}, Baseline Timestamp: {5} PeriodInMinutes: {6}", _OrganizationId, _Name, now, _RequestCount, _Frequency, _BaselineTimestamp, _PeriodInMinutes)));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                        string.Format("Validate::Audit failed; Request count {3} exceeds maximum of {4} calls for group [{0}][{1}] @  {2}, Baseline Timestamp: {5} PeriodInMinutes: {6}", _OrganizationId, _Name, now, _RequestCount, _Frequency, _BaselineTimestamp, _PeriodInMinutes)));
#endif

                    return false;
                }
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.Write(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Error:{1}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    string.Format("Validate::Error:{1}", exc.Message)));
#endif

                return false;
            }
        }

        public OrganizationAudit(int organizationId, int auditGroupId, string name, int periodInMinutes, int frequency)
        {
            try
            {
                _WebMethodNames = new List<string>();
                _AuditGroupId = auditGroupId;
                _Name = name;
                _PeriodInMinutes = periodInMinutes;
                _Frequency = frequency;
                _RequestCount = 0;
                _BaselineTimestamp = new DateTime();
                _OrganizationId = organizationId;
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("OrganizationAudit::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("OrganizationAudit::Error:{0}", exc.Message)));
#endif
            }
        }


    }

    public sealed class OrganizationAudits
    {
        const int LOCKTIMEOUT = 30000;

        public static event KeyRemovalHandler KeyRemoved;

        //const int frameNumber = 5; //3;
        //const int scanvengeperiod = 300000;
        const int scanvengeperiod = 180000;

        Timer _Scavenger;

        int _OrganizationId;
        public int OrganizationId { get { return _OrganizationId; } }

        SortedList<int, DateTime> _Users;

        SortedList<int, OrganizationAudit> _Audits;
        public SortedList<int, OrganizationAudit> Audits
        {
            get
            {
                try
                {
                    return _Audits;
                }
                catch (Exception exc)
                {
#if STANDALONE
                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("Audits::Error:{0}", exc.Message)));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("Audits::Error:{0}", exc.Message)));
#endif
                }
                return new SortedList<int, OrganizationAudit>();
            }
        }

        public OrganizationAudits(int organizationId)
        {
            try
            {
                AuditManager.Locker.AcquireWriterLock(LOCKTIMEOUT);
                try
                {
                    _Users = new SortedList<int, DateTime>();
                    _Audits = new SortedList<int, OrganizationAudit>();
                }
                finally
                {
                    AuditManager.Locker.ReleaseWriterLock();
                }
                _OrganizationId = organizationId;
                _Scavenger = new Timer(new TimerCallback(Scavenge), null, scanvengeperiod / 2, scanvengeperiod / 2);
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("OrganizationAudits::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("OrganizationAudits::Error:{0}", exc.Message)));
#endif
            }

        }

        void Scavenge(object state)
        {
            if (_Users == null)
                return;

            try
            {
                AuditManager.Locker.AcquireWriterLock(LOCKTIMEOUT);
                try
                {
                    for (int i = 0; i < _Users.Count; i++)
                    {
                        int key = _Users.Keys[i];
                        if (_Users.ContainsKey(key) && _Users[key].AddMinutes(scanvengeperiod / 60000) < DateTime.UtcNow)
                        {
#if STANDALONE
                            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                string.Format("Scavenge::Removing user key [{0}] for Organization [{1}]", key, _OrganizationId)));
#else
                                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                    string.Format("Scavenge::Removing user key [{0}] for Organization [{1}]", key, _OrganizationId)));
#endif


                            _Users.Remove(key);
                            if (KeyRemoved != null)
                                KeyRemoved(key);
                        }


                    }
                }
                finally
                {
                    AuditManager.Locker.ReleaseWriterLock();
                }
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Scavenge::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Scavenge::Error:{0}", exc.Message)));
#endif
            }
        }

        public bool Validate(int userId)
        {
            StackTrace oStack = new StackTrace(true);
            StackFrame frame = oStack.GetFrame(AuditManager.frame);
            string methodName = frame.GetMethod().Name;
            DateTime now = DateTime.UtcNow;

            for (int j = 0; j < oStack.FrameCount; j++)
            {
                string name = oStack.GetFrame(j).GetMethod().Name;
                //Trace.TraceInformation(string.Format("Frame method name [{0}] for frame # [{1}]", name, j));

            }

#if STANDALONE
            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                string.Format("Validate::Validate method name [{0}] for Organization [{1}]", methodName, _OrganizationId)));

#else
            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                string.Format("Validate::Validate method name [{0}] for Organization [{1}]", methodName, _OrganizationId)));

#endif


            try
            {
                if (!_Users.ContainsKey(userId))
                    _Users.Add(userId, now);
                else
                    _Users[userId] = now;

                if (_Audits != null)
                {
                    for (int i = 0; i < _Audits.Count; i++)
                    {
                        int auditKey = _Audits.Keys[i];
                        if (_Audits.ContainsKey(auditKey) && _Audits[auditKey].ContainsMethod(methodName))
                        {
                            bool interimResult = _Audits[auditKey].Validate();
                            //least restrictive...return if any are good...
                            if (interimResult == true)
                                return true;
                        }
                        else
                            return true;
                    }
                }
                else
                    return true;
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Validate::Error:{0}", exc.Message)));

#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Validate::Error:{0}", exc.Message)));

#endif
            }
            return false;
        }


    }

    public sealed class AuditManager
    {
        const int LOCKTIMEOUT = 30000;
        public static ReaderWriterLock Locker;
        Timer _Reloader;
        int _reloadIntervalInMinutes = 360;
        SortedList<int, OrganizationAudits> _OrganizationAuditItems;
        SortedList<int, int> _Users;
        public SortedList<int, int> Users { get { return _Users; } }
        string connectionString = string.Empty;
        public static int frame = 0;

        public AuditManager()
        {
            Locker = new ReaderWriterLock();
            _Users = new SortedList<int, int>();
            try
            {
                //connectionString = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionString"];
                connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
                frame = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AuditFrame"]);
                _reloadIntervalInMinutes = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["AuditManagerReloadIntervalInMinutes"]);
                _Reloader = new Timer(new TimerCallback(Reload), null, 0, _reloadIntervalInMinutes * 60000);
                OrganizationAudits.KeyRemoved += new KeyRemovalHandler(KeyRemoved);

            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("AuditManager::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("AuditManager::Error:{0}", exc.Message)));
#endif
            }

        }



        void Reload(object state)
        {
            LoadAuditInformation();
        }

        void LoadAuditInformation()
        {
            string xml = string.Empty;
            try
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    "LoadAuditInformation::Reloading Audit information..."));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                    "LoadAuditInformation::Reloading Audit information..."));           
#endif
                Organization org = new Organization(connectionString);

                DataSet ds = org.GetAuditGroupInfo();

                if (ds != null)
                {
#if STANDALONE
                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("LoadAuditInformation::Audit tables count:{1} ", this, ds.Tables.Count)));
#else
                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("LoadAuditInformation::Audit tables count:{1} ", this, ds.Tables.Count)));
#endif
                }
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable orgData = ds.Tables["Table"];

                    if (orgData != null)
                    {
#if STANDALONE
                        Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("LoadAuditInformation::Audit records:{1} ", this, orgData.Rows.Count)));
#else
                        Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                              string.Format("LoadAuditInformation::Audit records:{1} ", this, orgData.Rows.Count)));
#endif
                    }
                    if (orgData != null && orgData.Rows.Count > 0)
                    {
                        try
                        {
                            Locker.AcquireWriterLock(LOCKTIMEOUT);
                            try
                            {
                                _OrganizationAuditItems = new SortedList<int, OrganizationAudits>();


                                //Trace.TraceInformation("{0} :: Lock acquired:", this);
                                for (int i = 0; i < orgData.Rows.Count; i++)
                                {
                                    DataRow row = orgData.Rows[i];
                                    /*
                                        <OrganizationId>1</OrganizationId>
                                        <AuditGroupId>1</AuditGroupId>
                                        <GroupName>AuditTest</GroupName>
                                        <MethodName>AuditTest</MethodName>
                                        <Frequency>5</Frequency>
                                        <Period>5</Period>                                 
                                     */
                                    int organizationId = Convert.ToInt32(row["OrganizationId"]);
                                    int auditGroupId = Convert.ToInt32(row["AuditGroupId"]);
                                    string groupName = (string)row["GroupName"];
                                    string methodName = (string)row["MethodName"];
                                    int frequency = Convert.ToInt32(row["Frequency"]);
                                    int period = Convert.ToInt32(row["Period"]);

#if STANDALONE
                                    Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                          string.Format("LoadAuditInformation::Loading Audit -> Org:{1} AuditGroup:{2} GroupName:{3} MethodName:{4} Frequency:{5} Period:{6}",
                                                this,
                                                organizationId,
                                                auditGroupId,
                                                groupName,
                                                methodName,
                                                frequency,
                                                period
                                                )));
#else
                                          Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                          string.Format("LoadAuditInformation::Loading Audit -> Org:{1} AuditGroup:{2} GroupName:{3} MethodName:{4} Frequency:{5} Period:{6}",
                                                this,
                                                organizationId,
                                                auditGroupId,
                                                groupName,
                                                methodName,
                                                frequency,
                                                period
                                                )));                              
#endif


                                    if (!_OrganizationAuditItems.ContainsKey(organizationId))
                                        _OrganizationAuditItems.Add(organizationId, new OrganizationAudits(organizationId));

                                    if (!_OrganizationAuditItems[organizationId].Audits.ContainsKey(auditGroupId))
                                        _OrganizationAuditItems[organizationId].Audits.Add(auditGroupId, new OrganizationAudit(organizationId, auditGroupId, groupName, period, frequency));

                                    _OrganizationAuditItems[organizationId].Audits[auditGroupId].AddMethod(methodName);
                                }
                            }
                            finally
                            {
                                Locker.ReleaseWriterLock();
                            }

                        }
                        catch (Exception exc)
                        {
#if STANDALONE
                            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                                string.Format("LoadAuditInformation::Error:{0}", exc.Message)));
#else
                                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                          string.Format("LoadAuditInformation::Error:{0}", exc.Message)));

#endif
                        }

                    }
                }
                else
                    Trace.WriteLine(CLS.Util.TraceFormat("------  AuditManager not loaded ---------"));
            }
            catch { }

        }

        void KeyRemoved(int key)
        {
            if (_Users == null)
                return;

            try
            {
                Locker.AcquireWriterLock(LOCKTIMEOUT);
                try
                {
                    if (_Users.ContainsKey(key))
                    {
                        try
                        {
                            _Users.Remove(key);
#if STANDALONE
                            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                                            string.Format("KeyRemoved::Removing user key [{0}] from global cache", key)));
#else
                                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                                                string.Format("KeyRemoved::Removing user key [{0}] from global cache", key)));
#endif
                        }

                        catch (Exception exc)
                        {
#if STANDALONE
                            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                      string.Format("KeyRemoved::Error:{0}", exc.Message)));
#else
                            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                      string.Format("KeyRemoved::Error:{0}", exc.Message)));
#endif
                        }
                    }
                }
                finally
                {
                    Locker.ReleaseWriterLock();
                }
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("KeyRemoved::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("KeyRemoved::Error:{0}", exc.Message)));
#endif
            }
        }

        public bool Audit(int userId)
        {
            if (_Users == null)
                return true;
            int orgId = 0;
            try
            {
                Locker.AcquireWriterLock(LOCKTIMEOUT);
                try
                {
                    if (!_Users.ContainsKey(userId))
                    {
                        orgId = GetOrganizationId(userId);
                        if (orgId > 0)
                        {
                            if (!_Users.ContainsKey(userId))
                            {
                                _Users.Add(userId, orgId);
#if STANDALONE
                                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                    string.Format("Audit::Added User:[{0}] from orgId:[{1}]", userId, orgId)));
#else
                                    Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                        string.Format("Audit::Added User:[{0}] from orgId:[{1}]", userId, orgId)));
#endif

                            }
                        }
                        else
                            return false;
                    }
                    else
                        orgId = _Users[userId];

                    if (_OrganizationAuditItems.ContainsKey(orgId))
                    {
                        if (_OrganizationAuditItems[orgId] != null)
                            return _OrganizationAuditItems[orgId].Validate(userId);
                        else
                            return true;
                    }
                    else
                    {
                        try
                        {
                            _OrganizationAuditItems.Add(orgId, null);
                            if (_OrganizationAuditItems.ContainsKey(orgId))
                                if (_OrganizationAuditItems[orgId] != null)
                                {
                                    //_OrganizationAuditItems[orgId].KeyRemoved += new KeyRemovalHandler(KeyRemoved);
                                    return _OrganizationAuditItems[orgId].Validate(userId);
                                }
                                else
                                    return true;

                        }
                        catch (Exception exc)
                        {
#if STANDALONE
                            Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                      string.Format("OrganizationAuditItemsCheck::Error:{0}", exc.Message)));
#else
                            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                      string.Format("OrganizationAuditItemsCheck::Error:{0}", exc.Message)));
#endif
                        }
                    }

                }
                finally
                {
                    Locker.ReleaseWriterLock();
                }
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Audit::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("Audit::Error:{0}", exc.Message)));
#endif
            }
            return true;
        }

        int GetOrganizationId(int uid)
        {
            int orgid = 0;
            string xml = string.Empty;
            try
            {
                Organization org = new Organization(connectionString);
                DataSet ds = org.GetOrganizationInfoByUserId(uid);

                if (ds != null && ds.Tables.Count > 0)
                {
                    xml = ds.GetXml();
                }
                if (!string.IsNullOrEmpty(xml))
                {
                    string smatch = "<OrganizationId>";
                    string ematch = "</OrganizationId>";
                    int spos = xml.IndexOf(smatch) + smatch.Length;
                    int epos = xml.IndexOf(ematch);
                    orgid = Convert.ToInt32(xml.Substring(spos, epos - spos));
                }
            }
            catch (Exception exc)
            {
#if STANDALONE
                Trace.WriteLine(CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("GetOrganizationId::Error:{0}", exc.Message)));
#else
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                          string.Format("GetOrganizationId::Error:{0}", exc.Message)));
#endif
            }

            return orgid;
        }


    }
}
