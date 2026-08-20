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
    public float[] deadDelay = { 1f, 1f, 1f }; // 캐릭터별 게임종료 시간지연
    public bool isDead = false; // 플레이어 생존 여부
    Vector2 deadPosition; // 사망 당시의 위치를 저장할 변수

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

        if (isDead)
        {
            // 죽었다면 키보드 입력은 무시하고, 아까 박제해 둔 그 사망 위치로 매 프레임 꽂아버림
            // 이렇게 하면 물리 엔진(Dynamic)은 살아있어서 무한 맵(Reposition)은 정상 작동하지만, 
            // 캐릭터는 단 1mm도 미끄러지지 않고 그 자리에 얼어붙음
            rigid.linearVelocity = Vector2.zero; 
            rigid.MovePosition(deadPosition);
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
        if(isDead) { // 죽었다면, 관성을 없애주고 입력 원천봉쇄
            inputVec = Vector2.zero; 
            return; 
        }
        inputVec = value.Get<Vector2>(); // normalize를 패키지에서 설정했으니, 따로 구현 X
    }

    // 프레임이 종료 되기 전 실행되는 함수
    void LateUpdate()
    {
        if (!GameManager.instance.isLive || isDead) // 게임이 멈춰있다면 실행 X
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

        GameManager.instance.health -= Time.deltaTime * 50; // 틱당 10씩 감소

        // 플레이어의 일부 속성 비활성화 시키기
        // Spawner, HandLeft, HandRight 비활성화
        if (GameManager.instance.health < 0 && !isDead)
        {
            isDead = true; // 플레이어 사망
            
            // 1. 남은 물리 가속도만 깔끔하게 0으로 브레이크
            rigid.linearVelocity = Vector2.zero; 
            deadPosition = rigid.position; // 딱 죽은 그 순간의 좌표를 박제!

            // 2. 무기 비활성화
            foreach(Hand hand in hands) 
            {
                hand.gameObject.SetActive(false);
            }

            // 3. 사망 애니메이션 및 게임 오버 타이머 실행
            anim.SetTrigger("Dead"); 
            StartCoroutine(DelayLose(deadDelay[GameManager.instance.playerId])); 
        }
    }
    
    // 슬러그캣 사망 모션 출력을 위한 화면종료 지연함수
    IEnumerator DelayLose(float delayTime) 
    {   
            yield return new WaitForSeconds(delayTime); 
            GameManager.instance.GameOver(); // 게임 패배
    }
}
