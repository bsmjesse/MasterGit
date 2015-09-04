using System;

namespace SentinelFM
{

    /// <summary>
    /// Summary description for SentinelFMSessionBase
    /// </summary>
    /// 
    [Serializable]
    public class SentinelFMSessionBase
    {
        protected string userName;
        protected string key;
        protected int userID;
        protected string password;
        protected string secId;
        protected int superOrganizationId;
        protected string companyURL = "";
        protected string homePagePicture = "";
        protected string emailID;
        protected int loginUserID;
        


        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string SecId
        {
            get { return secId; }
            set { secId = value; }
        }

        public Int32 SuperOrganizationId
        {
            get { return superOrganizationId; }
            set { superOrganizationId = value; }
        }

        public string CompanyURL
        {
            get { return companyURL; }
            set { companyURL = value; }
        }

        public string HomePagePicture
        {
            get { return homePagePicture; }
            set { homePagePicture = value; }
        }

        protected string selectedLanguage = "en-US";
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set { selectedLanguage = value; }
        }

        protected Int16 interfacePrefrence = 0;
        public Int16 InterfacePrefrence
        {
            get { return interfacePrefrence; }
            set { interfacePrefrence = value; }
        }


        protected string previousDateFormat = " ";
        public string PreviousDateFormat
        {
            get { return previousDateFormat; }
            set { previousDateFormat = value; }
        }

        public int LoginUserID
        {
            get { return loginUserID; }
            set { loginUserID = value; }
        }

        public SentinelFMSessionBase()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}