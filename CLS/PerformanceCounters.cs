using System;
using System.Collections;
using System.Diagnostics;

namespace VLF.CLS
{
	/// <summary>
	/// Summary description for PerformanceCounters.
	/// </summary>
	public class PerformanceCounters
	{
		private SortedList countersCollection;
		private SortedList performanceCounters;

		private string category;

		public PerformanceCounters()
		{
			countersCollection = new SortedList();
			performanceCounters = new SortedList();
		}

		public void AddCounter( string key, PerformanceCounterType value )
		{
			countersCollection.Add( key, value );
		}

		public bool SetupCategory(string category, string help, bool updateNeeded)
		{     
			this.category = category;

			if(countersCollection.Count == 0)
				return false;

			if ( PerformanceCounterCategory.Exists( category ) )
			{
				if( updateNeeded == true )
					PerformanceCounterCategory.Delete( category );
				else
					return(true);
			}

			CounterCreationDataCollection baseCollection = new CounterCreationDataCollection();
        
			foreach( string counterName in countersCollection.Keys )
			{
				// Add the counter
				CounterCreationData ccd = new CounterCreationData();
				ccd.CounterType = (PerformanceCounterType)countersCollection[counterName];
				ccd.CounterName = counterName;
				baseCollection.Add(ccd);
			}
       
			// Create the category.
			PerformanceCounterCategory.Create( category, help, baseCollection );
            
			return(true);
		}

		public void CreateCounters()
		{
			foreach( string counterName in countersCollection.Keys)
			{
				performanceCounters.Add( counterName, new PerformanceCounter( category, counterName, false ) );
				((PerformanceCounter)performanceCounters[counterName]).RawValue = 0;
			}
		}

		public void UpdateCounter( string counterName )
		{
			PerformanceCounterType counterType = (PerformanceCounterType)countersCollection[counterName];
			if(counterType == PerformanceCounterType.NumberOfItems64)
			{
				((PerformanceCounter)performanceCounters[counterName]).Increment();
			}
			else if(counterType == PerformanceCounterType.RateOfCountsPerSecond32)
			{
				((PerformanceCounter)performanceCounters[counterName]).Increment();
			}
			else
			{
				((PerformanceCounter)performanceCounters[counterName]).Increment();
			}
		}

      public void UpdateCounter(string counterName, long val)
      {
         PerformanceCounterType counterType = (PerformanceCounterType)countersCollection[counterName];
         if (counterType == PerformanceCounterType.NumberOfItems64)
         {
            ((PerformanceCounter)performanceCounters[counterName]).IncrementBy(val) ;
         }
         else if (counterType == PerformanceCounterType.RateOfCountsPerSecond32)
         {
            ((PerformanceCounter)performanceCounters[counterName]).IncrementBy(val);
         }
         else
         {
            ((PerformanceCounter)performanceCounters[counterName]).IncrementBy(val);
         }
      }

		public void SetRawValue( string counterName, int rawValue )
		{
			((PerformanceCounter)performanceCounters[counterName]).RawValue = rawValue;
		}
	}
}
