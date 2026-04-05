using System.Text.Json;

public record LineData ( double[]? origin, double[]? end );

public class LineModule : ModuleCore
{
	public new static string ModuleType => "LineModule";
	public override string Type => ModuleType;

	public new static class Commands
	{
		public const string setState = ModuleCore.Commands.setState;	
		public const string updateLine = "UPDATE_LINE";	
	} 

	private double[] _origin = { 0, 0, 0 };
	private double[] _end = { 0, 0, 0 };

	public LineData Line => new (
		_origin.ToArray( ),	
		_end.ToArray( )
	);

	public LineModule ( Guid UUID ) : base ( UUID)
	{
		Console.WriteLine( "LineModule Constructor " + this.UUID );
		SetOnCommand( Commands.updateLine, OnUpdateLine );
	}

	private void OnUpdateLine ( JsonElement data )
	{
		var line = data.GetProperty( "line" );
		double[]? origin = line.TryGetProperty("origin", out var or)
			? or.EnumerateArray().Select(e => e.GetDouble()).ToArray()
			: null;
		double[]? end = line.TryGetProperty("end", out var en)
			? en.EnumerateArray().Select(e => e.GetDouble()).ToArray()
			: null;

		UpdateLine( new LineData( origin, end ) );
	} 

	public void UpdateLine ( LineData line, bool sync = false)
	{
		if ( line.origin is { } origin )
			Array.Copy( origin, _origin, 3 );
		if ( line.end is { } end )
			Array.Copy( end, _end, 3 );

		OnChange( Commands.updateLine, Line );
		if ( sync ) 
			Output( Commands.updateLine, new { line = Line } );
	}

	public override object GetState ( )
	{
		return new { line = Line };
	}

	public override void SetState ( JsonElement state )
	{
		Console.WriteLine( "LineModule - SetState" );
		OnUpdateLine( state );
	}
}