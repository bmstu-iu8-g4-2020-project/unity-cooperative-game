// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Player
// {
//     public class DelayedOperation
//     {
//         public delegate void OperationDelegate();
//
//         public event OperationDelegate OnOperationComplete;
//         public event OperationDelegate OnOperationCancelled;
//
//         public delegate void TickDelegate(float delay);
//         public event TickDelegate OnTimerTick;
//         
//         public float delay;
//
//         // ...
//         // // Constructor & convenience methods
//         // ...
//     }
//
// // Singleton
//     public class DelayedOperationsManager : MonoBehaviour
//     {
//         private readonly Queue<DelayedOperation> operations = new Queue<DelayedOperation>();
//
//         private DelayedOperation currentOperation;
//
//         private float timer = 0f;
//
//         public void QueueOperation(DelayedOperation newOperation)
//         {
//             operations.Enqueue(newOperation);
//             if (operations.Count == 1)
//             {
//                 StartTimer();
//             }
//         }
//
//         public void DequeueAllOperations()
//         {
//             foreach (DelayedOperation operation in operations)
//             {
//                 operation.OnOperationCancelled();
//             }
//
//             operations.Clear();
//             if (currentOperation != null)
//             {
//                 timer = 0f;
//                 currentOperation = null;
//             }
//         }
//
//         private void StartTimer()
//         {
//             currentOperation = operations.Peek();
//         }
//
//         private void Update()
//         {
//             if (currentOperation != null)
//             {
//                 timer -= Time.deltaTime;
//                 currentOperation.OnTimerTick(1f - timer / currentOperation.delay);
//                 if (timer <= 0f)
//                 {
//                     currentOperation.OnOperationComplete?.Invoke();
//                     operations.Dequeue();
//                     if (operations.Length > 0)
//                     {
//                         currentOperation = operations.Peek();
//                         timer = currentOperation.delay;
//                     }
//                     else
//                     {
//                         currentOperation = null;
//                         timer = 0f;
//                     }
//                 }
//             }
//         }
//         // ...
//         // // Singleton
//         // ...
//     }
//
// // Enqueue example
//
// // void CompleteOperation();
// // DelayedOperationsManager.Instance.QueueOperation(new DelayedOperation(CompleteOperation, 5f));
// }


