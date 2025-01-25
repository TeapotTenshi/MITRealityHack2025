using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haptikos.Misc
{
    [RequireComponent(typeof(Renderer))]
    public class CameraFader : MonoBehaviour, IFade
    {
        public bool fadeOnStart = true;
        public float fadeDuration = 2;
        public float secondsForFaderToVanish = 2f;
        public Color fadeColor;
        private Renderer rend;


        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<Renderer>();

            if (fadeOnStart)
                FadeOut(0f);
        }

        public void Fade(float alphaIn, float alphaOut)
        {
            StartCoroutine(FadeCoroutine(alphaIn, alphaOut, fadeDuration));
        }

        public void FadeIn(float duration = 1)
        {
            fadeDuration = duration;
            Fade(0, 1);
        }

        public void FadeOut(float duration = 1)
        {
            fadeDuration = duration;
            Fade(1, 0);

            
        }

        public IEnumerator FadeCoroutine(float alphaIn, float alphaOut, float duration)
        {
            float timer = 0;
            while (timer <= fadeDuration)
            {
                Color newColor = fadeColor;
                newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);

                rend.material.SetColor("_Color", newColor);

                timer += Time.deltaTime;
                yield return null;
            }

            Color newColor2 = fadeColor;
            newColor2.a = alphaOut;
            rend.material.SetColor("_Color", newColor2);

        }
    }
}

