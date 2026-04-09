#nullable enable
using System;

public class PrimitiveModule : TransformModule
{
	public new static string ModuleType => "PrimitiveModule";
	public override string Type => ModuleType;

	public new static class Commands
	{
		public const string setState = TransformModule.Commands.setState;	
		public const string updateTransform = TransformModule.Commands.updateTransform;
		public const string updatePrimitive = "UPDATE_PRIMITIVE";
	}

	public static class PrimitiveTypes
	{
		public const string Sphere = "Sphere";
		public const string Box = "Box";
	}

	private string _primitive = PrimitiveTypes.Sphere;
	public string Primitive => _primitive;

	public PrimitiveModule ( Guid UUID ) : base ( UUID)
	{
		SetOnCommand( Commands.updatePrimitive, OnUpdatePrimitive );
	}

	private void OnUpdatePrimitive ( IPayload data )
	{
		string primitive = data.GetString( "type" )!;
		UpdatePrimitive( primitive );
	}

	public void UpdatePrimitive ( string primitive, bool sync = false )
	{
		_primitive = primitive;

		OnChange( Commands.updatePrimitive, Primitive );
		if ( sync)
		{
			Output( Commands.updatePrimitive, new { primitive = Primitive } );
		}
	}

	public override object GetState ()
	{
		return new { 
			transform = Transform,
			primitive = Primitive
		};
	}

	public override void SetState ( IPayload state )
	{
		base.SetState( state );
		OnUpdatePrimitive( state );
	}
}