using System;
using System.IO;
using System.Collections;
using UnityEngine;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;

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
            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
                yield break;

            microphoneSource.clip = Microphone.Start(deviceName: null, loop: true, lengthSec: 60, frequency: 48000);
            
            while (Microphone.GetPosition(deviceName: null) <= 0)
                yield return new WaitForEndOfFrame();
            
            microphoneSource.timeSamples = Microphone.GetPosition(deviceName: null);
            microphoneSource.loop = true;
            microphoneSource.Play();

            microphoneActive = true;
        }

        void StopMicrophone()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
                return;

            Microphone.End(null);
            microphoneSource.Stop();

            microphoneActive = false;
        }

        IEnumerator CaptureScreen(Camera appCamera)
        {
            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture;
            Texture2D picture;

            bool previousFullScreenState = Screen.fullScreen;

            Screen.fullScreen = true;
            renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
            appCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            appCamera.Render();

            Rect source = new Rect(0f, 0f, Screen.width, Screen.height);      
            
            picture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);

            picture.ReadPixels(source, 0, 0);
            picture.Apply();

            Screen.fullScreen = previousFullScreenState;
            appCamera.targetTexture = null;
            RenderTexture.active = null;
            renderTexture.Release();

            OnPictureTaken?.Invoke(picture);
        }

        public void TakePicture(Camera appCamera)
        {
            StartCoroutine(CaptureScreen(appCamera));
        }

        public void StartRecording(Camera appCamera, bool recordMicrophone = true)
        {
            mediaRecorder = new MP4Recorder(targetWidth, 
                                            targetHeight, 
                                            targetFrameRate, 
                                            (recordMicrophone) ?  AudioSettings.outputSampleRate : 0,
                                            (recordMicrophone) ? (int)AudioSettings.speakerMode : 0,
                                            OnRecordingFinished);
            recordingClock = new RealtimeClock();
            cameraInput = new CameraInput(mediaRecorder, recordingClock, appCamera);
            
            if (recordMicrophone)
            {
                StartCoroutine(StartMicrophone());
                audioInput = new AudioInput(mediaRecorder, recordingClock, microphoneSource);
            }
        }

        public void StopRecording()
        {
            if (microphoneActive)
            {
                StopMicrophone();
                audioInput?.Dispose();
            }

            cameraInput?.Dispose();
            mediaRecorder?.Dispose();
        }
    }
}