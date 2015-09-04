using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;

namespace VLF.CLS
{
   /// <summary>
   /// JSON like class represents key-value pairs storage
   /// </summary>
   public class Json : IDisposable
   {
      private char _classSeparator;
      private char _keySeparator;
      private char _valueSeparator;
      private string _class;
      private Dictionary<string, string> _storage = new Dictionary<string, string>();

      /// <summary>
      /// Default constructor
      /// </summary>
      public Json()
      {
         _keySeparator = ';';
         _valueSeparator = '=';
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="keySeparator"></param>
      /// <param name="valueSeparator"></param>
      public Json(char keySeparator, char valueSeparator)
      {
         _keySeparator = keySeparator;
         _valueSeparator = valueSeparator;
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="clsSeparator"></param>
      /// <param name="keySeparator"></param>
      /// <param name="valueSeparator"></param>
      public Json(char clsSeparator, char keySeparator, char valueSeparator)
      {
         _classSeparator = clsSeparator;
         _keySeparator = keySeparator;
         _valueSeparator = valueSeparator;
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="paramString"></param>
      public Json(string paramString)
      {
          _keySeparator = ';';
          _valueSeparator = '=';
          SetList(paramString);
      }

      /// <summary>
      /// Constructor - no class
      /// </summary>
      /// <param name="paramString">Parameters string</param>
      /// <param name="keySeparator"></param>
      /// <param name="valueSeparator"></param>
      public Json(string paramString, char keySeparator, char valueSeparator)
      {
         _keySeparator = keySeparator;
         _valueSeparator = valueSeparator;
         SetList(paramString);
      }

      /// <summary>
      /// Constructor - class
      /// </summary>
      /// <param name="paramString">Parameters string</param>
      /// <param name="hdrSeparator"></param>
      /// <param name="keySeparator"></param>
      /// <param name="valueSeparator"></param>
      public Json(string paramString, char clsSeparator, char keySeparator, char valueSeparator)
      {
         _classSeparator = clsSeparator;
         _keySeparator = keySeparator;
         _valueSeparator = valueSeparator;
         SetList(paramString);
      }

      /// <summary>
      /// Parse key value pairs string into sorted list
      /// </summary>
      /// <param name="keyValuePairsString"></param>
      private void SetList(string dataString)
      {
         string keyValues = "";
         if (!String.IsNullOrEmpty(_classSeparator.ToString()) && dataString.Contains(_classSeparator.ToString()))
         {
            string[] data = dataString.Split(_classSeparator);
            if (data == null || data.Length < 2)
               return;
            _class = data[0].Trim();
            keyValues = dataString.Substring(dataString.IndexOf(_classSeparator) + 1);
         }
         else
            keyValues = dataString.Trim();
         string[] pairs = keyValues.Split(new char[] { _keySeparator }, StringSplitOptions.RemoveEmptyEntries);
         if (pairs == null || pairs.Length == 0)
            return;
         foreach (string pair in pairs)
         {
            if (!String.IsNullOrEmpty(pair) && pair.Contains(_valueSeparator.ToString()))
            {
               string[] args = pair.Split(_valueSeparator);
               Add(args[0], args[1]);
            }
         }
      }

      /// <summary>
      /// Add new entry if key doesn't exist
      /// </summary>
      /// <param name="key">Key</param>
      /// <param name="value">Object Value</param>
      public void Add(string key, string value)
      {
         if (!_storage.ContainsKey(key.Trim()))
            _storage.Add(key.Trim(), value);
         else
            Trace.WriteLine(String.Format("Current: {0} :: Duplicate key: {1}={2}", this.ToString(), key, value));
      }

      /// <summary>
      /// Item
      /// </summary>
      /// <param name="key"></param>
      /// <returns>value associated with the specified key</returns>
      public string this[string key]
      {
         get 
         {
            string result;
            if (_storage.TryGetValue(key.Trim(), out result))
               return result;
            else
               return "";
         }
         set 
         {
            if (_storage.ContainsKey(key.Trim()))
               _storage[key.Trim()] = value;
            else
               _storage.Add(key.Trim(), value);
         }
      }

      /// <summary>
      /// Check if an object Contains Key
      /// </summary>
      /// <param name="key"></param>
      /// <returns>True if yes, false if not</returns>
      public bool ContainsKey(string key)
      {
         return _storage.ContainsKey(key.Trim());
      }

      /// <summary>
      ///   check if keys are in the storage
      /// </summary>
      /// <param name="keys"></param>
      /// <returns></returns>
      public bool ContainsKeys(string[] keys)
      {
         if (null != keys && keys.Length > 0)
         {
            bool bRes = true ;
            foreach (string key in keys)
            {
               bRes &= ContainsKey(key);
               if (!bRes)
                  return false;
            }
            return bRes;
         }

         return true;
      }
      /// <summary>
      /// Check if an object Contains Value
      /// </summary>
      /// <param name="value"></param>
      /// <returns>True if yes, false if not</returns>
      public bool ContainsValue(string value)
      {
         return _storage.ContainsValue(value);
      }

      /// <summary>
      /// Remove key value pair by key
      /// </summary>
      /// <param name="key"></param>
      public void Remove(string key)
      {
         if (_storage.ContainsKey(key))
            _storage.Remove(key);
         else
            Trace.WriteLine(String.Format("Remove Key: [{0}] Not Found", key));
      }

      public ICollection Keys
      {
         get { return _storage.Keys; }
      }

      public ICollection Values
      {
         get { return _storage.Values; }
      }

      /// <summary>
      /// Get string representation of the key-value pairs list
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         StringBuilder sb = new StringBuilder();
         if (!String.IsNullOrEmpty(_class))
            sb.AppendFormat("{0}{1}", _class, _classSeparator);
         foreach (KeyValuePair<string, string> entry in _storage)
         {
            sb.AppendFormat("{0}{1}{2}{3}",
               entry.Key, _valueSeparator, entry.Value, _keySeparator);
         }
         return sb.ToString();
      }

      /// <summary>
      /// Clears the hashtable
      /// </summary>
      public void Clear()
      {
         this._storage.Clear();
      }

      /// <summary>
      /// Number of key-value pairs
      /// </summary>
      public int Count
      {
         get { return _storage.Count; }
      }

      /// <summary>
      /// Key - value delimiter
      /// </summary>
      public char KeyDelimiter
      {
         get { return _keySeparator; }
         set { _keySeparator = value; }
      }

      /// <summary>
      /// Pair delimiter
      /// </summary>
      public char ValueDelimiter
      {
         get { return _valueSeparator; }
         set { _valueSeparator = value; }
      }

      /// <summary>
      /// Header delimiter
      /// </summary>
      public char ClassDelimiter
      {
         get { return _classSeparator; }
         set { _classSeparator = value; }
      }

      /// <summary>
      /// Header
      /// </summary>
      public string ClassName
      {
         get { return _class; }
         set { _class = value; }
      }

      #region IDisposable Members

      void IDisposable.Dispose()
      {
         this._storage.Clear();
         this._storage = null;
      }

      #endregion
   }
}
