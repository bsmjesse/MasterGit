using System;
using System.Collections.Generic;
using System.Text;
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;
using System.Data;

namespace VLF.DAS.Logic
{
    public class LogEvent : IDisposable
    {
        private Logging logger;
        private bool isDisposed = false;
        public LogEvent(string connStr)
        {
            logger = new Logging(connStr);
        }

        public bool SaveToLog(string moduleName, int userId, int organizationId, string tableName, string primaryWhere, string action, 
            string remoteAddr, string applicationName, string sql, string description = null)
        {
            bool result = false;
            try
            {
                result = logger.SaveLog(moduleName, userId, organizationId, tableName, primaryWhere, action, remoteAddr,
                               applicationName, sql, description);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return result;
        }

        public bool ValidatePasswordInLastEight(string newPassword, int userId)
        {
            bool result = false;
            try
            {
                result = logger.ValidatePasswordInLastEight(newPassword, userId);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return result;
        }

        /// <summary>
        /// Retrieves current HGI user
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <param name="LoginUserSecId"></param>
        /// <returns>UserId, OrganizationId</returns>
        public DataSet GetCurrentHGIUser(int LoginUserId, string LoginUserSecId)
        {
            return logger.GetCurrentHGIUser(LoginUserId, LoginUserSecId);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool doingDispose)
        {
            if (!this.isDisposed)
            {
                if (doingDispose)
                {
                    logger.Dispose();
                    logger = null;
                    //Release all managed resources
                }
                //Release all Unmanaged resources over here.
                //So if doingDispose is FALSE, then only unmanaged resources will be released.
            }
            this.isDisposed = true;
        }
    }
}
