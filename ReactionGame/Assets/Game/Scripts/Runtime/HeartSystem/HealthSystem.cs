using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wokarol.MessageSystem;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event System.Action HealthChanged;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void OnEnable()
    {
        Messenger.Default.AddListener<GameplayEvents.SubtractHP>(_ => SubtractHP());
    }
    private void OnDisable()
    {
        Messenger.Default.RemoveAllListenersFor(this);
    }

    // Buttons
    [Button("-", ButtonAttribute.EnableMode.Playmode)]
    public void SubtractHP()
    {
        CurrentHealth -= 1;
        HealthChanged?.Invoke();

        if(CurrentHealth == 0) {
            Debug.Log("Game Over");
        }
    }
    [Button("+", ButtonAttribute.EnableMode.Playmode)]
    public void AddHP()
    {
        CurrentHealth += 1;
        HealthChanged?.Invoke();
    }
}
