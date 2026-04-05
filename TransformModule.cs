using System.Text.Json;

public record TransformData(double[]? translation, double[]? rotation, double[]? scale);

public class TransformModule : ModuleCore
{
	public new static string ModuleType => "TransformModule";
	public override string Type => ModuleType;

	public new static class Commands
	{
		public const string setState = ModuleCore.Commands.setState;	
		public const string updateTransform = "UPDATE_TRANSFORM";	
	} 

	private double[] _translation = { 0, 0, 0 };
	private double[] _rotation = { 0, 0, 0, 1 };
	private double[] _scale = { 1, 1, 1 };

	public TransformData Transform => new (
		_translation.ToArray( ),	
		_rotation.ToArray( ),	
		_scale.ToArray( )	
	);

	public TransformModule ( Guid UUID ) : base ( UUID )
	{
		Console.WriteLine( "TransformModule Constructor " + this.UUID );
		
		SetOnCommand( Commands.updateTransform, OnUpdateTransform );
	}

	private void OnUpdateTransform ( JsonElement data )
	{
		var transform = data.GetProperty("transform");

		double[]? translation = transform.TryGetProperty("translation", out var t)
			? t.EnumerateArray().Select(e => e.GetDouble()).ToArray()
			: null;

		double[]? rotation = transform.TryGetProperty("rotation", out var r)
			? r.EnumerateArray().Select(e => e.GetDouble()).ToArray()
			: null;

		double[]? scale = transform.TryGetProperty("scale", out var s)
			? s.EnumerateArray().Select(e => e.GetDouble()).ToArray()
			: null;

		UpdateTransform( new TransformData( translation, rotation, scale ) );
	}

	public void UpdateTransform ( TransformData transform, bool sync = false)
	{
		if ( transform.translation is { } translation )
			Array.Copy( translation, _translation, 3 );
		if ( transform.rotation is { } rotation )
			Array.Copy( rotation, _rotation, 4 );
		if ( transform.scale is { } scale )
			Array.Copy( scale, _scale, 3 );

		OnChange( Commands.updateTransform, Transform );
		
		if ( sync ) 
			Output( Commands.updateTransform, new { transform = Transform } );
	}

	public override object GetState ( )
	{
		return new { transform = Transform };
	}

	public override void SetState ( JsonElement state )
	{
		Console.WriteLine( "TransformModule - SetState" );

		OnUpdateTransform( state );
	}
}