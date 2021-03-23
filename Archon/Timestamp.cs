using System;
using System.Globalization;

namespace Archon
{
    public class Timestamp
    {
        private DateTime instantiationTime;

        public override string ToString()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            return $"[{instantiationTime.ToShortTimeString()}]";
        }

        public static Timestamp CreateFromString(string timestampString)
        {
            // remove brackets and split time from modifier
            string[] splits = timestampString.Substring(1, timestampString.Length - 2).Split(' ');
            string time = splits[0];
            string timeModifier = splits[1]; // AM or PM

            string[] timeSplits = time.Split(':');

            int hour = to24Hours(int.Parse(timeSplits[0]), timeModifier);
            int minute = int.Parse(timeSplits[1]);

            DateTime dateTime = new DateTime(DateTime.Today.Year,
                DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0);
            return new Timestamp(dateTime);
        }

        private static int to24Hours(int twelveHour, string timeModifier)
        {
            if (!(timeModifier == "AM" || timeModifier == "PM"))
            {
                throw new ArgumentException($"Cannot handle timeModifier {timeModifier}");
            }

            // (12, AM) -> 0
            // (1,  AM) -> 1
            // ...
            // (11, AM) -> 11
            // (12, PM) -> 12
            // (1,  PM) -> 13
            // ...
            // (11, PM) -> 23

           if (timeModifier == "AM")
           {
               if (twelveHour == 12)
                   return 0;
                return twelveHour;
           }
           else // PM
           {
               if (twelveHour == 12)
                   return 12;
               return twelveHour + 12;
           }
        }

        public Timestamp(DateTime dt)
        {
            instantiationTime = dt;
        }

        public Timestamp() : this(DateTime.Now)
        {
        }
    }
}
