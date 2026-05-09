using System;
using System.Net.Sockets;

namespace Idyie.Domain.Buisness.Service.Interface;

public interface IBSServerRedirecting
{

    public Task InputStreaming(TcpListener serverInput);

    public Task OutputStreaming(TcpListener serverOutput);

}
