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

        [Header("Microphone Properties")]
        [SerializeField] AudioSource microphoneSource = default;
        [SerializeField] bool recordMicrophone = false;

        public Texture2D LastPictureTaken { get; private set; }
        public string LastVideoTakenPath { get; private set; }
        public PictureFormat PictureFormat { get { return pictureFormat; } }
        public VideoFormat VideoFormat { get { return videoFormat; } }

        MP4Recorder videoRecorder;
        IClock recordingClock;
        CameraInput cameraInput;
        AudioInput audioInput;

        void OnDisable()
        {
            if (LastVideoTakenPath != null)
                File.Delete(LastVideoTakenPath);
        }

        void StartMicrophone()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return;

            microphoneSource.clip = Microphone.Start(null, true, 60, 48000);
            
            while (Microphone.GetPosition(null) <= 0) ;
            
            microphoneSource.timeSamples = Microphone.GetPosition(null);
            microphoneSource.loop = true;
            microphoneSource.Play();
        }

        void StopMicrophone()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return;

            Microphone.End(null);
            microphoneSource.Stop();
        }

        void LoadPreviewPicture(Texture2D picture)
        {
            LastPictureTaken = new Texture2D(picture.width, picture.height);
            byte[] pictureBytes = (pictureFormat == PictureFormat.JPG) ? picture.EncodeToJPG() : picture.EncodeToPNG();

            LastPictureTaken.LoadImage(pictureBytes);
        }

        void OnRecordingFinished(string videoPath, Action callback = null)
        {
            if (LastVideoTakenPath != null)
                File.Delete(LastVideoTakenPath);

            LastVideoTakenPath = videoPath;

            callback?.Invoke();
        }

        IEnumerator CreatePicture(Camera appCamera, Action callback = null)
        {
            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            renderTexture.Create();

            appCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            appCamera.Render();

            Texture2D picture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, mipChain: false);
            Rect source = new Rect(0f, 0f, Screen.width, Screen.height);

            picture.ReadPixels(source, 0, 0);
            picture.Apply();

            LoadPreviewPicture(picture);

            appCamera.targetTexture = null;
            RenderTexture.active = null;

            Destroy(picture);
            Destroy(renderTexture);

            callback?.Invoke();
        }

        public void TakePicture(Camera appCamera, Action callback = null)
        {
            StartCoroutine(CreatePicture(appCamera, callback));
        }

        public void StartRecording(Camera appCamera, Action callback = null)
        {
            recordingClock = new RealtimeClock();
            videoRecorder = new MP4Recorder(
                Screen.width,
                Screen.height,
                Application.targetFrameRate,
                recordMicrophone ? AudioSettings.outputSampleRate : 0,
                recordMicrophone ? (int)AudioSettings.speakerMode : 0,
                (path) => OnRecordingFinished(path)
            );

            cameraInput = new CameraInput(videoRecorder, recordingClock, appCamera);
            if (recordMicrophone)
            {
                StartMicrophone();
                audioInput = new AudioInput(videoRecorder, recordingClock, microphoneSource, true);
            }
        }

        public void StopRecording()
        {
            if (recordMicrophone)
            {
                StopMicrophone();
                audioInput.Dispose();
            }
            cameraInput.Dispose();
            videoRecorder.Dispose();
        }
    }
}