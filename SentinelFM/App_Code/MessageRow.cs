using System;

/// <summary>
/// Summary description for Message
/// Wrapper class for message 
/// </summary>
public class MessageRow
{
    #region Private Variables
        private string _messageId;
        private string _messageReferenceId;
        private string _messageFormatTypeId;
        private string _receivedTimestamp;
        private string _subject;
        private string _fieldValues;
        private string _sender;
        private string _senderId;
        private string _recipient;
        private string _isRead;
        private string _messageStatus;
    #endregion
    #region Constractors
        public MessageRow() { }
        public MessageRow(string messageId, string messageReferenceId, string messageFormatTypeId, string receivedTimestamp, string subject, string fieldValues, string sender,string senderId, string recipient, string isRead, string MessageStatus)
        {
            this._messageId = messageId;
            this._messageReferenceId = messageReferenceId;
            this._messageFormatTypeId = messageFormatTypeId;
            this._receivedTimestamp = receivedTimestamp;
            this._subject = subject;
            this._fieldValues = fieldValues;
            this._sender = sender;
            this._recipient = recipient;
            this._isRead = isRead;
            this._messageStatus = MessageStatus;
        }
    #endregion
    #region Getter/Setter 
        public string MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }
        public string MessageReferenceId
        {
            get { return _messageReferenceId; }
            set { _messageReferenceId = value; }
        }
        public string MessageFormatTypeId
        {
            get { return _messageFormatTypeId; }
            set { _messageFormatTypeId = value; }
        }
    
        public string ReceivedTimestamp
        {
            get { return _receivedTimestamp; }
            set { _receivedTimestamp = value; }
        }
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }
        public string FieldValues
        {
            get { return _fieldValues; }
            set { _fieldValues = value; }
        }
        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public string SenderId
        {
            get { return _senderId; }
            set { _senderId = value; }
        }
        public string Recipient
        {
            get { return _recipient; }
            set { _recipient = value; }
        }
        public string IsRead
        {
            get { return _isRead; }
            set { _isRead = value; }
        }
        public string MessageStatus
        {
            get { return _messageStatus; }
            set { _messageStatus = value; }
        }
    #endregion
}
