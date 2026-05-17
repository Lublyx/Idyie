
namespace Idyie.Domain.ValueObjects;

public class ObjectDetected
{
    public required string Label {get; set;}
    public required float Score {get; set;}
    public required int X {get; set;}
    public required int Y {get; set;}
    public required int W {get; set;}
    public required int H {get; set;}
    public required string Emotion {get; set;}
    public required DateTime EmotionTimeOut {get; set;}

    public bool ToDisplay()
    {
        return Label == "person";
    }

    public bool IsNormal()
    {
        return Emotion == Status.Emotions.Normal;
    }

    public bool IsDanger()
    {
        if (Status.DangerObjects.Contains<string>(Label))
        {
            Emotion = Status.Emotions.Danger;
            EmotionTimeOut = DateTime.Now;
        }
        return Emotion == Status.Emotions.Danger;
    }
}
