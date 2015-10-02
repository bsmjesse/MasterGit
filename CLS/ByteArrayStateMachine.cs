// OBSOLETE !!
using System;

namespace VLF.CLS
{
	
	public delegate void BASTMEvent( byte[] token, long stateTo );

	class ByteArrayTransition : BasicTransition
	{
		protected byte[] token ;
		protected BASTMEvent evtCallback ;
		public ByteArrayTransition( BasicStateMachine parent, long stateFrom, long stateTo, byte[] token, BASTMEvent evtCallback, TransitionType transitionType ):
							base( parent, stateFrom, stateTo, transitionType )
		{
			this.parent = parent ;
			this.stateFrom = stateFrom ;
			this.stateTo = stateTo ;
			this.token = token ;
			this.evtCallback = evtCallback ;
			this.transitionType = transitionType ;
			this.evtCallback = evtCallback ;
		}
		
		
		public bool processToken( long stateFrom, byte[] token )
		{
			
			// defalt transition are NOT for token processing...
			bool trans = ( transitionType == TransitionType.Normal ) ;
			bool state = ( this.stateFrom == stateFrom );
			
			bool tok = false ;// = ( this.token.Equals( token ) );
			
			switch( parent.GetCompareType() )
			{
				case CompareType.Exact :
					Util.ArraysEqual( token, this.token ) ;
					break ;
				case CompareType.Contains :
					tok = Util.ArrayContains( token , this.token ) ;
					break ;
				default : 
					tok = false ; // not supported
					break ;
			}
			
			return (  trans && 
					  state && 
					  tok ) ;
					  
		}
		/// <summary>
		/// invokes callback
		/// returns target state 
		/// </summary>
		/// <returns></returns>
		public long DoTransition( byte[] token )
		{
			if( evtCallback != null )
				evtCallback( token, stateTo ) ;
			return stateTo ;
		}
	
	}
	/// <summary>
	/// ByteArrayStateMachine.
	/// </summary>
	public class ByteArrayStateMachine : BasicStateMachine
	{
		public ByteArrayStateMachine( CompareType compareType ): base( compareType )
		{
		
		}

		public void AddTransition( long stateFrom, long stateTo, byte[] token, BASTMEvent evtCallback, TransitionType tType )
		{
			lstTransitions.Add( new ByteArrayTransition( this, stateFrom, stateTo, token, evtCallback, tType ) ) ;
		}
		
		// returns curent state
		public long ProcessToken( byte[] token )
		{
			ByteArrayTransition trDefault = null;

			foreach( ByteArrayTransition t in lstTransitions )
			{
				
				if( t.IsDefaultFor( currentState ) )
					trDefault = t ;

				if( t.processToken( currentState, token ) )
				{
					currentState = t.DoTransition( token ) ;
					return currentState ;
				}
			}

			// no transition found, go for default
			if( trDefault != null )
				currentState = trDefault.DoTransition( token ) ;
			return currentState ;
		}

	}
}
