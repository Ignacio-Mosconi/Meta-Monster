using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        [Header("Camera Components")]
        [SerializeField] Camera appCamera = default;
        [SerializeField] CameraRecorder cameraRecorder = default;
        [SerializeField] MediaSaver mediaSaver = default;

        [Header("Media Preview")]
        [SerializeField] GameObject mediaPreviewPanel = default;
        [SerializeField] Image previewPicture = default;
        [SerializeField] VideoPlayer previewVideo = default;

        UIAnimation[] previewAnimations;
        UIAnimation pictureAnimation;
        UIAnimation videoAnimation;
        AspectRatioFitter previewImageFitter;
        AspectRatioFitter previewVideoFitter = default;
        RawImage previewVideoRawImage;
        MediaType mediaInPreview;

        void Awake()
        {
            previewAnimations = mediaPreviewPanel.GetComponentsInChildren<UIAnimation>(includeInactive: true);
            pictureAnimation = previewPicture.GetComponent<UIAnimation>();
            videoAnimation = previewVideo.GetComponent<UIAnimation>();
            previewImageFitter = previewPicture.GetComponent<AspectRatioFitter>();
            previewVideoFitter = previewVideo.GetComponent<AspectRatioFitter>();
            previewVideoRawImage = previewVideo.GetComponent<RawImage>();
        }

        void Start()
        {
            for (int i = 0; i < previewAnimations.Length; i++)
                previewAnimations[i].SetUp();
            pictureAnimation.SetUp();
            videoAnimation.SetUp();

            mediaPreviewPanel.SetActive(false);
        }

        IEnumerator ShowVideoAfterPlayerPreparation()
        {
            while (!previewVideo.isPrepared)
                yield return new WaitForEndOfFrame();

            previewVideoRawImage.color = Color.white;
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

            showSequence.Insert(pictureAnimation.ShowStartUpTime, pictureAnimation.Show());
        }

        void PlayPreviewVideo(string path)
        {
            mediaInPreview = MediaType.Video;

            previewVideo.gameObject.SetActive(true);
            previewPicture.gameObject.SetActive(false);

            previewVideoRawImage.color = Color.clear;
            previewVideo.targetTexture.Release();
            previewVideo.url = path;
            previewVideo.Play();

            StartCoroutine(ShowVideoAfterPlayerPreparation());

            previewVideoFitter.aspectRatio = (float)Screen.width / Screen.height;
            mediaPreviewPanel.SetActive(true);

            Sequence showSequence = DOTween.Sequence();

            for (int i = 0; i < previewAnimations.Length; i++)
                showSequence.Insert(previewAnimations[i].ShowStartUpTime, previewAnimations[i].Show());

            showSequence.Insert(videoAnimation.ShowStartUpTime, videoAnimation.Show());
        }

        public void TakePicture()
        {
            cameraRecorder.TakePicture(appCamera, () =>
            {
                Texture2D picture = cameraRecorder.LastPictureTaken;
                PictureFormat pictureFormat = cameraRecorder.PictureFormat;

                mediaSaver.SavePicture(picture, pictureFormat);
                ShowPreviewPicture(picture);
            });
        }

        public void RecordVideo()
        {
            cameraRecorder.StartRecording(appCamera, () =>
            {
                string videoPath = cameraRecorder.LastVideoTakenPath;
                VideoFormat videoFormat = cameraRecorder.VideoFormat;

                mediaSaver.SaveVideo(videoPath, videoFormat);
                PlayPreviewVideo(videoPath);
            });
        }

        public void DismissMedia()
        {
            Sequence hideSequence = DOTween.Sequence();

            for (int i = 0; i < previewAnimations.Length; i++)
                hideSequence.Insert(previewAnimations[i].HideStartUpTime, previewAnimations[i].Hide());

            if (mediaInPreview == MediaType.Picture)
                hideSequence.Insert(pictureAnimation.HideStartUpTime, pictureAnimation.Hide());
            else
                hideSequence.Insert(videoAnimation.HideStartUpTime, videoAnimation.Hide());

            hideSequence.OnComplete(() => 
            {
                mediaInPreview = MediaType.None;
                mediaPreviewPanel.SetActive(false);
                previewPicture.gameObject.SetActive(false);
                previewVideo.gameObject.SetActive(false);
            });
        }
    }
}