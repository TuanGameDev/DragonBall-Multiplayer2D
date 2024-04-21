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
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("DamageMax"))
        {
            damage = PlayerPrefs.GetInt("DamageMax")* increaseAttack;
        }
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = moveDirection * speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag==("Enemy")&&isMine)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient,this.attackId,damage);
            Destroy(gameObject);
        }
    }
    public void Initialized(int attackId,bool isMine)
    {
        this.attackId = attackId;
        this.isMine = isMine;
    }
}
