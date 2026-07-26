using UnityEngine;

// 게임 결과화면 스크립트
public class Result : MonoBehaviour
{
    public GameObject[] titles; // 활성화 할 화면 리스트
    // 인스펙터에서 조절해야 함

    // 게임 패배
    public void Lose() 
    {
        titles[0].SetActive(true);
    }

    // 게임 승리
    public void Win()
    {
        titles[1].SetActive(true);
    }
}
