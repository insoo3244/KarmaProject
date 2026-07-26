using UnityEngine;
using UnityEngine.UI;

// HUD 제작 스크립트

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    // 열거형 자료형 enum // 세미콜론; 도 필요없음
    // 인스펙터에 슬롯을 만들어서 관리 가능
    
    public InfoType type; // 따로 선언해주기

    // 텍스트와 슬라이더
    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    // UI에 필요한 요소들 switch 문으로 관리
    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp: // 경험치
                // 슬라이더에 나타낼 값 : 현재 경험치 / 최대 경험치
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                // +260222 : 현재 레벨과, 경험치 통 길이(10) 둘 중 최솟값을 가져오기 : 최고 경험치 재활용 기능 추가
                // 10레벨을 넘기면 인덱스 범위 오류 방지

                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level: // 레벨
                // 레벨 텍스트 : Lv.{순번:형태(소수점 없음)}
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill: // 킬 수
                // 킬 수 텍스트 : {순번:형태(소수점 없음)}
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time: // 시간
                // 남은 시간 표현 : 최대 시간 - 현재 시간
                // 분과 초로 나누어서 표현하기
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);

                // 무조건 두 자리수 고정이니, D 사용. {0:D2} 하면 두 자리수 고정
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health: // 체력
                // 슬라이더에 나타낼 값 : 현재 체력 / 최대 체력
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;

                mySlider.value = curHealth / maxHealth;
                
                break;
            default:
                break;
        }
    }
}
