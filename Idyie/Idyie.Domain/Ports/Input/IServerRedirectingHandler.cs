using System.Net.Sockets;

namespace Idyie.Domain.Ports.Input;

public interface IServerRedirectingHandler
{

    public Task InputStreaming(TcpListener serverInput);

    public Task OutputStreaming(TcpListener serverOutput);

}
