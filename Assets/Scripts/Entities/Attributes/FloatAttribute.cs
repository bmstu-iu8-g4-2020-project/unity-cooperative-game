using System;
using UnityEngine;

namespace Entities.Attributes
{
    [Serializable]
    public class FloatAttribute
    {
        public delegate void MethodContainer(float newValue);

        private float _attributeAdditiveModifier = 1f;
        private float _attributeMultiplicativeModifier = 1f;
        [SerializeField]
        private float attributeRaw;

        private float _attributeScalableBonus;
        private float _attributeUnScalableBonus;

        //Access to all attributes
        public FloatAttribute(float rawValue)
        {
            attributeRaw = rawValue;
        }

        public event MethodContainer OnChange;

        public void SetRawValue(float rawValue)
        {
            attributeRaw = rawValue;
            OnChange?.Invoke(GetModified());
        }

        public void AddScalableBonus(float scalableBonus)
        {
            _attributeScalableBonus += scalableBonus;
            OnChange?.Invoke(GetModified());
        }

        public void AddUnScalableBonus(float unScalableBonus)
        {
            _attributeUnScalableBonus += unScalableBonus;
            OnChange?.Invoke(GetModified());
        }

        public void AddAdditiveModifier(float additiveModifier)
        {
            _attributeAdditiveModifier += additiveModifier;
            OnChange?.Invoke(GetModified());
        }

        public void AddMultiplicativeModifier(float multiplicativeModifier)
        {
            _attributeMultiplicativeModifier *= multiplicativeModifier;
            OnChange?.Invoke(GetModified());
        }

        public float GetModified()
        {
            return (attributeRaw + _attributeScalableBonus) * _attributeAdditiveModifier *
                _attributeMultiplicativeModifier + _attributeUnScalableBonus;
        }

        public void ResetModifiers(bool resetEvents = false)
        {
            _attributeScalableBonus = 0f;
            _attributeUnScalableBonus = 0f;
            _attributeAdditiveModifier = 1f;
            _attributeMultiplicativeModifier = 1f;

            if (resetEvents && OnChange != null)
                foreach (var d in OnChange.GetInvocationList())
                    OnChange -= d as MethodContainer;
        }
    }
}
