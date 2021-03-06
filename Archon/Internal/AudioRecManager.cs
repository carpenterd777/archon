using System;
using System.IO;

using Archon.Utils;

namespace Archon
{
    internal abstract class AudioRecManager
    {
        public static string ArchonRecordingsDir         // the name of the directory that will be written to 
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/ArchonRecordings";
            }
        }

        public string Filename { get; set; }               // the name of the file that will be written to after recording

        public const int MAX_RECORDING_SECONDS =         // the max amount of time in seconds that a recording can last 
            3 * // minutes
            60; // seconds
        public static readonly string RECORDING_STOPPED_NO_START =
        "Recording was not found to be in progress when StopRecording() was called";

        protected bool _isRecordingAudio = false;          // whether or not a recording is in progress
        protected RecordingManagerStatus _status;          // status of the recording manager (mic detected, invalid platform, etc)

        // Public API

        /// <summary>
        /// Checks if a valid microphone for recording audio is connected to this platform.
        /// </summary>
        abstract public void DetectMic();

        /// <summary>
        /// Starts recording using audio from a connected microphone.
        /// <summary/>
        abstract public void StartRecording();

        /// <summary>
        /// Stops recording audio from a connected microphone.
        /// <summary/>
        public virtual void StopRecording()
        {
            if (!_isRecordingAudio)
            {
                throw new AudioRecordingManagerException(RECORDING_STOPPED_NO_START);
            }
        }

        /// <summary>
        /// Returns whether or not the current recording manager status implies that a recording can begin.
        /// </summary>
        public bool CanRecord() => _status == RecordingManagerStatus.MicDetected;

        public static AudioRecManager GetPlatformSpecificAudioManager()
        {
            PlatformID platform = System.Environment.OSVersion.Platform;
            switch (platform)
            {
                case PlatformID.Unix:
                    return new UnixAudioRecManager();
                case PlatformID.Win32NT:
                    return new WinAudioRecManager();
                default:
                    throw new AudioRecordingManagerException(
                        Strings.GetUnsupportedPlatformForRecordingWarning(platform));
            }
        }

        public void Initialize()
        {
            // the audio recording manager is initialized 'with suspicion' that it is in the least valid state 
            // for recording. it continually makes checks after being initialized that clear up the 'suspicion'
            // until it reaches its most valid state and can record.

            // for that reason, the only check that should be made in method bodies for the
            // main API is to check that the staus is in fact 'MicDetected'. 

            if (!Directory.Exists(ArchonRecordingsDir))
                Directory.CreateDirectory(ArchonRecordingsDir);

            _status = RecordingManagerStatus.InvalidPlatform;
        }
    }

    internal enum RecordingManagerStatus
    {
        InvalidPlatform,
        NoCompatibleAPI,
        MicNotDetected,
        MicDetected
    }

    internal class AudioRecordingManagerException : Exception
    {
        public AudioRecordingManagerException(string errorMessage) : base(errorMessage)
        { }
    }
}
