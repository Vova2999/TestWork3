namespace Client.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandResultDataForAttribute : Attribute
{
    public string Command { get; }

    public CommandResultDataForAttribute(string command)
    {
        Command = command;
    }
}