using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.CLS
{
   public class PairT<T, U> : System.IEquatable<PairT<T, U>>
   {
      T first;
      U second;

      public PairT()
      {
      }

      public PairT(T first, U second)
      {
         this.first = first;
         this.second = second;
      }

      public T First { get { return first; } }
      public U Second { get { return second; } }

      public override string ToString()
      {
         return string.Format("( {0}, {1} )", first.ToString(), second.ToString());
      }
/*
      public override bool Equals(object obj)
      {
         if (obj is PairT<T, U>)
         {
            PairT<T, U> o = (PairT<T, U>)obj;
            return (first.Equals(o.First) && second.Equals(o.second));
         }
         return false;
      }
*/

      #region IEquatable<PairT<T,U>> Members

      bool IEquatable<PairT<T, U>>.Equals(PairT<T, U> other)
      {
         if (null != other)
            return (first.Equals(other.First) && second.Equals(other.second));

         return false;
      }

      public override int GetHashCode()
      {
         return string.Format("{0}!{1}!{2}", first, second).GetHashCode();
      }

      #endregion
   }

   public class TripleT<T, U, X> : System.IEquatable<TripleT<T, U, X>>
   {
      T first;
      U second;
      X third;

      public TripleT()
      {
      }

      public TripleT(T first, U second, X third)       {
         this.first = first;
         this.second = second;
         this.third = third;
      }

      public TripleT(T first, U second)
      {
         this.first = first;
         this.second = second;
      }

      public T First { get { return first; } }
      public U Second { get { return second; } }
      public X Third { get { return third; } }

      public override string ToString()
      {
         return string.Format("( {0}, {1}, {2} )", first.ToString(), second.ToString(), third.ToString());
      }
/*
      public override bool Equals(object obj)
      {
         if (obj is TripleT<T, U, X>)
         {
            TripleT<T, U, X> o = (TripleT<T, U, X>)obj;
            return (first.Equals(o.First) && second.Equals(o.second) && third.Equals(o.third));
         }

         return false;
      }
*/
      #region IEquatable<TripleT<T,U,X>> Members

      bool IEquatable<TripleT<T, U, X>>.Equals(TripleT<T, U, X> other)
      {
         if (null != other)
            return (first.Equals(other.First) && second.Equals(other.second) && third.Equals(other.third));

         return false;
      }

      public override int GetHashCode()
      {
         return string.Format("{0}!{1}!{2}", first, second, third).GetHashCode();
      }
      #endregion
   }

   interface IAutoTimer
   {
      long Frequency { set; get; }     ///< in miliseconds
      int Count { get; }
      int ObjectHandler(object obj);
   }

   /// <summary>
   ///      this class receives an object and a timer is called 
   /// </summary>
   public class TimelyOperation <IEnumerable> : IAutoTimer
   {
      int _count;
      long _frequency;

      #region IAutoTimer Members

      int IAutoTimer.Count
      {
         get { throw new Exception("The method or operation is not implemented."); }
      }

      int IAutoTimer.ObjectHandler(object obj)
      {
         throw new Exception("The method or operation is not implemented.");
      }

      long IAutoTimer.Frequency
      {
         get
         {
            throw new Exception("The method or operation is not implemented.");
         }
         set
         {
            throw new Exception("The method or operation is not implemented.");
         }
      }

      #endregion
   }
}
