using System.Collections.Generic;
using DOUKH.OBF.Raycast.Target;
using DOUKH.OBF.Raycast.Ray;
using DOUKH.Settings.Camera;
using DOUKH.Common.Helpers;
using DOUKH.OBF.Obstacle;
using DOUKH.Common.Enums;
using DOUKH.OBF.Numbers;
using DOUKH.OBF.Strings;
using DOUKH.OBF.Target;
using DOUKH.OBF.Enums;
using UnityEngine;

namespace DOUKH.OBF.Fader
{
    [AddComponentMenu("DOUKH/OBF/Fader")]
    public class FaderOBF : MonoBehaviour
    {
        [SerializeField]
        private bool _logging;

        private ShaderRenderMode _renderMode;
        private UpdateMode _updateMode;
        private Transform _rayPrefab;
        private float _fadingAlpha, _fadingOutDuration, _fadingInDuration, _weightToFade;
        //-------------------------------------------
        private readonly List<RayOBF> _rays = new();
        private List<RayTargetOBF> _rayTargets = new();
        private Transform _raysParent;
        private TargetOBF _target;
        private Camera _camera;
        //private float _distanceFactor;
        //-------------------------------------------
        //-------------------------------------------
        public void HitObstacle(Transform rayTransform)
        {
            var ray = rayTransform.GetComponent<RayOBF>();
            foreach (var hit in ray.Hits)
            {
                if (_logging) Helper.Log(hit.distance / ray.Length, string.Concat(ray.transform.name, ": K"));
                if (hit.distance / ray.Length < _target.DistanceFactor)
                    hit.transform.GetComponent<ObstacleOBF>()?.GetHit(_renderMode, _fadingAlpha, _fadingOutDuration, _fadingInDuration, _weightToFade, ray.Weight);
            }
        }

        public void Switch(bool enable, Transform character = null)
        {
            enabled = enable;
            if (enable) SetTarget(character);
        }

        private void Awake()
        {
            InitializeFields();
            InitializeSettings();
        }

        private void FixedUpdate() => UpdateRays(UpdateMode.FixedUpdate);

        private void Update() => UpdateRays(UpdateMode.Update);

        private void LateUpdate() => UpdateRays(UpdateMode.LateUpdate);

        private void InitializeFields()
        {
            _camera ??= Camera.main;
            _raysParent ??= new GameObject(NamesOBF.RaysParent).transform;
            _raysParent.SetParent(_camera.transform.parent, true);
        }

        private void InitializeSettings()
        {
            _fadingAlpha = CameraSettings.OBF.FadingAlpha;
            _fadingOutDuration = CameraSettings.OBF.FadingOutDuration;
            _fadingInDuration = CameraSettings.OBF.FadingInDuration;
            _rayPrefab = CameraSettings.OBF.RayPrefab;
            _renderMode = CameraSettings.OBF.RenderingMode;
            _updateMode = CameraSettings.OBF.UpdateMode;
            _weightToFade = CameraSettings.OBF.WeightToFade;
        }

        private void SetTarget(Transform character)
        {
            var target = character?.GetComponentInChildren<TargetOBF>();
            if (target && target.IsCorrect())
            {
                _target = target;
                _rayTargets = target.RayTargets;
            }
        }

        private void UpdateRays(UpdateMode updateMode)
        {
            if (updateMode == _updateMode)
            {
                int targetsCount = _rayTargets.Count;
                for (int r = 0; r < targetsCount; r++)
                {
                    Vector3 rayEndScreenPos = _camera.WorldToScreenPoint(_rayTargets[r].transform.position),
                        rayBeginScreenPos = _camera.ScreenToWorldPoint(new Vector3(rayEndScreenPos.x, rayEndScreenPos.y, _camera.nearClipPlane));
                    
                    if (_rays.Count <= r)
                    {
                        var rayObject = Instantiate(_rayPrefab);
                        rayObject.SetParent(_raysParent, true);
                        _rays.Add(rayObject.GetComponent<RayOBF>());
                    }
                    _rays[r].SetPosition(rayBeginScreenPos, _rayTargets[r]);
                }
                for (int r = targetsCount; r < _rays.Count; r++) _rays[r].Disable();
            }
        }
    }
}