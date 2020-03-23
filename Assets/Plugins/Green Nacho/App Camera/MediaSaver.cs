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

        public void SavePicture(Texture2D picture, PictureFormat pictureFormat)
        {
            string pictureName = GenerateMediaName(picturesBaseName, GetPictureFormatName(pictureFormat));
            NativeGallery.SaveImageToGallery(picture, mediaPath, pictureName);
        }

        public void SaveVideo(string videoPath, VideoFormat videoFormat)
        {
            string videoName = GenerateMediaName(videosBaseName, GetVideoFormatName(videoFormat));
            NativeGallery.SaveVideoToGallery(videoPath, mediaPath, videoName);
        }
    }
}