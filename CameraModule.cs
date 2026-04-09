#nullable enable
using System;

// public record CameraData ( double? fov, double? aspect, double? near, double? far );
public class CameraData
{
    public double? fov { get; }
    public double? aspect { get; }
    public double? near { get; }
    public double? far { get; }
    public CameraData(double? fov = null, double? aspect = null,double? near = null,double? far = null )
    {
        this.fov = fov;
        this.aspect = aspect;
        this.near = near;
        this.far = far;
    }
}
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
		SetOnCommand( Commands.updateCamera, OnUpdateCamera );
	}

	private void OnUpdateCamera ( IPayload data )
	{
		var camera = data.GetPayload("camera");
		double? fov = camera.HasProperty("fov") ? camera.GetDouble("fov") : null;
		double? aspect = camera.HasProperty("aspect") ? camera.GetDouble("aspect") : null;
		double? near = camera.HasProperty("near") ? camera.GetDouble("near") : null;
		double? far = camera.HasProperty("far") ? camera.GetDouble("far") : null;
	
		UpdateCamera( new CameraData( fov, aspect, near, far ) );
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

	public override void SetState ( IPayload state )
	{
		base.SetState( state );
		OnUpdateCamera( state );
	}

}