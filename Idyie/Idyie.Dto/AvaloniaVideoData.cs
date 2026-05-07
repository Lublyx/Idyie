using System;
using System.Diagnostics.Contracts;

namespace Idyie.Dto;

public class AvaloniaVideoData
{
    public int W {get; set;}
    public int H {get; set;}
    public int Size {get; set;}
    public required byte[] Pixels {get; set;}
}
