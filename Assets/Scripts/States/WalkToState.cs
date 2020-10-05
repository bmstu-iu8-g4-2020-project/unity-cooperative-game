﻿using System;
using UnityEngine;

namespace States
{
    public class WalkToState : ActionState
    {
        private readonly Transform _transform;
        private readonly Vector3 _destination;
        private readonly CharacterController _controller;

        private delegate void MoveToDelegate();

        private readonly MoveToDelegate _move;

        float _lastCheckTime = 0f;
        Vector3 _lastCheckPos;
        float _exitTime = 2.0f;
        float _minDistance = 0.8f;

        public WalkToState(Character character, StateMachine stateMachine, Vector3 destination, bool checkCollision) :
            base(character, stateMachine)
        {
            _transform = Character.Transform;
            _destination = destination;
            _controller = Character.Controller;
            if (checkCollision)
            {
                _move = MoveToWithCollision;
            }
            else
            {
                _move = MoveTo;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Character.LookAtXZ(_destination);

            IsExit = false;
            _lastCheckPos = -_transform.position;
        }

        public override void Update()
        {
            base.Update();
            _move();
            if (Vector3.Distance(_transform.position, _destination) < float.Epsilon)
            {
                IsExit = true;
            }
        }

        void MoveTo()
        {
            _transform.position =
                Vector3.MoveTowards(_transform.position, _destination, Character.WalkSpeed * Time.deltaTime);
        }

        void MoveToWithCollision()
        {
            var offset = _destination - _transform.position;
            if (offset.magnitude > .1f)
            {
                _controller.Move(offset.normalized * (Character.WalkSpeed * Time.deltaTime));
                if (Time.time - _lastCheckTime > _exitTime)
                {
                    if ((_transform.position - _lastCheckPos).magnitude < _minDistance)
                    {
                        IsExit = true;
                    }

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
    }
}
