using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;

public class SkillObject : MonoBehaviourPun
{
    public float speed;
    public Vector2 moveDirection;
    public Rigidbody2D rb;
    public int damage;
    public int increaseAttack;
    private int attackId;
    private bool isMine;
    public Transform target;

    private void Start()
    {
        if (PlayerPrefs.HasKey("DamageMax"))
        {
            damage = PlayerPrefs.GetInt("DamageMax") * increaseAttack;
        }
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroyObject",1);
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        rb.velocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isMine)
        {
            DealDamage(other.gameObject);
        }
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }
    private void DealDamage(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.photonView.RPC("TakeDamage", RpcTarget.All, this.attackId, damage);
        }
        Boss boss = enemyObject.GetComponent<Boss>();
        if (boss != null)
        {
            boss.photonView.RPC("TakeDamage", RpcTarget.All, this.attackId, damage);
        }
    }
    public void Initialized(int attackId, bool isMine)
    {
        this.attackId = attackId;
        this.isMine = isMine;
    }
}