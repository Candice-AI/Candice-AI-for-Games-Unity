﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceModuleCombat: CandiceBaseModule
    {
        Transform transform;
        public Action<bool> attackCompleteCallback;
        public CandiceModuleCombat(Transform transform, Action<bool> _attackCompleteCallback,string moduleName = "CandiceModuleCombat"):base(moduleName) {
            this.transform = transform;
            this.attackCompleteCallback = _attackCompleteCallback;
        }

        public void DealDamage(float damage,float attackRange, float damageAngle,List<string> tags)
        {
            //
            //Method Name : void Attack()
            //Purpose     : This method is called by the attack animation event. Deals the required damage to all targets in range..
            //Re-use      : none
            //Input       : none
            //Output      : none
            //

            RaycastHit[] hits;
            hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward);//Send a shere cast to see if it hits anything
            foreach (RaycastHit hit in hits)//for each object hit by the spherecast
            {
                bool isHittable = false;
                if (tags.Contains(hit.transform.gameObject.tag))
                    isHittable = true;
                //if the object is hittable
                if (isHittable)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
                    if (angle <= damageAngle / 2 && distance <= attackRange)//If the object is within the attack range and within the damage angle.
                    {
                        hit.transform.gameObject.SendMessage("ReceiveDamage", damage);//send the damage to the hit object. The hit object needs to have a script with the method ReceiveDamage(float damage);
                    }
                }
            }
            attackCompleteCallback(true);
        }

        public void DealDamage2D(float damage, float attackRange, float damageAngle, List<string> tags)
        {
            RaycastHit2D[] hits;
            hits = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), attackRange, transform.up);//Send a shere cast to see if it hits anything
            foreach (RaycastHit2D hit in hits)//for each object hit by the spherecast
            {
                bool isHittable = false;
                if (tags.Contains(hit.transform.gameObject.tag))
                    isHittable = true;
                //if the object is hittable
                if (isHittable)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance <= attackRange)//If the object is within the attack range and within the damage angle.
                    {
                        hit.transform.gameObject.SendMessage("ReceiveDamage", damage);//send the damage to the hit object. The hit object needs to have a script with the method ReceiveDamage(float damage);
                    }
                }
            }
            attackCompleteCallback(true);
        }
        public IEnumerator DealTimedDamage(float time, float damage, float attackRange, float damageAngle, List<string> tags, Action _attackCompleteCallback)
        {
            yield return new WaitForSecondsRealtime(time);
            DealDamage(damage, attackRange, damageAngle, tags);
            _attackCompleteCallback();
        }
        public IEnumerator DealTimedDamage2D(float time, float damage, float attackRange, float damageAngle, List<string> tags, Action _attackCompleteCallback)
        {
            yield return new WaitForSecondsRealtime(time);
            DealDamage2D(damage, attackRange, damageAngle, tags);
            _attackCompleteCallback();
        }
        public IEnumerator FireProjectile(GameObject attackTarget, GameObject attackProjectile,Vector3 spawnPosition,float time, Action _attackCompleteCallback)
        {
            //
            //Method Name : void AttackRange()
            //Purpose     : This method is called by the attack animation event. Deals the required damage to all targets in range..
            //Re-use      : none
            //Input       : none
            //Output      : none
            //
            yield return new WaitForSecondsRealtime(time);
            if (attackTarget != null)
            {
                GameObject projectile = UnityEngine.Object.Instantiate(attackProjectile, spawnPosition, Quaternion.identity);
                CandiceProjectile ai = projectile.GetComponent<CandiceProjectile>();
                ai.Fire(attackTarget);
            }
            _attackCompleteCallback();
        }

        public float ReceiveDamage(float damage,float currentHP)
        {
            //
            //Method Name : void ReceiveDamage(float damage)
            //Purpose     : This method receives damage from various sources and applies it to the character.
            //Re-use      : none
            //Input       : float damage
            //Output      : none
            //
            if (currentHP - damage <= 0)
            {
                currentHP = 0;
                //CharacterDead() method should be called after the death animation has finished playing using an Animation Event. 
                //Alternatively, you can implement your own logic here to suit your needs.
            }
            else
            {
                currentHP -= damage;
            }
            if (EnableDebug)
                Utils.Utils.LogDamageReceived(ModuleName, damage, currentHP);

            return currentHP;
            //if (CandiceConfig.enableDebug)
            //Debug.Log("Hit with: " + damage + " damage. New Health: " + HitPoints);
        }

        


    }
}