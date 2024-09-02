using System;

namespace CrazyPanda.UnityCore.Utils
{
    public interface IDateProvider
    {
        DateTime GetCurrentTime();

        string Marker { get; }
    }
}
