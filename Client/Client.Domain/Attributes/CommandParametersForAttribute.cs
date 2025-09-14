namespace Client.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandParametersForAttribute : Attribute
{
    public string Command { get; }

    public CommandParametersForAttribute(string command)
    {
        Command = command;
    }
}