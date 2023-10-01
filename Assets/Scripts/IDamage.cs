using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface for damageable game objects
public interface IDamage
{
    void TakeDamage(int amount);
}
