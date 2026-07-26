using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll; // 적 재생성 로직을 위한 충돌 변수

    void Awake()
    {
        coll = GetComponent<Collider2D>(); // 충돌 변수 초기화
    }

    // 트리거가 체크된 콜라이더에서 나갈 때를 위한 함수
    void OnTriggerExit2D(Collider2D collision) // 충돌 변수 collision
    {
        if (!collision.CompareTag("Area")){ // area와의 충돌에서 벗어나지 않았다
            return; // 그대로 종료
        }

        // 만약 벗어났다면 ? 재배치 로직 가동

        Vector3 playerPos = GameManager.instance.player.transform.position; // 플레이어 위치
        Vector3 myPos = transform.position; // 내 위치 (관찰자)

        // +) 260626 : 무한맵 재배치 로직 보완 (삭제)
        // // 나와 캐릭터 사이의 x축, y축 거리 구하기. 절댓값 함수 Mathf.Abs()
        // float diffX = Mathf.Abs(playerPos.x - myPos.x); 
        // float diffY = Mathf.Abs(playerPos.y - myPos.y);
        // // 플레이어 이동 방향 playerDir. 0 보다 작으면 음수 표시
        // Vector3 playerDir = GameManager.instance.player.inputVec;
        // float dirX = playerDir.x < 0 ? -1 : 1; 
        // float dirY = playerDir.y < 0 ? -1 : 1;

        // 태그에 따른 재생성 효과 분기점
        switch (transform.tag)
        {
            case "Ground": // 땅 재배치 로직

                // 두 오브젝트의 위치 차이를 활용한 로직 (유저와 타일맵 간의 상대위치)
                float diffX = playerPos.x - myPos.x; 
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1; 
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY) // 만약 Y축보다 X축 차이가 더 많이 난다면,
                {
                    // 위치를 옮김 transform // 인자 : 단위벡터 * 방향 * 크기(다음 재배치 할 맵을 위한 이동)
                    transform.Translate(Vector3.right * dirX * 40); // Vector3 right : 단위벡터 (1,0,0)
                }
                else if (diffY > diffX) // 만약 X축보다 Y축 차이가 더 많이 난다면,
                {
                    // Translate(인자) : 인자 만큼 이동하는 함수
                    transform.Translate(Vector3.up * dirY * 40); // Vector3 up : 단위벡터 (0,1,0)
                }
                break;

            case "Enemy":
                if (coll.enabled)
                {
                    // 몬스터와 유저 간 위치 차이 (상대위치)
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0); // 등장 방향 랜덤성 부여
                    
                    // +) 260626 : 플레이어 방향을 무시하고 상대위치로 계산
                    // 플레이어의 이동 방향에 따라 맞은 편에서 등장하도록 하기(플레이어 방향 * 거리 20 + 랜덤 위치)
                    transform.Translate(ran + dist * 2); 
                }
                break;

            default:
                break;
        }
    }
}
