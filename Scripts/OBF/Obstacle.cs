using System.Collections.Generic;
using UnityEngine.Rendering;
using DOUKH.Settings.Camera;
using DOUKH.Common.Helpers;
using DOUKH.Common.Enums;
using System.Collections;
using DOUKH.OBF.Strings;
using DOUKH.OBF.Numbers;
using DOUKH.OBF.Enums;
using UnityEngine;
using System;

namespace DOUKH.OBF.Obstacle
{
    [AddComponentMenu("DOUKH/OBF/Obstacle")]
    public class ObstacleOBF : MonoBehaviour
    {
        private float FadingDuration => _fadingState == FadingState.Out ? _fadingOutDuration : _fadingInDuration;
        private float CurrentAlpha => _materials[0].color.a;
        //-------------------------------------------------
        [Header("Fading")]
        [SerializeField]
        private bool _overrideAlpha;
        [SerializeField][Range(0, 1)]
        private float _fadingAlpha;
        [SerializeField][Space]
        private bool _overrideDuration;
        [SerializeField][Range(0, NumberOBF.MaxFadingDuration)]
        private float _fadingOutDuration;
        [SerializeField][Range(0, NumberOBF.MaxFadingDuration)]
        private float _fadingInDuration;
        [SerializeField][Space]
        private bool _overrideWeight;
        [SerializeField][Range(0.1f, 2)][Space(2)]
        private float _weightToFade;
        [Header("Rendering")]
        [SerializeField]
        private bool _overrideRender;
        [SerializeField]
        private ShaderRenderMode _fadingRenderMode;
        [Header("Debugging")]
        [SerializeField]
        private FadingState _fadingState;
        [SerializeField][Space(5)]
        private bool _logging;
        //[SerializeField] private bool bool _overrideDeltaTime;
        //-------------------------------------------------
        private readonly List<Renderer> _renderers = new();
        private readonly List<Material> _materials = new();
        private Material[] _initialMaterials;
        private DeltaTimeType _deltaTimeType;
        private IEnumerator _fadingIn, _fadingOut, _fading;
        private float _initialAlpha, _targetAlpha, _alphaRange, _hitsWeight, _fadingTime, _realFadingDuration, _startAlpha;
        private int _materialsCount;
        //-------------------------------------------------
        //-------------------------------------------------
        public void GetHit(ShaderRenderMode shaderRenderMode, float fadingAlpha, float fadingOutDuration, float fadingInDuration, float weightToFade = 1, float rayWeight = 1)
        {
            _hitsWeight += rayWeight;
            // TODO MEvent
            if (_fading is null) InitializeSettings(shaderRenderMode, fadingAlpha, fadingOutDuration, fadingInDuration, weightToFade);
            if (_fadingState == FadingState.None && IsHittedEnough())
            {
                InterruptFading();
                StartFading(_fadingState);
            }
        }

        private void Awake()
        {
            InitializeMaterials();
            InitializeFields();
        }

        private IEnumerator Fading()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                var isHittedEnough = IsHittedEnough();
                if (_fadingState == FadingState.Out && !isHittedEnough || _fadingState == FadingState.In && isHittedEnough) InterruptFading();
                if (_fadingState == FadingState.None)
                {                
                    if (IsFadingNeeded(FadingState.Out) && isHittedEnough) StartFading(FadingState.Out);
                    else if (IsFadingNeeded(FadingState.In) && !isHittedEnough) StartFading(FadingState.In);
                }
                ResetHits();
                //yield return null;
            }
        }

        private IEnumerator FadingOut()
        {
            _fadingState = FadingState.Out;
            SetRenderSettings();           
            SetTargetAlpha();
            SetFadeDuration();
            ResetFadingTime();
            while (IsFadingNeeded())
            {
                //yield return new WaitForFixedUpdate();
                Fade();     
                yield return null;
            }
            ResetState();
        }

        private IEnumerator FadingIn()
        {
            _fadingState = FadingState.In;
            SetTargetAlpha();
            SetFadeDuration();
            ResetFadingTime();
            while (IsFadingNeeded())
            {
                //yield return new WaitForFixedUpdate();
                Fade();
                yield return null;
            }
            ResetState();
            ResetMaterials();
            InterruptFading();
        }

        private void Fade()
        {
            _fadingTime += Helper.GetDeltaTime(_deltaTimeType);
            _materials.ForEach(m => SetMaterialAlpha(m));
        }

        private void StartFading(FadingState state)
        {
            _startAlpha = CurrentAlpha;
            switch (state)
            {
                case FadingState.In:
                    _fadingIn = FadingIn();
                    StartCoroutine(_fadingIn);
                    break;
                case FadingState.Out:
                    _fadingOut = FadingOut();
                    StartCoroutine(_fadingOut);
                    break;
                case FadingState.None:
                    _fading = Fading();
                    StartCoroutine(_fading);
                    break;
            }
        }

        private void InterruptFading()
        {
            var fading = _fadingState switch
            {
                FadingState.In => _fadingIn,
                FadingState.Out => _fadingOut,
                FadingState.None => _fading
            };
            if (fading is not null)
            {
                StopCoroutine(fading);
                fading = null;
            }
            ResetState();
        }

        private void InitializeFields()
        {
            _deltaTimeType = CameraSettings.OBF.DeltaTimeType;
            _initialAlpha = CurrentAlpha;
            _alphaRange = Helper.Diff(_initialAlpha, GetTargetAlpha(FadingState.Out));
        }

        private void InitializeMaterials()
        {
            if (_renderers.Count == 0) _renderers.AddRange(GetComponentsInChildren<Renderer>());
            _renderers.ForEach(r => _materials.AddRange(r.materials));

            _materialsCount = _materials.Count;
            _initialMaterials = new Material[_materialsCount];
            for (int m = 0; m < _materialsCount; m++)
            {
                _initialMaterials[m] = new Material(_materials[m].shader);
                _initialMaterials[m].CopyPropertiesFromMaterial(_materials[m]);
            }
        }

        private void InitializeSettings(ShaderRenderMode shaderRenderMode, float fadingAlpha, float fadingOutDuration, float fadingInDuration, float weightToFade)
        {
            _fadingRenderMode = GetFadingRenderMode(shaderRenderMode);
            _fadingAlpha = GetFadingAlpha(fadingAlpha);
            _fadingOutDuration = GetFadingOutDuration(fadingOutDuration);
            _fadingInDuration = GetFadingInDuration(fadingInDuration);
            _weightToFade = GetWeightToFade(weightToFade);
            //_deltaTimeType = GetDeltaTimeType(deltaTimeType);
        }

        private void SetRenderSettings()
        {
            foreach (var material in _materials)
                if (string.Equals(material.shader.name, NamesOBF.DefaultShader))
                {
                    material.SetOverrideTag(NamesOBF.RenderTag, NamesOBF.Transparent);
                    material.SetInt(PropertiesOBF.Depth, 0);
                    material.SetInt(PropertiesOBF.DestinationBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.renderQueue = material.GetFloat(PropertiesOBF.Mode) < 2 ? (int)RenderQueue.GeometryLast : (int)RenderQueue.Transparent;
                    //material.SetFloat(PropertiesOBF.Mode, (int)renderMode); 2-Fade | 3-Transp
                    switch (_fadingRenderMode)
                    {
                        case ShaderRenderMode.Fade:
                            material.SetInt(PropertiesOBF.SourceBlend, (int)BlendMode.SrcAlpha);
                            material.EnableKeyword(KeywordsOBF.Fade);
                            material.DisableKeyword(KeywordsOBF.Cutout);
                            material.DisableKeyword(KeywordsOBF.Transparent);
                            break;
                        case ShaderRenderMode.Transp:
                            material.SetInt(PropertiesOBF.SourceBlend, (int)BlendMode.One);
                            material.EnableKeyword(KeywordsOBF.Transparent);
                            material.DisableKeyword(KeywordsOBF.Cutout);
                            material.DisableKeyword(KeywordsOBF.Fade);
                            break;
                        default:
                            // URP TODO
                            //material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                            //material.SetShaderPassEnabled("DepthOnly", false);
                            //material.SetShaderPassEnabled("ShadowCaster", true);
                            //material.SetInt("_Surface", 1);
                            break;
                    }
                }
        }

        private void SetMaterialAlpha(Material mat)
        {
            if (mat.HasProperty(PropertiesOBF.Color))
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, _realFadingDuration > 0 ? Mathf.Lerp(_startAlpha, _targetAlpha, _fadingTime / _realFadingDuration) : _targetAlpha);
        }

        private void SetTargetAlpha() => _targetAlpha = GetTargetAlpha(_fadingState);

        private float GetTargetAlpha(FadingState state) => state == FadingState.Out ? _fadingAlpha : _initialAlpha;

        private void SetFadeDuration()
        {
            _realFadingDuration = FadingDuration * Helper.Diff(CurrentAlpha, _targetAlpha) / _alphaRange;
            if (_logging) Helper.Log("Target Alpha", _targetAlpha.ToString(), true);
        }

        private void ResetMaterials() { for (int m = 0; m < _materialsCount; m++) _materials[m].CopyPropertiesFromMaterial(_initialMaterials[m]); }

        private void ResetHits() => _hitsWeight = 0;

        private void ResetState() => _fadingState = FadingState.None;

        private void ResetFadingTime() => _fadingTime = 0;

        private bool IsFadingNeeded(FadingState state) => Helper.AreDifferent(CurrentAlpha, GetTargetAlpha(state));

        private bool IsFadingNeeded() => Helper.AreDifferent(CurrentAlpha, _targetAlpha);

        private bool IsHittedEnough(float requiredWeight = 1) => _hitsWeight >= requiredWeight;

        private ShaderRenderMode GetFadingRenderMode(ShaderRenderMode faderValue) => _overrideRender ? _fadingRenderMode : faderValue;

        private float GetFadingAlpha(float faderValue) => _overrideAlpha ? _fadingAlpha : faderValue;

        private float GetFadingOutDuration(float faderValue) => _overrideDuration ? _fadingOutDuration : faderValue;

        private float GetFadingInDuration(float faderValue) => _overrideDuration ? _fadingInDuration : faderValue;

        private float GetWeightToFade(float faderValue) => _overrideWeight ? _weightToFade : faderValue;

        //private DeltaTimeType GetDeltaTimeType(DeltaTimeType valueOBF) => _overrideDeltaTime ? _deltaTimeType : valueOBF;
    }
}