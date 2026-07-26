using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData; // 스크립트 내에서 선언한 스폰데이터(코드 맨 밑에 있음)를 변수로써 선언
    // 자식 오브젝트의 트랜스폼을 담을 배열 변수
    public Transform[] spawnPoint;
    public float levelTime; // 레벨구간 결정 변수


    int level; // 소환 수준을 나눌 레벨 변수
    float timer; // 소환 타이머를 위한 변수

    // 변수 초기화
    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); // 자기 자신을 포함하는 자식 컴퍼넌트 모두 담기

        // +260626 : levelTime 초기화 추가 : 최대 시간을 몬스터 데이터 크기로 나누어서 자동으로 구간 계산
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }
        
       timer += Time.deltaTime; // 한 프레임 당 시간(deltaTime) 계속 더하기
       
       // +) 260626 : 레벨 구간을 levelTime으로 나누어서 결정
       // 게임 시간을 10으로 나누어 수준 설정. FloorToInt로 소수점 버리기(내림) 올림은 CeilToInt
       // -> Min 함수로 인덱스 에러 해결 (레벨 변수가 인덱스의 범위를 벗어날 경우, Min()으로 강제 전환 (spawnData의 길이는 현재 2))
       level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1); 

        if(timer > spawnData[level].spawnTime) // 타이머 1프레임 당 다음 실행 -> level 수준에 따라 실행(Spawner.spawnData[]의 요소를 결정)
        {
            timer = 0; // 다시 0초로 초기화
            Spawn(); // 스폰
        }
    }

    // 스폰 함수 
    void Spawn()
    {
        // 적의 종류를 랜덤으로 가져오는 Get 반환 값을 받는 enemy 변수 생성
        // -> 레벨 수준에 따라 적을 다르게 생성
        // -> 다시 0으로 바꿈 : enemy 프리펩을 한 개로 줄였기 때문에, 하나만 가져와서 Enemy.animCon 에서 조정
        GameObject enemy = GameManager.instance.pool.Get(0); 

        // 자기 자신을 제외하기 위해, 1 부터 시작하는 난수 대입
        enemy.transform.position = spawnPoint[UnityEngine.Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]); // Enemy.Init() 으로 enemy의 상태 초기화
    }
}

// 직렬화 진행 : 개체를 전송하기 위함 -> 이걸로 인스펙터 창에서 보이지 않던 옵션을 조작할 수 있음
[System.Serializable]
public class SpawnData
{
    public float spawnTime; // 소환 시간
    public int spriteType; // 스프라이트 타입 ex) 0: 해골, 1: 좀비 ...
    public int health; // 몬스터의 체력
    public float speed; // 몬스터의 속도
}