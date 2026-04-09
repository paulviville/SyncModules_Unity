#nullable enable
using System;
using System.Collections.Generic;
public class ModuleCore
{
	public static string ModuleType => "ModuleCore";
	public virtual string Type => ModuleType;

	public static class Commands
	{
		public const string setState = "SET_STATE";	
	} 


	private Guid _UUID;
	public Guid UUID => _UUID;

	protected Action<object>? _outputFn;
	private Dictionary<string, List<Action<IPayload>>> _commandCallbacks = new ( );
	private Dictionary<string, List<Action<object>>> _changeCallbacks = new ( );


	public ModuleCore( Guid UUID )
	{
		// Console.WriteLine( "ModuleCore Constructor " + UUID );
		_UUID = UUID;

		SetOnCommand( Commands.setState, SetState );
	}

	public void SetOutputFn ( Action<object>? outputFn)
	{
		_outputFn = outputFn;
	}

	public void Input ( IPayload payload )
	{
		string command = payload.GetString( "command" )!;
		IPayload data = payload.GetPayload( "data" );

		OnCommand( command, data );
	}

	public void Output ( string command, object data )
	{
		var payload = new
		{
			moduleUUID = UUID,
			command,
			data
		};
		if( _outputFn != null )
			_outputFn( payload );
	}

	protected void SetOnCommand ( string command, Action<IPayload> callback)
	{
		if ( !_commandCallbacks.ContainsKey( command ))
			_commandCallbacks[ command ] = new ( );

		_commandCallbacks[ command ].Add( callback );
	}

	protected void OnCommand ( string command, IPayload data)
	{
		if ( !_commandCallbacks.ContainsKey( command ))
			return;
		
		_commandCallbacks[ command ].ForEach( 
			callback => callback( data )
		);
	}

	public void SetOnChange ( string change, Action<object> callback)
	{
		if ( !_changeCallbacks.ContainsKey( change ))
			_changeCallbacks[ change ] = new ( );

		_changeCallbacks[ change ].Add( callback );
	}

	protected void OnChange ( string change, object data)
	{
		if ( !_changeCallbacks.ContainsKey( change ))
			return;
		
		_changeCallbacks[ change ].ForEach( 
			callback => callback( data )
		);
	}

	public virtual object GetState ( )
	{
		return new { };
	}

	public virtual void SetState ( IPayload state )
	{
		
	}

	public virtual void Delete ( )
	{
		
	}
}