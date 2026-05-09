using System.Net;
using System.Net.Sockets;
using ServiceInterface.Interfaces;

namespace Idyie.Tcp;

public class Server
{

    private readonly ISIServerUtils _serverUtils;
    private const string _ipAdress = "127.0.0.1";
    private const int _portInput = 5001;
    private const int _portOutput = 5002;

    public Server(ISIServerUtils serverUtils)
    {
        _serverUtils = serverUtils;
    }

    public async Task StartServer()
    {
        TcpListener serverInput = new TcpListener(IPAddress.Parse(_ipAdress), _portInput);
        TcpListener serverOutput = new TcpListener(IPAddress.Parse(_ipAdress), _portOutput);

        serverInput.Start();
        serverOutput.Start();

        await Task.WhenAll(_serverUtils.StartInputStreaming(serverInput), _serverUtils.StartOutputStreaming(serverOutput));
    }
}
