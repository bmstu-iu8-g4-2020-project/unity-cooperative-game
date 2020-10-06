using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
    public class ClimbingState : State
    {
        private Transform _climbingTarget;
        private float _timeLeft;
        private Vector3 _relativePosition;

        private float _radius;
        private float _angle;
        private float _finalAngle;
        private float _deltaAngle;
        private float _speed;


        public ClimbingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
            _timeLeft = Character.ClimbingDuration;
        }

        public override void OnEnter()
        {
            // todo reset everything because its reusable state
            base.OnEnter();
            Character.GetComponent<Collider>().enabled = false;

            _climbingTarget = Character.ClimbingTarget;
            if (!_climbingTarget) Debug.LogError("No target for climbing");

            _timeLeft = Character.ClimbingDuration;

            _relativePosition = _climbingTarget.InverseTransformPoint(Character.Transform.position);
            _relativePosition.z = 0;

            //mb provide functionality for starting WalkTo state here

            Character.LookAtXZ(_climbingTarget.position);

            _radius = _relativePosition.magnitude;
            _angle = Mathf.Acos(_relativePosition.x / _radius);
            _finalAngle = Mathf.Acos(-_relativePosition.x / _radius);
            _deltaAngle = Mathf.DeltaAngle(_angle, _finalAngle);
            _speed = 2 * _deltaAngle / Character.ClimbingDuration;
            _relativePosition.x *= -1;
        }

        public override void OnExit()
        {
            base.OnExit();
            Character.Transform.position = _climbingTarget.TransformPoint(_relativePosition);

            Character.GetComponent<Collider>().enabled = true;
        }

        public override void Tick()
        {
            base.Tick();

            _timeLeft -= Time.deltaTime;

            _angle += _speed * Time.deltaTime;
            _timeLeft -= Time.deltaTime;
            float x = Mathf.Cos(_angle) * _radius;
            float y = Mathf.Sin(_angle) * _radius;
            Character.Transform.position = _climbingTarget.TransformPoint(new Vector3(x, y, 0));
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            if (_timeLeft <= float.Epsilon) // TODO float.Epsilon
            {
                StateMachine.ChangeState(Character.WalkState);
            }
        }
    }
}
