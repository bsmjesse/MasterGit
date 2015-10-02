using System;
using System.Data ;


namespace SentinelFM
{
	/// <summary>
	/// Summary description for SentinelFMSession.
	/// </summary>
	public class SentinelFMSession
	{
		public long dbgSessionID ;
		private string	userName ;
		private int		userID ;
		private string	password ;
		private string  secId ;
		private clsUser user;
		
		private clsAdmin admin;
		
		protected string messageText="";
		protected string activeMenu="lnkHome";

		public string ActiveMenu
		{
			get{return activeMenu;}
			set{activeMenu = value;}
		}

		public clsAdmin Admin
		{
			get{return admin;}
			set{admin = value;}
		}


		public clsUser User
		{
			get{return user;}
			set{user = value;}
		}
		

		public string MessageText
		{
			get{return messageText;}
			set{messageText = value;}
		}

		
		
		
	
		public int UserID
		{
			get{ return userID ; }
			set{ userID = value ; }
		}
		public string UserName
		{
			get{return userName;}
			set{userName = value;}
		}
		
		public string Password
		{
			get{return password;}
			set{password = value;}
		}

		public string SecId
		{
			get{return secId;}
			set{secId = value;}
		}


		
		
		public SentinelFMSession()
		{
			 dbgSessionID = counter++ ;
			 user=new clsUser(); 
 			 admin=new clsAdmin(); 
		}


	
		private static long counter = 0;
	}
}
