using System;

using Archon.Utils;

namespace Archon
{
    internal abstract class AudioPlaybackManager
    {
        public string Filename { get; set; } // the name of the recording to play

        abstract public void Play();

        public static AudioPlaybackManager GetPlatformSpecificAudioManager()
        {
            PlatformID platform = Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Unix:
                    return new UnixAudioPlaybackManager();
                // case PlatformID.Win32NT:
                default:
                    throw new AudioRecordingManagerException(
                        MessageStrings.GetUnsupportedPlatformForRecordingWarning(platform));
            }
        }
    }
}
