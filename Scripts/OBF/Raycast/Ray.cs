using MalbersAnimations.Events;
using DOUKH.OBF.Raycast.Target;
using DOUKH.Settings.Camera;
using DOUKH.Common.Helpers;
using DOUKH.OBF.Numbers;
using UnityEngine;
using System.Linq;
using System;

namespace DOUKH.OBF.Raycast.Ray
{
    public class RayOBF : MonoBehaviour
    {
        public float Weight => _target.IsCollided ? 0 : _weight;
        public float Length => Vector3.Distance(_begin, _end);
        public RaycastHit[] Hits => _reservedHits.Where(h => h.distance > 0).ToArray();
        //------------------------------------------------------
        private Vector3 Begin => _begin;
        private Vector3 Direction => (_end - _begin).normalized;
        //------------------------------------------------------
        private MEvent _onHit;
        private readonly RaycastHit[] _reservedHits = new RaycastHit[NumberOBF.MaxRayHitsCount];
        private Color _normalColor, _hittedColor;
        private LayerMask _fadingLayers;
        private RayTargetOBF _target;
        private Transform _transform;
        private Vector3 _begin, _end;
        private bool _isHitted = false;
        private float _weight = 1;

        public void SetPosition(Vector3 begin, RayTargetOBF target)
        {
            if (!gameObject.activeSelf) Enable();
            _begin = begin;
            _end = target.transform.position;
            _target = target;
        }

        public void Disable() => gameObject.SetActive(false);

        private void Awake() => Initialize();

        private void FixedUpdate()
        {
            Reset();
            Cast();
#if UNITY_EDITOR
            Draw();
#endif
        }  

        private void Initialize()
        {
            _transform = transform;
            _fadingLayers = Helper.GetDefault(CameraSettings.OBF.LayersToFade.value, NumberOBF.LayersToFade);
            _normalColor = CameraSettings.OBF.NormalRayColor;
            _hittedColor = CameraSettings.OBF.HittedRayColor;
            _onHit = CameraSettings.OBF.OnRayHit;
        }

        private void Cast()
        {
            Physics.RaycastNonAlloc(Begin, Direction, _reservedHits, Length, _fadingLayers);
            if (!_isHitted && Hits.Length > 0) _isHitted = true;       
            _onHit.Invoke(_transform);
        }

        private void Draw() => Debug.DrawLine(_begin, _end, _isHitted ? _hittedColor : _normalColor); //Debug.DrawRay(Begin, Direction * Length, _isHitted ? _hittedColor : _normalColor);

        private void Reset()
        {
            Array.Clear(_reservedHits, 0, NumberOBF.MaxRayHitsCount);
            _isHitted = false;
        }

        private void Enable() => gameObject.SetActive(true);

    }
}