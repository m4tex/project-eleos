using UnityEngine;

namespace _Scripts.Managers
{
    [CreateAssetMenu(fileName = "ZombieWave", menuName = "Zombie Wave")]
    public class ZombieWave : ScriptableObject
    {
        public int zombieCount;
        public float spawnrate;
        public bool regularZFilter, spiderFilter, crawlerFilter, bigZFilter;
    }
}