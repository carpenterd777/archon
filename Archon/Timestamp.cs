using System;

namespace Archon
{
    public class Timestamp
    {
        private DateTime instantiationTime;

        public override string ToString() =>
            $"[{instantiationTime.ToShortTimeString()}]";

        public Timestamp(DateTime dt)
        {
            instantiationTime = dt;
        }

        public Timestamp() : this(DateTime.Now)
        {
        }
    }
}
