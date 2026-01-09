namespace Explorer.BuildingBlocks.Core.Exceptions;

public class AlreadyExistsException : DomainException
{
    public string? Field { get; }

    public AlreadyExistsException(string message, string? field = null) : base(message)
    {
        Field = field;
    }
}
