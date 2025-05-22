using System;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    public float teamMoney = 0f;

    public event Action<float> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional if persistent across scenes
    }

    public bool TrySpendMoney(float amount)
    {
        if (teamMoney >= amount)
        {
            teamMoney -= amount;
            OnMoneyChanged?.Invoke(teamMoney);
            return true;
        }

        return false;
    }

    public void AddMoney(float amount)
    {
        teamMoney += amount;
        OnMoneyChanged?.Invoke(teamMoney);
    }

    public float GetMoney() => teamMoney;
}