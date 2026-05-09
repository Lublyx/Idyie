using System;
using Idyie.Dto;
using ServiceInterface.Services;

namespace ServiceInterface.Interfaces;

public interface ISIStreaming
{
    public Task StartStreaming();
}
