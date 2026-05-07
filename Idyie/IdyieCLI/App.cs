using System;
using ServiceInterface.Interfaces;

namespace IdyieCLI;

public class App
{
    private readonly IStreaming _streaming;
    
    public App(IStreaming streaming)
    {
        _streaming = streaming;
    }

    public void Run()
    {
    CancellationTokenSource cts = new CancellationTokenSource();

    _streaming.StartStreaming(data =>
    {
        Console.WriteLine(data.Pixels);
    }, cts.Token);

    Console.ReadLine();
    cts.Dispose();    
    }
}
