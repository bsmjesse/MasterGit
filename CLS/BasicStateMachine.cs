// OBSOLETE !!
using System;
using System.Collections ;

namespace VLF.CLS
{
	
	public class STMException : Exception {}
	
	public enum CompareType
	{
		Exact,
		Trim,
		Contains
	}

	public enum TransitionType
	{
		Normal,
		Default
	}

	public delegate void STMEvent( object token, long stateTo );
	
	/// <summary>
	/// Transition object
	/// </summary>
	abstract class BasicTransition
	{
		protected TransitionType transitionType ;
		protected long stateFrom ;
		protected long stateTo ;
		
		protected BasicStateMachine parent ;

		public BasicTransition( BasicStateMachine parent, long stateFrom, long stateTo, TransitionType transitionType )
		{
			this.parent = parent ;
			this.stateFrom = stateFrom ;
			this.stateTo = stateTo ;
			//this.token = token ;
			//this.evtCallback = evtCallback ;
			this.transitionType = transitionType ;
		}

		public bool IsDefaultFor( long state )
		{
			return ((state == stateFrom) && 
					( transitionType == TransitionType.Default )) ;
		}
		
	}

	/// <summary>
	/// Simple State Machine implementation
	/// </summary>
	public abstract class BasicStateMachine
	{
		protected long currentState ;
		protected ArrayList lstTransitions ;
		protected CompareType compareType;
		public BasicStateMachine( CompareType compareType )
		{
			lstTransitions = new ArrayList() ;
			this.compareType = compareType ;
		}
		/// <summary>
		/// resets curent state to zero
		/// </summary>
		public void Start()
		{
			currentState = 0 ;
		}
		
		public CompareType GetCompareType()
		{
			return compareType ;
		}
	}
}
