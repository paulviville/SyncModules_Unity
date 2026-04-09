#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

public class ModulesRegistry : ModuleCore
{
	public new static string ModuleType => "ModuleRegistry";
	public override string Type => ModuleType;

	public new static class Commands
	{
		public const string setState = ModuleCore.Commands.setState;	
		public const string addModule = "ADD_MODULE";	
		public const string removeModule = "REMOVE_MODULE";
	} 

	private Dictionary< Guid, ModuleCore > _modules = new (); 

	public ModulesRegistry ( Action<object> outputFn ) :
		base ( new Guid( "00000000-0000-0000-0000-000000000000" ) )
	{
		SetOutputFn( outputFn );

		SetOnCommand( Commands.addModule, OnAddModule );
		SetOnCommand( Commands.removeModule, OnRemoveModule );
	}

	private void OnAddModule ( IPayload data )
	{
		string type = data.GetString( "type" )!;
		Guid UUID = data.GetGuid( "UUID" );

		AddModule( type, UUID );
	}

	private void OnRemoveModule ( IPayload data )
	{
		Guid UUID = data.GetGuid( "UUID" );

		RemoveModule( UUID );
	}

	public ModuleCore? AddModule ( string type, Guid UUID, bool sync = false)
	{
		if( _modules.ContainsKey( UUID ) )
			return null;

		var module = ModuleTypes.Create( type, UUID );
		module.SetOutputFn( _outputFn );
		
		_modules[ module.UUID ] = module;

		OnChange(Commands.addModule, module);
		
		if ( sync )
		{
			Output( Commands.addModule, new { type, UUID } );
		}

		Console.WriteLine( _modules.Keys.ToArray().Length );

		return module;
	}

	public void RemoveModule ( Guid UUID )
	{
		if (_modules.TryGetValue( UUID, out var module))
        {
            _modules.Remove( UUID );
            OnChange( Commands.removeModule, module );
            module.Delete( );
        }
		Console.WriteLine( _modules.Keys.ToArray().Length );
	}

	public ModuleCore? GetModule ( Guid UUID )
	{
		_modules.TryGetValue( UUID, out var module );
   		return module;
	}

	public override object GetState ( )
	{
		return new { };
		/// TODO
	}

	public override void SetState ( IPayload state )
	{
		/// TODO
	}
}