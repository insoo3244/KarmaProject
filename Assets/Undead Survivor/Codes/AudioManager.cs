using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 나 자신 선언

    // 총 3개의 BGM
    [Header("#BGM")]
    public AudioClip bgmClip; // 클립
    public float bgmVolume; // 볼륨 크기
    AudioSource bgmPlayer; // 브금 플레이어
    AudioHighPassFilter bgmEffect; // 오디오 필터

    // 총 3개의 효과음
    [Header("#SFX")]
    public AudioClip[] sfxClips; // 클립
    public float sfxVolume; // 볼륨 크기
    public int channels; // 채널 개수
    AudioSource[] sfxPlayers; // 효과음 플레이어
    int channelIndex; // 채널 인덱스 개수

    // 효과음 열거형 변수 ( LevelUp=3 -> 임의로 숫자대응 가능)
    public enum Sfx { Dead, Hit, LevelUp=3, Lose, Melee, Range = 7, Select, Win }

    // 객체 초기화
    void Awake()
    {
        instance = this;
        Init(); // 초기화
    }

    void Init()
    {
        // == 배경음 플레이어 초기화 ==
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트 생성
        
        // 오디오 소스를 생성하고 변수(bgmPlayer)에 저장함
        bgmPlayer = bgmObject.AddComponent<AudioSource>();

        // 배경음 세팅
        bgmPlayer.playOnAwake = false; // 항상 켜지지 않게 false;
        bgmPlayer.loop = true; // 무한 반복
        bgmPlayer.volume = bgmVolume; // 볼륨 크기
        bgmPlayer.clip = bgmClip; // 플레이 할 파일 정하기
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();


        // == 효과음 플레이어 초기화 ==
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트 생성
        sfxPlayers = new AudioSource[channels]; // 채널 개수 만큼 오디오 소스 배열 생성

        // 오디오 소스를 생성하고 변수(sfxPlayer)에 저장, 이후 효과음 세팅
        for(int index = 0; index < sfxPlayers.Length; index++){
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; // 효과음은 오디오필터 패스
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    // 효과음 재생 함수 : 인자 ( 열거형 sfx 효과음 목록 )
    public void PlaySfx(Sfx sfx)
    {
        // 채널 개수 만큼 순회하도록 channelIndex 활용
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            // 환형배열 모듈러 원리 -> 절대 인덱스 범위를 벗어나지 않음
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            // 이미 재생 중인 효과음이 존재할 경우 건너뛰기
            // 재생 안하는 플레이어를 찾기
            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            // 피격, 근접 공격의 사운드는 두 개. 둘 중에 랜덤으로 효과음 발생 -> ranIndex;
            int ranIndex = 0;
            if(sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;

            // sfxPlayer 하나를 가져와서 클립을 설정
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex]; // (int)열거형 = 해당 열거형 원소의 인덱스 번호를 가져옴
            sfxPlayers[loopIndex].Play(); // 클립 변경 후 재생

            break; // 효과음이 재생이 되었다면 반복문 종료 (중요함)
        }
    }

    // 배경음악 재생 함수
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();    
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    // 오디오 필터
    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }
}
