using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("UI Connect")]
    public RectTransform characterContainer; // Group 패널 (RectTransform)
    public Button leftButton;
    public Button rightButton;

    [Header("Slide Settings")]
    public float slideDuration = 0.3f; // 이동 시간

    private int currentIndex = 0;
    private int maxIndex;
    private bool isMoving = false;
    private Vector2 targetPos;

    void Start()
    {
        // 자식 카드 개수 - 1 이 최대 인덱스
        maxIndex = characterContainer.childCount - 1;
        
        // 시작 시 현재 포지션 저장
        targetPos = characterContainer.anchoredPosition;
        UpdateButtons();
    }

    public void OnClickNext()
    {
        if (isMoving || currentIndex >= maxIndex) return;

        currentIndex++;
        MoveToCurrentIndex();
    }

    public void OnClickPrev()
    {
        if (isMoving || currentIndex <= 0) return;

        currentIndex--;
        MoveToCurrentIndex();
    }

    void MoveToCurrentIndex()
    {
        // 💡 핵심: 자식 RectTransform의 X 좌표를 읽어서 정확히 그 위치 반대로 이동!
        RectTransform targetChild = characterContainer.GetChild(currentIndex).GetComponent<RectTransform>();
        targetPos = new Vector2(-targetChild.anchoredPosition.x, characterContainer.anchoredPosition.y);

        StartCoroutine(SlideRoutine());
    }

    IEnumerator SlideRoutine()
    {
        isMoving = true;
        Vector2 startPos = characterContainer.anchoredPosition;
        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            
            // anchoredPosition 끼리 Lerp!
            characterContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        characterContainer.anchoredPosition = targetPos;
        isMoving = false;

        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (leftButton != null) leftButton.interactable = (currentIndex > 0);
        if (rightButton != null) rightButton.interactable = (currentIndex < maxIndex);
    }
}