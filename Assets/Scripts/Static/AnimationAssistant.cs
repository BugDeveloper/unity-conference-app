using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class AnimationAssistant
{
    public const float AnimationSpeedDefault = 0.5f;

    public static void HideABit(CanvasGroup cg)
    {
        cg.DOFade(0.5f, AnimationSpeedDefault);
        cg.blocksRaycasts = false;
    }

    public static void FadeText(Text text, float alpha)
    {
        text.DOFade(alpha, AnimationSpeedDefault);
    }

    public static void MoveX(Transform transform, float pos)
    {
        transform.DOMoveX(pos, AnimationSpeedDefault);
    }

    public static bool IsShown(CanvasGroup cg)
    {
        return cg.blocksRaycasts;
    }

    public static void ButtonEffect(Transform transform)
    {
        transform.DOComplete();
        transform.DOPunchScale(new Vector3(-0.13f, -0.13f, -0.13f), AnimationSpeedDefault, 3);
    }

    public static void QuestionAnimation(CanvasGroup front, CanvasGroup back)
    {
        DOTween.CompleteAll();
        var sequence = DOTween.Sequence();
        sequence.Append(front.DOFade(0f, AnimationSpeedDefault));
        sequence.Append(back.DOFade(1f, AnimationSpeedDefault));
        sequence.AppendInterval(5f);
        sequence.Append(back.DOFade(0f, AnimationSpeedDefault));
        sequence.Append(front.DOFade(1f, AnimationSpeedDefault));
        sequence.Play();
    }

    public static void ExpandAndFade(RectTransform rectTransform, Vector2 vector2, CanvasGroup canvasGroup,
        float fadeTo, bool forward)
    {

        DOTween.CompleteAll();
        var expanding = DOTween.Sequence();

        if (forward)
        {
            expanding.Append(rectTransform.DOSizeDelta(vector2, AnimationSpeedDefault / 2));
            canvasGroup.blocksRaycasts = true;
            expanding.Append(canvasGroup.DOFade(fadeTo, AnimationSpeedDefault / 2));
        }
        else
        {
            expanding.Append(canvasGroup.DOFade(fadeTo, AnimationSpeedDefault / 2));
            canvasGroup.blocksRaycasts = false;
            expanding.Append(rectTransform.DOSizeDelta(vector2, AnimationSpeedDefault / 2));
        }

        expanding.Play();
        //LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    public static void ImageColor(Image image, Color color)
    {
        image.DOKill();
        image.DOColor(color, AnimationSpeedDefault);
    }

    public static void LocalRotateZ180(Transform transform)
    {
        transform.DOComplete();
        transform.DOLocalRotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 180f),
            AnimationSpeedDefault, RotateMode.LocalAxisAdd);
    }

    public static void Show(CanvasGroup cg)
    {
        cg.DOComplete();
        cg.DOFade(1f, AnimationSpeedDefault);
        cg.blocksRaycasts = true;
    }

    public static void ExpandRect(RectTransform rect, Vector2 size)
    {
        rect.DOSizeDelta(size, AnimationSpeedDefault);
    }

    public static void MoveLocalY(Transform transform, float pos)
    {
        transform.DOLocalMoveY(pos, AnimationSpeedDefault);
    }

    public static void MoveY(Transform transform, float pos)
    {
        transform.DOComplete();
        transform.DOMoveY(pos, AnimationSpeedDefault);
    }

    public static void Hide(CanvasGroup cg)
    {
        cg.DOComplete();
        cg.DOFade(0f, AnimationSpeedDefault);
        cg.blocksRaycasts = false;
    }

    public static void Hide(CanvasGroup cg, float speed)
    {
        cg.DOFade(0f, speed);
        cg.blocksRaycasts = false;
    }

    public static void SwitchFromTo(CanvasGroup fromCG, CanvasGroup toCG)
    {
        DOTween.CompleteAll();

        var switching = DOTween.Sequence();
        switching.Append(fromCG.DOFade(0f, AnimationSpeedDefault / 2));
        switching.Append(toCG.DOFade(1f, AnimationSpeedDefault / 2));

        switching.OnStart(() =>
        {
            fromCG.blocksRaycasts = false;
            toCG.blocksRaycasts = true;
        });

        switching.Play();
    }

    public static void ChangeRectScale(RectTransform rectObj, float scale)
    {
        rectObj.DOKill();
        rectObj.DOScale(scale, AnimationSpeedDefault);
    }

    public static void ChangeTextColor(Text text, Color color)
    {
        text.DOKill();
        text.DOColor(color, AnimationSpeedDefault);
    }
}