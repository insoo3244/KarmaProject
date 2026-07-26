using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed; // 속도 변수
    public float health; // 현재 체력
    public float maxHealth; // 최대 체력
    public RuntimeAnimatorController[] animCon; // 런타임에 따른 애니메이션 컨트롤러 배열
    public Rigidbody2D target; // 리지드바디2D로 선언해서 물리적인 추적 구현(타겟은 플레이어)

    bool isLive; // 생존여부

    Rigidbody2D rigid; // 리지드바디2d 활용 변수
    Collider2D coll; // 충돌 변수
    Animator anim; // 애니메이터 변수
    SpriteRenderer spriter; // 스프라이트 렌더러 활용 변수
    WaitForFixedUpdate wait; // Fixed업뎃이 일어나기 전까지 기다리는 시간 변수

    // 변수 초기화
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    // 프레임 종료 시 작동하는 함수
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }

        // 죽어있으면, 바로 종료
        // +260212 : 넉백 상태 고려 OR연산자 추가 -> 현재 애니메이션 상태를 불러옴 인자 : 상태 애니메이션 레이어 인덱스넘버
        // Get..Info()에 IsName을 추가 호출해서 해당 상태의 이름이 지정된 행동과 같은지 확인하기 -> boolean
        // 이렇게 되면, 잠깐 맞을 때 넉백 발생
        if ((!isLive) || (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")))
        {
            return;
        }

        // 추적 로직 구현
        // 위치 차이 = 타겟 위치 - 내 위치
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // 움직일 위치 : 방향 단위벡터 * 속도 * 모든 프레임에서 동일 속도
        
        // 플레이어 키 입력값을 더한 이동 = 몬스터의 방향 값을 더한 이동
        rigid.MovePosition(rigid.position + nextVec); // 움직이기 (순간이동 식)
        rigid.linearVelocity = Vector2.zero; // 리지드의 물리속도 영향을 없애주기 위한 고정값 // zero : 영벡터
    }

    // 프레임이 종료 되기 전 실행되는 함수
    void LateUpdate()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }
        
        // 죽어있으면, 바로 종료
        if (!isLive)
        {
            return;
        }

        // 타겟의 포지션이 나보다 작으면 (왼쪽에 있으면) 스프라이트 뒤집기
        spriter.flipX = target.position.x < rigid.position.x;
    }

    // 스크립트가 활성화 될 때 작동되는 함수
    // 적들을 재활용하기 위한 함수
    void OnEnable() 
    {
        // 타겟 초기화 : 게임매니저의 인스턴스 플레이어의 리지드바디2D 형식의 컴포넌트
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true; // 생존 여부 초기화
        health = maxHealth; // 풀링 할 때마다 최대체력으로 초기화

        isLive = true; // 부활
        coll.enabled = true; // 충돌변수 true
        rigid.simulated = true; // 시뮬레이터 true

        spriter.sortingOrder = 2; // 스프라이트의 레이어 순서 2로 높이기
    }

    // 스포너 인스펙터의 데이터(체력, 속도, 스프라이트 등)를 받아올 함수
    // enemy의 상태를 받아오는 것
    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType]; // 스프라이트 데이터 받아오기
        speed = data.speed; // 속도 받아오기
        maxHealth = data.health; // 최대 체력 받아오기
        health = data.health; // 현재 체력 받아오기
    }

    // 적이 탄환과 충돌할 때
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌 오브젝트가 탄환이 아니면,
        // +260212 : 살아있는 것이 아니면 조건 추가, 경험치를 시체에서 얻으면 안돼기 때문
        if (!collision.CompareTag("Bullet") || !isLive)
        {
            return; // 종료
        }
        
        health -= collision.GetComponent<Bullet>().damage; // 탄환에 피격될 때 데미지를 받음
        StartCoroutine(KnockBack()); // 코루틴 함수 호출 방법 : StartCoroutine() 안에 코루틴 함수 호출 넉백함수를 "KnockBack"으로 써도 무방함.

        if(health > 0) // 살아 있음
        {
            // 피격 시,
            anim.SetTrigger("Hit"); // 애니메이션 발동
            
            // 적 피격 효과음 재생
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else // 사망 시,
        {
            isLive = false; // 죽음
            coll.enabled = false; // 충돌변수 false
            rigid.simulated = false; // 시뮬레이터 false;

            spriter.sortingOrder = 1; // 스프라이트의 레이어 순서 1로 낮추기

            // 죽을 때 나오는 애니메이션 재생
            anim.SetBool("Dead", true);

            // 죽을 때 발생하는 경험치 정보
            GameManager.instance.kill++; // 킬 수 증가
            GameManager.instance.GetExp(); // 경험치 증가

            // 적 사망 효과음 재생 -> 단, 적이 살아있을 경우만. (게임 종료 시, 적 몰살되는 문제 때문)
            if(GameManager.instance.isLive) { AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); }
        }
    }

    // Coroutine : 생명주기와 비동기처럼 실행되는 함수
    // I : 코루틴의 변환형 인터페이스
    // 넉백 구현
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이 
        // 충돌 직후, 적이 받은 데미지 연산이 끝난 다음에야 해당 연산을 진행함 -> 적이 받은 데미지 연산이 꼬이지 않게끔 고려

        Vector3 playerPos = GameManager.instance.player.transform.position; // 플레이어 위치
        Vector3 dirVec = transform.position - playerPos; // 플레이어의 반대 방향 벡터

        // 넉백 발생 
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // 인자 : 방향과 크기 (임시 3), 즉발적인 힘(Impulse) 속성
    }

    // 적 사망 시 발동 함수
    void Dead()
    {

        gameObject.SetActive(false); // 오브젝트 비활성화
    }
}
