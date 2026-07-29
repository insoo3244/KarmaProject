using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec; // 방향 입력
    public float speed; // 속도 제어
    public Scanner scanner; // 스캔 변수
    public Hand[] hands; // 양 손 관리
    public RuntimeAnimatorController[] animCon; // 플레이어 애니메이션 관리

    SpriteRenderer spriter; // 스프라이트 불러오기

    Animator anim; // 애니메이션 제어

    Rigidbody2D rigid; // 캐릭터 객체

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // 게임이 시작될 때 한 번 호출되는 함수
    void Awake() // 변수 초기화 목록
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();

        spriter.flipX = true; // 스프라이트 좌/우 찐빠로 인해.. 평소에 flipX를 켜두고있기

        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true); // true 로 인해서, 비활성화 된 오브젝트도 인식
    }

    void OnEnable()
    {
        speed *= Character.Speed; // 이동속도 할당
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId]; // 애니메이션 할당
    }

    // Update is called once per frame
    // 하나의 프레임마다 호출되는 함수
    void Update()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }

        // inputVec.x = Input.GetAxisRaw("Horizontal");
        // inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }

        // // 1. 힘을 준다
        // rigid.AddForce(inputVec);

        // // 2. 속도 제어
        // rigid.velocity = inputVec;

        // 3. 위치 이동
        // 대각선 이동 시에도, 평균적인 속도를 내기 위함
        // 평균화 * 속도 * 물리 프레임 하나가 소비한 시간
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        // 내가 움직일 방향 + nextVec 만큼 움직이기
        rigid.MovePosition(rigid.position + nextVec);
    }

    // InputVector 를 이용한 움직임 구현
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>(); // normalize를 패키지에서 설정했으니, 따로 구현 X
    }

    // 프레임이 종료 되기 전 실행되는 함수
    void LateUpdate()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }
        
        // 속도에 따라 달리는 애니메이션 재생 (anim과 inputVec 연결)
        anim.SetFloat("Speed", inputVec.magnitude); // Magnitude : 벡터의 크기 값만 반환

        if (inputVec.x != 0){ // x축 움직임이 0이 아닐 때
            spriter.flipX = inputVec.x > 0; // 우로 가는 경우 반전
        }
    }

    // 플레이어 피격 구현 함수
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive) // 죽어있으면 종료
        {
            return;
        }

        GameManager.instance.health -= Time.deltaTime * 10; // 틱당 10씩 감소

        // 플레이어의 일부 속성 비활성화 시키기
        // Spawner, HandLeft, HandRight 비활성화
        if (GameManager.instance.health < 0)
        {
            // Spawner 인덱스부터 시작 ~ 자식 오브젝트 개수 가져오기 childCount
            for(int index = 2; index < transform.childCount; index++)
            {
                // 인자에 해당하는 번호의 자식 가져오기 -> transform이 반환되므로, gameObject로 이어주기 -> 비활성화
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead"); // 죽음 애니메이션 실행
            GameManager.instance.GameOver(); // 게임 패배
        }
    }
}
