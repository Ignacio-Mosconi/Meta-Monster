using System;
using System.IO;
using System.Collections;
using UnityEngine;
using NatSuite.Recorders;
using NatSuite.Recorders.Clocks;
using NatSuite.Recorders.Inputs;

namespace GreenNacho.AppCamera
{
    public enum PictureFormat
    {
        JPG, PNG
    }

    public enum VideoFormat
    {
        MP4
    }

    public class CameraRecorder : MonoBehaviour
    {
        [Header("Media Formats")]
        [SerializeField] PictureFormat pictureFormat = default;
        [SerializeField] VideoFormat videoFormat = default;

        [Header("Recording Properties")]
        [SerializeField] AudioSource microphoneSource = default;
        [SerializeField, Range(12, 60)] int targetFrameRate = default; 
        [SerializeField, Range(0, 1920)] int targetWidth = default;
        [SerializeField, Range(0, 1920)] int targetHeight = default;

        public PictureFormat PictureFormat { get { return pictureFormat; } }
        public VideoFormat VideoFormat { get { return videoFormat; } }
        public Action<Texture2D> OnPictureTaken { get; set; }
        public Action<string> OnRecordingSaved { get; set; }

        IClock recordingClock;
        IMediaRecorder mediaRecorder;
        CameraInput cameraInput;
        AudioInput audioInput;
        string lastVideoPath;
        bool microphoneActive = false;

        void OnDestroy()
        {
            DeleteLastVideo();
        }

        void DeleteLastVideo()
        {
            if (lastVideoPath != null)
                File.Delete(lastVideoPath);
        }

        void OnRecordingFinished(string filePath)
        {
            DeleteLastVideo();
            lastVideoPath = filePath;
            OnRecordingSaved?.Invoke(filePath);
        }

        IEnumerator StartMicrophone()
        {
            int minFrequency, maxFrequency;
            Microphone.GetDeviceCaps(deviceName: null, out minFrequency, out maxFrequency);

            microphoneSource.mute = false;
            microphoneSource.loop = true;
            microphoneSource.bypassEffects = false;
            microphoneSource.bypassListenerEffects = false;
            microphoneSource.clip = Microphone.Start(deviceName: null, loop: true, lengthSec: 60, maxFrequency);

            yield return new WaitUntil(() => Microphone.GetPosition(deviceName: null) <= 0);
            
            microphoneSource.Play();

            microphoneActive = true;
        }

        void StopMicrophone()
        {
            microphoneSource.Stop();
            Microphone.End(null);

            microphoneActive = false;
        }

        IEnumerator CaptureScreen()
        {
            yield return new WaitForEndOfFrame();

            Texture2D picture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
            Rect source = new Rect(0f, 0f, Screen.width, Screen.height);      
            bool previousFullScreenState = Screen.fullScreen;

            Screen.fullScreen = true;

            picture.ReadPixels(source, 0, 0);
            picture.Apply();

            Screen.fullScreen = previousFullScreenState;

            OnPictureTaken?.Invoke(picture);
        }

        public void TakePicture()
        {
            StartCoroutine(CaptureScreen());
        }

        public void StartRecording(Camera camera, bool recordMicrophone = true)
        {
            int sampleRate = 0;
            int channelCount = 0;

            if (recordMicrophone)
            {
                sampleRate = AudioSettings.outputSampleRate;
                channelCount = (int)AudioSettings.speakerMode;
            }

            mediaRecorder = new MP4Recorder(targetWidth, targetHeight, targetFrameRate, sampleRate, channelCount);
            recordingClock = new RealtimeClock();
            cameraInput = new CameraInput(mediaRecorder, recordingClock, camera);
            
            if (recordMicrophone)
            {
                StartCoroutine(StartMicrophone());
                audioInput = new AudioInput(mediaRecorder, recordingClock, microphoneSource, mute: true);
            }
        }

        public async void StopRecording()
        {
            if (microphoneActive)
            {
                StopMicrophone();
                audioInput?.Dispose();
            }

            cameraInput?.Dispose();
            
            string videoPath = await mediaRecorder.FinishWriting();

            OnRecordingFinished(videoPath);
        }
    }
}