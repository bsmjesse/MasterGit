using System;
using System.IO ;

namespace VLF.ERR
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class LOG
	{
		public LOG()
		{
		
		}
		
		public static void LogFile( string directoryName, string prefix, string text )
		{
			
			string logFileName = directoryName + "/" + prefix + "_" + DateTime.Now.ToShortDateString()
				.Replace(@"/",@"-").Replace(@"\",@"-") + ".log";

			string log = DateTime.Now.ToString() + ">" + text + Convert.ToChar(13) + Convert.ToChar(10); 
			try
			{
				StreamWriter streamWriter = new StreamWriter(logFileName,true);
				streamWriter.Write(log);
				streamWriter.Flush();
				streamWriter.Close();
			}
			catch(Exception)
			{
				// Q: can't log; do nothing here?
			}
		}
	}
}
