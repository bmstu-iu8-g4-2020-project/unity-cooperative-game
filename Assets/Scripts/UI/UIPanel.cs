using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// Generic script for any UI panel, can be inherited 
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        public float displaySpeed = 2f;

        public UnityAction OnShow;
        public UnityAction OnHide;

        private CanvasGroup _canvasGroup;
        private bool _visible;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _visible = false;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            float add = _visible ? displaySpeed : -displaySpeed;
            float alpha = Mathf.Clamp01(_canvasGroup.alpha + add * Time.deltaTime);
            _canvasGroup.alpha = alpha;

            if (!_visible && alpha < 0.01f)
                AfterHide();
        }

        public virtual void Toggle(bool instant = false)
        {
            if (IsVisible())
                Hide(instant);
            else
                Show(instant);
        }

        public virtual void Show(bool instant = false)
        {
            _visible = true;
            gameObject.SetActive(true);

            if (instant || displaySpeed < 0.01f)
                _canvasGroup.alpha = 1f;

            OnShow?.Invoke();
        }

        public virtual void Hide(bool instant = false)
        {
            _visible = false;
            if (instant || displaySpeed < 0.01f)
                _canvasGroup.alpha = 0f;

            OnHide?.Invoke();
        }

        public void SetVisible(bool visi)
        {
            if (!_visible && visi)
                Show();
            else if (_visible && !visi)
                Hide();
        }

        public virtual void AfterHide() => gameObject.SetActive(false);

        public bool IsVisible() => _visible;

        public float GetAlpha() => _canvasGroup.alpha;
    }
}
