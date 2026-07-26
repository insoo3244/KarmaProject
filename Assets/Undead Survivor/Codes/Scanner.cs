using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적을 스캔하기 위한 클래스
public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위 변수
    public LayerMask targetLayer; // 타겟 레이어 변수
    public RaycastHit2D[] targets; // 다수 타겟 리스트
    public Transform nearestTarget; // 가장 가까운 타겟


    void FixedUpdate()
    {   
        // 원형의 캐스트를 쏘고 모든 결과를 반환하는 함수 CircleCastAll()
        // 인자 : 캐스팅 시작 위치, 원의 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest(); // 가장 가까이 있는 적 찾기 (아래 함수 참고)
    }

    Transform GetNearest()
    {
        Transform result = null; // 결과 반환
        float diff = 100; // 정말 큰 거리

        // 가장 가까이 있는 적 찾기
        foreach (RaycastHit2D target in targets) // 탄환에 맞은 적들 리스트 내 탐색
        {
            Vector3 myPos = transform.position; // 플레이어 위치
            Vector3 targetPos = target.transform.position; // 타겟의 위치
            float curDiff = Vector3.Distance(myPos, targetPos); // 서로 간의 거리

            if (curDiff < diff) // 만약 거리가 내가 정한 거리보다 작으면
            {
                diff = curDiff; // 초기화
                result = target.transform; // 반환 위치는 타겟의 위치
            }
        }

        return result;
    }
    
}
