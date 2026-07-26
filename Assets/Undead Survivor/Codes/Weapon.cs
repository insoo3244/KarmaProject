using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

// 플레이어가 가진 무기들을 관리하는 곳
public class Weapon : MonoBehaviour
{
    public int id; // 무기의 ID
    public int prefabId; // 무기 프리펩 ID
    public float damage; // 무기 데미지
    public int count; // 무기 개수
    public float speed; // 무기 속도

    float timer; // 탄환 시간
    Player player; // 플레이어 변수

    void Awake()
    {
        player = GameManager.instance.player; // 부모 컴포넌트 받아오기
        //+260214 : 이제 Weapon이 player의 자식이 아님. player를 게임 매니저의 player로 가져오기
    }

    // +260214 Start() 함수 삭제 : Init()을 굳이 여기서 초기화하지 않아도 됨

    // Update is called once per frame
    // 무기 프레임 당 변화 조절
    void Update()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }
        
        switch (id)
        {
            case 0: // 회전 삽
                // 회전 구현하기 -> Rotate() 함수
                // 단위 벡터(0,0,-1) (-1인 이유는 Init()에서 회전 속도를 양수로 설정했기 때문) * 속도 * 프레임 당 시간
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:
                timer += Time.deltaTime; // 탄환이 날아가는 시간 구현

                if (timer > speed) // 이 시간이 속도보다 크다면
                {
                    timer = 0f; // 0초로 초기화
                    Fire();
                }
                break;
        }

        // Test code
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    // 레벨에 따른 무기 배치
    public void LevelUp(float damage, int count)
    {
        // 데미지와 개수 업그레이드
        this.damage = damage * Character.Damage; // 배율(곱연산) // +) 260303 : 캐릭터 기본 스텟을 반영
        this.count += count; // 개수(합연산)

        if(id == 0)
        {
            Batch();
        }

        // BroadcastMessage : 특정 함수 호출을 모든 자식에게 방송하는 함수
        // -> player의 ApplyGear 함수가 있는 모든 자식들에게 호출을 명령함
        // 레벨업 -> 장비 획득 -> 레벨업을 할 시, 장비의 업그레이드가 먹히지 않을 수 있음.
        // 이를 위해서 무기를 초기화하고 레벨업할 때 마다 이 함수를 호출
        // 두 번째 인자는 기어 없이, 무기를 레벨업, 초기화 할 경우 전달해줄 자식이 없는 경우를 대비한 것
        // 전달해줄 자식이 없으면 전달하지 않아도 된다는 것을 의미함
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        
    }

    // 무기 ID에 따른 변수 초기화
    public void Init(ItemData data)
    {
        // Basic set (기본 세팅)
        name = "Weapon " + data.itemId; // 무기 이름짓기
        transform.parent = player.transform; // 위치 초기화
        transform.localPosition = Vector3.zero; // 지역 위치를 0, 0, 0 으로 초기화

        // Property set (고유 세팅)
        id = data.itemId; // id 가져오기
        damage = data.baseDamage * Character.Damage; // 데미지 // +) 260303 : 캐릭터 기본 스텟을 반영
        count = data.baseCount + Character.Count; // 개수(관통력) 가져오기 // +) 260303 : 캐릭터 기본 스텟을 반영

        // 프리펩 배열 훑기
        // 스크립트블 오브젝트의 독립성을 위해 인덱스가 아닌 프리펩으로 설정해주기
        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            // 프리펩 아이디는 pool매니저의 변수에서 찾기
            if(data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0: // 회전 삽
                speed = 150 * Character.WeaponSpeed; // 회전속도 // +) 260303 : 캐릭터 기본 스텟을 반영
                Batch(); // 무기 배치

                break;

            default:
                speed = 0.3f * Character.WeaponRate; // 공격 속도는 0.3초 마다 . . // +) 260303 : 캐릭터 기본 스텟을 반영
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType]; // enum은 배열처럼 인덱싱 되있음. int형으로 형변환하여 활용하기
        hand.spriter.sprite = data.hand; // 스프라이트 적용
        hand.gameObject.SetActive(true); // 왼손, 오른손 활성화

        // BroadcastMessage : 특정 함수 호출을 모든 자식에게 방송하는 함수
        // -> player의 ApplyGear 함수가 있는 모든 자식들에게 호출을 명령함
        // 레벨업 -> 장비 획득 -> 레벨업을 할 시, 장비의 업그레이드가 먹히지 않을 수 있음.
        // 이를 위해서 무기를 초기화하고 레벨업할 때 마다 이 함수를 호출
        // 두 번째 인자는 기어 없이, 무기를 레벨업, 초기화 할 경우 전달해줄 자식이 없는 경우를 대비한 것
        // 전달해줄 자식이 없으면 전달하지 않아도 된다는 것을 의미함
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 무기 배치 함수
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            // 탄환 지역변수 선언 -> 프리팹 아이디에 따라, 무기 배치
            Transform bullet;

            // 기존에 가지고 있던 무기 재활용, 개수가 부족하면 모자란 것 풀링 로직
            // 현재 인덱스가 지금 가지고 있는 자식 오브젝트(무기) 보다 크면
            // 즉, 내가 가지고 있는 무기가 있다면
            if(index < transform.childCount) // 자신의 자식 오브젝트 개수 가져오기 : childCount
            {
                bullet = transform.GetChild(index); // 기존에 가지고 있던 무기 재활용하기
            }
            else // 없다면
            {
                // 새로 가져오기
                bullet = GameManager.instance.pool.Get(prefabId).transform;
            }
            bullet.parent = transform; // 부모 클래스 변경 : PoolManager -> Weapon 0

            // 위치, 회전 초기화
            bullet.localPosition = Vector3.zero; // 0 벡터 : 플레이어 위치
            bullet.localRotation = Quaternion.identity; // 4원수 : Quaternion (4차원 벡터), 초기값 (전부 0) : identity


            // 회전 벡터 (삽 개수에 따라서 나뉘어짐)
            Vector3 rotVec = Vector3.forward * 360 * index / count;

            bullet.Rotate(rotVec); // 회전 벡터 적용
            bullet.Translate(bullet.up * 1.5f, Space.World); // 위치 이동 (위 방향으로 1.5만큼, 월드 기준으로)

            
            // 데미지 관통력 결정
            // +) 260626 : 근거리 무기는 관통력을 -100으로 재설정
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // 인자 : 데미지, 관통력(-100은 무한 관통)
            // +260211 추가 : Bullet.Init() 함수 인자 업데이트로 인해, 방향벡터 인자 추가
        }
    }

    // 탄환 발사 로직
    void Fire()
    {
        if (!player.scanner.nearestTarget) // 가장 가까운 타겟이 null 이면?
        {
            return; // 종료
        }

        Vector3 targetPos = player.scanner.nearestTarget.position; // 가장 가까운 적의 위치
        Vector3 dir = targetPos - transform.position; // 탄환이 날아갈 벡터 구하기
        dir = dir.normalized; // normalized(정규화) : 단위벡터로 만들어 버림

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform; // 탄환 위치 가져오기
        bullet.position = transform.position; // 탄환 위치는 내 위치에서 시작

        // 지정된 축을 중심으로 목표를 향해 회전하는 함수 FromToRotation
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 총알 회전

        // 탄환 속성 설정
        bullet.GetComponent<Bullet>().Init(damage, count, dir); // 인자 : 데미지, 관통력(-1은 무한 관통)

        // 원거리 무기 발사 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}