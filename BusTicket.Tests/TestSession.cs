using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TestSession : ISession
{
    private Dictionary<string, byte[]> _storage = new();

    public IEnumerable<string> Keys => _storage.Keys;

    public string Id => "TestSession";

    public bool IsAvailable => true;

    public void Clear() => _storage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public void Remove(string key) => _storage.Remove(key);

    public void Set(string key, byte[] value) => _storage[key] = value;

    public bool TryGetValue(string key, out byte[] value)
        => _storage.TryGetValue(key, out value);
}