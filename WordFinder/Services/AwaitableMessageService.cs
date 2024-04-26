namespace WordFinder.Services;

public class AwaitableMessageService
{
    public delegate Task AwaitTaskHandler(object args);
    private Dictionary<string, List<AwaitTaskHandler>> _funcs = new();

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

        foreach (var task in _funcs[name])
            await task(args);
    }
}