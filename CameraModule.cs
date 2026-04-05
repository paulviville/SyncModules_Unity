
using System.Text.Json;

public record CameraData ( double? fov, double? aspect, double? near, double? far );

public class CameraModule : TransformModule
{
	public new static string ModuleType => "CameraModule";
	public override string Type => ModuleType;

	public new static class Commands
	{
		public const string setState = TransformModule.Commands.setState;	
		public const string updateTransform = TransformModule.Commands.updateTransform;
		public const string updateCamera = "UPDATE_CAMERA";
	}

	private double _fov = 50.0;
	private double _aspect = 4.0 / 3.0;
	private double _near = 0.1;
	private double _far = 1.0;
	
	public CameraData Camera => new( _fov, _aspect, _near, _far );

	public CameraModule ( Guid UUID ) : base ( UUID)
	{
		Console.WriteLine( "CameraModule Constructor " + this.UUID );
		SetOnCommand( Commands.updateCamera, OnUpdateCamera );
	}

	private void OnUpdateCamera ( JsonElement data )
	{
		double? fov = data.TryGetProperty("fov", out var f) ? f.GetDouble() : null;
		double? aspect = data.TryGetProperty("aspect", out var a) ? a.GetDouble() : null;
		double? near = data.TryGetProperty("near", out var n) ? n.GetDouble() : null;
		double? far = data.TryGetProperty("far", out var ff) ? ff.GetDouble() : null;
	}

	public void UpdateCamera ( CameraData camera, bool sync = false)
	{
		if ( camera.fov is { } fov )
			_fov = fov;
		if ( camera.aspect is { } aspect )
			_aspect = aspect;
		if ( camera.near is { } near )
			_near = near;	
		if ( camera.far is { } far )
			_far = far;

		OnChange( Commands.updateCamera, Camera );
		if ( sync ) 
			Output( Commands.updateCamera, new { camera = Camera } );
	}

	public override object GetState ()
	{
		return new { 
			transform = Transform,
			camera = Camera 
		};
	}

	public override void SetState ( JsonElement state )
	{
		Console.WriteLine( "CameraModule - SetState" );
		base.SetState( state );
		OnUpdateCamera( state );
	}

}