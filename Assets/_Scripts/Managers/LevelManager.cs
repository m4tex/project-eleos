using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemies;
using _Scripts.Environment;
using _Scripts.Player;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        public List<Spawner> spawners = new();
        public List<GameObject> zombiePrefabs = new();
        // private readonly Dictionary<Transform, ZState> _spawners = new();

        [Header("Spawn Rates")] 
        public int regularZSpawnrate = 100;
        public int spiderSpawnrate = 30;
        public int crawlerSpawnrate = 20;
        public int bigZSpawnrate = 10;

        [FormerlySerializedAs("roundPatterns")]
        public List<ZombieWave> wavePatterns = new();
        
        [HideInInspector]
        public List<GameObject> zombies = new();

        private void Start()
        {
            Instance = this;
        }

        public void Begin()
        {
            StartCoroutine(_waves(wavePatterns.Count));
        }

        private IEnumerator _waves(int count)
        {
            yield return new WaitForSeconds(2f);
            
            for (int i = 0; i < count; i++)
            {
                StatsManager.currentWave++;
                AudioManager.NextRound();
                UIManager.Round(i+1);
                yield return new WaitForSeconds(2f);
                
                int zCount = wavePatterns[i].zombieCount;

                for (int j = 0; j < zCount; j += spawners.Count)
                {
                    foreach (var spawner in spawners)
                    {
                        Transform spawnerT = spawner.transform;
                        GameObject zombie = Instantiate(zombiePrefabs[(int)DrawOneType(i)], spawnerT.position, spawnerT.rotation);
                        zombie.GetComponent<Zombie>().SetState(spawner.spawningState);
                        zombies.Add(zombie);
                    }
                
                    yield return new WaitForSeconds(wavePatterns[i].spawnrate);
                }

                while (zombies.Count > 0) yield return null;

                AudioManager.WaveEnd();
                yield return new WaitForSeconds(12f);
            }
        }

        private ZType DrawOneType(int wave)
        {
            bool regularFilter = wavePatterns[wave].regularZFilter;
            bool spiderFilter = wavePatterns[wave].spiderFilter;
            bool crawlerFilter = wavePatterns[wave].crawlerFilter;
            bool bigZFilter = wavePatterns[wave].bigZFilter;

            int total = (regularFilter ? regularZSpawnrate : 0) + (spiderFilter ? spiderSpawnrate : 0) +
                        (crawlerFilter ? crawlerSpawnrate : 0) + (bigZFilter ? bigZSpawnrate : 0);

            int draw = Random.Range(0, total);

            if (draw < regularZSpawnrate && regularFilter)
                return ZType.Regular;
            if (draw < regularZSpawnrate + spiderSpawnrate && spiderFilter)
                return ZType.Spider;
            if (draw < regularZSpawnrate + spiderSpawnrate + crawlerSpawnrate && crawlerFilter)
                return ZType.Crawler;

            if (bigZFilter)
                return ZType.Big;

            throw new Exception("All Zombies filtered or incorrect wave draw executed");
        }
    }
}
