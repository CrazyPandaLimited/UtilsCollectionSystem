using System;

namespace CrazyPanda.UnityCore.Utils
{
    public class SimpleDateProvider : IDateProvider
    {
        private readonly string _marker;
        public SimpleDateProvider(string marker = "l")
        {
            _marker = marker;
        }

        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public string Marker => _marker;
    }
}
