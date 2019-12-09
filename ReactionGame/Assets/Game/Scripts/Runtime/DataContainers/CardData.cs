using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    [SerializeField] private Sprite pattern = null;

    public Sprite Pattern => pattern;
}
