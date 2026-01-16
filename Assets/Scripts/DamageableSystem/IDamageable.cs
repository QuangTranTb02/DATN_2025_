using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    void ApplyDamage(int damage);
    void CalculateDamage(ref int damage);
    void CheckState();
}
