using System;
using System.Collections;

namespace VLF.CLS
{
	/// <summary>
	/// Provides with functionality to control Deny of Service (DoS) attack.
	/// </summary>
	/// <remarks>
	/// IPTrafficInfoTiming built to check DoS attack based on timing intervals.
	/// </remarks>
	public class IPTrafficInfoTiming
	{
		private long lastReported;
		private long firstStarted;

		private int count;
		private int totalLength;
		private int numAllowed;
		private int dosInterval;
		private int releaseTime;

		public long LastReported
		{
			get { return lastReported; }
			set { lastReported = value; }
		}
		public long FirstStarted
		{
			get { return firstStarted; }
			set { firstStarted = value; }
		}
		public int NumIgnored
		{
			get { return (count > numAllowed)?(count - numAllowed):0; }
		}

		public int TotalLength
		{
			get { return totalLength; }
		}
		/// <summary>
		/// Initializes a new instance of the IPTrafficInfoTiming class.
		/// </summary>
		/// <param name="numAllowed">Number of first allowed messages preceeding to DoS attack.</param>
		/// <param name="dosInterval">Allowed interval between messages in milliseconds.</param>
		/// <param name="releaseTime">Interval between messages that releases restrictions in milliseconds.</param>
		public IPTrafficInfoTiming( int numAllowed, int dosInterval, int releaseTime )
		{
			this.dosInterval = dosInterval;
			this.numAllowed = numAllowed;
			this.releaseTime = releaseTime;
			firstStarted = 0;
			totalLength = 0;
			lastReported = System.DateTime.Now.Ticks;
			count = 0;
		}

		/// <summary>
		/// Updates information about this IP address.
		/// </summary>
		/// <param name="currTime">Current time.</param>
		/// <param name="messLength">Message length.</param>
		/// <returns>Returns True if no action is required. If it returns false than
		/// message is supposed to be ignored.</returns>
		public bool UpdateInfo( long currTime, int messLength )
		{
			bool ok = true;

			// allowed interval
			if( ( new TimeSpan(currTime - lastReported).Ticks/10000 ) >= dosInterval )
			{
				lastReported = currTime;
				// quarantine !!! keep counting for previously ignored messages
				if( count != 0 && count > numAllowed) 
				{
					totalLength += messLength;
					count++;
				}
				else
					count = 0;
				ok = ( count == 0 );
			}
				// allowed number of messages
			else if( count < numAllowed )
			{
				count++;
				lastReported = currTime;
			}
				// ignore message
			else if( firstStarted == 0 )
			{
				firstStarted = lastReported;
				lastReported = currTime;
				count++;
				totalLength += messLength;
				ok = false;
			}
			else
			{
				lastReported = currTime;
				count++;
				totalLength += messLength;
				ok = false;
			}
			return ok;
		}

		/// <summary>
		/// Checks if DoS attack has stopped.
		/// </summary>
		/// <param name="currTime">Current time.</param>
		/// <returns>Returns true if DoS attack has stopped.</returns>
		public bool IsDoSStopped(long currTime)
		{
			return ( new TimeSpan(currTime - lastReported).Ticks/10000 ) >= releaseTime ;
		}

		/// <summary>
		/// Resets all counters. 
		/// </summary>
		public void CleanInfo()
		{
			firstStarted = 0;
			totalLength = 0;
			count = 0;
		}
	}

	/// <summary>
	/// Provides with functionality to control number of messages from the same box
	/// for predefined period of time.
	/// </summary>
	public class TrafficControlInfo
	{
		private long firstStarted;
		private int count;
		private int totalLength;
		private int numAllowed;
		private int numDaysCheck;

		/// <summary>
		/// Gets total length of ignored messages.
		/// </summary>
		public int TotalLength
		{
			get { return totalLength; }
		}
		/// <summary>
		/// Gets the time when the quarantine was started.
		/// </summary>
		public string FirstStarted
		{
			get { return new DateTime(firstStarted).ToString(); }
		}
		/// <summary>
		/// Initializes a new instance of the TrafficControlInfo class.
		/// </summary>
		/// <param name="numAllowed">number of allowed messages for box per numCheckDays days</param>
		/// <param name="numAllowed">number of days for counting messages</param>
		public TrafficControlInfo( int numAllowed, int numDaysCheck )
		{
			this.numAllowed = numAllowed;
			this.numDaysCheck = numDaysCheck;
			firstStarted = System.DateTime.Now.Ticks ;
			totalLength = 0;
			count = 0;
		}
		/// <summary>
		/// Updates information about this unit.
		/// </summary>
		/// <param name="currTime">Current time.</param>
		/// <param name="messLength">Message length.</param>
		/// <returns>Returns True if no action is required. If it returns false than
		/// message is supposed to be ignored.</returns>
		public bool UpdateTrafficInfo( long currTime, int messLength )
		{
			count++;
			bool ok = true;
			totalLength += messLength;
			if( count > numAllowed )
			{
				ok = false;
//				ResetInfo(currTime,messLength);
			}
			else
			{
				// check if more than numCheckDays days past from checking period
				// if so - reset counters
				if( ( new TimeSpan(currTime - firstStarted).TotalDays) >= numDaysCheck )
				{
					ResetInfo(currTime,messLength);
				}
			}
			return ok;
		}
		/// <summary>
		/// Resets information about this unit.
		/// </summary>
		/// <param name="currTime"></param>
		/// <param name="messLength"></param>
		public void ResetInfo( long currTime, int messLength )
		{
			firstStarted = currTime ;
			totalLength = messLength;
			count = 1;
		}
	}

	/// <summary>
	/// Provides with functionality to control high volume traffic.
	/// </summary>
	/// <remarks>IPTrafficInfoTiming built to check IP traffic based on predefined period.
	/// </remarks>
	public class IPTrafficInfo
	{
		public enum IPStatus
		{
			Normal, // no restrictions
			Warning, // under consideration
			Critical, // ignoring all incoming messages
		}

		// ??
		public long lastReported;
		public long firstStarted;
		private int ignoredCount;
		private int ignoredLength;

		// counter of all incoming messages in predefined period
		private int totalCount;

		// new algorithm
		private int time_period;
		private int messages_to_ignore;
		private int messages_to_warn;
		private int messages_to_release;

		private IPStatus ipStatus;

		// last Status is only used by DCS
		private IPStatus lastStatus;
		public IPStatus LastStatus
		{
			get { return lastStatus; }
			set { lastStatus = value; }
		}

		// time of the begining of checking
		private long firstCheckingTime;

		public void ResetIgnoredCount()
		{
			firstStarted = 0;
			ignoredLength = 0;
			ignoredCount = 0;
		}

		public void CleanInfo()
		{
			ResetIgnoredCount();
			totalCount = 0;
			lastReported = 0;
			lastStatus = IPTrafficInfo.IPStatus.Normal;
		}

		private void IncIgnoredCount( long currTime, int messLength)
		{
			if(firstStarted == 0)
				firstStarted = currTime;
			ignoredCount++;
			ignoredLength+=messLength;
		}

		public int NumIgnored
		{
			get { return ignoredCount; }
		}

		public int TotalLength
		{
			get { return ignoredLength; }
		}
		/// <summary>
		/// IPInfo class constructor
		/// </summary>
		/// <param name="numAllowed">number of first allowed messages preceeding to DoS attack</param>
		/// <param name="dosInterval">allowed interval between messages in milliseconds</param>
		/// <param name="releaseTime">interval between messages that releases restrictions in milliseconds</param>
		public IPTrafficInfo( SortedList specParam )
		{
			firstStarted = 0;
			ignoredLength = 0;
			ignoredCount = 0;

			time_period = 30000; // msec
			try 
			{ 
				time_period = Convert.ToInt32( specParam["time_period"] ); 
				if( time_period == 0 )
					time_period = 30;
				time_period*=1000;
			}
			catch {}
			messages_to_ignore = 60;
			try 
			{ 
				messages_to_ignore = Convert.ToInt32( specParam["mess_ignore"] ); 
				if( messages_to_ignore == 0 )
					messages_to_ignore = 60;
			}
			catch {}
			messages_to_warn = 30;
			try 
			{ 
				messages_to_warn = Convert.ToInt32( specParam["mess_warn"] ); 
				if( messages_to_warn == 0 )
					messages_to_warn = 30;
			}
			catch {}
			messages_to_release = 6;
			try 
			{ 
				messages_to_release = Convert.ToInt32( specParam["mess_release"] ); 
				if( messages_to_release == 0 )
					messages_to_release = 6;
			}
			catch {}

			totalCount = 1;
			firstCheckingTime = System.DateTime.Now.Ticks;
			lastReported = firstCheckingTime;
			ipStatus = IPStatus.Normal;
			lastStatus = IPStatus.Normal;
		}

		public IPStatus UpdateTrafficInfo( long currTime, int messLength )
		{
			lastReported = currTime;
			if( ( new TimeSpan(currTime - firstCheckingTime).Ticks/10000 ) >= time_period )
			{
				// time for making decisions
				if( totalCount <= messages_to_release )
				{
					// release all restrictions
					ipStatus = IPStatus.Normal;
				}
				else if( totalCount < messages_to_warn )
				{
					// if status is Normal or Warning - nothing to do
					// if status is Critical - decrease level of status to Warning
					if( ipStatus == IPStatus.Critical )
						ipStatus = IPStatus.Warning;
				}
				else if( totalCount < messages_to_ignore )
				{
					ipStatus = IPStatus.Warning;
				}
				else // more than messages_to_ignore
				{
					ipStatus = IPStatus.Critical;
					// calc message length
					IncIgnoredCount( currTime, messLength );
				}
				totalCount = 1;
				firstCheckingTime = System.DateTime.Now.Ticks;
			}
			else
			{
				// keep counting
				totalCount++;

				// wait for next time out ( 30 sec )
				if( ipStatus == IPStatus.Critical )
				{
					// calc message length
					IncIgnoredCount( currTime, messLength );
				}
				else
				{
					if( totalCount >= messages_to_ignore )
					{
						ipStatus = IPStatus.Critical;
						// calc message length
						IncIgnoredCount( currTime, messLength );
					}
					else if( totalCount >= messages_to_warn )
					{
						ipStatus = IPStatus.Warning;
					}
				}
			}
			return ipStatus;
		}
		public bool IsTrafficStopped(long currTime)
		{
			return ( new TimeSpan(currTime - lastReported).Ticks/10000 ) >= time_period ;
		}
	}
}
