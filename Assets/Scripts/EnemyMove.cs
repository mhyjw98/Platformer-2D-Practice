using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Think();

        Invoke("Think", 5);
    }

    

    void FixedUpdate()
    {
        // Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.4f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Turn();
        }
        
    }

    // 재귀 함수 
    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite (캐릭터 이미지 방향 전환)
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 5);
    }

    public void OnDamaged()
    {      
            // 색 투명하게
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            // 캐릭터 뒤집기
            spriteRenderer.flipY = true;
            // 캐릭터 충돌 비활성화
            capsuleCollider.enabled = false;
            // 위로 점프
            rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // 5초후 비활성화
        StartCoroutine(DeadCoroutine(5f));
    }

    public void ResetEnemy()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        spriteRenderer.flipY = false;
        capsuleCollider.enabled = true;
        Debug.Log("몬스터 초기화");
    }
    
    void DeActive()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DeadCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        DeActive();
    }
}
