using UnityEngine;

// 아이템 관리 생성 스크립트
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")] // 커스텀 에셋메뉴 생성
public class ItemData : ScriptableObject
{
    // 아이템 목록
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    public ItemType itemType; // 아이템 타입 enum 변수
    public int itemId; // 아이템 Id
    public string itemName; // 아이템 이름
    [TextArea] // 텍스트를 넣을 공간을 넓힘 -> 줄바꿈 가능
    public string itemDesc; // 아이템 설명
    public Sprite itemIcon; // 아이템 아이콘
    
    [Header("# Level Data")]
    public float baseDamage; // 0lv 기본 데미지
    public int baseCount; // 0lv 기본 개수 & 관통
    public float[] damages; // 레벨 별 데미지
    public int[] counts; // 레벨 별 개수 & 관통

    [Header("# Weapon")]
    public GameObject projectile; // 무기, 투사체 프리펩들 관리
    public Sprite hand; // 손 & 무기 관리
}
