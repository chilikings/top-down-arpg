using UnityEngine;

namespace DOUKH.OBF.Raycast.Target
{
    public class RayTargetOBF : MonoBehaviour
    {
        public bool IsCollided => _collisionCount > 0;

        private Transform _transform;
        private int _collisionCount = 0;

        //============================================

        public void SetVisibility(bool isVisible = false) => GetComponent<MeshRenderer>().enabled = isVisible;

        public void MultiplyScale(float k) => _transform.localScale *= k;

        public void AttachToObject(Transform obj) => _transform.SetParent(obj, true);

        private void Awake() => _transform = transform;

        private void OnTriggerEnter(Collider collider) => _collisionCount++;

        private void OnTriggerExit(Collider collider) => _collisionCount--;

    }
}