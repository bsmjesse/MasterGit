using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;

/// <summary>
/// Summary description for SessionState
/// </summary>
public class SessionState
{
    string _SessionID;
    public string SessionID { get { return _SessionID; } }

    string _User;
    public string User { get { return _User; } }

    string _Token;
    public string Token { get { return _Token; } }

    int _UserID;
    public int UserID { get { return _UserID; } }

    bool _IsAuthenticated;
    public bool IsAuthenticated { get { return _IsAuthenticated; } }

    DateTime _Created;
    public DateTime Created { get { return _Created; } }

    DateTime _Touched;
    public DateTime Touched { get { return _Created; } }

    public SessionState(string id)
    {
        _Touched = _Created = DateTime.UtcNow;
        if (id.Length == 24)
        {
            _Token = id;
            _IsAuthenticated = SessionHandler.Get.Authenticate(id) > 0;
        }
        else
        {
            _User = id;
            _UserID = 892349;
        }
        _SessionID = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public SessionState(string id, string hash, string passkey)
    {
        _Touched = DateTime.UtcNow;
        string pwd = "";
        _UserID = SessionHandler.Get.Authenticate(id, pwd);
        _IsAuthenticated = _UserID > 0;
    }
}