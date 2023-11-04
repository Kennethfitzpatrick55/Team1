using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{

    public float shootRate;
    public int shootDamage;
    public int shootDist;
    public int ammmoCur;
    public int ammmoMax;

    public GameObject model;
    public ParticleSystem hiteffect;
    public AudioClip shootsound;
    [Range(0, 1)] public float shootsoundvol;

}
