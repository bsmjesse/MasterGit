
using NLog;
using System;
using System.Diagnostics; // for EventLog

namespace VLF.ERR
{
    #region VLF Exceptions
    public class VLFException : ApplicationException
    {
        protected static int errorCount = 0;
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private void ErrorCount()
        {
            logger.Error("Error Count: {0}", errorCount);
            errorCount++;
        }

        public VLFException(string message) : base(message)
        {
            logger.Error(message, this);
            ErrorCount();
        }
        public VLFException() : base()
        {
            logger.Error(this);
            ErrorCount();
        }
        /// <summary>
        /// Get detail log message (Source,Message,Call Stack)
        /// </summary>
        public string DetailLogMsg
        {
            get
            {
                return this.Message;
            }
            //			get
            //			{
            //				return ("Source: " + this.Source + Convert.ToChar(13) + Convert.ToChar(10) + 
            //					Convert.ToChar(13) + Convert.ToChar(10) +	// empty line
            //					"Exception type: " + this.GetType() + Convert.ToChar(13) + Convert.ToChar(10) + 
            //					Convert.ToChar(13) + Convert.ToChar(10) +	// empty line
            //					"Message: " + this.Message + Convert.ToChar(13) + Convert.ToChar(10) + 
            //					Convert.ToChar(13) + Convert.ToChar(10) +	// empty line
            //					"Call Stack: " + this.StackTrace + Convert.ToChar(13) + Convert.ToChar(10) + 
            //					"----------------------------------------------------------------------------");
            //			}
        }
    }

    public class VLFNotImplementedException : VLFException
    {
        public VLFNotImplementedException(string message) : base(message) { }
        public VLFNotImplementedException() : base() { }
    }

    public class VLFInstantiationFailedException : VLFException
    {
        public VLFInstantiationFailedException(string message) : base(message) { }
        public VLFInstantiationFailedException() : base() { }
    }

    #endregion

    #region ACM/DCL Exceptions
    public class VLFServiceException : VLFException
    {
        public VLFServiceException(string message) : base(message) { }
        public VLFServiceException() : base() { }
    }

    public class ACMCannotRegisterServiceException : VLFServiceException
    {
        public ACMCannotRegisterServiceException(string message) : base(message) { }
        public ACMCannotRegisterServiceException() : base() { }
    }

    public class ACMCannotStartServiceException : VLFServiceException
    {
        public ACMCannotStartServiceException(string message) : base(message) { }
        public ACMCannotStartServiceException() : base() { }
    }

    #endregion ACM/DCL Exceptions

    #region DAS Exceptions
    public class DASException : VLFException
    {
        public DASException(string message) : base(message)
        {
        }
        public DASException() : base() { }
    }

    #region DAS Database Exceptions
    public class DASDbException : DASException
    {
        public DASDbException(string message) : base(message) { }
        public DASDbException() : base() { }
    }
    public class DASDbConnectionClosed : VLFException
    {
        public DASDbConnectionClosed(string message) : base(message) { }
        public DASDbConnectionClosed() : base() { }
    }
    #endregion

    #region DAS Application Exceptions
    /// <summary>
    /// General Application Exceptions
    /// </summary>
    public class DASAppException : DASException
    {
        public DASAppException(string message) : base(message)
        {
        }
        public DASAppException() : base() { }

    }
    /// <summary>
    /// SqlException.ErrorCode = -2147217901
    /// </summary>
    public class DASAppInvalidValueException : DASAppException
    {
        public DASAppInvalidValueException(string message) : base(message) { }
        public DASAppInvalidValueException() : base() { }
    }
    /// <summary>
    /// SqlException.ErrorCode = -2147217873
    /// A specified value violated the integrity constraints for a column or table
    /// </summary>
    public class DASAppViolatedIntegrityConstraintsException : DASAppException
    {
        public DASAppViolatedIntegrityConstraintsException(string message) : base(message) { }
        public DASAppViolatedIntegrityConstraintsException() : base() { }
    }
    /// <summary>
    /// SqlException.ErrorCode = -2147217862
    /// Precision was invalid
    /// </summary>
    public class DASAppInvalidPrecisionException : DASAppException
    {
        public DASAppInvalidPrecisionException(string message) : base(message) { }
        public DASAppInvalidPrecisionException() : base() { }
    }
    /// <summary>
    /// SqlException.ErrorCode = -2147217861
    /// A specified scale was invalid
    /// </summary>
    public class DASAppInvalidScaleException : DASAppException
    {
        public DASAppInvalidScaleException(string message) : base(message) { }
        public DASAppInvalidScaleException() : base() { }
    }
    /// <summary>
    /// SqlException.ErrorCode = -2147217833
    /// A literal value in the command overflowed the range of the type of the associated column
    /// </summary>
    public class DASAppLiteralRangeOverflowedException : DASAppException
    {
        public DASAppLiteralRangeOverflowedException(string message) : base(message) { }
        public DASAppLiteralRangeOverflowedException() : base() { }
    }
    /// <summary>
    /// A specified database request has not been found. Return result is empty.
    /// </summary>
    public class DASAppResultNotFoundException : DASAppException
    {
        public DASAppResultNotFoundException(string message) : base(message)
        {
        }
        public DASAppResultNotFoundException() : base() { }
    }
    /// <summary>
    /// A specified database already exists.
    /// </summary>
    public class DASAppDataAlreadyExistsException : DASAppException
    {
        public DASAppDataAlreadyExistsException(string message) : base(message)
        {
        }
        public DASAppDataAlreadyExistsException() : base() { }
    }
    /// <summary>
    /// Duplicated message.
    /// </summary>
    public class DASAppDuplicatedMessageException : DASAppException
    {
        public DASAppDuplicatedMessageException(string message) : base(message)
        {
        }
        public DASAppDuplicatedMessageException() : base() { }
    }
    /// <summary>
    /// Wrong result has been found.
    /// Example: 
    ///  - multiple results for unique field.
    /// </summary>
    public class DASAppWrongResultException : DASAppException
    {
        public DASAppWrongResultException(string message) : base(message) { }
        public DASAppWrongResultException() : base() { }
    }
    public class DASConnectionsLimitReached : DASAppException
    {
        public DASConnectionsLimitReached(string message) : base(message) { }
        public DASConnectionsLimitReached() : base() { }
    }
    public class DASAuthorizationException : DASAppException
    {
        public DASAuthorizationException(string message) : base(message) { }
        public DASAuthorizationException() : base() { }
    }

    #endregion

    #endregion

    #region DCS Exceptions
    public class DCSException : VLFException
    {
        public DCSException(string message) : base(message) { }
        public DCSException() : base() { }
    }

    public class DCSExceedingSocketNumsException : DCSException
    {
        public DCSExceedingSocketNumsException(string message) : base(message) { }
        public DCSExceedingSocketNumsException() : base() { }
    }

    public class DCSCannotBindSocketException : DCSException
    {
        public DCSCannotBindSocketException(string message) : base(message) { }
        public DCSCannotBindSocketException() : base() { }
    }

    public class DCSNullIndexException : DCSException
    {
        public DCSNullIndexException(string message) : base(message) { }
        public DCSNullIndexException() : base() { }
    }

    public class DCSForbiddenCallException : DCSException
    {
        public DCSForbiddenCallException(string message) : base(message) { }
        public DCSForbiddenCallException() : base() { }
    }

    public class DCSCanNotOpenComPortException : DCSException
    {
        public DCSCanNotOpenComPortException(string message) : base(message) { }
        public DCSCanNotOpenComPortException() : base() { }
    }

    public class DCSChannelNotFoundException : DCSException
    {
        public DCSChannelNotFoundException(string message) : base(message) { }
        public DCSChannelNotFoundException() : base() { }
    }

    public class DCSNoFreeChannelException : DCSException
    {
        public DCSNoFreeChannelException(string message) : base(message) { }
        public DCSNoFreeChannelException() : base() { }
    }

    public class DCSChannelAlreadyExistsException : DCSException
    {
        public DCSChannelAlreadyExistsException(string message) : base(message) { }
        public DCSChannelAlreadyExistsException() : base() { }
    }

    public class DCSNotInitializedException : DCSException
    {
        public DCSNotInitializedException(string message) : base(message) { }
        public DCSNotInitializedException() : base() { }
    }

    #endregion

    #region Pas Exceptions
    public class PASException : VLFException
    {
        public PASException(string message) : base(message) { }
        public PASException() : base() { }
    }

    public class PASSOPEOPException : PASException
    {
        public PASSOPEOPException(string message) : base(message) { }
        public PASSOPEOPException() : base() { }
    }

    public class PASProtocolException : PASException
    {
        public PASProtocolException(string message) : base(message) { }
        public PASProtocolException() : base() { }
    }

    public class PASProtocolChecksumException : PASException
    {
        public PASProtocolChecksumException(string message) : base(message) { }
        public PASProtocolChecksumException() : base() { }
    }

    public class PASWrongProtocolException : PASException
    {
        public PASWrongProtocolException(string message) : base(message) { }
        public PASWrongProtocolException() : base() { }
    }

    public class PASCommandNotSupportedException : PASException
    {
        public PASCommandNotSupportedException(string message) : base(message) { }
        public PASCommandNotSupportedException() : base() { }
    }
    public class PASServiceMessageException : PASException
    {
        public PASServiceMessageException(string message) : base(message) { }
        public PASServiceMessageException() : base() { }
    }

    #endregion

    #region ASI Exceptions

    public class ASIException : VLFException
    {
        public ASIException(string message) : base(message) { }
        public ASIException() : base() { }
    }

    public class ASIAuthorizationFailedException : ASIException
    {
        public ASIAuthorizationFailedException(string message) : base(message) { }
        public ASIAuthorizationFailedException() : base() { }
    }

    public class ASICallFrequencyExceededException : ASIException
    {
        public ASICallFrequencyExceededException(string message) : base(message) { }
        public ASICallFrequencyExceededException() : base() { }
    }

    public class ASIAuthenticationFailedException : ASIException
    {
        public ASIAuthenticationFailedException(string message) : base(message) { }
        public ASIAuthenticationFailedException() : base() { }
    }

    public class ASIPermissionDeniedException : ASIException
    {
        public ASIPermissionDeniedException(string message) : base(message) { }
        public ASIPermissionDeniedException() : base() { }
    }

    public class ASIPassKeyExpiredException : ASIException
    {
        public ASIPassKeyExpiredException(string message) : base(message) { }
        public ASIPassKeyExpiredException() : base() { }
    }

    public class ASIDataNotFoundException : ASIException
    {
        public ASIDataNotFoundException(string message) : base(message) { }
        public ASIDataNotFoundException() : base() { }
    }

    public class ASISessionNotFound : ASIException
    {
        public ASISessionNotFound(string message) : base(message) { }
        public ASISessionNotFound() : base() { }
    }

    public class ASIServerBusy : ASIException
    {
        public ASIServerBusy(string message) : base(message) { }
        public ASIServerBusy() : base() { }
    }

    public class ASIWrongOperation : ASIException
    {
        public ASIWrongOperation(string message) : base(message) { }
        public ASIWrongOperation() : base() { }
    }

    public class ASIUserExpired : ASIException
    {
        public ASIUserExpired(string message) : base(message) { }
        public ASIUserExpired() : base() { }
    }
    #endregion ASI Exceptions

    #region SLS Exceptions

    public class SLSException : VLFException
    {
        public SLSException(string message) : base(message) { }
        public SLSException() : base() { }
    }

    public class SLSSessionNotFound : SLSException
    {
        public SLSSessionNotFound(string message) : base(message) { }
        public SLSSessionNotFound() : base() { }
    }

    public class SLSVehicleConfigurationErrorException : ASIException
    {
        public SLSVehicleConfigurationErrorException(string message) : base(message) { }
        public SLSVehicleConfigurationErrorException() : base() { }
    }

    public class SLSOperationNotSupported : ASIException
    {
        public SLSOperationNotSupported(string message) : base(message) { }
        public SLSOperationNotSupported() : base() { }
    }

    public class SLSCommunicationInformationErrorException : ASIException
    {
        public SLSCommunicationInformationErrorException(string message) : base(message) { }
        public SLSCommunicationInformationErrorException() : base() { }
    }

    public class SLSSessionIsBusyException : ASIException
    {
        public SLSSessionIsBusyException(string message) : base(message) { }
        public SLSSessionIsBusyException() : base() { }
    }
    #endregion ASI Exceptions

    #region AMS Exceptions

    public class AMSException : VLFException
    {
        public AMSException(string message) : base(message) { }
        public AMSException() : base() { }
    }
    public class AMSVehicleConfigurationErrorException : ASIException
    {
        public AMSVehicleConfigurationErrorException(string message) : base(message) { }
        public AMSVehicleConfigurationErrorException() : base() { }
    }

    public class AMSOperationNotSupported : ASIException
    {
        public AMSOperationNotSupported(string message) : base(message) { }
        public AMSOperationNotSupported() : base() { }
    }
    #endregion AMS Exceptions

    #region Mail Exceptions

    public class MailException : VLFException
    {
        public MailException(string message) : base(message) { }
        public MailException() : base() { }
    }
    public class MailWrongAuthenticationException : MailException
    {
        public MailWrongAuthenticationException(string message) : base(message) { }
        public MailWrongAuthenticationException() : base() { }
    }
    #endregion Mail Exceptions

    #region WMI Exceptions
    public class WMIException : ApplicationException
    {
        public WMIException(string message) : base(message) { }
        public WMIException() : base() { }
        /// <summary>
        /// Get detail log message (Source,Message,Call Stack)
        /// </summary>
        public string DetailLogMsg
        {
            get
            {
                return ("Source: " + this.Source + Convert.ToChar(13) + Convert.ToChar(10) +
                    Convert.ToChar(13) + Convert.ToChar(10) +   // empty line
                    "Exception type: " + this.GetType() + Convert.ToChar(13) + Convert.ToChar(10) +
                    Convert.ToChar(13) + Convert.ToChar(10) +   // empty line
                    "Message: " + this.Message + Convert.ToChar(13) + Convert.ToChar(10) +
                    Convert.ToChar(13) + Convert.ToChar(10) +   // empty line
                    "Call Stack: " + this.StackTrace + Convert.ToChar(13) + Convert.ToChar(10) +
                    "----------------------------------------------------------------------------");
            }
        }
    }
    #endregion

    #region MAP Exceptions
    public class MapException : VLFException
    {
        public MapException(string message) : base(message) { }
        public MapException() : base() { }
    }
    public class UninitializedMapServiceException : MapException
    {
        public UninitializedMapServiceException(string message) : base(message) { }
        public UninitializedMapServiceException() : base() { }
    }
    public class RenderServiceSoapException : MapException
    {
        public RenderServiceSoapException(string message) : base(message) { }
        public RenderServiceSoapException() : base() { }
    }
    public class FindServiceSoapException : MapException
    {
        public FindServiceSoapException(string message) : base(message) { }
        public FindServiceSoapException() : base() { }
    }
    public class PolygonsNotSupportedException : MapException
    {
        public PolygonsNotSupportedException(string message) : base(message) { }
        public PolygonsNotSupportedException() : base() { }
    }
    #endregion MAP Exceptions
}
