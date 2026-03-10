using DOUKH.Settings.Camera;
using DOUKH.Common.Helpers;
using System.Collections;
using MalbersAnimations;
using Unity.Cinemachine;
using DOUKH.CM3.Target;
using UnityEngine;

namespace DOUKH.CM3.Controller
{
    [AddComponentMenu("DOUKH/CM3/Controller")]
    public class ControllerCM3 : MonoBehaviour
    {
        public Transform Target => _camera.Target.TrackingTarget;

        private float Distance { set => _composer.CameraDistance = value; }
        private float DistFactor { get => _distFactor; set => _distFactor = Mathf.Clamp(value, 0, 1); }
        private float TiltAngle { get => _panTilt.TiltAxis.Value; set => _panTilt.TiltAxis.Value = ClampTiltAngle(value); }
        private float TargetTiltAngle { get => _targetTiltAngle; set => _targetTiltAngle = ClampTiltAngle(value); }
        private float TargetDistFactor { get => _targetDistFactor; set => _targetDistFactor = Mathf.Clamp(value, 0, 1); }
        private float PanAngle { get => _panTilt.PanAxis.Value; set => _panTilt.PanAxis.Value = value; }
        private Vector3 Damping { set => _composer.Damping = value; }

        private MInput _mInput;
        private TargetCM3 _cameraTarget;
        private CinemachineCamera _camera;
        private CinemachinePanTilt _panTilt;
        private CinemachinePositionComposer _composer;
        private CinemachineInputAxisController _inputAxis;
        private int _panSensitivity, _zoomLerp, _tiltLerp, _maxTiltAngle, _minTiltAngle;
        private float _zoomSensitivity, _distFactor, _tiltDistFactor, _targetTiltAngle, _targetDistFactor;
        [SerializeField]
        private bool _logging;
        //[SerializeField] //Debug
        private bool _isZooming, _isTilting;
        //----------------------------------------------------------------
        //----------------------------------------------------------------
        public void Pan(bool isKeyPressed)
        {
            SetPanInputGain(isKeyPressed);
            if (_logging) Helper.Log($"Panning is {isKeyPressed}");
        }

        public void Zoom(float scrollDelta)
        {
            if (enabled)
            {
                SetTargetPosition(scrollDelta);
                if (!_isZooming) StartCoroutine(Zooming()); // _zooming ??= StartCoroutine(Zooming());
                if (!_isTilting) StartCoroutine(Tilting()); // _tilting ??= StartCoroutine(Tilting());
                if (_logging) Helper.Log($"Zooming is {_isZooming}");
            }
        }

        public void Switch(bool enable)
        {
            gameObject.SetActive(enable);
            enabled = enable;
            _camera.enabled = enable;
            _panTilt.enabled = enable;
            _inputAxis.enabled = enable;
            SwitchInput(false);
        }

        public void SwitchInput(bool enable) => _mInput.enabled = enable;

        private void OnDisable() => Reset();

        public void SetTarget(Transform target) 
        {
            if (target)
            { 
                _camera.Target.TrackingTarget = target;
                _cameraTarget = target.GetComponent<TargetCM3>();
            }
        }

        public void SetPriority(int priority) => _camera.Priority.Value = priority;

        public void Teleport(ControllerCM3 camera)
        {
            if (camera)
            {               
                DistFactor = TargetDistFactor = camera.DistFactor;
                Distance = _cameraTarget.GetDistance(DistFactor);
                TiltAngle = CalcTiltAngle(DistFactor);
                PanAngle = camera.PanAngle;
            }
        }

        private void Awake()
        {
            InitializeComponents();
            InitializeSettings();
        }

        private void InitializeComponents()
        {
            _camera = GetComponent<CinemachineCamera>();
            _panTilt = GetComponent<CinemachinePanTilt>();
            _composer = GetComponent<CinemachinePositionComposer>();
            _inputAxis = GetComponent<CinemachineInputAxisController>();
            _mInput = GetComponent<MInput>();
        }

        private void InitializeSettings()
        {
            _panSensitivity = CameraSettings.CM3.PanSensitivity;
            _zoomSensitivity = CameraSettings.CM3.ZoomSensitivity;
            _zoomLerp = CameraSettings.CM3.ZoomLerp;
            _tiltLerp = CameraSettings.CM3.TiltLerp;
            _tiltDistFactor = CameraSettings.CM3.TiltDistFactor;
            _maxTiltAngle = CameraSettings.CM3.MaxTiltAngle;
            _minTiltAngle = CameraSettings.CM3.MinTiltAngle;

            Damping = CameraSettings.CM3.Damping;
            DistFactor = CameraSettings.CM3.IniDistFactor;
            TargetDistFactor = CameraSettings.CM3.IniDistFactor;
            TiltAngle = CameraSettings.CM3.MaxTiltAngle;
            TargetTiltAngle = CameraSettings.CM3.MaxTiltAngle;
        }

        private IEnumerator Zooming()
        {
            _isZooming = true;
            while (Helper.AreDifferent(DistFactor, TargetDistFactor))
            {
                DistFactor = CalcSmoothedDistFactor();
                Distance = _cameraTarget.GetDistance(DistFactor);
                yield return null;
            }
            _isZooming = false; //_zooming = null;
        }

        private IEnumerator Tilting()
        {
            _isTilting = true;
            while (Helper.AreDifferent(TiltAngle, TargetTiltAngle))
            {
                TiltAngle = CalcSmoothedAngle();
                yield return null;
            }
            _isTilting = false; //_tilting = null;
        }

        private void SetTargetPosition(float scrollDelta)
        {
            TargetDistFactor -= scrollDelta * _zoomSensitivity;
            TargetTiltAngle = CalcTiltAngle(TargetDistFactor);
        }

        private void SetPanInputGain(bool isKeyPressed) => _inputAxis.Controllers[0].Input.LegacyGain = isKeyPressed ? _panSensitivity : 0;

        private void Reset()
        {
            _isZooming = false;
            _isTilting = false;
            SetPanInputGain(false);
            //_inputAxis.Controllers[0].InputValue = 0;
            if (_cameraTarget)
            {
                TargetDistFactor = DistFactor;
                TargetTiltAngle = TiltAngle;
            }
            if (_logging) Helper.Log("OnDisable() -> Reset()");
        }

        private float ClampTiltAngle(float angle) => Mathf.Clamp(angle, _minTiltAngle, _maxTiltAngle);

        private float CalcTiltAngle(float distFactor) => Mathf.Lerp(_minTiltAngle, _maxTiltAngle, distFactor / _tiltDistFactor);

        private float CalcSmoothedAngle() => Mathf.LerpAngle(TiltAngle, TargetTiltAngle, _tiltLerp * Time.deltaTime);

        private float CalcSmoothedDistFactor() => Mathf.Lerp(DistFactor, TargetDistFactor, _zoomLerp * Time.deltaTime);
    }
}