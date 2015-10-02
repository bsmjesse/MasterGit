using System;
using System.Collections;	// for SortedList
using System.Data;			// for DataSet
using System.Data.SqlClient ;	// for SqlException
using VLF.DAS.DB;
using VLF.ERR;
using VLF.CLS;
using VLF.CLS.Def;

namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to box functionality in database
    /// </summary>
    public class FirmwareManager : Das
    {
        //private BoxCommInfo boxCommInfo = null;
        private DB.FirmwareOTA firmware;
        
        #region Implement General Interfaces
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString"></param>
        public FirmwareManager(string connectionString) : base(connectionString)
		{
			//boxCommInfo = new BoxCommInfo(sqlExec);
			firmware = new DB.FirmwareOTA(sqlExec);
        }

		/// <summary>
		/// Distructor
		/// </summary>
		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

        public void Save(Object obj)
        {
            if (obj is FirmwareInfo)
            {
                FirmwareInfo f = (FirmwareInfo) obj;
                firmware.Save(f);
            }
        }
        
        public void Delete(int id)
        {

        }
        

    }

}