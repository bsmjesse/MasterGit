using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace VLF.CLS
{

   /// <summary>
   ///      the class prevents the problem where the tracelisteners are modified during a write operation
   ///      Trace.WriteXXX iterates through listeners and if the write operation changes the list
   ///      it will throw an exception
   /// </summary>
   /// <comment>
   ///      plus is much faster than the previous implementation 
   /// </comment>
	public class File4DayTraceListener : TextWriterTraceListener
	{
      DateTime _dtCreated;
      object _obj = new object();
           

      private bool RenewFile()
      {
         lock (_obj)
         {
            if (DateTime.Now.DayOfYear == _dtCreated.DayOfYear)
               return false;

            // get short current date
            string newData = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-");

            // find date section in full log file name
            int before = Name.LastIndexOf('_');
            int after = Name.LastIndexOf('.');
            string oldData = Name.Substring(before + 1, after - before - 1);

            // haven't changed - use current log file
            if (newData == oldData)
               return false;

            _dtCreated = DateTime.Now;
/*           
            // close old log file and open new one
            Flush();
            Trace.Listeners.Remove(Name);
            Close();

            // open new Trace Listener
            string newFileName = Name.Substring(0, before + 1) + newData + Name.Substring(after);
            File4DayTraceListener logFile = new File4DayTraceListener(newFileName, newFileName);
            Trace.Listeners.Add(logFile);

            return true;
*/

            base.Writer.Flush();
            base.Writer.Close();
            string newFileName = Name.Substring(0, before + 1) + newData + Name.Substring(after);
            base.Writer = new StreamWriter(newFileName);
            Name = newFileName;

            return false;
         }

      }
      private bool CheckDate()
      {
         if (DateTime.Now.DayOfYear ==_dtCreated.DayOfYear )
             return false;

         return RenewFile();
      }
      public File4DayTraceListener(string fileName, string name) :
         base(fileName, name)
      {
         _dtCreated = DateTime.Now;
      }

      public override void Write(object o)
      {
         if (false == CheckDate())
            base.Write(o);
      }

      public override void Write(object o, string category)
      {
         if (false == CheckDate())
            base.Write(o, category);
      }

      public override void Write(string message)
      {
         if (false == CheckDate())
            base.Write(message);
      }

      public override void Write(string message, string category)
      {
         if (false == CheckDate())
            base.Write(message, category);
      }

      public override void WriteLine(object o)
      {
         if (false == CheckDate())
            base.WriteLine(o);
      }

      public override void WriteLine(object o, string category)
      {
         if (false == CheckDate())
            base.WriteLine(o, category);
      }

      public override void WriteLine(string message)
      {
         if (false == CheckDate())
            base.WriteLine(message);
      }

      public override void WriteLine(string message, string category)
      {
         if (false == CheckDate())
            base.WriteLine(message, category);
      }

      protected override void WriteIndent()
      {
         if (false == CheckDate())
            base.WriteIndent();
      }
	}
}
