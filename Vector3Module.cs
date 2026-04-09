#nullable enable
using System;
using System.Linq;

public class Vector3Module : ModuleCore
{
	public new static string ModuleType => "Vector3Module";
	public override string Type => ModuleType;
	public new static class Commands
	{
		public const string setState = ModuleCore.Commands.setState;	
		public const string updateVector = "UPDATE_VECTOR";	
	} 

	private double[] _vector = { 0, 0, 0 };
	public double[] Vector => _vector.ToArray( );

	public Vector3Module ( Guid UUID ) : base ( UUID)
	{
		SetOnCommand( Commands.updateVector, OnUpdateVector );
	}

	private void OnUpdateVector ( IPayload data )
	{
		double[]? vector = data.GetDoubleArray("vector");
		if ( vector != null )
	        UpdateVector( vector );
	} 

	public void UpdateVector ( double[] vector, bool sync = false)
	{
		Array.Copy( vector, _vector, 3 );

		OnChange( Commands.updateVector, Vector );

		if ( sync )
		{
			Output( Commands.updateVector, new { vector = Vector } );
		}
	}

	public override object GetState ( )
	{
		return new { vector = Vector };
	}

	public override void SetState ( IPayload state )
	{
		OnUpdateVector( state );
	}
}