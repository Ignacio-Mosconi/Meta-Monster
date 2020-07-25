using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Video;
using DG.Tweening;
using GreenNacho.AppCamera;
using GreenNacho.UI;

namespace MetaMonster
{
    public enum MediaType
    {
        Picture,
        Video,
        None
    }

    public class AppCameraController : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] Camera appCamera = default;

        [Header("Camera Components")]
        [SerializeField] CameraRecorder cameraRecorder = default;
        [SerializeField] MediaSaver mediaSaver = default;

        [Header("Recording Icons")]
        [SerializeField] GameObject recordingIndicator = default;

        [Header("Media Preview")]
        [SerializeField] GameObject mediaPreviewPanel = default;
        [SerializeField] VideoPlayer videoPlayer = default;
        [SerializeField] Image previewPicture = default;
        [SerializeField] RawImage previewVideo = default;

        [Header("Media Frames")]
        [SerializeField] GameObject pictureFrame = default;

        [Header("Other Properties")]
        [SerializeField, Range(1f, 2f)] float holdTimeToStartRecording = default;

        UIAnimation[] previewAnimations;
        AspectRatioFitter previewImageFitter;
        AspectRatioFitter previewVideoFitter;
        Coroutine recordingAttempt;
        MediaType mediaInPreview;
        Texture2D lastPictureTaken;
        string lastVideoPath;
        float timeAtCameraButtonPressed;

        public UnityEvent OnStartedTakingFootage { get; private set; } = new UnityEvent();
        public UnityEvent OnFootageDismissed { get; private set; } = new UnityEvent();

        void Awake()
        {
            previewAnimations = mediaPreviewPanel.GetComponentsInChildren<UIAnimation>(includeInactive: true);
            previewImageFitter = previewPicture.GetComponent<AspectRatioFitter>();
            previewVideoFitter = previewVideo.GetComponent<AspectRatioFitter>();
        }

        void OnEnable()
        {
            cameraRecorder.OnPictureTaken += OnPictureTaken;
            cameraRecorder.OnRecordingSaved += OnRecordingSaved;

            mediaPreviewPanel.SetActive(false);
            recordingIndicator.SetActive(false);
            pictureFrame.SetActive(false);
        }

        void OnDisable()
        {
            cameraRecorder.OnPictureTaken -= OnPictureTaken;
            cameraRecorder.OnRecordingSaved -= OnRecordingSaved;
        }

        void Start()
        {
            for (int i = 0; i < previewAnimations.Length; i++)
                previewAnimations[i].SetUp();
        }

        void OnPictureTaken(Texture2D picture)
        {
            lastPictureTaken = picture;

            pictureFrame.SetActive(false);
            ShowPreviewPicture(picture);
        }

        void OnRecordingSaved(string videoPath)
        {
            lastVideoPath = videoPath;
            PlayPreviewVideo(videoPath);
        }

        IEnumerator ShowVideoAfterPlayerPreparation()
        {
            previewVideo.color = Color.clear;

            while (!videoPlayer.isPrepared)
                yield return new WaitForEndOfFrame();

            previewVideo.color = Color.white;
        }

        void ShowPreviewPicture(Texture2D picture)
        {
            Rect rect = new Rect(0, 0, picture.width, picture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite sprite = Sprite.Create(picture, rect, pivot);

            mediaInPreview = MediaType.Picture;

            previewPicture.gameObject.SetActive(true);
            previewVideo.gameObject.SetActive(false);

            previewPicture.sprite = sprite;
            previewImageFitter.aspectRatio = (float)picture.width / picture.height;
            mediaPreviewPanel.SetActive(true);

            Sequence showSequence = DOTween.Sequence();

            for (int i = 0; i < previewAnimations.Length; i++)
                showSequence.Insert(previewAnimations[i].ShowStartUpTime, previewAnimations[i].Show());
        }

        void PlayPreviewVideo(string path)
        {
            mediaInPreview = MediaType.Video;

            previewVideo.gameObject.SetActive(true);
            previewPicture.gameObject.SetActive(false);

            mediaPreviewPanel.SetActive(true);

            videoPlayer.targetTexture.Release();
            videoPlayer.url = path;
            videoPlayer.Play();

            StartCoroutine(ShowVideoAfterPlayerPreparation());

            previewVideoFitter.aspectRatio = (float)Screen.width / Screen.height;

            Sequence showSequence = DOTween.Sequence();

            for (int i = 0; i < previewAnimations.Length; i++)
                showSequence.Insert(previewAnimations[i].ShowStartUpTime, previewAnimations[i].Show());
        }

        void StopRecordingAttempt()
        {
            if (recordingAttempt != null)
            {
                StopCoroutine(recordingAttempt);
                recordingAttempt = null;
            }
        }

        IEnumerator AttemptToRecord()
        {
            yield return new WaitForSeconds(holdTimeToStartRecording);
            
            recordingAttempt = null;
            cameraRecorder.StartRecording(appCamera);
            recordingIndicator.SetActive(true);

            OnStartedTakingFootage.Invoke();
        }

        public void OnCameraButtonDown()
        {
            timeAtCameraButtonPressed = Time.time;
            recordingAttempt = StartCoroutine(AttemptToRecord());
        }

        public void OnCameraButtonClick()
        {
            float timeDiff = Time.time - timeAtCameraButtonPressed;

            timeAtCameraButtonPressed = 0f;

            if (timeDiff <= holdTimeToStartRecording)
            {
                StopRecordingAttempt();
                pictureFrame.SetActive(true);
                OnStartedTakingFootage.Invoke();
                cameraRecorder.TakePicture();
            }
            else
            {
                cameraRecorder.StopRecording();
                recordingIndicator.SetActive(false);
            }
        }

        public void DismissMedia()
        {
            Sequence hideSequence = DOTween.Sequence();

            for (int i = 0; i < previewAnimations.Length; i++)
                hideSequence.Insert(previewAnimations[i].HideStartUpTime, previewAnimations[i].Hide());

            hideSequence.OnComplete((TweenCallback)(() => 
            {
                mediaInPreview = MediaType.None;
                mediaPreviewPanel.SetActive(false);
                previewPicture.gameObject.SetActive(false);
                previewVideo.gameObject.SetActive(false);

                OnFootageDismissed.Invoke();
            }));
        }

        public void SaveMedia()
        {
            if (mediaInPreview == MediaType.Picture)
            {
                PictureFormat pictureFormat = cameraRecorder.PictureFormat;  
                mediaSaver.SavePicture(lastPictureTaken, pictureFormat);
            }
            else
            {
                VideoFormat videoFormat = cameraRecorder.VideoFormat;
                mediaSaver.SaveVideo(lastVideoPath, videoFormat);
            }

            DismissMedia();
        }
    }
}