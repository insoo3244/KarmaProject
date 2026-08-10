using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 슬러그캣 개별 이동 및 모션 전용 스크립트
public class SlugcatMotion : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    public Animator anim;
    private RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;
    private Image uiImage;

    [Header("스프라이트 기본 방향 설정")]
    [Tooltip("기본 도트 스프라이트 파일 자체가 왼쪽을 바라보고 있다면 체크")]
    public bool defaultFacingLeft = false;

    private Vector3 originalScale;
    private Coroutine moveCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (anim == null)
            anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        uiImage = GetComponent<Image>();

        originalScale = transform.localScale;
    }

    // 목표 좌표로 이동하는 함수
    public void MoveToPosition(Vector2 targetAnchoredPos, float duration, AnimationCurve curve)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetAnchoredPos, duration, curve));
    }

    private IEnumerator MoveRoutine(Vector2 targetPos, float duration, AnimationCurve curve)
    {
        Vector2 startPos = rectTransform.anchoredPosition;

        // 1. 이동 방향 판별 및 스프라이트 좌/우 반전
        float deltaX = targetPos.x - startPos.x;
        if (Mathf.Abs(deltaX) > 1f) // 유의미한 이동 거리가 있을 때
        {
            bool isMovingRight = deltaX > 0;
            SetFacingDirection(isMovingRight);
        }

        // 2. 뛰기 애니메이션(isRun = true) 실행
        if (anim != null)
            anim.SetBool("isRun", true);

        // 3. RectTransform 실제 좌표 이동
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float curveT = (curve != null && curve.keys.Length > 0) ? curve.Evaluate(t) : t;

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveT);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;

        // 4. 도착 후 스탠드 애니메이션(isRun = false)으로 전환
        if (anim != null)
            anim.SetBool("isRun", false);
    }

    // 바라보는 방향 설정 (오른쪽 이동: isRight = true, 왼쪽 이동: isRight = false)
    public void SetFacingDirection(bool isRight)
    {
        bool shouldFlip = defaultFacingLeft ? isRight : !isRight;

        // SpriteRenderer인 경우
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = shouldFlip;
        }
        // UI Image 또는 RectTransform Transform인 경우 (Scale.x 조절)
        else
        {
            float scaleX = Mathf.Abs(originalScale.x) * (shouldFlip ? -1f : 1f);
            transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
        }
    }

    // 게임 시작 시 초기 위치 즉시 세팅 (애니메이션 없이 순간이동)
    public void SetPositionInstant(Vector2 pos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = pos;

        if (anim != null)
            anim.SetBool("isRun", false);
    }
}