using System;
using System.Net.Sockets;

namespace ServiceInterface.Interfaces;

public interface ISIServerUtils
{

    public Task StartInputStreaming(TcpListener serverInput);
    public Task StartOutputStreaming(TcpListener serverOutput);
}
