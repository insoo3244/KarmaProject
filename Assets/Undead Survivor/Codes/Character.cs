using UnityEngine;

// 캐릭터 별 특징 구현
public class Character : MonoBehaviour
{
    // 속성 선언 { } 붙이기 : 똑같이 클래스의 점 연산자로 이어주면 됨
    public static float Speed
    {
        get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; } // 캐릭터 0은 이동속도 증가
    }
    public static float WeaponSpeed
    {
        get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; } // 근거리 무기 : 캐릭터 1은 회전 속도 증가
    }

    public static float WeaponRate
    {
        get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; } // 원거리 무기 : 캐릭터 1을 위한 연사속도 증가
    }

    public static float Damage
    {
        get { return GameManager.instance.playerId == 2 ? 1.2f : 1f; } // 캐릭터 2는 데미지 증가
    }

    public static int Count
    {
        get { return GameManager.instance.playerId == 3 ? 1 : 0; } // 캐릭터 3은 (근거리 : 무기 개수 / 원거리 : 관통력) 증가
    }
}
