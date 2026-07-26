using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리펩들을 보관할 변수
    public GameObject[] prefabs;

    // 풀 담당을 하는 리스트들
    List<GameObject>[] pools;
    // 위 둘(프리펩 보관 변수 & 풀 담당 리스트)은 개수 비율이 같아야 함

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length]; // pool 초기화 : 프리펩과 크기가 똑같도록 동적?할당

        // 풀 리스트 전부 초기화
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }
    
    // 풀링 함수 작성 (몬스터 무한 생성)
    
    // 오브젝트를 반환 하는 함수
    public GameObject Get(int index) 
    {
        GameObject select = null; // 비어있는 게임 오브젝트

        // 선택한 풀의 놀고 있는(비활성화 된) 게임 오브젝트 접근
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf) // 비활성화 되어있나 ?
            {   
                // 발견하면 select에 할당
                select = item;
                select.SetActive(true); // 활성화
                break;
            }
        }
        // 못 찾으면 ?
        if(select == null) // (!select 와 동일한 표현)
        {
            // 새롭게 생성하고 select에 할당
            // 임시 생성 함수 인자 : (자료형:오브젝트, 자기 자신(풀 매니저 폴더 안에 프리펩 정리))
            select = Instantiate(prefabs[index], transform); 

            // 생성된 오브젝트를 pools에 추가 (Add 함수)
            pools[index].Add(select);
        }

        return select; // 오브젝트 반환
    }
}
