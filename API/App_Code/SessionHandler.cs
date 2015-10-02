using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

/// <summary>
/// Summary description for SessionHandler
/// </summary>
public class SessionHandler
{
    Dictionary<int, Dictionary<string, SessionState>> _SessionsByUser;
    Dictionary<string, SessionState> _SessionsByToken;
    Dictionary<string, AuthSet> _TestAuth;

    public class AuthSet
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Token { get; set; }
    }


    public delegate void ExceptionHandler(Exception exception);
    public static event ExceptionHandler ExceptionEvent;

    static SessionHandler _Instance;

    public static SessionHandler Get
    {
        get
        {
            if (_Instance == null)
                _Instance = new SessionHandler();
            return _Instance;
        }
    }


    private SessionHandler()
    {
        _SessionsByUser = new Dictionary<int, Dictionary<string, SessionState>>();
        _SessionsByToken = new Dictionary<string, SessionState>();
        LoadAuth();
    }

    public int Organization(string token)
    {
        int orgID = 0;
        if (_TestAuth.ContainsKey(token))
        {
            orgID = _TestAuth[token].OrganizationId;
        }
        return orgID;
    }

    public int User(string token)
    {
        int userID = 0;
        if (_TestAuth.ContainsKey(token))
        {
            userID = _TestAuth[token].UserId;
        }
        return userID;
    }

    void LoadAuth()
    {
        try
        {
            _TestAuth = new Dictionary<string, AuthSet>();
            foreach (string o in ConfigurationManager.AppSettings)
            {
                if (o.StartsWith("HASH"))
                {
                    try
                    {
                        int org = 0;
                        int user = 0;
                        AuthSet auth = new AuthSet();
                        string[] s = ConfigurationManager.AppSettings[o].Split(':');
                        if (s.Length < 4)
                        {
                            throw new Exception(string.Format("Invalid Token string [{0}]", ConfigurationManager.AppSettings[o]));
                        }
                        auth.Token = s[0];
                        int.TryParse(s[1], out org);
                        auth.OrganizationId = org;
                        int.TryParse(s[3], out user);
                        auth.UserId = user;
                        auth.OrganizationName = s[2];
                        _TestAuth.Add(auth.Token, auth);
                    }
                    catch (Exception exc)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(exc);
                    }
                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
    }


    public int Authenticate(string id)
    {
        return Authenticate(id, string.Empty);
    }

    public int Authenticate(string id, string pwd)
    {
        if (_TestAuth.ContainsKey(id))
        {
            if (string.IsNullOrEmpty(pwd))
                return _TestAuth[id].UserId;
            else
                return (_TestAuth[id].Token == pwd) ? _TestAuth[id].UserId : -1;
        }
        return 0;
    }

    public SessionState Session(string id)
    {
        SessionState ss = null;
        try
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (id.Length == 24)
                {
                    if (_SessionsByToken.ContainsKey(id))
                        return _SessionsByToken[id];
                    ss = new SessionState(id);
                    return ss;
                }
                else
                {
                    if (_TestAuth.ContainsKey(id))
                    {

                    }
                    else
                        return ss;
                }
                ss = new SessionState(id);
                if (!_SessionsByUser.ContainsKey(ss.UserID))
                    _SessionsByUser.Add(ss.UserID, new Dictionary<string, SessionState>());
                _SessionsByUser[ss.UserID].Add(ss.SessionID, ss);
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return ss;
    }

}