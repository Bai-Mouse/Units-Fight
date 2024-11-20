using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObject/UnitData",order = 0)]
public class CharacterData : ScriptableObject
{
    public float cost;
    public float health;
    public float speed;
    public float strength;
    public Texture2D icon;
    public GameObject instance;
}
