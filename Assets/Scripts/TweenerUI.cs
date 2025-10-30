using UnityEngine;
using DG.Tweening;

public class TweenerUI : MonoBehaviour
{
    public void ButtonPressDown(Transform tweenTarget)
    {
        if (tweenTarget != null)
        {
            tweenTarget.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.1f).SetEase(Ease.OutCubic);
        }
    }
    public void ButtonPressUp(Transform tweenTarget)
    {
        if (tweenTarget != null)
        {
            tweenTarget.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.OutCubic);
        }
    }

    public void ButtonPointerEnter(Transform tweenTarget)
    {
        if (tweenTarget != null)
        {
            tweenTarget.DOScale(new Vector3(0.93f, 0.93f, 0.93f), 0.1f).SetEase(Ease.OutCubic);
        }
    }
    public void ButtonPointerExit(Transform tweenTarget)
    {
        if (tweenTarget != null)
        {
            tweenTarget.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetEase(Ease.OutCubic);
        }
    }
}
