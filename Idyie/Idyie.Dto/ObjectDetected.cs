using System;

namespace Idyie.Dto;

public class ObjectDetected
{
    public required string Label {get; set;}
    public required float Score {get; set;}
    public required int X {get; set;}
    public required int Y {get; set;}
    public required int W {get; set;}
    public required int H {get; set;}
    public required bool ToDisplay {get; set;}
}
