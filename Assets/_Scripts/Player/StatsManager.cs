using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.UI;
using _Scripts.Weapons;
using UnityEngine;

namespace _Scripts.Player
{
    public class StatsManager : MonoBehaviour
    {
        public static int Health { get; set; } = 100;
        //Could be negative to inflict damage over time
        public static float HealthRegenFactor { get; private set; }
        public static float SpeedFactor { get; private set; } = 1;
        public static float DamageFactor { get; private set; }

        private static int _points = 0;

        public static int Points
        {
            get => _points;
            set
            {
                UIManager.UpdatePoints(value);
                _points = value;
            }
        }

        public static float PointGainFactor { get; private set; }

        private static StatsManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        //Use coroutines to remove an effect after certain amount of time
        private static List<float> _speedEffects = new();
        private static List<float> _damageEffects = new();
        private static List<float> _pointGainEffects = new();
        private static List<float> _healthRegenEffects = new();

        public List<float> speedEffects = new();

        public enum EffectType
        {
            HealthRegen,
            Speed,
            Damage,
            PointGain
        }

        private static IEnumerator ApplyEffect(ICollection<float> effectList, float factor, float duration)
        {
            effectList.Add(factor);
            UpdateStats();
            yield return new WaitForSeconds(duration);
            effectList.Remove(factor);
            UpdateStats();
        }
    
        public static void AddEffect(EffectType type, float factor, float duration)
        {
            switch (type)
            {
                case EffectType.Speed:
                    _instance.StartCoroutine(ApplyEffect(_speedEffects, factor, duration));
                    break;
                case EffectType.Damage:
                    _instance.StartCoroutine(ApplyEffect(_damageEffects, factor, duration));
                    break;
                case EffectType.PointGain:
                    _instance.StartCoroutine(ApplyEffect(_pointGainEffects, factor, duration));
                    break;
                case EffectType.HealthRegen:
                    _instance.StartCoroutine(ApplyEffect(_healthRegenEffects, factor, duration));
                    break;
                default:
                    throw new Exception("Unknown effect type in StatsManager");
            }
        }

        private static void UpdateStats()
        {
            DamageFactor = _damageEffects.Aggregate(1, (float result, float factor) => result * factor);
            SpeedFactor = _speedEffects.Aggregate(1, (float result, float factor) => result * factor);
            PointGainFactor = _pointGainEffects.Aggregate(1, (float result, float factor) => result * factor);
            HealthRegenFactor = _healthRegenEffects.Aggregate(1, (float result, float factor) => result * factor);

            _instance.speedEffects = _speedEffects;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                Points += 1000;
        }
    }
}