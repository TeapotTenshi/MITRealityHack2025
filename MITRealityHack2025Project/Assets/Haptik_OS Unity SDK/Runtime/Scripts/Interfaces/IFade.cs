using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFade 
{
    public void FadeIn(float duration = 1);

    public void FadeOut(float duration = 1);

    public IEnumerator FadeCoroutine(float alphaIn, float alphaOut, float duration);
}
