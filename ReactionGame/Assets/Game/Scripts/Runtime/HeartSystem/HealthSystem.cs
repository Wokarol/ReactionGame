using DG.Tweening;
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
        Messenger.Default.AddListener<GameplayEvents.LiveLost>(_ => SubtractHP());
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

        var seq = DOTween.Sequence();
        seq.Append(DOVirtual.Float(Time.timeScale, 0.2f, 0.2f, v => Time.timeScale = v));
        seq.AppendInterval(0.2f);
        seq.Append(DOVirtual.Float(0.2f, Time.timeScale, 0.1f, v => Time.timeScale = v));

        seq.SetUpdate(true);

        if (CurrentHealth == 0) {
            Messenger.Default.SendMessage(new GameplayEvents.GameOver());
        }
    }
    [Button("+", ButtonAttribute.EnableMode.Playmode)]
    public void AddHP()
    {
        CurrentHealth += 1;
        HealthChanged?.Invoke();
    }
}
