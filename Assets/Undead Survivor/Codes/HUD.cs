using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    // 새로 추가할 배열 : 인스펙터에서 6개의 구슬을 직접 연결해줄 변수
    [Header("# Karma UI")]
    public GameObject[] expSlots; // 구슬의 부모 오브젝트 (켜고 끄기 용도)
    public Image[] expFills;      // Fill

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                // GameManager에서 현재 경험치와 현재 캐릭터의 최대 경험치 가져오기
                float curExp = GameManager.instance.exp;
                int maxExp = GameManager.instance.maxExpPerCharacter[Mathf.Min(GameManager.instance.playerId, GameManager.instance.maxExpPerCharacter.Length - 1)];

                // 💡 6개의 구슬을 순회하면서 끄고 켜기 + 게이지 채우기
                for (int i = 0; i < expSlots.Length; i++)
                {
                    // 1. 캐릭터의 최대 카르마 개수(maxExp) 안쪽에 있는 슬롯만 켜기
                    if (i < maxExp)
                    {
                        expSlots[i].SetActive(true);

                        // 2. 물 차오르듯이 게이지 채우기 로직
                        // Mathf.Clamp01 : 계산값을 무조건 0~1 사이로 고정하는 함수
                        // (curExp - i)를 하면 0번 구슬부터 차례대로 1(꽉참)이 되고, 남은 소수점만큼 다음 구슬이 차오름
                        expFills[i].fillAmount = Mathf.Clamp01(curExp - i);
                    }
                    else
                    {
                        // 안 쓰는 잉여 슬롯은 화면에서 숨기기
                        expSlots[i].SetActive(false); 
                    }
                }
                break;
                
            case InfoType.Level: 
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill: 
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health: 
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            default:
                break;
        }
    }
}