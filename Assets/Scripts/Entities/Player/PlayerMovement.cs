using Mirror;
using UnityEngine;

namespace Entities.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        #region Veriables

        #region Network Variables

        private Vector3 _newPosition = Vector3.zero;
        private Quaternion _newRotation;
        private Transform _transform;

        private const float SendDelay = 1 / 30f;

        private readonly bool _isInterpolate = true;

        #endregion

        private Camera _playerCamera;

        private Vector3 _forward;
        private Vector3 _right;

        #endregion

        #region MonoBeheviour callbacks

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            if (isLocalPlayer || isServer) InvokeRepeating(nameof(NetworkUpdate), SendDelay, SendDelay);

            #region Variables Init

            _playerCamera = GameManager.Instance.GetCamera();

            _forward = _playerCamera.transform.forward;
            _forward.y = 0;
            _forward = Vector3.Normalize(_forward);
            _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

            #endregion
        }

        private void Update()
        {
            if (!isLocalPlayer && isClient)
            {
                transform.position =
                    _isInterpolate ? Vector3.Lerp(_transform.position, _newPosition, 0.5f) : _newPosition;

                _transform.rotation = _newRotation;
            }
        }

        #endregion

        #region Networking

        private void NetworkUpdate()
        {
            if (isLocalPlayer) CmdUpdateTransform(_transform.position, _transform.rotation);

            if (isServer) RpcSetTransform(_newPosition, _newRotation);
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

        #region Methods

        [Client]
        public void Move(float speed, float verticalOffset, float horizontalOffset)
        {
            var velocity = _forward * horizontalOffset + _right * verticalOffset;
            velocity = velocity.normalized * speed;

            if (velocity.magnitude != 0) _transform.forward = velocity.normalized;

            Player.LocalPlayer.Controller.SimpleMove(velocity);
        }

        [Client]
        public void FaceToMouse()
        {
            Vector2 screenPos = _playerCamera.WorldToScreenPoint(_transform.position);
            Vector2 mousePos = Input.mousePosition;
            var direction = new Vector3(mousePos.x - screenPos.x, 0, mousePos.y - screenPos.y);
            direction = _playerCamera.transform.TransformDirection(direction);
            direction.y = 0;
            _transform.forward = direction.normalized;
        }

        [Client]
        public void LookAtXZ(Vector3 target)
        {
            var targetPosition = new Vector3(target.x,
                _transform.position.y,
                target.z);
            _transform.LookAt(targetPosition);
        }

        #endregion
    }
}
