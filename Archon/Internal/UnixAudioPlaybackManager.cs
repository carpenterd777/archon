using System.Diagnostics;

using Archon.Utils;

namespace Archon
{
    internal class UnixAudioPlaybackManager : AudioPlaybackManager
    {
        public override void Play()
        {
            Process aplay = Utilities.CreateLinuxProcess($"aplay {Filename} -q");
            aplay.Start();
            aplay.WaitForExit();
            aplay.Dispose();
        }
    }
}
