namespace Explorer.BuildingBlocks.Core.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}