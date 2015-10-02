using System;
using Microsoft.Reporting.WebForms;
/// <summary>
/// Summary description for ReportServerCredential
/// </summary>
public class ReportServerCredentials : IReportServerCredentials 
{

    protected string username;
    protected string pwd;
    protected string domain;

	public ReportServerCredentials(string UserName, string Password, string Domain)
    {
        this.username = UserName;
        this.pwd = Password;
        this.domain = Domain;
    }

    public System.Security.Principal.WindowsIdentity ImpersonationUser
    {
        get
        {
            return null;  // Use default identity.
        }
    }

    public System.Net.ICredentials NetworkCredentials
    {
        get
        {
            return new System.Net.NetworkCredential(username, pwd, domain);
        }
    }

    public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string password, out string authority)
    {
        authCookie = null;
        user = password = authority = null;
        return false;  // Not use forms credentials to authenticate.
    }
}