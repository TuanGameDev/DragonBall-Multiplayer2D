using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireKame : MonoBehaviourPun
{
    public float speed;
    public Vector2 moveDirection;
    private Rigidbody2D rb;
    public int damage;
    public int increaseAttack;
    private int attackId;
    private bool isMine;

    void Start()
    {
        if (PlayerPrefs.HasKey("DamageMax"))
        {
            damage = PlayerPrefs.GetInt("DamageMax") * increaseAttack;
        }
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroyObject", 2);
    }

    void Update()
    {
        rb.velocity = moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isMine)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, this.attackId, damage);
            //photonView.RPC("DestroyObject", RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    void DestroyObject()
    {
        Destroy(gameObject);
    }
    public void Initialized(int attackId, bool isMine)
    {
        this.attackId = attackId;
        this.isMine = isMine;
    }
}