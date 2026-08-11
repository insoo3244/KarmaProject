using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlugcatMotion : MonoBehaviour
{
    public Animator anim;
    private RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;

    public bool defaultFacingLeft = false;
    private Vector3 originalScale;
    private Coroutine moveCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim == null)
            anim = GetComponent<Animator>();

        originalScale = transform.localScale;
    }

    public void MoveToPosition(Vector2 targetPos, float duration, AnimationCurve curve)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetPos, duration, curve));
    }

    private IEnumerator MoveRoutine(Vector2 targetPos, float duration, AnimationCurve curve)
    {
        // 에러 방어막: RectTransform이 없으면 일반 Transform 사용
        Vector2 startPos = rectTransform != null ? rectTransform.anchoredPosition : (Vector2)transform.localPosition;

        float deltaX = targetPos.x - startPos.x;
        if (Mathf.Abs(deltaX) > 1f)
        {
            bool isMovingRight = deltaX > 0;
            SetFacingDirection(isMovingRight);
        }

        if (anim != null)
        {
            anim.SetBool("isRun", true);
        }

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float curveT = (curve != null && curve.keys.Length > 0) ? curve.Evaluate(t) : t;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, curveT);

            if (rectTransform != null)
                rectTransform.anchoredPosition = currentPos;
            else
                transform.localPosition = currentPos;

            yield return null;
        }

        if (rectTransform != null)
            rectTransform.anchoredPosition = targetPos;
        else
            transform.localPosition = targetPos;


        if (anim != null)
            anim.SetBool("isRun", false);
    }

    public void SetFacingDirection(bool isRight)
    {
        bool shouldFlip = defaultFacingLeft ? isRight : !isRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = shouldFlip;
        else
        {
            float scaleX = Mathf.Abs(originalScale.x) * (shouldFlip ? -1f : 1f);
            transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
        }
    }

    public void SetPositionInstant(Vector2 pos)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null) rectTransform.anchoredPosition = pos;
        else transform.localPosition = pos;

        if (anim != null) anim.SetBool("isRun", false);
    }
}