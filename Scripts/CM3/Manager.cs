using System.Collections.Generic;
using MalbersAnimations.Events;
using DOUKH.Common.Helpers;
using DOUKH.Settings.Camera;
using DOUKH.CM3.Controller;
using System.Collections;
using Unity.Cinemachine;
using DOUKH.CM3.Target;
using DOUKH.OBF.Fader;
using UnityEngine;


namespace DOUKH.CM3.Manager
{
    [AddComponentMenu("DOUKH/CM3/Manager")]
    public class ManagerCM3 : MonoBehaviour
    {
        [SerializeField]
        private List<ControllerCM3> _cameras;
        [SerializeField][Space]
        private bool _logging;
        //----------------------------------------
        private FaderOBF _fader;
        private ControllerCM3 CurrentCamera => _currentCameraIndex >= 0 && _cameras.Count > 0 ? _cameras[_currentCameraIndex]: null;
        private ControllerCM3 NextCamera => _nextCameraIndex >= 0 && _cameras.Count > 0 ? _cameras[_nextCameraIndex] : null;
        private Transform CurrentTarget => CurrentCamera?.Target;
        private Transform NextTarget => NextCamera?.Target;
        //----------------------------------------
        //private Coroutine _blending;
        private CinemachineBrain _mainCamera;
        private int _currentCameraIndex = 0, _nextCameraIndex = 0, _blendDistance;
        private float _minBlendTime, _maxBlendTime;
        private bool _isBlending = false;
        //----------------------------------------
        //----------------------------------------
        public void BlendBetweenCameras(Transform character)
        {
            if (character && !_mainCamera.IsBlending)
            {
                var target = character.GetComponentInChildren<TargetCM3>()?.Transform;
                if (target)
                {
                    SetBlendTime(target.position);
                    SwapCameras();
                    SetCamerasTarget(target);
                    CurrentCamera.Teleport(NextCamera); // _onCameraSwitch.Invoke(CurrentCamera.transform);
                    if (_logging) Debug.Log(CurrentCamera.transform.name + " -> " + NextCamera.transform.name);
                    if (!_isBlending) StartCoroutine(Blending()); // _blending ??= StartCoroutine(Blending());
                }
            }
            if (_logging) LogCameras();
        }

        private void Awake()
        {
            _mainCamera = GetComponent<CinemachineBrain>();
            _fader = GetComponent<FaderOBF>();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            _minBlendTime = CameraSettings.CM3.MinBlendTime;
            _maxBlendTime = CameraSettings.CM3.MaxBlendTime;
            _blendDistance = CameraSettings.CM3.BlendDistance;
        }

        private IEnumerator Blending()
        {
            _fader.Switch(false); //_onCameraBlend.Invoke(false);
            //_onBlend.Invoke(false);
            SwapCamerasPrioroty();
            while (_mainCamera.IsBlending || !_isBlending)
            {
                _isBlending = true;
                yield return null;
            }
            NextCamera.SetTarget(CurrentTarget);
            _isBlending = false; //_blending = null;
            //_onBlend.Invoke(true);
            CurrentCamera.SwitchInput(true);
            _fader.Switch(true, CurrentTarget.parent); //_onCameraBlend.Invoke(true);
        }

        private void SwapCameras()
        {
            _currentCameraIndex = _nextCameraIndex;
            _nextCameraIndex = (_currentCameraIndex + 1) % 2;
            CurrentCamera.Switch(true);
            NextCamera.Switch(false);
        }

        private void SetCamerasTarget(Transform target)
        {
            CurrentCamera.SetTarget(target);
            if (!NextTarget || Helper.IsGuise(NextTarget)) NextCamera.SetTarget(target);
        }

        private void SwapCamerasPrioroty()
        {
            CurrentCamera.SetPriority(1);
            NextCamera.SetPriority(0);
        }

        private void SetBlendTime(Vector3 targetPosition) => _mainCamera.DefaultBlend.Time = CurrentTarget ? Mathf.Lerp(_minBlendTime, _maxBlendTime, Vector3.Distance(CurrentTarget.position, targetPosition) / _blendDistance) : 0;

        private void LogCameras()
        {
            Helper.Log(CurrentCamera.transform.position, "CurrentCamera");
            Helper.Log(NextCamera.transform.position, "NextCamera", true);
        }
    }
}