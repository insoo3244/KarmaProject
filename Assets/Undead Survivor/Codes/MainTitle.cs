using UnityEngine;

public class MainTitle : MonoBehaviour
{
    public GameObject mainTitle;
    public GameObject CharacterSelect;

    public void GameStart()
    {
        // 1. 메인 타이틀을 끈다
        mainTitle.SetActive(false);
        
        // 2. 캐릭터 선택창을 켠다
        CharacterSelect.SetActive(true);
    }
}
