using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class VisibilityController : MonoBehaviour
    {
        private MeshRenderer _renderer;
        public MeshRenderer[] ChildrenRenderers { get; private set; }


        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            ChildrenRenderers = GetComponentsInChildren<MeshRenderer>();
            _renderer.enabled = true;
        }

        private void Start()
        {
            Hide();
            StartCoroutine(HideWithDelay(0.2f));
        }

        public void Hide()
        {
            _renderer.enabled = false;
            foreach (var childRenderer in ChildrenRenderers)
            {
                childRenderer.enabled = false;
            }
        }

        public void Show()
        {
            _renderer.enabled = true;
            foreach (var childRenderer in ChildrenRenderers)
            {
                childRenderer.enabled = true;
            }
        }

        private IEnumerator HideWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                Hide();
            }
        }
    }
}
