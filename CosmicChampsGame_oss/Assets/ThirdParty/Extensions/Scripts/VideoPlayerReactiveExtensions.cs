using System;
using UniRx;
using UnityEngine.Video;

namespace ThirdParty.Extensions
{
    public static class VideoPlayerReactiveExtensions
    {
        public static IObservable<VideoPlayer> OnStartedAsObservable (this VideoPlayer videoPlayer) =>
            Observable.FromEvent<VideoPlayer.EventHandler, VideoPlayer> (
                h => x => h (x),
                h => videoPlayer.started += h,
                h => videoPlayer.started -= h);

        public static IObservable<VideoPlayer> OnPrepareCompletedAsObservable (this VideoPlayer videoPlayer) =>
            Observable.FromEvent<VideoPlayer.EventHandler, VideoPlayer> (
                h => x => h (x),
                h => videoPlayer.prepareCompleted += h,
                h => videoPlayer.prepareCompleted -= h);

        public static IObservable<VideoPlayer> OnSeekCompletedAsObservable (this VideoPlayer videoPlayer) =>
            Observable.FromEvent<VideoPlayer.EventHandler, VideoPlayer> (
                h => x => h (x),
                h => videoPlayer.seekCompleted += h,
                h => videoPlayer.seekCompleted -= h);

        public static IObservable<VideoPlayer> OnLoopPointReachedAsObservable (this VideoPlayer videoPlayer) =>
            Observable.FromEvent<VideoPlayer.EventHandler, VideoPlayer> (
                h => x => h (x),
                h => videoPlayer.loopPointReached += h,
                h => videoPlayer.loopPointReached -= h);

        public static IObservable<(VideoPlayer videoPlayer, long frameIndex)> OnFrameReadyAsObservable (
            this VideoPlayer videoPlayer) =>
            //
            Observable.FromEvent<VideoPlayer.FrameReadyEventHandler, (VideoPlayer videoPlayer, long frameIndex)> (
                h => (x, i) => h ((x, i)),
                h => videoPlayer.frameReady += h,
                h => videoPlayer.frameReady -= h);
    }
}