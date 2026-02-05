using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.API.Recommendations;

public interface IRecommendationsStore
{
    List<int> GetNewTourIds(string userKey);
    void MarkSeen(string userKey, int tourId);
    void AckAll(string userKey);
    void SeedNew(string userKey, IEnumerable<int> tourIds);
}

public sealed class InMemoryRecommendationsStore : IRecommendationsStore
{
    private readonly ConcurrentDictionary<string, HashSet<int>> _newByUser = new();

    public List<int> GetNewTourIds(string userKey)
    {
        var set = _newByUser.GetOrAdd(userKey, _ => new HashSet<int>());
        lock (set)
        {
            return set.ToList();
        }
    }

    public void MarkSeen(string userKey, int tourId)
    {
        var set = _newByUser.GetOrAdd(userKey, _ => new HashSet<int>());
        lock (set)
        {
            set.Remove(tourId);
        }
    }

    public void AckAll(string userKey)
    {
        var set = _newByUser.GetOrAdd(userKey, _ => new HashSet<int>());
        lock (set)
        {
            set.Clear();
        }
    }

    public void SeedNew(string userKey, IEnumerable<int> tourIds)
    {
        var set = _newByUser.GetOrAdd(userKey, _ => new HashSet<int>());
        lock (set)
        {
            foreach (var id in tourIds)
            {
                set.Add(id);
            }
        }
    }
}
