namespace Explorer.BuildingBlocks.Core.Domain;

public abstract class ValueObject
{
    /// <summary>
    /// Provides the components that are used to determine equality for the derived type.
    /// </summary>
    /// <example>
    /// Here's how to implement a concrete ValueObject using yield statements to return equality components:
    /// 
    /// <code>
    /// public class Address : ValueObject
    /// {
    ///     public string Street { get; }
    ///     public string City { get; }
    ///     
    ///     public Address(string street, string city, string zipCode)
    ///     {
    ///         Street = street ?? throw new ArgumentException(nameof(street));
    ///         City = city ?? throw new ArgumentException(nameof(city));
    ///     }
    ///     
    ///     protected override IEnumerable GetEqualityComponents()
    ///     {
    ///         // Use yield return to provide each property that determines equality
    ///         yield return Street;
    ///         yield return City;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <returns>An <see cref="IEnumerable{T}"/> of objects representing the equality components of the object.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var components = GetEqualityComponents().ToList();
        if (components.Count == 0) return 0;

        return components
            .Select((x, i) => (x?.GetHashCode() ?? 0) * (i + 1))
            .Aggregate((x, y) => x ^ y);
    }
}