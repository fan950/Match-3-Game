using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCanvas : Singleton<GlobalCanvas>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fadeImg;

    private float fFadeTime = 0;

    private const float fFadeTimeSpeed = 1.5f;
    public void SetCamera()
    {
        canvas.worldCamera = Camera.main;
    }

    public void Fade(bool isIn, Action action)
    {
        if (isIn)
            StartCoroutine(FadeIn(action));
        else
            StartCoroutine(FadeOut());
    }
    private IEnumerator FadeIn(Action action)
    {
        fFadeTime = 0;

        fadeImg.gameObject.SetActive(true);
        fadeImg.color = new Color(0, 0, 0, 0);

        while (true)
        {
            yield return null;

            fFadeTime += Time.deltaTime * fFadeTimeSpeed;

            fadeImg.color = new Color(0, 0, 0, fFadeTime);

            if (fFadeTime >= 1)
            {
                fFadeTime = 1;
                action();
                break;
            }
        }
    }

    private IEnumerator FadeOut()
    {
        SetCamera();

        fFadeTime = 1;
        while (true)
        {
            yield return null;

            fFadeTime -= Time.deltaTime * fFadeTimeSpeed;

            fadeImg.color = new Color(0, 0, 0, fFadeTime);

            if (fFadeTime <= 0)
            {
                fFadeTime = 0;
                fadeImg.gameObject.SetActive(false);
                break;
            }
        }
    }
}
