namespace Archon {
    internal class WinAudioRecManager : AudioRecManager
    {
        public override void DetectMic()
        {
            throw new System.NotImplementedException();
        }

        public override void StartRecording()
        {
            throw new System.NotImplementedException();
        }

        public override void StopRecording()
        {
            base.StopRecording();
            throw new System.NotImplementedException();
        }

        public WinAudioRecManager()
        {
            throw new System.NotImplementedException();
        }
    }
}