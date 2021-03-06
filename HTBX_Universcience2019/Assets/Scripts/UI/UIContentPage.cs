﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CRI.HitBox.Settings;
using CRI.HitBox.Lang;

namespace CRI.HitBox.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIContentPage : UIPage<ContentPageSettings>
    {
        [SerializeField]
        protected TranslatedText _content;
        [SerializeField]
        protected RawImage _rawImage;
        [SerializeField]
        protected RawImage _videoTexture;

        private Texture[] _frames;

        private int _frameIndex;

        private int _framerate;

        private float _timeShow;

        /// <summary>
        /// The path of the video clip that will be played when the path is shown.
        /// </summary>
        [Tooltip("The path of the video clip that will be played when the path is shown.")]
        private string _videoClipPath = "";
        /// <summary>
        /// The path of the audio clip that will be played when the path is shown.
        /// </summary>
        [Tooltip("The path of the audio clip that will be played when the page is shown.")]
        private StringCommon _audioClipPath;

        public override void Hide()
        {
            base.Hide();
            if (!String.IsNullOrEmpty(_videoClipPath) && _videoTexture.enabled)
                VideoManager.instance.StopClip(_videoClipPath);
            if (!String.IsNullOrEmpty(_audioClipPath.key) && AudioManager.instance.isLoaded)
                AudioManager.instance.StopClip(_audioClipPath.key, _audioClipPath.common);
            _frameIndex = 0;
        }

        public override void Show()
        {
            base.Show();
            if (!String.IsNullOrEmpty(_videoClipPath) && _videoTexture.enabled)
                VideoManager.instance.PlayClip(_videoClipPath, (RenderTexture)_videoTexture.texture);
            if (!String.IsNullOrEmpty(_audioClipPath.key) && AudioManager.instance.isLoaded)
                AudioManager.instance.PlayClip(_audioClipPath.key, _audioClipPath.common);
            _frameIndex = 0;
            _timeShow = Time.time;
        }

        public override void Init(ContentPageSettings contentPageSettings)
        {
            base.Init(contentPageSettings);
            _content.InitTranslatedText(contentPageSettings.content);
            if (!String.IsNullOrEmpty(contentPageSettings.imagePath.key) && TextureManager.instance.HasTexture(contentPageSettings.imagePath.key))
                _rawImage.texture = TextureManager.instance.GetTexture(contentPageSettings.imagePath);
            if (contentPageSettings.gif != null)
            {
                _frames = LoadGif(contentPageSettings.gif);
                _framerate = contentPageSettings.gif.framerate;
            }
            if (AudioManager.instance.HasClip(contentPageSettings.audioPath.key))
                _audioClipPath = contentPageSettings.audioPath;
            _videoTexture.enabled = !String.IsNullOrEmpty(contentPageSettings.videoPath.key);
            if (_videoTexture.enabled)
                _videoClipPath = contentPageSettings.videoPath.key;
        }

        public void Update()
        {
            if (_frames != null && _frames.Length > 0 && _visible)
            {
                _frameIndex = ((int)((Time.time - _timeShow) * _framerate) % _frames.Length);
                _rawImage.texture = _frames[_frameIndex];
            }
        }
    }
}