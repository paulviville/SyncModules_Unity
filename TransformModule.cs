#nullable enable
using System;
using System.Linq;

public class TransformData
{
    public double[]? translation { get; }
    public double[]? rotation    { get; }
    public double[]? scale       { get; }
    public TransformData(double[]? translation = null, double[]? rotation = null, double[]? scale = null)
    {
        this.translation = translation;
        this.rotation = rotation;
        this.scale = scale;
    }
}

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
		SetOnCommand( Commands.updateTransform, OnUpdateTransform );
	}

	private void OnUpdateTransform ( IPayload data )
	{
		var transform = data.GetPayload("transform");

		double[]? translation = transform.HasProperty("translation")
			? transform.GetDoubleArray("translation")
			: null;
			
		double[]? rotation = transform.HasProperty("rotation")
			? transform.GetDoubleArray("rotation")
			: null;
			
		double[]? scale = transform.HasProperty("scale")
			? transform.GetDoubleArray("scale")
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

	public override void SetState ( IPayload state )
	{
		OnUpdateTransform( state );
	}
}