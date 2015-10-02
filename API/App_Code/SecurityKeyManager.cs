using System;
using System.Security.Cryptography;
using System.Collections;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Configuration;
using System.Diagnostics;


namespace VLF.ASI
{
    public enum ValidationResult
    {
        Ok,
        Failed,
        Expired,
        CallFrequencyExceeded
    }

    /// <summary>
    /// Public key structure.
    /// </summary>
    [Serializable]
    public class PassKeyRecord
    {
        private string passKey;
        private DateTime created;

        /// <summary>
        /// Public key creation datetime
        /// </summary>
        public DateTime Created
        {
            get { return created; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="passKey"></param>
        public PassKeyRecord(string passKey)
        {
            UpdatePassKey(passKey);
        }
        /// <summary>
        /// Update public key information
        /// </summary>
        public void UpdatePassKey(string passKey)
        {
            this.passKey = passKey;
            created = DateTime.Now;
        }
    }
    /// <summary>
    /// SecurityKeyManager is responsible for public key creation, validation, 
    /// and expiration policy
    /// This class is implemented as a Singleton
    /// </summary>
    public class SecurityKeyManager //: ASLBase
    {
        private byte[] secretKey;
        private TimeSpan expirationPeriod;
        //private AuditManager auditManager;
        private MemcachedClient mc;

        /// <summary>
        /// Constriuctor
        /// </summary>
        /// <param name="expirationPeriod">Specifies time-to-live for a single PassKey record</param>
        private SecurityKeyManager()
        {
            Guid secretKeyGuid = Guid.NewGuid();
            secretKey = new byte[16];
            mc = new MemcachedClient();
            if (mc.Get("secretKey") == null)
            {
                secretKeyGuid.ToByteArray().CopyTo(secretKey, 0);
                mc.Store(StoreMode.Set, "secretKey", secretKey);
            }

            // TODO: take expiration parameter from config file
            this.expirationPeriod = new TimeSpan(0, 25, 0); // 25 minutes default

            //try
            //{
            //    auditManager = new AuditManager();
            //}
            //catch (Exception exc)
            //{

            //}
        }

        /// <summary>
        /// Set expiration period for PassKey
        /// </summary>
        public TimeSpan ExpirationPeriod
        {
            set { expirationPeriod = value; }
        }
        /// <summary>
        /// Calculate validation hash for Key GUID
        /// </summary>
        /// <param name="inKey"></param>
        /// <returns></returns>
        protected byte[] CalcValidationHash(byte[] inKey)
        {
            // Create array to hold input which is the concatenation
            // of the Key GUID and the Secret GUID...
            byte[] hashInArray = new byte[32];

            // Create an instance of Hashing object...
            SHA1CryptoServiceProvider hashGenerator =
               new SHA1CryptoServiceProvider();

            // Combine Key and Secret into the input array...
            inKey.CopyTo(hashInArray, 0);

            secretKey = (byte[])mc.Get("secretKey");

            secretKey.CopyTo(hashInArray, 16);

            // Calculate Hash...
            byte[] hashArray = hashGenerator.ComputeHash(hashInArray);

            return hashArray;
        }

        /// <summary>
        /// Create PassKey
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string CreatePassKey(int userId)
        {

            byte[] passKeyArray = new byte[36];

            //Generate new Key...
            Guid newKeyGuid = Guid.NewGuid();

            // Get validation hash...
            byte[] hashArray = CalcValidationHash(newKeyGuid.ToByteArray());

            // Combo of Key and Hash is the Passkey...
            newKeyGuid.ToByteArray().CopyTo(passKeyArray, 0);
            hashArray.CopyTo(passKeyArray, 16);

            // Save PassKey for future reference
            string strPassKey = Convert.ToBase64String(passKeyArray);

            // if record exists, just change a passkey...

            PassKeyRecord passKeyRecord = GetPassKeyRecord(userId.ToString());

            if (passKeyRecord == null)
                passKeyRecord = new PassKeyRecord(strPassKey);
            else
                passKeyRecord.UpdatePassKey(strPassKey);

            mc.Store(StoreMode.Set, userId.ToString(), passKeyRecord);

            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("CreatePassKey( userId = {0},key={1} )", userId, strPassKey)));

            // Return PassKey as a string...
            return strPassKey;
        }

        /// <summary>
        /// Delete PassKey
        /// </summary>
        /// <param name="userId"></param>
        public void DeletePassKey(int userId)
        {
            //userKeys.Remove(userId);
            //mc.Store(StoreMode.Set, "userKeys", userKeys);
        }

        /// <summary>
        /// Validate PassKey
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passKey"></param>
        /// <returns></returns>
        public ValidationResult ValidatePasskey(int userId, string passKey)
        {
            PassKeyRecord passKeyRecord = GetPassKeyRecord(userId.ToString()); 
            if (passKeyRecord == null)
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("ValidatePasskey( userId = {0},key = {1},storedKeyCreated={2} )", userId, passKey, "Null - key from the Memcached")));
                return ValidationResult.Failed;
            }

            byte[] passedKeyArray = new Byte[16];
            byte[] passedHashArray = new Byte[20];
            byte[] reCalcHash = null;

            if (string.IsNullOrEmpty(passKey) || Convert.FromBase64String(passKey).Length < 35)
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("ValidatePasskey( userId = {0},key = {1},storedKeyCreated={2} )", userId, passKey, " passKey is empty.")));

           

            // Check if passkey have expired
            if (DateTime.Now - passKeyRecord.Created < expirationPeriod)
            {

                // The passkey must resolve to 36 byte array
                // if not a FormatException will be thrown.
                try
                {
                    // First 16 bytes (elements) are the Key...
                    Array.Copy(Convert.FromBase64String(passKey), passedKeyArray, 16);
                    // Next 20 elements are the Hash..
                    Array.Copy(Convert.FromBase64String(passKey), 16, passedHashArray, 0, 20);
                }
                catch
                {


                    // Bad Passkey...
                    return ValidationResult.Failed;
                }

                // Recalculate the Hash using the Key...
                reCalcHash = CalcValidationHash(passedKeyArray);
                // If the recalculated Hash equals the passed Hash then Valid...

                bool result = String.Compare(Convert.ToBase64String(passedHashArray), Convert.ToBase64String(reCalcHash)) == 0;
                //if (result == true && (auditManager != null))
                //    if (!auditManager.Audit(userId))
                //        return ValidationResult.CallFrequencyExceeded;




                if (result)
                    return ValidationResult.Ok;

                return ValidationResult.Failed;
            }

            Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                              CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                 string.Format("ValidatePasskeyFailed( userId = {0},passedHashArray = {1},reCalcHash={2} )", userId, Convert.ToBase64String(passedHashArray), reCalcHash == null ? "null" : Convert.ToBase64String(reCalcHash))));

            return ValidationResult.Expired;
        }

       
        private PassKeyRecord GetPassKeyRecord(string userId)
        {
            try
            {
                object obj = mc.Get(userId.ToString());
                PassKeyRecord pk=null;
                if (null != obj)
                {
                    try
                    {
                        pk=(PassKeyRecord)obj;
                    }
                    catch 
                    {
                        CreatePassKey(Convert.ToInt32(userId));
                        pk = (PassKeyRecord)mc.Get(userId.ToString());
                        Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("NewPassKeyRecord( userId = {0}) ", userId)));
                    }
                }

                    return  pk;

                

            }
            catch (Exception exc)
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces, string.Format("GetPassKeyRecord( userId = {0}, EXC={1}) ", userId, exc.Message)));
            }

            return null;
        }

        #region Singleton functionality
        private static SecurityKeyManager instance = null;
        public static SecurityKeyManager GetInstance()
        {
            lock (typeof(SecurityKeyManager))
            {
                if (instance == null)
                    instance = new SecurityKeyManager();
                return instance;
            }
        }
        #endregion Singleton functionality       
    }

    internal class SecurityKeyBypassManager
    {
        SecurityKeyBypassManager()
        {
        }

        //Get UserId from password string
        internal static int GetUserIdFromPasswordStr(string pwdStr)
        {
            int uId = -1;
            string decData = string.Empty;
            try
            {
                if (pwdStr.Contains("="))
                {
                    decData = Base64Decode(Base64Decode(pwdStr.Substring(0, pwdStr.IndexOf("=") + 1)));
                    int.TryParse(decData, out uId);
                }
            }
            catch { uId = -1; }
            return uId;
        }

        //Encode
        static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Decode
        static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //Need to rewrite (ref AHA)
        #region Singleton functionality
        //private static SecurityKeyBypassManager instance = null;
        //public static SecurityKeyBypassManager GetInstance()
        //{
        //    lock (typeof(SecurityKeyBypassManager))
        //    {
        //        if (instance == null)
        //            instance = new SecurityKeyBypassManager();
        //        return instance;
        //    }
        //}
        #endregion Singleton functionality
    }
}
