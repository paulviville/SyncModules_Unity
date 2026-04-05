using System.Text.Json;

public class ScalarModule : ModuleCore
{
	public new static string ModuleType => "ScalarModule";
	public override string Type => ModuleType;
	public new static class Commands
	{
		public const string setState = ModuleCore.Commands.setState;	
		public const string updateValue = "UPDATE_VALUE";	
	} 

	private double _value;
	public double Value => _value;

	public ScalarModule ( Guid UUID ) : base ( UUID)
	{
		Console.WriteLine( "ScalarModule Constructor " + this.UUID );
		SetOnCommand( Commands.updateValue, OnUpdateValue );
	}

	private void OnUpdateValue ( JsonElement data )
	{
		double value = data.GetProperty( "value" ).GetDouble( );
		UpdateValue( value );
	}

	public void UpdateValue ( double value, bool sync = false)
	{
		_value = value;

		OnChange( Commands.updateValue, Value );

		if ( sync ) 
			Output( Commands.updateValue, new { value = Value } );
	}

	public override object GetState ( )
	{
		return new { value = Value };
	}

	public override void SetState ( JsonElement state )
	{
		Console.WriteLine( "ScalarModule - SetState" );
		double value = state.GetProperty( "value" ).GetDouble( );
		UpdateValue( value );
	}
}