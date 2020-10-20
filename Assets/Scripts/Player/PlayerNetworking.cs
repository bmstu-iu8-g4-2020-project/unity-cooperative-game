using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerNetworking : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] scriptsOnlyForLocalPlayer;

        [SerializeField]
        private GameObject[] childGameObjectsForLocalPlayerOnly;

        private Vector3 _newPosition = Vector3.zero;
        private Quaternion _newRotation;
        private Transform _transform;

        private const float SendDelay = 1 / 30f;

        private bool _isInterpolate = true;

        
        #region Initialize

        private void Awake()
        {
            _transform = transform;
        }
        
        private void Start()
        {
            if (isLocalPlayer || isServer)
            {
                InvokeRepeating(nameof(NetworkUpdate), SendDelay, SendDelay);
            }

            foreach (var script in scriptsOnlyForLocalPlayer)
            {
                if (script != null)
                {
                    script.enabled = isLocalPlayer;
                }
            }

            foreach (var gObject in childGameObjectsForLocalPlayerOnly)
            {
                if (gObject != null)
                {
                    gObject.SetActive(isLocalPlayer);
                }
            }
        }

        public override void OnStartLocalPlayer()
        {
            GameManager.Instance.SetLocalPlayer(gameObject);
            gameObject.name = "LocalPlayer";
            
            GameManager.Instance.GetCamera().gameObject.GetComponentInParent<CameraManager>().SetTarget(transform);

            GameManager.Instance.AddPlayer(gameObject);
        }

        #endregion

        #region Networking

        private void Update()
        {
            if (!isLocalPlayer && isClient)
            {
                transform.position =
                    _isInterpolate ? Vector3.Lerp(_transform.position, _newPosition, 0.5f) : _newPosition;

                _transform.rotation = _newRotation;
            }
        }

        private void NetworkUpdate()
        {
            if (isLocalPlayer)
            {
                CmdUpdateTransform(_transform.position, _transform.rotation);
            }

            if (isServer)
            {
                RpcSetTransform(_newPosition, _newRotation);
            }
        }


        [ClientRpc]
        private void RpcSetTransform(Vector3 newPosition, Quaternion newRotation)
        {
            if (isLocalPlayer) return;
            _newPosition = newPosition;
            _newRotation = newRotation;
        }

        [Command]
        private void CmdUpdateTransform(Vector3 newPosition, Quaternion newRotation)
        {
            _newPosition = newPosition;
            _newRotation = newRotation;
        }

        #endregion
    }
}
