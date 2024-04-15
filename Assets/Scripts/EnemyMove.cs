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

    // ��� �Լ� 
    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite (ĳ���� �̹��� ���� ��ȯ)
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
            // �� �����ϰ�
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            // ĳ���� ������
            spriteRenderer.flipY = true;
            // ĳ���� �浹 ��Ȱ��ȭ
            capsuleCollider.enabled = false;
            // ���� ����
            rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // 5���� ��Ȱ��ȭ
        StartCoroutine(DeadCoroutine(5f));
    }

    public void ResetEnemy()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        spriteRenderer.flipY = false;
        capsuleCollider.enabled = true;
        Debug.Log("���� �ʱ�ȭ");
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
