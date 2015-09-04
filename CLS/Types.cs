using System;

namespace VLF.CLS.Types
{
	public class UDoubleLong
	{
		private ulong lowLong;  //64 bits 
		private ulong highLong; //64 bits 
		private int index;
		public int Index
		{
			get{return index;}
			set
			{
				if(value > 128)
					index=128;
				else
					index=value;
			}
		}
		public ulong LowLong
		{
			get{return lowLong;}
			set{lowLong=value;}
		}
		public ulong HighLong
		{
			get{return highLong;}
			set{highLong=value;}
		}
		private void Initialize( int numBits, byte[] values )
		{
			index = numBits;
			lowLong = 0;
			highLong = 0;
			for(int i=0;i<values.Length;i++)
			{
				if(i>=21)
				{
					ulong tmp = (ulong)lowLong&0xE000000000000000;
					tmp >>= 61;
					highLong <<= 3;
					highLong |= (ulong)tmp;
				}
				lowLong <<= 3;
				lowLong |= (ulong)values[i]&0x0F;
			}
		}
		public UDoubleLong( ulong lowLong, ulong highLong, int numBits )
		{
			this.lowLong = lowLong;
			this.highLong = highLong;
			index = numBits;
		}
		public UDoubleLong(int numBits, byte[] values, int startIndex)
		{
			byte[] newPacket = new byte[values.Length-startIndex];
			Array.Copy(values,startIndex,newPacket,0,values.Length-startIndex);
			Initialize(numBits,newPacket);
		}

		public UDoubleLong(int numBits, byte[] values)
		{
			Initialize(numBits,values);
		}
		public static ulong operator &(UDoubleLong oldValue, uint mask) 
		{
			return (ulong)oldValue.LowLong&mask;
		}

		public static ulong operator &(UDoubleLong oldValue, int numBits) 
		{
			if(numBits > 64)
				return 0;

			ulong mask = 0;
			for(int i=0;i<numBits;i++)
			{
				mask<<=1;
				mask|=1;
			}
			return (ulong)oldValue.LowLong&mask;
		}
		public static UDoubleLong operator >>(UDoubleLong oldValue, int numBits) 
		{ 
			if(oldValue.Index > 0)
			{
				if(oldValue.Index < numBits)
					numBits = oldValue.Index;
				
				if( oldValue.Index <= 64 )
				{
					oldValue.LowLong>>=numBits;
					return new UDoubleLong(oldValue.LowLong,0,oldValue.Index-numBits);
				}
				else
				{
					oldValue.LowLong>>=numBits;
					int mask = 0;
					for(int i=0;i<numBits;i++)
					{
						mask<<=1;
						mask|=1;
					}
					ulong tmp = (ulong)oldValue.HighLong&(ulong)mask;
					tmp<<=(64-numBits);
					oldValue.LowLong|=tmp;
					oldValue.HighLong>>=numBits;

					return new UDoubleLong(oldValue.LowLong,oldValue.HighLong,oldValue.Index-numBits);
				}
			}
			else
				return new UDoubleLong(0,null);
		} 
		public override string ToString()
		{
			return "HighLong=" + highLong.ToString() + ", LowLong="+lowLong.ToString();
		}
	}
}
