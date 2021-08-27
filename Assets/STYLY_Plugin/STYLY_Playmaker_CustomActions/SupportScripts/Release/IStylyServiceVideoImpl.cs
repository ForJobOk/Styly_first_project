using System;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// cache type for styly video player
    /// ***** 値の追加は可能だが、対応する数値を変更してはダメ *****
    /// PlayMaker CustomActions (VideoInit) で利用しており、AssetBundleに保存された内容に関係するため。
    /// </summary>
    public enum VideoCacheType
    {
        /// <summary>
        /// never use cache. use streaming
        /// </summary>
        Never = 0,

        /// <summary>
        /// automatically use cache.
        /// </summary>
        Auto = 1,

        /// <summary>
        /// always use cache. play after download.
        /// </summary>
        Always = 2,
    }

    /// <summary>
    /// video player parameters
    /// VideoCacheTypeと異なり、状況に応じて変更しても大きな問題はない。
    /// </summary>
    public class VideoParams
    {
        public string uri;
        public bool autoPlay = true;
        public bool loop = true;
        public VideoCacheType cacheType = VideoCacheType.Auto;
        public float volume;
    };

    /// <summary>
    /// interface to handle video player requests
    /// </summary>
    public interface IStylyServiceVideoImpl
    {
        /// <summary>
        /// initialize video player.
        /// </summary>
        /// <param name="targetObj">ビデオプレイヤーが追加されるGameObject。スクリーンとなるメッシュを保持している必要がある。</param>
        /// <param name="videoParams"></param>
        /// <param name="onFinished"></param>
        void VideoInit(GameObject targetObj, VideoParams videoParams, Action<Exception> onFinished);

        /// <summary>
        /// play video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        void VideoPlay(GameObject targetObj, Action<Exception> onFinished);

        /// <summary>
        /// stop playing video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        void VideoStop(GameObject targetObj, Action<Exception> onFinished);

        /// <summary>
        /// pause playing video with video player
        /// it can be resumed by calling VideoPlay().
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        void VideoPause(GameObject targetObj, Action<Exception> onFinished);

        /// <summary>
        /// setting audio volume of video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="volume">ボリューム値 (0～1)</param>
        /// <param name="onFinished"></param>
        void VideoSetVolume(GameObject targetObj, float volume, Action<Exception> onFinished);
    }
}
