using System;

namespace Archon
{
    internal class AudioRecordingManager
    {
        
        public string Filename {get; set;}               // the name of the file that will be written to after recording

        public const int MAX_RECORDING_MS = 3 * 60 * 60; // the max amount of time in ms that a recording can last

        private PlatformID _platform;                    // the identified platform this is running on
        private bool _isRecording = false;               // whether or not a recording is in progress

        // Public API

        /// <summary>
        /// Checks if a valid microphone for recording audio is connected to this platform.
        /// </summary>
        public MicrophoneStatus DetectMic()
        {
           switch (_platform)
           {
                case PlatformID.Unix:
                   return detectMicUnix();
                case PlatformID.Win32NT:
                   return detectMicWin();
                default:
                   warn($"Unsupported platform {_platform}. Recording could not be performed.");
                   return MicrophoneStatus.InvalidPlatform;
           }
        }

        /// <summary>
        /// Starts recording using audio from a connected microphone.
        /// <summary/>
        public void StartRecording()
        {
            MicrophoneStatus micStatus = DetectMic();
            if (micStatus == MicrophoneStatus.Detected)
            {
                switch (_platform)
                {
                    case PlatformID.Unix:
                        startRecordingUnix();
                        break;
                    case PlatformID.Win32NT:
                        startRecordingWin();
                        break;
                    default:
                        // this should never happen
                        warn("Could not start recording.");
                        return;
                }
            } 
            else if (micStatus == MicrophoneStatus.NotDetected)
            {
               warn("Could not find a connected microphone."); 
            }
            else if (micStatus == MicrophoneStatus.InvalidPlatform)
            {
                // a warning message has already printed from calling DetectMic()
                return;
            }
            else
            {
                // this should not happen, and means something really bad I guess
                throw new InvalidOperationException($"Could not handle microphone status {micStatus}");
            }
        }

        /// <summary>
        /// Stops recording audio from a connected microphone.
        /// <summary/>
        public void StopRecording()
        {
            if (!_isRecording)
            {
                throw new InvalidOperationException("Recording was not found to be in progress when StopRecording() was called");
            }
            else
            {
                switch (_platform)
                {
                    case PlatformID.Unix:
                        stopRecordingUnix();
                        break;
                    case PlatformID.Win32NT:
                        stopRecordingWin();
                        break;
                    default:
                        // this should never happen
                        warn("Could not stop recording.");
                        return;
                }
            }
        }

        // Private methods

        private MicrophoneStatus detectMicUnix()
        {
            return MicrophoneStatus.InvalidPlatform; // Not implemented
        }

        private MicrophoneStatus detectMicWin()
        {
            return MicrophoneStatus.InvalidPlatform; // Not implemented
        }

        private void startRecordingUnix()
        {
            return; // Not implemented
        }

        private void startRecordingWin()
        {
            return; // Not implemented
        }

        private void stopRecordingUnix()
        {
            return; // Not implemented
        }

        private void stopRecordingWin()
        {
            return; // Not implemented
        }

        private void warn(string text) => Warner.Warn(Console.Out, text);

        // Constructors
        
        public AudioRecordingManager()
        {
            _platform = System.Environment.OSVersion.Platform;            
        }
    }

    public enum MicrophoneStatus
    {
        InvalidPlatform,
        NotDetected,
        Detected
    }

}
