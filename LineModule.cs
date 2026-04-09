#nullable enable
using System;
using System.Linq;

public class LineData
{
    public double[]? origin { get; }
    public double[]? end    { get; }
    public LineData(double[]? origin = null, double[]? end = null)
    {
        this.origin = origin;
        this.end = end;
    }
}

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
		SetOnCommand( Commands.updateLine, OnUpdateLine );
	}

	private void OnUpdateLine ( IPayload data )
	{
		var line = data.GetPayload( "line" );
		double[]? origin = line.HasProperty("origin")
			? line.GetDoubleArray("origin")
			: null;
		double[]? end = line.HasProperty("end")
			? line.GetDoubleArray("end")
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

	public override void SetState ( IPayload state )
	{
		OnUpdateLine( state );
	}
}