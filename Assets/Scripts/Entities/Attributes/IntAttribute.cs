using System;
using UnityEngine;

namespace Entities.Attributes
{
    [Serializable]
    public class IntAttribute
    {
        public delegate void MethodContainer(int newValue);

        private readonly bool _isUnsigned;

        private int _attributeBaseBonus;
        [SerializeField]
        private int attributeRaw;

        public IntAttribute(int rawValue, bool isU = true)
        {
            _isUnsigned = isU;
            attributeRaw = rawValue;
        }

        public event MethodContainer OnChange;

        public void SetRawValue(int rawValue)
        {
            attributeRaw = rawValue;
            OnChange?.Invoke(GetModified());
        }

        public void AddBaseBonus(int baseBonus)
        {
            _attributeBaseBonus += baseBonus;
            OnChange?.Invoke(GetModified());
        }

        public int GetModified()
        {
            if (_isUnsigned && attributeRaw + _attributeBaseBonus < 0) return 0;

            return attributeRaw + _attributeBaseBonus;
        }

        public void ResetModifiers()
        {
            _attributeBaseBonus = 0;

            if (OnChange != null)
                foreach (var d in OnChange.GetInvocationList())
                    OnChange -= d as MethodContainer;
        }
    }
}
