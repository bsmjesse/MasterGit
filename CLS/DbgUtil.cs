using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.CLS
{
	public class DbgUtil
	{
      public static void MemThrow(object obj, string msg)
      {
         if (null == obj)
            throw new ArgumentNullException(msg);
      }

	}
}
