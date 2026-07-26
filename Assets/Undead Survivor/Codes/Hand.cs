using System.Collections.Generic;
using UnityEngine;

// 손 & 무기 구현

public class Hand : MonoBehaviour
{
    public bool isLeft; // 왼손 오른손 구분 (인스펙터에서 왼손 설정)
    public SpriteRenderer spriter; // 스프라이트렌더러 변수

    SpriteRenderer player; // 플레이어 스프라이트렌더러 변수 선언

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0); // 오른손 위치
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0); // 반전 오른손

    Quaternion leftRot = Quaternion.Euler(0, 0, -35); // 왼손 위치 (오일러함수 : 인자 x, y, z 축)
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135); // 반전 왼손

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1]; // 자기 자신을 제외한 부모의 스프라이터 가져오기
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX; // 플레이어 방향이 반대방향인가?

        if (isLeft) // 근접무기 (왼손)
        {
            // 지역 위치(4원수)는 = 반대 방향 ? 반전 왼손 : 그냥 왼손
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;

            spriter.sortingOrder = isReverse ? 4 : 6; // 레이어 우선순위 변경 (왼손, 오른손을 서로 바꾸기)
        }
        else // 원거리무기 (오른손)
        {
            // 지역 위치(벡터)는 = 반대 방향 ? 반전 오른손 : 그냥 오른손
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;

            spriter.sortingOrder = isReverse ? 6 : 4; // 레이어 우선순위 변경 (왼손, 오른손을 서로 바꾸기)
        }
    }
}
