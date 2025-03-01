using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public AnimationCurve slideAnimation = AnimationCurve.Linear(0, 0, 1, 1);

    public Vector3 inPosition;

    public Vector3 outPosition;

    public float duration = 1.0f;

    //bottomを押すとコルーチン実行
    public void SlideIn()
    {
        StartCoroutine(StartSlidePanel(true));
    }

    public void SlideOut()
    {
        StartCoroutine(StartSlidePanel(false));
    }

    private IEnumerator StartSlidePanel(bool isSlideIn)
    {
        float startTime = Time.time;

        Vector3 startPos = transform.localPosition;

        Vector3 moveDistance;

        if (isSlideIn)
        {
            moveDistance = inPosition - startPos;
        }
        else
        {
            moveDistance= outPosition - startPos;
        }

        while ((Time.time - startTime) < duration)
        {
            transform.localPosition = startPos + moveDistance * slideAnimation.Evaluate((Time.time - startTime) / duration);

            yield return null;
        }
        transform.localPosition = startPos + moveDistance;
    }
}
