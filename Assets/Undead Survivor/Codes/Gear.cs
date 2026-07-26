using UnityEngine;

// 장비 스크립트 (무기 아님)
public class Gear : MonoBehaviour
{
    public ItemData.ItemType type; // 아이템 목록 가져오기
    public float rate; // 장비 타입과 수치 변수

    // 장비 데이터 초기화
    public void Init(ItemData data)
    {
        // Basic Set
        name = "Gear " + data.itemId; // 장비 이름짓기
        transform.parent = GameManager.instance.player.transform; // 위치 초기화
        transform.localPosition = Vector3.zero; // 지역 위치 0, 0, 0 으로 초기화

        // Property Set
        type = data.itemType; // 아이템 타입 가져오기
        rate = data.damages[0]; // 장비 수치 가져오기

        ApplyGear(); // 장비를 얻을 때, 장비에 맞는 효과 업그레이드
    }

    // 레벨업 함수
    public void LevelUp(float rate)
    {
        this.rate = rate; // rate 초기화
        ApplyGear(); // 레벨업 하면서 로직을 적용시킬 용도로 호출
    }

    // 장비 기능 호출 (RateUP, SpeedUp)
    void ApplyGear()
    {
        switch (type) // 타입 받아와서 각 장비에 맞는 업그레이드 함수 호출
        {  
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    // 장비 특수효과 구현. 장갑 : 연사력 올리기
    void RateUp()
    {
        // 무기 가져오기
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        // 가져온 무기들을 순회하면서 업그레이드
        foreach(Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0: // 회전 삽 업그레이드
                    float speed = 150 * Character.WeaponSpeed; // +) 260303 : 캐릭터 기본 스텟을 반영
                    weapon.speed = speed + (speed * rate);
                    break;
                default: // 나머지 무기 업그레이드
                    speed = 0.5f * Character.WeaponRate; // +) 260303 : 캐릭터 기본 스텟을 반영
                    weapon.speed = speed * (1f - rate);
                    break;
            }
        }
    }


    // 장비 특수효과 구현. 신발 : 이동속도 올리기
    void SpeedUp()
    {
        float speed = 3 * Character.Speed; // 기본 이동속도 +) 260303 : 캐릭터 기본 이동속도에 곱해주기
        GameManager.instance.player.speed = speed + speed * rate; // 이동속도 업그레이드
    }
}
