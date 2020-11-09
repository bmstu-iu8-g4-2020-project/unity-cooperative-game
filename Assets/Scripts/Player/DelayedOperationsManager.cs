using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Player
{
    public class DelayedOperation
    {
        public delegate void OperationDelegate();

        public readonly OperationDelegate OnOperationComplete;
        public readonly OperationDelegate OnOperationCancelled;

        public delegate void TickDelegate(float delay);

        public readonly TickDelegate OnTimerTick; //For bar animation

        public readonly float Delay; //Its depend on weight of the items being moved

        public DelayedOperation(float delay, OperationDelegate onOperationComplete,
            TickDelegate onTimerTick = null,
            OperationDelegate onOperationCancelled = null)
        {
            this.Delay = delay;
            OnOperationComplete = onOperationComplete;
            OnTimerTick = onTimerTick ?? (value => { UIController.Instance.OperationBarUI.fillAmount = value;});
            OnOperationCancelled = onOperationCancelled ?? (() => { UIController.Instance.OperationBarUI.fillAmount = 0; });
        }
    }

    // Singleton
    public class DelayedOperationsManager : MonoBehaviour
    {
        private readonly Queue<DelayedOperation> _operations = new Queue<DelayedOperation>();

        private DelayedOperation _currentOperation;

        private float _timer = 0f;

        #region singleton

        public static DelayedOperationsManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning($"More than one instance of a script in a scene. {gameObject.name}");
            }
        }

        #endregion

        public void QueueOperation(DelayedOperation newOperation)
        {
            _operations.Enqueue(newOperation);
            if (_operations.Count == 1)
            {
                StartTimer();
            }
        }

        public void DequeueAllOperations()
        {
            foreach (DelayedOperation operation in _operations)
            {
                operation?.OnOperationCancelled();
            }

            _operations.Clear();
            if (_currentOperation != null)
            {
                _timer = 0f;
                _currentOperation = null;
            }
        }

        private void StartTimer()
        {
            _currentOperation = _operations.Peek();
            _timer = _currentOperation.Delay;
        }

        private void Update()
        {
            if (_currentOperation != null)
            {
                _timer -= Time.deltaTime;
                _currentOperation.OnTimerTick(/*1f - */_timer / _currentOperation.Delay);
                if (_timer <= 0f)
                {
                    _currentOperation.OnOperationComplete?.Invoke();
                    _operations.Dequeue();
                    if (_operations.Count > 0)
                    {
                        _currentOperation = _operations.Peek();
                        _timer = _currentOperation.Delay;
                    }
                    else
                    {
                        _currentOperation = null;
                        _timer = 0f;
                    }
                }
            }
        }
    }

// Enqueue example

// void CompleteOperation();
// DelayedOperationsManager.Instance.QueueOperation(new DelayedOperation(CompleteOperation, 5f));
}
