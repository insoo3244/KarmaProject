using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 메모리 얹기 ? 정적 변수로 선언하기
    // GameManager를 모든 메모리에서 언급이 가능해짐

    [Header("# Game Control")] // 인스펙터 카테고리 분류
    public bool isLive; // 게임 일시정지 여부
    public float gameTime; // 실제로 흐르는 게임 내부 시간
    public float maxGameTime = 2 * 10f; // 최대 게임 시간 할당

    // 적 처치 시 얻는 정보들
    [Header("# Player Info")] // 인스펙터 카테고리 분류
    // +) 260302 : 체력 변수들을 int -> float로 변경
    public int playerId; // 캐릭터 ID 저장
    public float health; // 현재 체력
    public float maxHealth = 100; // 최대 체력 
    public int level; // 레벨
    public int kill; // 킬 수
    public int exp; // 경험치
   public int[] maxExpPerCharacter = { 3, 4, 6 }; // 경험치 통 / 수도승, 생존자, 사냥꾼

    [Header("# GameObject")] // 인스펙터 카테고리 분류
    public PoolManager pool; // 풀 매니저
    public Player player; // 플레이어
    public LevelUp uiLevelUp; // 레벨업 창 
    public Result uiResult; // 게임 결과 창 (GameObject -> Result(Result 스크립트 작성 후 변경))
    public Transform uiJoy; // 조이스틱 설정 변수
    public GameObject enemyCleaner; // 적 청소기

    
    [Header("# HUD Active")]
    public GameObject uiCanvas; // UI 큰 오브젝트
    public GameObject hudCanvas; // hud UI

    void Awake()
    {
        instance = this; // 자기 자신으로 초기화

        // targetFrameRate : 게임 프레임률 지정
        Application.targetFrameRate= 60;
    }

    // +) 260302 : Start -> GameStart 이름 변경 후, public 속성 변경.
    // 게임 시작 버튼과 연동하기 위함
    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        // 캐릭터 선택이 끝나면, 플레이어 등장
        player.gameObject.SetActive(true);

        // 게임 시작 시, HUD 활성화
        if (hudCanvas != null)
        {
            uiCanvas.SetActive(true);
            hudCanvas.SetActive(true);
        }

        // temp
        uiLevelUp.Select(playerId % 2); // 현재 무기가 삽, 총 밖에 없어서 2로 나눈 나머지로 선택
        isLive = true;
        Resume(); // 게임 재생

        // 게임 시작 배경음악 재생
        AudioManager.instance.PlayBgm(true);

        // 캐릭터 선택 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 게임 패배
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    // 시간 지연을 위한 코루틴함수
    IEnumerator GameOverRoutine()
    {
        isLive = false; // 죽음

        yield return new WaitForSeconds(0.5f); // 시간 지연 (묘비 애니메이션 재생시간 고려)

        uiResult.gameObject.SetActive(true); // 게임 결과 화면 활성화
        uiResult.Lose(); // 패배 화면
        Stop(); // 정지

        // 게임 시작 배경음악 재생 종료
        AudioManager.instance.PlayBgm(false);

        // 게임 종료 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    // 게임 승리
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    // 시간 지연을 위한 코루틴함수
    IEnumerator GameVictoryRoutine()
    {
        isLive = false; // 죽음
        enemyCleaner.SetActive(true); // 적 청소기 작동

        yield return new WaitForSeconds(0.5f); // 시간 지연 (적 사망 애니메이션 재생시간 고려)

        uiResult.gameObject.SetActive(true); // 게임 결과 화면 활성화
        uiResult.Win(); // 승리 화면
        Stop(); // 정지

        // 게임 시작 배경음악 재생 종료
        AudioManager.instance.PlayBgm(false);

        // 게임 종료 효과음 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    // 게임 재시작
    public void GameRetry()
    {
        // LoadScene : 이름 혹은 인덱스로 장면을 새롭게 부르는 함수
        // 이 파일의 Samplescene 인덱스 번호는 0임
        SceneManager.LoadScene(0);
    }

    // 게임 종료
    public void GameQuit()
    {
        Application.Quit();
    }


    void Update()
    {
        if (!isLive) // 게임이 멈춰있다면 실행 X
        {
            return;
        }

        gameTime += Time.deltaTime; // 한 프레임 당 시간(deltaTime) 계속 더하기

        if(gameTime > maxGameTime) // 게임 시간이 최대시간보다 크면
        {
            gameTime = maxGameTime; // 최대시간으로 초기화
            GameVictory(); // 게임 승리
        }
    }

    // 경험치 얻기 함수
    public void GetExp()
    {
        // 죽어있다면 종료 -> 적 청소기가 죽이는 적들의 경험치는 계산 X
        if (!isLive)
        {
            return;
        }

        exp++;

        // 레벨업 기능
        int maxExp = maxExpPerCharacter[Mathf.Min(playerId, maxExpPerCharacter.Length - 1)];

        if (exp >= maxExp)
        {
            level++; // 레벨(카르마 단계) 상승
            exp = 0; // 카르마(경험치) 초기화
            uiLevelUp.Show(); 
        }
    }

    // 멈추기
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; // Time.timeScale : 유니티의 시간 속도(배율)

        uiJoy.localScale = Vector3.zero; // 게임이 멈추면, 조이스틱 invisible
    }

    // 재생
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1; // 시간이 n배로 빨라짐

        uiJoy.localScale = Vector3.one; // 게임이 시작되면, 원벡터
    }
}
