using DOUKH.Settings.Camera;
using DOUKH.Common.Enums;
using UnityEngine;

namespace DOUKH.CM3.Target
{
    [AddComponentMenu("DOUKH/CM3/Target")]
    public class TargetCM3 : MonoBehaviour
    {
        public Transform Transform => _transform;

        [SerializeField]
        private Size _size;

        private Transform _transform;
        private float _minDistance, _maxDistance;
        //---------------------------------------
        //---------------------------------------
        public float GetDistance(float factor) => Mathf.Lerp(_minDistance, _maxDistance, factor);

        private void Awake()
        {
            _transform = transform;
            SetDistanceLimits(CameraSettings.CM3.GetDistanceLimits(_size));
        }

        private void SetDistanceLimits(Vector2 limits)
        {
            _minDistance = limits.x;
            _maxDistance = limits.y;
        }
    }
}