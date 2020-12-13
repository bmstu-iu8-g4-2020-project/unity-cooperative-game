using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [DisallowMultipleComponent]
    public class VisibilityController : MonoBehaviour
    {
        [SerializeField]
        private float hideTime = 0.2f;

        [SerializeField]
        private float hideDelayTime = 0.3f;

        private readonly List<MeshRenderer> _renderers = new List<MeshRenderer>();
        private IEnumerator _currentCoroutine;

        private bool _isVisible; //TODO переменная переклинвает
        private float _timer;

        private void Start()
        {
            _renderers.Add(GetComponent<MeshRenderer>());
            _renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
            _timer = hideDelayTime;
            Hide();
        }

        private void Update()
        {
            if (!_isVisible) return;

            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                return;
            }

            //Start Hide
            if (_currentCoroutine == null)
            {
                _currentCoroutine = FadeAndHide();
                StartCoroutine(_currentCoroutine);
            }
        }

        private IEnumerator FadeAndHide()
        {
            while (_renderers[0].material.color.a >= 0)
            {
                foreach (var renderer1 in _renderers)
                {
                    var c = renderer1.material.color;
                    c.a -= Time.deltaTime / hideTime;
                    renderer1.material.color = c;
                }

                yield return null;
            }

            Hide();
            _currentCoroutine = null;
        }

        private IEnumerator Appear()
        {
            while (_renderers[0].material.color.a <= 1)
            {
                foreach (var renderer1 in _renderers)
                {
                    var c = renderer1.material.color;
                    c.a += Time.deltaTime / hideTime;
                    renderer1.material.color = c;
                }

                yield return null;
            }

            _currentCoroutine = null;
        }

        public void Show()
        {
            _isVisible = true;
            _timer = hideDelayTime; //reset timer

            //Stop coroutine
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _currentCoroutine = Appear();
            StartCoroutine(_currentCoroutine);

            ResetAlpha();
            foreach (var renderer1 in _renderers) renderer1.enabled = true;
        }

        public void Hide()
        {
            _isVisible = false;
            foreach (var renderer1 in _renderers) renderer1.enabled = false;
        }

        private void ResetAlpha()
        {
            foreach (var renderer1 in _renderers)
            {
                var c = renderer1.material.color;
                c.a = 1;
                renderer1.material.color = c;
            }
        }
    }
}
