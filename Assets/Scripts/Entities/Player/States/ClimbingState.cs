using UnityEngine;

namespace Entities.Player.States
{
    public class ClimbingState : State //TODO check falling after climb out of the second floor window
    {
        private float _angle;
        private readonly Transform _climbingTarget;
        private float _deltaAngle;
        private float _finalAngle;

        private float _radius;
        private Vector3 _relativePosition;
        private float _speed;
        private float _timeLeft;


        public ClimbingState(PlayerController character, StateMachine stateMachine, Transform climbingTarget) : base(
            character, stateMachine)
        {
            _timeLeft = TheData.Instance.PlayerData.ClimbingDuration;
            _climbingTarget = climbingTarget;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Character.GetComponent<Collider>().enabled = false;

            if (!_climbingTarget) Debug.LogError("No target for climbing");

            var climbingDuration = TheData.Instance.PlayerData.ClimbingDuration;
            _timeLeft = climbingDuration;

            _relativePosition = _climbingTarget.InverseTransformPoint(Character.transform.position);
            _relativePosition.z = 0;

            //mb provide functionality for starting WalkTo state here

            Character.PlayerMovement.LookAtXZ(_climbingTarget.position);

            _radius = _relativePosition.magnitude;
            _angle = Mathf.Acos(_relativePosition.x / _radius);
            _finalAngle = Mathf.Acos(-_relativePosition.x / _radius);
            _deltaAngle = Mathf.DeltaAngle(_angle, _finalAngle);
            _speed = 2 * _deltaAngle / climbingDuration;
            _relativePosition.x *= -1;
        }

        public override void OnExit()
        {
            base.OnExit();
            Character.transform.position = _climbingTarget.TransformPoint(_relativePosition);

            Character.GetComponent<Collider>().enabled = true;
        }

        public override void Tick()
        {
            base.Tick();

            _timeLeft -= Time.deltaTime;

            _angle += _speed * Time.deltaTime;
            _timeLeft -= Time.deltaTime;
            var x = Mathf.Cos(_angle) * _radius;
            var y = Mathf.Sin(_angle) * _radius;
            Character.transform.position = _climbingTarget.TransformPoint(new Vector3(x, y, 0));
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (_timeLeft <= float.Epsilon) StateMachine.ChangeState(Character.WalkState);
        }
    }
}
