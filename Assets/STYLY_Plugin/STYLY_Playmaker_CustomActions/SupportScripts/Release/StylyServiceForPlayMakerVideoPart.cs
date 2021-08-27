using System;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// video part of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        private IStylyServiceVideoImpl videoImpl;

        public void SetVideoImpl(IStylyServiceVideoImpl impl)
        {
            this.videoImpl = impl;
        }

        /// <summary>
        /// initialize video player.
        /// </summary>
        /// <param name="targetObj">ビデオプレイヤーが追加されるGameObject。スクリーンとなるメッシュを保持している必要がある。</param>
        /// <param name="videoParams"></param>
        /// <param name="onFinished"></param>
        public void VideoInit(GameObject targetObj, VideoParams videoParams, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoInit", onFinished)?.VideoInit(targetObj, videoParams, onFinished);
        }

        /// <summary>
        /// play video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoPlay(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoPlay", onFinished)?.VideoPlay(targetObj, onFinished);
        }

        /// <summary>
        /// stop playing video with video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoStop(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoStop", onFinished)?.VideoStop(targetObj, onFinished);
        }

        /// <summary>
        /// pause playing video with video player
        /// it can be resumed by calling VideoPlay().
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="onFinished"></param>
        public void VideoPause(GameObject targetObj, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoPause", onFinished)?.VideoPause(targetObj, onFinished);
        }

        /// <summary>
        /// setting audio volume of video player
        /// </summary>
        /// <param name="targetObj">VideoInitによりビデオプレイヤーが追加されたGameObject</param>
        /// <param name="volume">ボリューム値 (0～1)</param>
        /// <param name="onFinished"></param>
        public void VideoSetVolume(GameObject targetObj, float volume, Action<Exception> onFinished)
        {
            GetVideoImplOrError("VideoSetVolume", onFinished)?.VideoSetVolume(targetObj, volume, onFinished);
        }

        /// <summary>
        /// videoのimplがあればそれを返却し、ない場合はonFinishedをエラー引数で呼んでnullを返す便利メソッド
        /// </summary>
        /// <param name="actionName">ログ表示用アクション名</param>
        /// <param name="onFinished"></param>
        /// <returns>IStylyServiceVideoImplの実装、またはなければnull</returns>
        private IStylyServiceVideoImpl GetVideoImplOrError(string actionName, Action<Exception> onFinished)
        {
            if (videoImpl != null)
            {
                return videoImpl;
            }
            else
            {
                var msg = $"<{actionName}> called, but the IStylyServiceVideoImpl implementation is not available.";
                Debug.LogError(msg);
                onFinished(new Exception(msg));
                return null;
            }
        }
    }
}
