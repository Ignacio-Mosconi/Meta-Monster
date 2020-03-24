using System;
using System.IO;
using UnityEngine;

namespace GreenNacho.AppCamera
{
    public class MediaSaver: MonoBehaviour
    {
        [Header("Media Base Names")]
        [SerializeField] string picturesBaseName = default;
        [SerializeField] string videosBaseName = default;
        [SerializeField] string dateSuffixFormat = default; 

        [Header("Media Gallery Properties")]
        [SerializeField] string mediaPath = default;

        void Start()
        {
            if (Application.isEditor)
            {
                mediaPath = Path.Combine(Application.persistentDataPath, mediaPath);
                Directory.CreateDirectory(mediaPath);
            }
        }

        string GetPictureFormatName(PictureFormat pictureFormat)
        {
            return ("." + pictureFormat.ToString().ToLower());
        }

        string GetVideoFormatName(VideoFormat videoFormat)
        {
            return ("." + videoFormat.ToString().ToLower());
        }

        string GenerateMediaName(string baseMediaName, string fileExtension)
        {
            string dateSuffix = System.DateTime.Now.ToString(dateSuffixFormat);
            string mediaName = baseMediaName + dateSuffix + fileExtension;

            return mediaName;
        }

        void SavePictureForEditor(Texture2D picture, PictureFormat pictureFormat, string pictureName)
        {
            byte[] imageData = null;
            string imagePath = Path.Combine(mediaPath, pictureName);

            switch (pictureFormat)
            {
                case PictureFormat.JPG:
                    imageData = picture.EncodeToJPG();
                    break;
                case PictureFormat.PNG:
                    imageData = picture.EncodeToPNG();
                    break;
                default:
                    break;
            }

            if (imageData != null)
                File.WriteAllBytes(imagePath, imageData);
        }

        void SaveVideoForEditor(string videoPath, string videoName)
        {
            if (!String.IsNullOrEmpty(videoPath))
                File.Copy(videoPath, Path.Combine(mediaPath, videoName));
        }

        public void SavePicture(Texture2D picture, PictureFormat pictureFormat)
        {
            string pictureName = GenerateMediaName(picturesBaseName, GetPictureFormatName(pictureFormat));
            
            if (!Application.isEditor)
                NativeGallery.SaveImageToGallery(picture, mediaPath, pictureName);
            else
                SavePictureForEditor(picture, pictureFormat, pictureName);
        }

        public void SaveVideo(string videoPath, VideoFormat videoFormat)
        {
            string videoName = GenerateMediaName(videosBaseName, GetVideoFormatName(videoFormat));

            if (!Application.isEditor)
                NativeGallery.SaveVideoToGallery(videoPath, mediaPath, videoName);
            else
                SaveVideoForEditor(videoPath, videoName);
        }
    }
}