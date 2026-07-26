using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

// 도전과제 스크립트 (캐릭터 해금)
public class AchiveManager : MonoBehaviour
{
    // 해금 된 캐릭터와 되지 않은 캐릭터
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    
    public GameObject uiNotice; // 알림 변수

    // 감자농부, 콩농부 열거형 변수
    enum Achive{ UnlockPotato, UnlockBean }
    Achive[] achives; // 열거형 변수 저장 배열
    WaitForSecondsRealtime wait; // 멈추지 않는 실제 시간 변수 (5초로 초기화)

    void Awake()
    {
        // GetValues = enum의 모든 변수 가져오기 -> 반환 값 Array -> Achive[]로 형변환
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);

        // 키 값을 가지고 있으면 도전과제 초기화
        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    // 도전과제 초기화 함수
    void Init()
    {
        // PlayerPrefs : 간단한 저장 기능 (유니티 클래스)
        // SetInt(key값, 정수)
        PlayerPrefs.SetInt("MyData", 1);

        // 업적 데이터 초기화
        foreach(Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    void Start()
    {
        UnlockCharacter();
    }

    // 캐릭터 해금 함수
    void UnlockCharacter()
    {
        // 잠금 캐릭터 배열 순회
        for(int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; // 진짜 잠겨있나 ?

            lockCharacter[index].SetActive(!isUnlock); // 잠금 여부에 따라 (비)활성화
            unlockCharacter[index].SetActive(isUnlock); // 잠금 여부에 따라 (비)활성화
        }
    }

    // 프레임마다 도전과제 달성 체크
    void LateUpdate()
    {
        foreach(Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }

    // 도전과제 달성 체크 함수
    void CheckAchive(Achive achive)
    {   
        bool isAchive = false;

        switch (achive)
        {
            case Achive.UnlockPotato: // 킬 수 10 이상 달성 시, 감자농부 해금
                isAchive = GameManager.instance.kill >= 10;
                break;
            
            case Achive.UnlockBean: // 생존 성공 시, 콩농부 해금
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        // 도전과제 조건을 완수하고, 도전과제 달성 상태가 0일 때
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            //  -> 도전과제 달성 상태를 1로 바꿈
            PlayerPrefs.SetInt(achive.ToString(), 1);

            // uiNotice의 자식 개수 만큼 순회
            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                // 어떤 캐릭터가 해금되었는지 내용 결정
                bool isActive = index == (int)achive; // 도전과제 달성 유무 검사 변수
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive); // 자식 활성화
            }

            StartCoroutine(NoticeRoutine()); // 도전과제 알림 활성화
        }
    }

    // 알림 코루틴 함수 (5초 동안 활성화 되어있다가 꺼짐)
    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        
        // 업적 클리어 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotice.SetActive(false);
    }
}
