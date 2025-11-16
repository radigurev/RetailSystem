namespace Shared.Exceptions;

public class ConfigMissingException(string message, string key) : Exception(message)
{
    public string Key => key;
}