using System.Net.Sockets;

namespace Idyie.Domain.Ports.Input;

public interface IServerRedirectingUseCase
{

    public Task InputStreaming(TcpListener serverInput);

    public Task OutputStreaming(TcpListener serverOutput);

}
