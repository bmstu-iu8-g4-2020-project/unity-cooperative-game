using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Player
{
    /// <summary>
    /// Mouse controls manager
    /// </summary>
    public class PlayerControlsMouse : MonoBehaviour
    {
        public LayerMask selectableLayer = ~0;
        public LayerMask floorLayer = (1 << 9); //Put to none to always return 0 as floor height

        public event UnityAction<Vector3> OnClick; //Always triggered on left click
        public event UnityAction<Vector3> OnRightClick; //Always triggered on right click
        public event UnityAction<Vector3> OnLongClick; //When holding the left click down for 1+ sec
        public event UnityAction<Vector3> OnHold; //While holding the left click down
        public event UnityAction<Vector3> OnClickFloor; //When click on floor

        public event UnityAction<Interactable> OnClickObject; //When click on object

        private bool _usingMouse = false;
        private float _mouseScroll = 0f;
        private Vector2 _mouseDelta = Vector2.zero;

        private float _usingTimer = 0f;
        private float _holdTimer = 0f;
        private bool _isHolding = false;
        private bool _canLongClick = false;
        private Vector3 _holdStart;
        private Vector3 _lastPos;
        private Vector3 _floorPos; //World position the floor pointing at

        private RaycastHit[] _raycastHits = new RaycastHit[10];
        private readonly HashSet<GameObject> _raycastList = new HashSet<GameObject>();

        private static PlayerControlsMouse _instance;

        #region singltone

        public static PlayerControlsMouse Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Removed duplicate singltone script on {gameObject.name}");
                Destroy(this);
            }

            _lastPos = Input.mousePosition;
        }

        #endregion

        void Update()
        {
            RaycastInteractables();
            RaycastFloorPos();

            //Mouse click
            if (Input.GetMouseButtonDown(0))
            {
                _holdStart = Input.mousePosition;
                _isHolding = true;
                _canLongClick = true;
                _holdTimer = 0f;
                OnMouseClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseClick();
            }

            //Mouse scroll
            _mouseScroll = Input.mouseScrollDelta.y;

            //Mouse delta
            _mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            //Check for mouse usage
            Vector3 diff = (Input.mousePosition - _lastPos);
            float dist = diff.magnitude;
            if (dist > 0.01f)
            {
                _usingMouse = true;
                _usingTimer = 1f;
                _lastPos = Input.mousePosition;
            }

            bool mouseHold = Input.GetMouseButton(0);
            if (mouseHold)
                _usingTimer = 1f;

            //Long mouse click
            float distHold = (Input.mousePosition - _holdStart).magnitude;
            _isHolding = _isHolding && mouseHold;
            _canLongClick = _canLongClick && mouseHold && distHold < 5f;

            if (_isHolding)
            {
                _holdTimer += Time.deltaTime;
                if (_canLongClick && _holdTimer > 0.5f)
                {
                    _canLongClick = false;
                    _holdTimer = 0f;
                    OnLongMouseClick();
                }
                else if (!_canLongClick && _holdTimer > 0.2f)
                {
                    OnMouseHold();
                }
            }

            //Is using mouse? (vs keyboard)
            _usingTimer -= Time.deltaTime;
            _usingMouse = _usingTimer > 0f;
        }

        public void RaycastInteractables()
        {
            _raycastList.Clear();
            var size = Physics.RaycastNonAlloc(GameManager.Instance.GetCamera().ScreenPointToRay(Input.mousePosition), _raycastHits, 99f, selectableLayer.value);
            foreach (RaycastHit hit in _raycastHits)
            {
                if (hit.collider != null)
                {
                    Interactable select = hit.collider.GetComponentInParent<Interactable>();
                    if (select != null)
                        _raycastList.Add(select.gameObject);
                }
            }
        }

        public void RaycastFloorPos()
        {
            Ray ray = GameManager.Instance.GetCamera().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool success = Physics.Raycast(ray, out hit, 100f, floorLayer.value);
            if (success)
            {
                _floorPos = ray.GetPoint(hit.distance);
            }
            else
            {
                Plane plane = new Plane(Vector3.up, 0f);
                float dist;
                bool phit = plane.Raycast(ray, out dist);
                if (phit)
                {
                    _floorPos = ray.GetPoint(dist);
                }
            }

            Debug.DrawLine(GameManager.Instance.GetCamera().transform.position, _floorPos);
        }

        private void OnMouseClick()
        {
            if (IsMouseOverUI())
                return;

            Interactable hovered = GetNearestRaycastList(_floorPos);
            if (hovered != null)
            {
                OnClick?.Invoke(hovered.transform.position);
                OnClickObject?.Invoke(hovered);
            }
            else
            {
                OnClick?.Invoke(_floorPos);
                OnClickFloor?.Invoke(_floorPos);
            }
        }

        private void OnRightMouseClick()
        {
            if (IsMouseOverUI())
                return;

            OnRightClick?.Invoke(_floorPos);
        }

        //When holding for 1+ sec
        private void OnLongMouseClick()
        {
            if (IsMouseOverUI())
                return;

            OnLongClick?.Invoke(_floorPos);
        }

        private void OnMouseHold()
        {
            if (IsMouseOverUI())
                return;

            OnHold?.Invoke(_floorPos);
        }

        public Interactable GetNearestRaycastList(Vector3 pos)
        {
            Interactable nearest = null;
            float minDist = 99f;
            foreach (GameObject obj in _raycastList)
            {
                Interactable select = obj.GetComponent<Interactable>();
                if (select != null)
                {
                    float dist = (select.transform.position - pos).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = select;
                    }
                }
            }

            return nearest;
        }

        public ItemSlot GetInventorySelectedSlot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// In percentage
        /// </summary>
        public Vector2 GetScreenPos()
        {
            Vector3 mpos = Input.mousePosition;
            return new Vector2(mpos.x / (float) Screen.width, mpos.y / (float) Screen.height);
        }

        public bool IsInRaycast(GameObject obj)
        {
            return _raycastList.Contains(obj);
        }

        public bool IsUsingMouse()
        {
            return _usingMouse;
        }

        public float GetMouseScroll()
        {
            return _mouseScroll;
        }

        public Vector2 GetMouseDelta()
        {
            return _mouseDelta;
        }

        //Check if mouse is on top of any UI element
        public bool IsMouseOverUI()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
