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
            string noBrackets = timestampString.Substring(1, timestampString.Length - 2);

            string[] splits = noBrackets.Split(' ');
            string time = splits[0];
            string timeModifier = splits[1]; // AM or PM

            string[] timeSplits = time.Split(':');

            int hour;
            int.TryParse(timeSplits[0], out hour);
            hour = timeModifier == "AM" ? hour : hour + 12;

            if (timeModifier == "AM" && hour == 12)
            {
                hour = 0;
            }

            int minute;
            int.TryParse(timeSplits[1], out minute);

            DateTime dateTime = new DateTime(DateTime.Today.Year,
                DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0);
            return new Timestamp(dateTime);
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
