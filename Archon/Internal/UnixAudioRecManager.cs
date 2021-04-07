using System;
using System.IO;
using System.Diagnostics;

using Archon.Utils;

namespace Archon
{
    /// <remarks>
    /// Uses the Diagnostics namespace to facilitate audio recording through console commands.
    /// Depends on the ALSA driver and its commands for UNIX support, which may hinder portability.
    /// </remarks>
    internal class UnixAudioRecManager : AudioRecManager
    {
        private Process _activeAlsa; // the process to record audio
        private TextWriter _consoleOut; // represents the console output
        public override void DetectMic()
        {
            Process arecord = Utilities.CreateLinuxProcess("arecord -l");
            arecord.Start();
            string result = arecord.StandardOutput.ReadToEnd();
            arecord.WaitForExit();

            string expectedarecordCaptureDeviceOutput = "card 0";

            // this is an extraordinarily brittle test, but my understanding of arecord/aplay 
            // is limited at the moment

            try
            {
                if (result.Split("\n")[1].Substring(0, 6) == expectedarecordCaptureDeviceOutput)
                {
                    _status = RecordingManagerStatus.MicDetected;
                }
            }
            catch (IndexOutOfRangeException)
            {
                // result did not pick up any mics if the string was too short
                // to substring
            }
        }

        public override void StartRecording()
        {
            _isRecordingAudio = true;
            _activeAlsa = alsaRecord(Filename);
            _activeAlsa.Start();
        }

        public override void StopRecording()
        {
            base.StopRecording();
            _isRecordingAudio = false;
            _activeAlsa.Kill(); // this seems excessive, there's probably a more elegant way to handle this
        }

        // Private methods
        private void confirmAlsaInstall()
        {
            Process arecord = Utilities.CreateLinuxProcess("arecord --version");

            arecord.Start();
            string result = arecord.StandardOutput.ReadToEnd();
            arecord.WaitForExit();

            // TODO: replace with regex to match all version 1s
            if (result == "arecord: version 1.2.2 by Jaroslav Kysela <perex@perex.cz>\n")
                _status = RecordingManagerStatus.MicNotDetected; // mic detection happens separately
            else // alsa possibly not installed
                _status = RecordingManagerStatus.NoCompatibleAPI;
        }

        private Process alsaRecord(
         string filename,
         int durationSeconds = MAX_RECORDING_SECONDS,
         string format = "cd" // 16 bit little endian, 44100, stereo 
        )
        {
            string command = $"arecord --duration={durationSeconds} --format={format} --nonblock --quiet {filename}";
            Process arecord = Utilities.CreateLinuxProcess(command);

            return arecord;
        }

        // Constructors

        public UnixAudioRecManager(TextWriter consoleOut)
        {
            _consoleOut = consoleOut;
            confirmAlsaInstall();

            if (_status == RecordingManagerStatus.MicNotDetected)
            {
                Initialize();
            }
            else
            {
                MessageStrings.Warn(_consoleOut, MessageStrings.NO_ALSA);
            }
        }
    }
}
