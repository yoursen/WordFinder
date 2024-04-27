using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace WordFinder.Services;

public class AwaitableMessageService
{
    public delegate Task AwaitTaskHandler(object args);
    private Dictionary<string, List<AwaitTaskHandler>> _funcs = new();
    private readonly ILogger<AwaitableMessageService> _logger;

    public AwaitableMessageService(ILogger<AwaitableMessageService> logger)
    {
        _logger = logger;
    }

    public void Register(string name, AwaitTaskHandler task)
    {
        if (!_funcs.ContainsKey(name))
            _funcs.Add(name, new List<AwaitTaskHandler>());

        _funcs[name].Add(task);
    }

    public void Unregister(string name, AwaitTaskHandler task)
    {
        _funcs[name].Remove(task);
    }

    public async Task Send(string name, object args = null)
    {
        if (!_funcs.ContainsKey(name))
            return;

        try
        {
            foreach (var task in _funcs[name])
                await task(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during send messege.");
        }
    }
}