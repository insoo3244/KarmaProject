using UnityEngine;

// 레벨업 스크립트
public class LevelUp : MonoBehaviour
{
    RectTransform rect; // UI 위치 변수
    Item[] items; // 아이템 변수

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true); // 비활성화 된 오브젝트 포함하기
    }

    // 보이기 함수
    public void Show()
    {
        Next(); // 아이템 뽑기
        rect.localScale = Vector3.one; // one : (1, 1, 1)
        GameManager.instance.Stop(); // 시간 멈추기

        // 레벨업 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        // 오디오 필터 적용 배경음악 재생
        AudioManager.instance.EffectBgm(true);
    }

    // 숨기기 함수
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume(); // 시간 재생

        // 스킬 선택 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        // 오디오 필터 적용 배경음악 재생 종료
        AudioManager.instance.EffectBgm(false);
    }

    // 버튼 누르기 함수
    public void Select(int index)
    {
        items[index].Onclick();
    }

    // 랜덤으로 아이템 뽑기 함수
    void Next()
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 그중에서 랜덤 3개 아이템만 활성화
        int[] ran = new int[3]; // 난수 변수 선언

        // 2 - 1. 난수 점검
        while (true)
        {   
            // 난수 범위 : 0 부터 아이템 배열 길이 만큼
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            // 모두가 같지 않을 때 실행하기
            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
            {
                break;
            }
        }

        // 2 - 2. 랜덤 아이템 활성화
        for(int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]]; 
            
            // 3. 최고레벨 아이템은 소비아이템으로 대체 (더 이상 레벨업이벤트에 등장 X)
            if(ranItem.level == ranItem.data.damages.Length) // 데미지 배열 길이(최고 레벨)과 같으면,
            {
                items[4].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);    
            }
        }

        
    }
}
