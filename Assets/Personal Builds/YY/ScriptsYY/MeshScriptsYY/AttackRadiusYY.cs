using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class AttackRadiusYY : MonoBehaviour
{
   private List<IDamageableYY> Damageables = new List<IDamageableYY>();
   public int Damage = 10;
   public float AttackDelay = 0.5f;
   public delegate void AttackEvent(IDamageableYY Target);
   public AttackEvent OnAttack;
   private Coroutine AttackCoroutine;

   private void OnTriggerEnter(Collider other)
   {
      IDamageableYY damageable = other.GetComponent<IDamageableYY>();
      if (damageable != null)
         Damageables.Add(damageable);
      if (AttackCoroutine == null)
      {
         AttackCoroutine = StartCoroutine(Attack());
      }
   }

   private void OnTriggerExit(Collider other)
   {
      IDamageableYY damageable = other.GetComponent<IDamageableYY>();
      if (damageable != null)
      {
         Damageables.Remove(damageable);
         if (Damageables.Count == 0)
         {
            StopCoroutine(AttackCoroutine);
            AttackCoroutine = null;
         }
      }
   }

      private IEnumerator Attack()
      {
         WaitForSeconds Wait = new WaitForSeconds(AttackDelay);
         yield return Wait;

         IDamageableYY closestDamageable = null;
         float closestDistance = float.MaxValue;

         while (Damageables.Count > 0)
         {
            for (int i = 0; i < Damageables.Count; i++)
            {
               Transform damageableTransform = Damageables[i].GetTransform();
               float distance = Vector3.Distance(transform.position, damageableTransform.position);

               if (distance < closestDistance)
               {
                  closestDistance = distance;
                  closestDamageable = Damageables[i];
               }
            }

            if (closestDamageable != null)
            {
               OnAttack?.Invoke(closestDamageable);
               closestDamageable.TakeDamage(Damage);
               
            }

            closestDamageable = null;
            closestDistance = float.MaxValue;
            yield return Wait;
            Damageables.RemoveAll(DisabledDamageables);
         }

         AttackCoroutine = null;
      }

      private bool DisabledDamageables(IDamageableYY Damageable)
      {
         return Damageable != null && !Damageable.GetTransform().gameObject.activeSelf;
      
      }
}