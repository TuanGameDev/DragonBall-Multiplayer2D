using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public Animator aim;
    public SpriteRenderer sr;
    public Player photonPlayer;
    public static PlayerController me;
    public int id;
    [Header("Tấn Công")]
    public Transform attackPoint;
    public int damageMin;
    public int damageMax;
    public float attackRange;
    public float attackDelay;
    private float lastAttackTime;
    [Header("Di chuyển và Nhảy")]
    public Joystick joystick;
    public int moveSpeed;
    public bool faceRight = false;
    [Header("UI")]
    public TextMeshProUGUI playernameTag;
    public Canvas canvas;
    [PunRPC]
    public void InitializePlayer(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        if (player.IsLocal)
            me = this;
        else
            rb.isKinematic = false;
        if (GameManager.gamemanager.spawnPositions != null && id <= GameManager.gamemanager.spawnPositions.Length)
        {
            transform.position = GameManager.gamemanager.spawnPositions[id - 1].position;
            transform.rotation = GameManager.gamemanager.spawnPositions[id - 1].rotation;
        }
        UpdateNametag(player.NickName);
    }
    private void Start()
    {
        if (!photonView.IsMine)
        {
            canvas.enabled = false;
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;
        MoveCharacter();
    }
    #region Di chuyển
    void MoveCharacter()
    {
        float x, y;
        x = joystick.Horizontal;
        y = joystick.Vertical;
        MoveJoystick(x, y);
    }
    void MoveJoystick(float x, float y)
    {
        rb.velocity = new Vector2(x, y) * moveSpeed;
        if (x != 0 || y != 0)
        {
            aim.SetBool("Move", true);
            if (x > 0)
            {
                photonView.RPC("FlipRight", RpcTarget.All);
            }
            else
            {
                photonView.RPC("FlipLeft", RpcTarget.All);

            }
        }
        else
        {
            aim.SetBool("Move", false);
        }
    }
    [PunRPC]
    void FlipRight()
    {
        sr.flipX = false;
        faceRight = true;
        attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
    }
    [PunRPC]
    void FlipLeft()
    {
        sr.flipX = true;
        faceRight = false;
        attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
    }
    #endregion
    #region Tấn Công
    public void Attack()
    {
        lastAttackTime = Time.time;
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, transform.right, attackRange);
        aim.SetTrigger("Attack");
    }
    #endregion
    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
    #region Health + Name
    [PunRPC]
    public void UpdateNametag(string name)
    {
        playernameTag.text = ""+name;
    }
   
    #endregion
}
