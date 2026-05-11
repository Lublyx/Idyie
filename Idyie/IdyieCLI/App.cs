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
        while (true)
        {
            try
            {
                await _siStreaming.StartStreaming();
            }
            catch (Exception e)
            {
                throw new EndOfStreamException(e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        
    }
}

