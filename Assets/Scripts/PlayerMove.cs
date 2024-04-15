using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gamaManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioItem;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    void Update()
    {
        // jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
         
        }
            

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        // Move Speed 
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if(rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }else if(rigid.velocity.x < maxSpeed * (-1))
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        // Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJumping", false);
            }
        }     
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // Jump Attack
            if(rigid.velocity.y<0 && transform.position.y> collision.transform.position.y && collision.gameObject.name != "Spike")
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else
            OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            // Point
            bool isCopper = collision.gameObject.name.Contains("Copper");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            
            if(isCopper)
                gamaManager.stagePoint += 50;
            else if(isSilver)
                gamaManager.stagePoint += 100;
            else if(isGold)
                gamaManager.stagePoint += 300;

            // Deactive Item
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            // Next Stage
            gamaManager.NextStage();

            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {      
        // Point 
        gamaManager.stagePoint += 500;
        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();             
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gamaManager.HealthDown();

        // change Layer 
        gameObject.layer = 9;
        
        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*10, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 색 투명하게
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        // 캐릭터 뒤집기
        spriteRenderer.flipY = true;
        // 캐릭터 충돌 비활성화
        capsuleCollider.enabled = false;
        // 위로 점프
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        
    }

    public void Resurrection()
    {
        // 색 원래대로
        spriteRenderer.color = new Color(1, 1, 1, 1);
        // 캐릭터 뒤집기
        spriteRenderer.flipY = false;
        // 캐릭터 충돌 활성화
        capsuleCollider.enabled = true;
        // 원래 위치로 이동
        gamaManager.PlayerReposition();
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
