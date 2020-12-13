using UnityEngine;

namespace Entities.Player.States
{
    public class WalkToState : ActionState
    {
        private readonly CharacterController _controller;
        private readonly Vector3 _destination;

        private readonly MoveToDelegate _move;
        private readonly Transform _transform;
        private readonly float _exitTime = 2.0f;
        private Vector3 _lastCheckPos;

        private float _lastCheckTime;
        private readonly float _minDistance = 0.8f;

        private float _walkSpeed;

        public WalkToState(PlayerController character, StateMachine stateMachine, Vector3 destination,
            bool checkCollision) :
            base(character, stateMachine)
        {
            _transform = Character.transform;
            _destination = destination;
            _controller = Character.Controller;
            if (checkCollision)
                _move = MoveToWithCollision;
            else
                _move = MoveTo;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Character.PlayerMovement.LookAtXZ(_destination);

            IsExit = false;
            _lastCheckPos = -_transform.position;

            _walkSpeed = TheData.Instance.PlayerData.WalkSpeed;
        }

        public override void Tick()
        {
            base.Tick();
            _move();
            if (Vector3.Distance(_transform.position, _destination) < float.Epsilon) IsExit = true;
        }

        private void MoveTo()
        {
            _transform.position =
                Vector3.MoveTowards(_transform.position, _destination, _walkSpeed * Time.deltaTime);
        }

        private void MoveToWithCollision()
        {
            var offset = _destination - _transform.position;
            if (offset.magnitude > 0.5f)
            {
                _controller.Move(offset.normalized * (_walkSpeed * Time.deltaTime));
                if (Time.time - _lastCheckTime > _exitTime)
                {
                    if ((_transform.position - _lastCheckPos).magnitude < _minDistance) IsExit = true;

                    _lastCheckPos = _transform.position;
                    _lastCheckTime = Time.time;
                }
            }
            else
            {
                _transform.position = _destination;
                IsExit = true;
            }
        }

        private delegate void MoveToDelegate();
    }
}
