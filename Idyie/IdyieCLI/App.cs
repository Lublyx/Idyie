using Idyie.Domain.Ports.Input;

namespace IdyieCLI;

public class App
{
    private readonly IStreamVideoUseCase _streamVideoUseCase;

    public App(IStreamVideoUseCase streamVideoUseCase)
    {
        _streamVideoUseCase = streamVideoUseCase;
    }

    public async Task Run()
    {
        while (true)
        {
            try
            {
                await _streamVideoUseCase.StreamVideo();
            }
            catch (Exception e)
            {
                throw new EndOfStreamException(e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        
    }
}

