using DOUKH.Common.Helpers;
using UnityEngine;

namespace DOUKH.Interaction.Collisions
{
    public class CollisionDamper : MonoBehaviour
    {
        [Space(4)][SerializeField] private LayerMask _collidableLayers;
        [Space(4)][SerializeField] private bool _logging;
        
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (IsCollidable(collider))
            {
                FreezePosition();
                if (_logging) Debug.Log($"{collider.gameObject.name}: {collider.name} | {collider.gameObject.layer}");
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            if (IsCollidable(collider)) FreezePosition();
        }

        private void OnTriggerExit(Collider collider) 
        {
            if (IsCollidable(collider)) UnfreezePosition();
        }

        private bool IsCollidable(Collider collider)
        {
            var collidedObject = collider.gameObject;
            var layer = collidedObject.layer;
            var layerName = LayerMask.LayerToName(layer);
            return enabled && Helper.IsInLayerMask(collidedObject, _collidableLayers) &&
                   collidedObject.name.Contains(layerName) && collider is CapsuleCollider && collider.isTrigger;
        }

        private void FreezePosition() => _rb.constraints = RigidbodyConstraints.FreezeAll;

        private void UnfreezePosition() => _rb.constraints = RigidbodyConstraints.FreezeRotation;

    }
}