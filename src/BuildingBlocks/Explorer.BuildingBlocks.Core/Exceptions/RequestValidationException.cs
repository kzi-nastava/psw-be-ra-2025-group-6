using System.Collections.Generic;
using System.Linq;

namespace Explorer.BuildingBlocks.Core.Exceptions;

public class RequestValidationException : DomainException
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public RequestValidationException(IEnumerable<ValidationError> errors) : base("Validation failed.")
    {
        Errors = errors.ToList();
    }
}
