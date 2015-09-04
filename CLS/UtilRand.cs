using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.CLS
{
   public class UtilRand
   {
      private static Random _objRandom = new Random();

      public static short RandomShort()
      {
         return (short)_objRandom.Next((int)0x0FFFF);
      }


      public static int RandomInt32()
      {
         return _objRandom.Next((int)0x0FFFFFFF);
      }

      public static int RandomInt32(int low, int high)
      {
         return _objRandom.Next(low, high);
      }

      public static long RandomLong(long low, long high)
      {
         return ((long)RandomDouble(low, high));
      }

      /// <summary>
      ///         a double between 0..1
      /// </summary>
      /// <returns></returns>
      public static double RandomDouble()
      {
         return _objRandom.NextDouble();
      }

      public static double RandomDouble(double low, double high)
      {
         return ((high - low) * _objRandom.NextDouble()) + low;
      }

      public static DateTime RandomDateTimeToday(int secs)
      {
         return new DateTime(
                     RandomLong(DateTime.Now.Ticks - secs, DateTime.Now.Ticks + secs));
      }


      public static char RandomChar()
      {
         return Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _objRandom.NextDouble() + 65)));
      }

      public static string RandomString(uint size, bool lowerCase)
      {
         StringBuilder builder = new StringBuilder();
         char ch;
         for (int i = 0; i < size; ++i)
         {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _objRandom.NextDouble() + 65)));
            builder.Append(ch);
         }
         return (lowerCase ? builder.ToString().ToLower() : builder.ToString());
      }


      public static string RandomString(int maxSize) // max size
      {
         StringBuilder builder = new StringBuilder();
         char ch;
         int size = RandomInt32(0, maxSize);
         for (int i = 0; i < size; ++i)
         {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _objRandom.NextDouble() + 65)));
            builder.Append(ch);
         }
         return builder.ToString();
      }



   }
}
