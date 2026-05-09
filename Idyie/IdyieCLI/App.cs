using ServiceInterface.Interfaces;

namespace IdyieCLI;

public class App
{
    private readonly ISIStreaming _siStreaming;

    public App(ISIStreaming siStreaming)
    {
        _siStreaming = siStreaming;
    }

    public async Task Run()
    {
        DateTime lastTry = DateTime.Now;

        TaskCompletionSource awaitTask = new TaskCompletionSource();
        while (true)
        {
            if (lastTry < DateTime.Now.AddSeconds(-10))
            {
                if (!await _siStreaming.StartStreaming())
                    awaitTask.SetResult();
                
                await awaitTask.Task;
                lastTry = DateTime.Now;
                awaitTask = new TaskCompletionSource();
            }
        }
    }
}
