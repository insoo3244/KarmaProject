using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage; // 탄환의 데미지
    public int per; // 탄환의 관통력

    Rigidbody2D rigid; // 탄환 리지드 추가

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // 변수 초기화 함수
    // 인자 : 데미지, 관통력, 방향
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        // +) 260626 : 주석 오류 변경 (근거리 무기일 때 -> 원거리 무기일 때)
        // 원거리 무기 총알을 발사할 때
        if(per >= 0) 
        {  
            rigid.linearVelocity = dir * 15f; // 탄환 속도는 방향을 따라감 // 뒤 15는 탄환속도 크기
        }
    }

    // 다른 오브젝트에 충돌할 때 발생하는 함수
    void OnTriggerEnter2D(Collider2D collision)
    {
        // +) 260626 : 근거리 무기 관통력을 -100으로 변경함
        // 적과 충돌한 것이 아니면 or 근접 무기일 경우 종료
        if (!collision.CompareTag("Enemy") || per == -100)
        {
            return;
        }

        per--; // 관통함

        // +) 260626 : 로직을 안전하게 보완. 관통력이 음수일 경우 무조건 작동
        if(per < 0) // 관통력을 모두 소진했다면,
        {
            rigid.linearVelocity = Vector2.zero; // 재활용을 위한 속도 초기화
            gameObject.SetActive(false); // 비활성화
        }
    }

    // 투사체 삭제 로직
    void OnTriggerExit2D(Collider2D collision)
    {
        // 근접무기는 로직이 필요없음
        if (!collision.CompareTag("Area") || per == -100)
        {
            return;
        }
        
        // 플레이어 구역을 벗어나면, 오브젝트 비활성화
        gameObject.SetActive(false);
    }
}
