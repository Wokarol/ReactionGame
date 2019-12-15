using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDisplay : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    private HeartContainer[] hearts;

    private void Awake()
    {
        InitHeartContainers();
    }

    private void OnEnable()
    {
        healthSystem.HealthChanged += HealthChanged;
    }
    private void OnDisable()
    {
        healthSystem.HealthChanged -= HealthChanged;
    }

    private void HealthChanged()
    {
        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].SetState(healthSystem.CurrentHealth > i);
        }
    }

    private void InitHeartContainers()
    {
        hearts = new HeartContainer[healthSystem.MaxHealth];
        HeartContainer[] precreatedHearts = GetComponentsInChildren<HeartContainer>();

        // TODO: Debug this code

        // Precreated > Target
        if (precreatedHearts.Length > hearts.Length) {
            // Copies [target] hearts to array
            for (int i = 0; i < hearts.Length; i++) {
                hearts[i] = precreatedHearts[i];
            }
            // Removes all not needed ones
            for (int i = hearts.Length; i < precreatedHearts.Length; i++) {
                Destroy(precreatedHearts[i].gameObject);
            }
        }

        // Precreated < Target
        if (precreatedHearts.Length <= hearts.Length) {
            // Copies [target] hearts to array
            for (int i = 0; i < precreatedHearts.Length; i++) {
                hearts[i] = precreatedHearts[i];
            }

            HeartContainer last = precreatedHearts[precreatedHearts.Length - 1];
            // Removes all not needed ones
            for (int i = precreatedHearts.Length; i < hearts.Length; i++) {
                hearts[i] = Instantiate(last, last.transform.parent);
            }
        }
    }
}
