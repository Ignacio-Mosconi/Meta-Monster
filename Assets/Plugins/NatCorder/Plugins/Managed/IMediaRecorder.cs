/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder {

    using System;

    /// <summary>
    /// A recorder capable of recording video frames (and optionally audio frames) to a media output.
    /// All recorder methods are thread safe, and as such can be called from any thread.
    /// </summary>
    public interface IMediaRecorder : IDisposable {
        /// <summary>
        /// Video width
        /// </summary>
        int pixelWidth { get; }
        /// <summary>
        /// Video height
        /// </summary>
        int pixelHeight { get; }
        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST in the RGBA32 format.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds</param>
        void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : struct;
        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST in the RGBA32 format.
        /// </summary>
        /// <param name="nativeBuffer">Pixel buffer in native memory to commit</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds</param>
        void CommitFrame (IntPtr nativeBuffer, long timestamp);
        /// <summary>
        /// Commit an audio sample buffer for encoding
        /// </summary>
        /// <param name="sampleBuffer">Raw PCM audio sample buffer, interleaved by channel</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds</param>
        void CommitSamples (float[] sampleBuffer, long timestamp);
    }

    namespace Internal {

        public interface IAbstractRecorder {
            IMediaRecorder recorder { get; }
        }
    }
}