using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingAnimation : MonoBehaviour
{
    Coroutine scale = null;
    private void OnEnable()
    {
        if (scale == null)
            scale = StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        if (scale != null)
        {
            StopCoroutine(scale);
            scale = null;
        }
    }

    IEnumerator Animation()
    {
        while (true)
        {
            LeanTween.scale(gameObject, new Vector3(2, 2, 2), 3);
            yield return new WaitForSeconds(3);

            LeanTween.scale(gameObject, new Vector3(1, 1, 1), 3);
            yield return new WaitForSeconds(3);
        }
    }
}
