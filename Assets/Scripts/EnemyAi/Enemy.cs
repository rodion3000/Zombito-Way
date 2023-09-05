using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public abstract class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    public int currentHealth;
    public Transform player;
    public bool isFlipped = false;
    Rigidbody2D rb ;
    public float speedRun = 2.5f;
    public float attackRangeRun = 2.5f;
    public float agroDistance = 4.5f;
    bool reachedEndOfPath = false;
    SpriteRenderer sprite;
    public float attackRange = 0.5f;
    public LayerMask playerLayers;
    public int attackDamage = 40;
    public float attackRate = 2f;
    public Transform attackPoint;
    public float DamageTimeSec = 0.5f;
    private Material attackMaterial;
    private Material defMaterial;

    
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        attackMaterial = Resources.Load("Damage", typeof(Material)) as Material;
        defMaterial = sprite.material;
        Physics2D.IgnoreLayerCollision(6, 6, true);
        Physics2D.IgnoreLayerCollision(6, 3, true);

    }



    private void FixedUpdate()
    {
        LookAtPlayer();
        float distToPlayer = Vector2.Distance(rb.position, player.position);

        if (distToPlayer <= agroDistance && distToPlayer >= 1.2)
        {
            animator.SetBool("Run", true);
            var step = speedRun * Time.fixedDeltaTime;
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            rb.transform.position = Vector2.MoveTowards(rb.position, target, step);
            return;


        }

        else if (distToPlayer >= 8)
        {
            animator.SetBool("Run", false);
           
        } 
 

        if (distToPlayer <= attackRangeRun)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("Run", false);

        }
    }
       
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        sprite.material = attackMaterial;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("ResetMaterial", .2f);
        }
    }

    void ResetMaterial()
    {
        sprite.material = defMaterial;
    }

    public virtual void Die()
    {
        animator.SetTrigger("IsDead");
       GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
        sprite.material = defMaterial;
    }
    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;
        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    public void Attack()
    {

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayers);
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }

    }
    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void StartEffect()
    {
        StopCoroutine(nameof(StartEffectCoroutine));
        StartCoroutine(nameof(StartEffectCoroutine));
    }

    private IEnumerator StartEffectCoroutine()
    {
        float time = 0;
        float step = 1f / DamageTimeSec;

        while (time < DamageTimeSec)
        {
            time += Time.deltaTime;

            yield return null;
        }
    }



}