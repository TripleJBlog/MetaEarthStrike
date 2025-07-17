using UnityEngine;
using System;

namespace MetaEarthStrike.Core
{
    public class GoldManager : MonoBehaviour
    {
        public static GoldManager Instance { get; private set; }
        
        [SerializeField] private int startingGold = 100;
        private int currentGold;

        public event Action<int> OnGoldChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            currentGold = startingGold;
            OnGoldChanged?.Invoke(currentGold);
        }

        public bool SpendGold(int amount)
        {
            if (amount <= currentGold)
            {
                currentGold -= amount;
                OnGoldChanged?.Invoke(currentGold);
                return true;
            }
            return false;
        }

        public void AddGold(int amount)
        {
            currentGold += amount;
            OnGoldChanged?.Invoke(currentGold);
        }

        public int GetCurrentGold()
        {
            return currentGold;
        }
    }
}