// ModuleTypes.cs
public static class ModuleTypes
{
    private static readonly Dictionary<string, Func<Guid, ModuleCore>> _types = new()
    {
        { ScalarModule.ModuleType, ( UUID ) => new ScalarModule( UUID ) },
        { Vector3Module.ModuleType, ( UUID ) => new Vector3Module( UUID ) },
        { TransformModule.ModuleType, ( UUID ) => new TransformModule( UUID ) },
        { CameraModule.ModuleType, ( UUID ) => new CameraModule( UUID ) },
    };

    public static ModuleCore Create(string type, Guid UUID)
    {
		var constructor = _types.GetValueOrDefault( type ) ?? ( ( UUID ) => new ModuleCore( UUID ) );
		return constructor( UUID );
	}
}