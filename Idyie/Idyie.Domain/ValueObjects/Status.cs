using System.Collections.Immutable;

namespace Idyie.Dto;

public static class Status
{
    public static class Emotions
    {
        public static readonly string Happy = "happy";
        public static readonly string Normal = "normal";
        public static readonly string Danger = "danger";
    }

    public enum Relations
    {
        Owner,
        Friends,
        Ordinary,
        Enemy,
    }

    public static readonly string[] DangerObjects = ["knife", "cell phone"];
}
