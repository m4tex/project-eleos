using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Player
{
    public class StatsManager : MonoBehaviour
    {
        public static int Health
        {
            get => _health;
            set
            {
                if (value <= 0 && !_instance._dead)
                    _instance.Die();

                if (value < _health)
                    AudioManager.Damage();
                UIManager.UpdateHealth(value);
                _health = value;
            }
        }

        public static float HealthRegenFactor { get; private set; }  //Could be negative to inflict damage over time
        public static float SpeedFactor { get; private set; } = 1;
        public static float DamageFactor { get; private set; }

        private static int _points = 0;
        private static int _health = 100;

        private static int _score = 0;
        private bool _dead = false; 
        
        // public static int Score
        // {
        //     get => _score; 
        //     set => _score = value;
        // }

        public static int killedZombies;
        public static int currentWave = 1;
        
        public static int Points
        {
            get => _points;
            set
            {
                UIManager.UpdatePoints(value);
                _points = value;
            }   
        }

        public static float pointGainFactor { get; private set; }

        private static StatsManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void Start() => Reset();
        
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
            pointGainFactor = _pointGainEffects.Aggregate(1, (float result, float factor) => result * factor);
            HealthRegenFactor = _healthRegenEffects.Aggregate(1, (float result, float factor) => result * factor);

            _instance.speedEffects = _speedEffects;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                Points += 1000;
            if (Input.GetKeyDown(KeyCode.M))
                Health -= 20;
        }

        private void Die()
        {
            UIManager.ControllsLock = true;
            AudioManager.Death();
            int score = killedZombies * currentWave * 11;
            int highScore = PlayerPrefs.GetInt("HighScore");
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", score);
            }
            
            UIManager.Death(score, highScore);

            _dead = true;
        }

        public static void Reset()
        {
            Health = 100;
            Points = 0;
            _score = 0;
            _instance._dead = false;
            currentWave = 0;
            killedZombies = 0;
        }
    }
}