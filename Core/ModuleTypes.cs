// ModuleTypes.cs
public static class ModuleTypes
{
    private static readonly Dictionary<string, Func<Guid, ModuleCore>> _types = new()
    {
        { ScalarModule.Type, ( UUID ) => new ScalarModule( UUID ) },
        { Vector3Module.Type, ( UUID ) => new Vector3Module( UUID ) },
        { TransformModule.Type, ( UUID ) => new TransformModule( UUID ) },
    };

    public static ModuleCore Create(string type, Guid UUID)
    {
		var constructor = _types.GetValueOrDefault( type ) ?? ( ( UUID ) => new ModuleCore( UUID ) );
		return constructor( UUID );
	}
}