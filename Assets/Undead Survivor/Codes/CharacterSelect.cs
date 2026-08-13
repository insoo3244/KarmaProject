using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("UI Connect")]
    public RectTransform characterContainer; // Group 패널
    public Button leftButton;
    public Button rightButton;
    public GameObject characterSelectGroup; // CharacterSelectManager 등록
    public GameObject characterSelectPanel; // 캐릭터 선택창 전체 패널 (비활성화용)
    public GameObject slugcatParentObj;      // 슬러그캣들이 들어있는 SlugCat 최상위 오브젝트

    [Header("Slide & Easing Settings")]
    public float uiSlideDuration = 0.5f;       // 💡 배경 UI 템플릿이 넘어가는 시간 (빠르게)
    public float slugcatMoveDuration = 1.5f;   // 💡 슬러그캣이 뛰어오는 시간 (느긋하게)
    public AnimationCurve slideCurve;

    [Header("CharacterTemplate Fade & Scale Settings")]
    public float fadeDistance = 1500f;
    public float minAlpha = 0.0f;
    public float minScale = 0.7f;

    [Header("Slugcat Motion Connect")]
    public SlugcatMotion[] slugcatMotions; // 0: Survivor, 1: Monk, 2: Hunter

    [Header("SlugCat Moving Point")]
    public float leftOutsideX = -1300f; // 화면 밖 왼쪽 X
    public float centerPosX = -250f;    // 화면 안 중앙 X (생존자 시작 위치)
    public float rightOutsideX = 800f;  // 화면 밖 오른쪽 X (수도승/사냥꾼 시작 위치)
    public float slugcatPosY = -1250f;  // Y 좌표 공통 기준

    private int currentIndex = 0;
    private int maxIndex;
    private bool isMoving = false;
    private Vector2 targetPos;

    private CanvasGroup[] cardCanvasGroups;

    void Start()
    {
        maxIndex = characterContainer.childCount - 1;
        targetPos = characterContainer.anchoredPosition;

        int childCount = characterContainer.childCount;
        cardCanvasGroups = new CanvasGroup[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform child = characterContainer.GetChild(i);
            cardCanvasGroups[i] = child.GetComponent<CanvasGroup>();
            if (cardCanvasGroups[i] == null)
                cardCanvasGroups[i] = child.gameObject.AddComponent<CanvasGroup>();
        }

        // 게임 시작 시 슬러그캣 위치 배치 (0번은 중앙, 나머지는 오른쪽 밖)
        InitSlugcatPositions();

        UpdateButtons();
        UpdateCardEffects();
    }
    
public void OnClickStartGame()
{
    if (isMoving) return; // 이동 중 클릭 방지

    // 1. GameManager에 선택된 캐릭터 ID 전달
    if (GameManager.instance != null)
    {
        GameManager.instance.playerId = currentIndex;
        GameManager.instance.GameStart(currentIndex); // 게임 시작 로직 실행
    }

    // 2. Canvas는 냅두고, 캐릭터 선택창 묶음만 쏙 끄기!
    if (characterSelectGroup != null)
    {
        characterSelectGroup.SetActive(false);
    }
    else
    {
        // 만약 이 스크립트 자체가 CharacterSelect 오브젝트에 붙어있다면 자신을 끄기
        gameObject.SetActive(false); 
    }
}

    // 슬러그캣들의 초기 위치 배치
    void InitSlugcatPositions()
    {
        if (slugcatMotions == null) return;

        for (int i = 0; i < slugcatMotions.Length; i++)
        {
            if (slugcatMotions[i] == null) continue;

            if (i < currentIndex)
            {
                slugcatMotions[i].SetPositionInstant(new Vector2(leftOutsideX, slugcatPosY));
                slugcatMotions[i].SetFacingDirection(true); 
            }
            else if (i == currentIndex)
            {
                slugcatMotions[i].SetPositionInstant(new Vector2(centerPosX, slugcatPosY));
                slugcatMotions[i].SetFacingDirection(true); 
            }
            else
            {
                slugcatMotions[i].SetPositionInstant(new Vector2(rightOutsideX, slugcatPosY));
                slugcatMotions[i].SetFacingDirection(false); 
            }
        }
    }

    public void OnClickNext()
    {
        if (isMoving || currentIndex >= maxIndex) return;

        int prevIndex = currentIndex;
        currentIndex++;

        // 1. UI 배경 카드 슬라이드 (UI 전용 속도 적용)
        MoveToCurrentIndex();

        // 2. 슬러그캣 이동 연출 (슬러그캣 전용 속도 적용!)
        if (slugcatMotions != null)
        {
            if (prevIndex < slugcatMotions.Length && slugcatMotions[prevIndex] != null)
                slugcatMotions[prevIndex].MoveToPosition(new Vector2(leftOutsideX, slugcatPosY), slugcatMoveDuration, slideCurve);

            if (currentIndex < slugcatMotions.Length && slugcatMotions[currentIndex] != null)
                slugcatMotions[currentIndex].MoveToPosition(new Vector2(centerPosX, slugcatPosY), slugcatMoveDuration, slideCurve);
        }
    }

    public void OnClickPrev()
    {
        if (isMoving || currentIndex <= 0) return;

        int prevIndex = currentIndex;
        currentIndex--;

        // 1. UI 배경 카드 슬라이드 (UI 전용 속도 적용)
        MoveToCurrentIndex();

        // 2. 슬러그캣 이동 연출 (슬러그캣 전용 속도 적용!)
        if (slugcatMotions != null)
        {
            if (prevIndex < slugcatMotions.Length && slugcatMotions[prevIndex] != null)
                slugcatMotions[prevIndex].MoveToPosition(new Vector2(rightOutsideX, slugcatPosY), slugcatMoveDuration, slideCurve);

            if (currentIndex < slugcatMotions.Length && slugcatMotions[currentIndex] != null)
                slugcatMotions[currentIndex].MoveToPosition(new Vector2(centerPosX, slugcatPosY), slugcatMoveDuration, slideCurve);
        }
    }

    void MoveToCurrentIndex()
    {
        RectTransform targetChild = characterContainer.GetChild(currentIndex).GetComponent<RectTransform>();
        targetPos = new Vector2(-targetChild.anchoredPosition.x, characterContainer.anchoredPosition.y);

        StartCoroutine(SlideRoutine());
    }

    IEnumerator SlideRoutine()
    {
        isMoving = true;

        Vector2 startPos = characterContainer.anchoredPosition;
        float time = 0f;

        // 💡 UI 슬라이드 전용 시간(uiSlideDuration) 사용
        while (time < uiSlideDuration)
        {
            time += Time.deltaTime;
            float t = time / uiSlideDuration;
            float curveT = (slideCurve != null && slideCurve.keys.Length > 0) ? slideCurve.Evaluate(t) : t;

            characterContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveT);

            UpdateCardEffects();
            yield return null;
        }

        characterContainer.anchoredPosition = targetPos;
        UpdateCardEffects();

        isMoving = false;
        UpdateButtons();
    }

    void UpdateCardEffects()
    {
        for (int i = 0; i < characterContainer.childCount; i++)
        {
            // 1. 불투명도 연출: 거리에 상관없이 현재 인덱스면 즉시 1(선명), minAlpha(0.2)로 팍 줄임
            if (cardCanvasGroups[i] != null)
            {
                cardCanvasGroups[i].alpha = (i == currentIndex) ? 1f : minAlpha;
            }

            // 2. 크기 연출: 크기는 슬라이드되는 동안 부드럽게 커지고 작아지도록 기존 유지
            RectTransform child = characterContainer.GetChild(i).GetComponent<RectTransform>();
            float currentCardX = characterContainer.anchoredPosition.x + child.anchoredPosition.x;
            float distanceFromCenter = Mathf.Abs(currentCardX);
            float tFactor = Mathf.Clamp01(1f - (distanceFromCenter / fadeDistance));

            Transform scaleGroup = child.Find("ScaleGroup");
            if (scaleGroup != null)
            {
                float cardScale = Mathf.Lerp(minScale, 1f, tFactor);
                scaleGroup.localScale = new Vector3(cardScale, cardScale, 1f);
            }
        }
    }

    void UpdateButtons()
    {
        if (leftButton != null) leftButton.interactable = (currentIndex > 0);
        if (rightButton != null) rightButton.interactable = (currentIndex < maxIndex);
    }
}