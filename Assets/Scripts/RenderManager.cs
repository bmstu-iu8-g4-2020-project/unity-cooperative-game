// using Gameplay;
// using UnityEngine;
//
// /// <summary>
// /// Optimize rendering and apply visual effects
// /// </summary>
// public class RenderManager : MonoBehaviour
// {
//     [Header("Optimization")]
//     public float refreshRate = 0.5f; //In seconds, interval at which selectable are shown/hidden
//
//     public float distanceMultiplier = 1f; //will make all selectable active_range multiplied
//
//     public bool
//         turnOffGameObjects = false; //If on, will turn off the whole gameObjects, otherwise will just turn off scripts
//
//     private float _updateTimer = 0f;
//
//     void Update()
//     {
//         //Slow update
//         _updateTimer += Time.deltaTime;
//         if (_updateTimer > refreshRate)
//         {
//             _updateTimer = 0f;
//             SlowUpdate();
//         }
//     }
//
//     void SlowUpdate()
//     {
//         //Optimization
//         Vector3 center_pos = GameManager.Instance.GetLocalPlayer().transform.position;
//         foreach (Selectable select in Selectable.GetAll())
//         {
//             float dist = (select.GetPosition() - center_pos).magnitude;
//             select.SetActive(dist < select.activeRange * distanceMultiplier, turnOffGameObjects);
//         }
//     }
// }


