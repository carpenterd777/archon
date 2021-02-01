using System;
using System.IO;
using System.Diagnostics;

namespace Archon
{
    /// <remarks>
    /// Uses the Diagnostics namespace to facilitate audio recording through console commands.
    /// Depends on the ALSA driver and its commands for UNIX support, which may hinder portability.
    /// </remarks>
    internal class AudioRecordingManager
    {
        public static string ArchonRecordingsDir         // the name of the directory that will be written to 
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)  + "/ArchonRecordings";
            }
        }

        public string Filename {get; set;}               // the name of the file that will be written to after recording

        public const int MAX_RECORDING_SECONDS =         // the max amount of time in seconds that a recording can last 
            3  * // minutes
            60 ; // seconds

        private PlatformID _platform;                    // the identified platform this is running on
        private bool _isRecordingAudio = false;          // whether or not a recording is in progress
        private RecordingManagerStatus _status;          // status of the recording manager (mic detected, invalid platform, etc)
        private Process _activeAlsa;                     // the process to record audio


        // Public API

        /// <summary>
        /// Checks if a valid microphone for recording audio is connected to this platform.
        /// </summary>
        public void DetectMic()
        {
            takeActionBasedOnPlatform(detectMicUnix, detectMicWin, () => 
                    throw new AudioRecordingManagerException($"Failed to check for status {_status}"));
        }

        /// <summary>
        /// Starts recording using audio from a connected microphone.
        /// <summary/>
        public void StartRecording()
        {
            takeActionBasedOnPlatform(startRecordingUnix, startRecordingWin, () => 
                    throw new AudioRecordingManagerException($"Failed to check for status {_status}"));
        }

        /// <summary>
        /// Stops recording audio from a connected microphone.
        /// <summary/>
        public void StopRecording()
        {
            if (_isRecordingAudio) 
            {
                takeActionBasedOnPlatform(stopRecordingUnix, stopRecordingWin, () => 
                        throw new AudioRecordingManagerException($"Failed to check for status {_status}"));
            }
            else
            {
                throw new AudioRecordingManagerException(
                        "Recording was not found to be in progress when StopRecording() was called");
            }
        }

        /// <summary>
        /// Returns whether or not the current recording manager status implies that a recording can begin.
        /// </summary>
        public bool CanRecord() => _status == RecordingManagerStatus.MicDetected;

        // Private methods

        private void detectMicUnix()
        {
            Process arecord = createLinuxProcess("arecord -l");
            arecord.Start();
            string result = arecord.StandardOutput.ReadToEnd();
            arecord.WaitForExit();

            if (result.Split("\n")[1].Substring(0, 6) == "card 0")
            {
                _status = RecordingManagerStatus.MicDetected;
            } 
        }

        private void detectMicWin()
        {
            return; // Not implemented
        }

        private void startRecordingUnix()
        {
            _isRecordingAudio = true;
            _activeAlsa = alsaRecord(Filename);
            _activeAlsa.Start();
            // _activeAlsa.WaitForExit();
        }

        private void startRecordingWin()
        {
            return; // Not implemented
        }

        private void stopRecordingUnix()
        {
            _isRecordingAudio = false;
            _activeAlsa.Kill(); // this seems excessive, there's probably a more elegant way to handle this
        }

        private void stopRecordingWin()
        {
            return; // Not implemented
        }

        private void warn(string text) => Warner.Warn(Console.Out, text);

        private Process createLinuxProcess(string command)
        {
            Process proc = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName               = "/bin/bash",
                    Arguments              = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                }
            };
            
            return proc;
        }

        private void confirmAlsaInstall()
        {
            Process arecord = createLinuxProcess("arecord --version");

            arecord.Start();
            string result = arecord.StandardOutput.ReadToEnd();
            arecord.WaitForExit();

            // TODO: replace with regex to match all version 1s
            if (result == "arecord: version 1.2.2 by Jaroslav Kysela <perex@perex.cz>\n")
                _status =  RecordingManagerStatus.MicNotDetected; // mic detection happens separately
            else // alsa possibly not installed
                _status =  RecordingManagerStatus.NoCompatibleAPI;
        }

        private Process alsaRecord
        (
         string filename,
         int durationSeconds = MAX_RECORDING_SECONDS, 
         string format = "cd" // 16 bit little endian, 44100, stereo 
        )
        {
            string command = $"arecord --duration={durationSeconds} --format={format} --nonblock --quiet {filename}";
            Process arecord = createLinuxProcess(command);

            return arecord;
        }

        private void takeActionBasedOnPlatform(Action unixAction, Action winAction, Action defaultAction)
        {
            switch (_platform)
            {
                case PlatformID.Unix:
                    unixAction();
                    break;
                case PlatformID.Win32NT:
                    winAction();
                    break;
                default:
                    defaultAction();
                    break;
            }
        }

        // Constructors
        
        public AudioRecordingManager()
        {

            // the audio recording manager is initialized 'with suspicion' that it is in the least valid state 
            // for recording. it continually makes checks after being initialized that clear up the 'suspicion'
            // until it reaches its most valid state and can record.
        
            // for that reason, the only check that should be made in method bodies for the
            // main API is to check that the staus is in fact 'MicDetected'. 

            if (!Directory.Exists(ArchonRecordingsDir))
                Directory.CreateDirectory(ArchonRecordingsDir);

            _platform = System.Environment.OSVersion.Platform;            
            _status = RecordingManagerStatus.InvalidPlatform;

            // confirm dependency apis are here

            Action confirmWinApi = () => {return; /* Not implemented */};
            Action unsupportedPlatformAction = () =>
            {
                warn($"Unsupported platform {_platform}. Recording cannot be performed.");
            };

            takeActionBasedOnPlatform(confirmAlsaInstall, confirmWinApi, unsupportedPlatformAction);
        
            if (_status == RecordingManagerStatus.MicNotDetected)
                DetectMic();
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
        {  } 
    }
}
