using System.Collections.Generic;
using DOUKH.OBF.Raycast.Target;
using DOUKH.Settings.Camera;
using DOUKH.OBF.Numbers;
using DOUKH.OBF.Strings;
using UnityEngine;
using System;

namespace DOUKH.OBF.Target
{
    [AddComponentMenu("DOUKH/OBF/Target")]
    public class TargetOBF : MonoBehaviour
    {
        public List<RayTargetOBF> RayTargets => _rayTargets;
        public float DistanceFactor => _distanceFactor;

        [SerializeField][Range(NumberOBF.MinDistanceFactor, 1)][Space(4)]
        private float _distanceFactor;
        [SerializeField][Range(NumberOBF.MinSlicesCount, NumberOBF.MaxSlicesCount)][Space(2)]
        private int _slicesCount;
        [SerializeField][Space(3)]
        private bool _extended;
        [SerializeField]
        private bool _displayTargets;

        private List<RayTargetOBF> _rayTargets = new();
        private Transform _targetPrefab, _transform;
        private Collider _collider;
        private float _scaleMultiplier;
        //======================================
        public bool IsCorrect() => enabled && _collider && _collider.isTrigger;

        private void Awake()
        {
            InitializeFields();
            InitializeRayTargets();
        }

        private void InitializeFields()
        {
            _targetPrefab = CameraSettings.OBF.RayTargetPrefab;
            _collider = GetComponent<Collider>();
            _transform = transform;
            _scaleMultiplier = _transform.parent.lossyScale.x;
        }

        private void InitializeRayTargets()
        {
            //var rayTargetsParent = transform.Find(NamesOBF.RayTargetsParent);
            var parent = new GameObject(NamesOBF.RayTargetsParent).transform;
            parent.SetParent(_transform, false);
            SetRayTargets(CalcCapsulePoints(), parent);
            parent.localRotation = _transform.rotation;
            parent.localScale = _transform.parent.lossyScale;
        }

        private void SetRayTargets(List<Vector3> points, Transform parent)
        {
            if (_targetPrefab)
                foreach (var point in points)
                {
                    var rayTarget = Instantiate(_targetPrefab, point, Quaternion.identity).GetComponent<RayTargetOBF>();
                    rayTarget.AttachToObject(parent);
                    rayTarget.MultiplyScale(1 / _scaleMultiplier);
                    rayTarget.SetVisibility(_displayTargets);
                    _rayTargets.Add(rayTarget);
                }
        }

        private List<Vector3> CalcCapsulePoints()
        {
            var points = new List<Vector3>();
            var capsule = (CapsuleCollider)_collider;
            int dir = capsule.direction, slicesCount = _slicesCount;
            var center = capsule.bounds.center;
            float radius = capsule.radius, height = capsule.height, step = slicesCount > 1 ? (height - 2 * radius) / (slicesCount - 1) : (height - 2 * radius) / 2,
                y0 = center.y - height / 2, z0 = center.z - height / 2, x, y = 0, z = 0, a = radius / Mathf.Sqrt(2),
                b = radius - a, d = Mathf.Sqrt((radius * radius - a * a) / 2), f = radius - d;

            for (int k = 0; k < 2; k++)
            {
                if (dir == 1 && k == 1) break;
                if (dir == 1) points.Add(new Vector3(center.x, y0 + height, center.z));
                else if (dir == 2) points.Add(new Vector3(center.x, center.y, z0 + height * k));

                for (int p = 0; p < 4 && _extended; p++)
                {
                    var sign = Mathf.Pow(-1, p);
                    x = center.x + a * (p < 2 ? 1 : 0) * sign;
                    if (dir == 1)
                    {
                        float xe = center.x + d * sign, ze = center.z + d * (p < 2 ? -1 : 1);
                        y = y0 + height - b;
                        z = center.z + a * (p < 2 ? 0 : 1) * sign;
                        points.Add(new Vector3(xe, y, ze));
                    }
                    else if (dir == 2)
                    {
                        if (p == 3) break;
                        y = center.y + a * (p < 2 ? 0 : 1) * sign;
                        z = z0 + b + (height - 2 * b) * k;
                        if (p < 2)
                        {
                            float xe = center.x + d * sign, ye = center.y + a, ze = z0 + f + (height - 2 * f) * k;
                            points.Add(new Vector3(xe, ye, ze));
                        }
                    }
                    points.Add(new Vector3(x, y, z));
                }
            }

            for (int k = 0; k < slicesCount; k++)
            {
                if (dir == 1) y = y0 + radius + (slicesCount > 1 ? step * k : step);
                else if (dir == 2) z = z0 + radius + (slicesCount > 1 ? step * k : step);

                for (int p = 0; p < 4; p++)
                {
                    var sign = Mathf.Pow(-1, p);
                    x = center.x + (p < 2 ? radius * sign : 0);
                    if (dir == 1)
                    {
                        z = center.z + (p < 2 ? 0 : radius * sign);
                        if (_extended)
                        {
                            float xe = center.x + a * sign, ze = center.z + a * (p < 2 ? -1 : 1);
                            points.Add(new Vector3(xe, y, ze));
                        }
                    }
                    else if (dir == 2)
                    {
                        if (p == 3) break;
                        y = center.y + (p < 2 ? 0 : radius * sign);
                        if (_extended && p < 2)
                        {
                            float xe = center.x + a * sign, ye = center.y + a;
                            points.Add(new Vector3(xe, ye, z));
                        }
                    }
                    points.Add(new Vector3(x, y, z));
                }
            }
            return points;
        }

    }
}