using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class AttributeResolver : MonoBehaviour
    {
        [SerializeField]
        private AttributeData[] attributesList;

        [SerializeField]
        private float updateRate = 1f;

        ///Attr type -> current value of attribute
        public readonly Dictionary<AttributeType, float> Attributes = new Dictionary<AttributeType, float>();

        private void Start()
        {
            foreach (var attr in attributesList)
            {
                if (!HasAttribute(attr.type))
                    SetAttributeValue(attr.type, attr.startValue);
            }

            InvokeRepeating(nameof(ResolveAttributeEffects), updateRate, updateRate);
        }

        //TODO move hp to Destructible
        private void ResolveAttributeEffects()
        {
            float gameSpeed = GameManager.Instance.GetGameTimeSpeedPerSec(); // TODO get from GameManager from GameData

            //Update Attributes
            foreach (AttributeData attr in attributesList)
            {
                float updateValue = attr.valuePerHour /*+ GetTotalBonus(BonusEffectData...)*/;
                updateValue = updateValue * gameSpeed * Time.deltaTime;
                AddAttributeValue(attr.type, updateValue, attr.maxValue);
                //Update UI bars
                UIController.Instance.AttributeBarsUI.UpdateBar(attr.type, AttributeInPercent(attr.type));
            }

            float healthMax = GetAttributeMax(AttributeType.Health);
            float health = GetAttributeValue(AttributeType.Health);

            foreach (AttributeData attr in attributesList)
            {
                if (GetAttributeValue(attr.type) < 0.01f)
                {
                    PlayerData.Get().SpeedMultiplier *= attr.depleteMoveMult;
                    float updateValue = attr.depleteHpLoss * gameSpeed * Time.deltaTime;
                    AddAttributeValue(AttributeType.Health, updateValue, healthMax);
                }
            }

            //Dying
            health = GetAttributeValue(AttributeType.Health);
            if (health < 0.01f)
                /*Kill()*/ ;
        }

        public float GetAttributeMax(AttributeType type)
        {
            AttributeData adata = GetAttribute(type);
            if (adata != null)
                return adata.maxValue;
            return 0f;
        }

        public AttributeData GetAttribute(AttributeType type)
        {
            foreach (AttributeData attr in attributesList)
            {
                if (attr.type == type)
                    return attr;
            }

            return null;
        }

        public bool HasAttribute(AttributeType type)
        {
            return Attributes.ContainsKey(type);
        }

        public float GetAttributeValue(AttributeType type)
        {
            if (Attributes.ContainsKey(type))
                return Attributes[type];
            return 0f;
        }

        public void SetAttributeValue(AttributeType type, float value)
        {
            Attributes[type] = value;
        }

        public void AddAttributeValue(AttributeType type, float value, float max)
        {
            if (!Attributes.ContainsKey(type))
                Attributes[type] = value;
            else
                Attributes[type] += value;

            Attributes[type] = Mathf.Clamp(Attributes[type], 0f, max);
        }

        public float AttributeInPercent(AttributeType type)
        {
            return GetAttributeValue(type) / GetAttributeMax(type) * 100;
        }
    }
}
