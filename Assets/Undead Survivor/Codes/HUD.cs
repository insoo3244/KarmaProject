using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    [Header("# Karma UI")]
    public GameObject[] expSlots; // 구슬 부모 오브젝트 6개 (ExpSlot_0 ~ 5)
    public Image[] expFills;      // 구슬 내부 하얀 Fill 이미지 6개

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
                // 1. 현재 경험치 수치와 다음 레벨 필요 경험치(GetMaxExp) 가져오기
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.GetMaxExp();

                // 2. 캐릭터 ID에 맞는 구체 노출 개수 (수도승:3, 생존자:4, 사냥꾼:6)
                int characterId = GameManager.instance.playerId;
                int orbCount = GameManager.instance.maxOrbPerCharacter[Mathf.Min(characterId, GameManager.instance.maxOrbPerCharacter.Length - 1)];

                // 3. 현재 레벨 승급 에너지의 총량 비율 (0.0 ~ 1.0)
                float expRatio = Mathf.Clamp01(curExp / maxExp);

                // 4. 비율을 활성화된 구체 개수 비례로 펼치기 (0.0 ~ orbCount)
                float totalFill = expRatio * orbCount;

                // 5. 구체 6개 순회하며 켜고 끄기 및 게이지 채우기
                for (int i = 0; i < expSlots.Length; i++)
                {
                    if (i < orbCount)
                    {
                        expSlots[i].SetActive(true);

                        // 각 구체별로 0.0 ~ 1.0 값 수직 게이지 적용
                        expFills[i].fillAmount = Mathf.Clamp01(totalFill - i);
                    }
                    else
                    {
                        // 그 캐릭터가 안 쓰는 나머지 구체는 숨김 처리
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