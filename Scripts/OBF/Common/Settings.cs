using MalbersAnimations.Events;
using DOUKH.Common.Enums;
using DOUKH.OBF.Numbers;
using DOUKH.OBF.Enums;
using UnityEngine;

namespace DOUKH.OBF.Settings
{
    [CreateAssetMenu(fileName = "OBF", menuName = "DOUKH/OBF/Settings")]
    public class SettingsOBF : ScriptableObject
    {
        public float FadingAlpha => _targetAlpha;
        public float FadingOutDuration => _OutDuration;
        public float FadingInDuration => _InDuration;
        public float WeightToFade => _weightToFade;
        public LayerMask LayersToFade => _layers;
        public ShaderRenderMode RenderingMode => _rendering;
        public UpdateMode UpdateMode => _rayUpdate;
        public DeltaTimeType DeltaTimeType => _deltaTime;
        public Transform RayTargetPrefab => _rayTarget;
        public Transform RayPrefab => _ray;
        public Color NormalRayColor => _normalColor;
        public Color HittedRayColor => _hittedColor;
        public MEvent OnRayHit => _onHit;

        [Header("Fading")]
        [SerializeField][Range(0, 1)]
        private float _targetAlpha;
        [SerializeField][Range(0, NumberOBF.MaxFadingDuration)][Space(2)]
        private float _OutDuration;
        [SerializeField][Range(0, NumberOBF.MaxFadingDuration)]
        private float _InDuration;
        [SerializeField][Range(0.1f, 2)][Space(2)]
        private float _weightToFade;
        //[SerializeField][Range(ConstsOBF.MinDistanceFactor, 1)][Space(3)]
        //private float _distanceFactor;
        [Header("Modes")]
        [SerializeField][Space(2)]
        private ShaderRenderMode _rendering;
        [SerializeField]
        private UpdateMode _rayUpdate;
        [SerializeField]
        private DeltaTimeType _deltaTime;
        [SerializeField][Space(2)]
        private LayerMask _layers;
        [Header("Raycasting")]
        [SerializeField]
        private Transform _rayTarget;
        [SerializeField]
        private Transform _ray;
        [SerializeField][Space(2)]
        private MEvent _onHit;
        [SerializeField][Space(4)]
        private Color _normalColor;
        [SerializeField]
        private Color _hittedColor;

    }
}