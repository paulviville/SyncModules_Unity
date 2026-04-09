#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

public class PointData
{
    public double[]? position { get; }
	public Guid UUID { get; }

	public PointData( Guid UUID, double[]? position)
	{
		this.position = position;
		this.UUID = UUID;
	}
	
}

public class PointsModule : ModuleCore
{
	public new static string ModuleType => "PointsModule";
    public override string Type => ModuleType;

    public new static class Commands
    {
        public const string setState = ModuleCore.Commands.setState;
        public const string addPoints = "ADD_POINTS";
        public const string removePoints = "REMOVE_POINTS";
        public const string updatePoints = "UPDATE_POINTS";
        public const string clear = "CLEAR";
    }

	private Dictionary<Guid, double[]> _points = new ( );
	public PointData[] Points => _points.Select( kvp => new
			PointData( kvp.Key, kvp.Value) 
			).ToArray();

	public PointsModule ( Guid UUID ) : base ( UUID)
	{
		Console.WriteLine( "PointsModule Constructor " + this.UUID );
		SetOnCommand( Commands.addPoints, OnAddPoints );
		SetOnCommand( Commands.removePoints, OnRemovePoints );
		SetOnCommand( Commands.updatePoints, OnUpdatePoints );
		SetOnCommand( Commands.clear, OnClear );
	}

	private PointData[] ParsePoints( IPayload data )
	{
		return data.GetArray("points")
			.Select(p =>
			{
				Guid uuid = p.GetGuid("UUID");
				double[]? position = p.HasProperty("position")
                       ? p.GetDoubleArray("position")
                       : null;

				return new PointData(uuid, position);
			})
			.ToArray();
	}

	private object[] SerializePoints( PointData[] points )
	{
		return points.Select( p => (object)new { UUID = p.UUID, position = p.position }).ToArray();
	}

	private void OnAddPoints ( IPayload data )
	{
		PointData[] points = ParsePoints( data );
		AddPoints( points );
	}
	
	private void OnRemovePoints ( IPayload data )
	{
		PointData[] points = ParsePoints( data );
		RemovePoints( points );
	}

	private void OnUpdatePoints ( IPayload data )
	{
		PointData[] points = ParsePoints( data );
		UpdatePoints( points );
	}

	private void OnClear ( IPayload data )
	{
		Clear( );
	}

	public void AddPoints ( PointData[] points, bool sync = false )
	{
		foreach ( var point in points )
		{
			if ( !_points.ContainsKey( point.UUID ) )
				_points[ point.UUID ] = new double[ 3 ];
			if ( point.position != null )
				Array.Copy( point.position, _points[ point.UUID ], 3 );
		}

		OnChange( Commands.addPoints, points );
		if (sync)
            Output(Commands.addPoints, new { points = SerializePoints( points ) } );
	}

	public void RemovePoints ( PointData[] points, bool sync = false )
    {
        foreach (var point in points )
			if ( _points.ContainsKey( point.UUID ) )
           		_points.Remove( point.UUID );

        OnChange(Commands.removePoints, points);

        if (sync)
            Output(Commands.removePoints, new { points = SerializePoints( points ) });
    }

	public void UpdatePoints ( PointData[] points, bool sync = false )
	{
		foreach ( var point in points )
		{
			if ( _points.ContainsKey( point.UUID ) && point.position != null )
				Array.Copy( point.position, _points[ point.UUID ], 3 );
		}

		OnChange( Commands.updatePoints, points );
		if (sync)
            Output(Commands.updatePoints, new { points = SerializePoints( points ) } );
	}


	public void Clear ( bool sync = false )
	{
		_points.Clear( );

		if( sync )
            Output(Commands.clear, new { } );
	}

	public override object GetState( )
	{
		return new
		{
			points = _points.Select( kvp => new
			{
				UUID = kvp.Key,
				position = kvp.Value	
			} ).ToArray()
		};
	}

	public override void SetState( IPayload state )
	{
		_points.Clear( );
		OnAddPoints( state );
	}
}