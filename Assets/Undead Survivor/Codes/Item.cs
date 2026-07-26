using UnityEngine;
using UnityEngine.UI;

// 아이템 스클비트
public class Item : MonoBehaviour
{
    public ItemData data; // 아이템 데이터
    public int level; // 아이템 레벨
    public Weapon weapon; // 무기 속성 가져오기
    public Gear gear; // 장비 속성 가져오기

    Image icon; // 아이콘
    Text textLevel; // 레벨 텍스트
    Text textName; // 템 이름 텍스트
    Text textDesc; // 템 설명 텍스트

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1]; // 자식 아이콘 중 자기 자신을 제외한 두 번째를 가져옴
        icon.sprite = data.itemIcon; // ItemData.itemIcon 가져오기

        Text[] texts = GetComponentsInChildren<Text>(); // 모든 자식을 가져옴

        // 자식 순서 대로임
        textLevel = texts[0]; // 무조건 0 번째 텍스트임
        textName = texts[1]; // 무조건 1 번째 텍스트임 
        textDesc = texts[2]; // 무조건 2 번째 텍스트임 

        textName.text = data.itemName; // 초기화

    }

    // 스크립트 활성화 시 작동되는 함수
    void OnEnable()
    {
        // 레벨 텍스트 갱신
        textLevel.text = "LV." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                // 템 설명 {0}, {1} 자리에 들어갈 데미지와 관통력 텍스트 넣기 (+ 데미지 증가량 백분율 표기를 위해 * 100)
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                // 템 설명 {0}, {1} 자리에 들어갈 텍스트 넣기 (+ 백분율 표기를 위해 * 100)
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default: // 회복 약 텍스트
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

        
    }

    // 누를 시 작동되는 함수
    public void Onclick()
    {
        // 아이템 타입 별 반응
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if(level == 0)
                {
                    // 새로운 게임오브젝트 생성 및 웨폰 컴퍼넌트 추가
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>(); // <- 반환 값 weapon 자료형 변수
                    weapon.Init(data); // 무기 데이터 초기화
                }
                else
                {
                    // 다음 데미지, 개수(관통력) 설정
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    // 레벨에 따른 데미지, 개수(관통력) 증가
                    nextDamage += data.baseDamage * data.damages[level]; // damages에 저장된 값들은 데미지 배율이라, 기본 데미지와 곱해주는 것
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                
                level++; // 레벨업
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0)
                {
                    // 새로운 게임오브젝트 생성 및 기어 컴퍼넌트 추가
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>(); // <- 반환 값 Gear 자료형 변수
                    gear.Init(data); // 무기 데이터 초기화
                }
                else
                {   
                    // 다음 수치 설정
                    float nextRate = data.damages[level];

                    // 레벨업을 통해 다음 수치 적용
                    gear.LevelUp(nextRate);
                }

                level++;
                break; // 레벨업
            case ItemData.ItemType.Heal:
                // 체력 회복 : 1회성 아이템
                // 횟수 제한 X 
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
            default:
                break;
        }

        // level이 ItemData.damages 의 길이(최대 레벨에 도달했을 때,) 
        if(level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false; // 버튼 상호작용 비활성화
        }
    }
}
