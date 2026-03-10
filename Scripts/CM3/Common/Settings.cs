using DOUKH.Common.Enums;
using UnityEngine;

namespace DOUKH.CM3.Settings
{
    [CreateAssetMenu(fileName = "CM3", menuName = "DOUKH/CM3/Settings")]
    public class SettingsCM3 : ScriptableObject
    {
        public Vector3 Damping => _damping;
        public int PanSensitivity => _panSensitivity;
        public float ZoomSensitivity => _zoomSensitivity;
        public int ZoomLerp => _zoomLerp;
        public int TiltLerp => _tiltLerp;
        public float IniDistFactor => _iniDistFactor;
        public float TiltDistFactor => _tiltDistFactor;
        public int MaxTiltAngle => _maxTiltAngle;
        public int MinTiltAngle => _minTiltAngle;
        public int BlendDistance => _blendDistance;
        public float MinBlendTime => _minBlendTime;
        public float MaxBlendTime => _maxBlendTime;
        //--------------------------------------------------------------
        [Header("Blending")]
        [SerializeField][Range(0, 50)]
        private int _blendDistance;
        [SerializeField][Range(0f, 2f)][Space(2)]
        private float _minBlendTime;
        [SerializeField][Range(0f, 2f)]
        private float _maxBlendTime;
        //[SerializeField]
        //private MEvent _onCameraSwitch, _onCameraBlend;
        //[SerializeField]
        //private MEvent _onBlend;
        [Header("Controls")]
        [SerializeField][Range(0, 2000)][Space(2)]
        private int _panSensitivity;
        [SerializeField][Range(0, 1.0f)]
        private float _zoomSensitivity;
        [Header("Smoothing")]
        [SerializeField]
        private Vector3 _damping;
        [SerializeField][Range(1, 20)][Space(2)]
        private int _zoomLerp;
        [SerializeField][Range(1, 20)]
        private int _tiltLerp;
        [Header("Limits")]
        [SerializeField][Range(0, 1)]
        private float _iniDistFactor;
        [SerializeField][Range(0, 1)]
        private float _tiltDistFactor;
        [SerializeField][Range(40, 60)][Space(2)]
        private int _maxTiltAngle;
        [SerializeField][Range(10, 40)]
        private int _minTiltAngle;
        [SerializeField][Space(4)]
        private Vector2 TinyLimits;
        [SerializeField]
        private Vector2 SmallLimits, MediumLimits, LargeLimits, GiantLimits;

        public Vector2 GetDistanceLimits(Size targetSize) => targetSize switch
        {
            Size.Tiny => TinyLimits,
            Size.Small => SmallLimits,
            Size.Medium => MediumLimits,
            Size.Large => LargeLimits,
            Size.Giant => GiantLimits
        };
    }
}