#nullable enable
using System;
public interface IPayload
{
    string?   GetString     (string property);
    double    GetDouble     (string property);
    double[]? GetDoubleArray(string property);
    Guid      GetGuid       (string property);
    bool      HasProperty   (string property);
    IPayload   GetPayload   (string property);
    IPayload[] GetArray     (string property);
}