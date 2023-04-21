using System.Collections;
using UnityEngine;

namespace _Scripts.Utilities
{
    public static class Animate
    {
        private static readonly int ColorID = Shader.PropertyToID("_BaseColor");

        private static IEnumerator ColorTransition(Material mat, Color target, float duration)
        {
            float elapsed = 0.0f;
            Color init = mat.GetColor(ColorID);
            
            
            while (true)
            {
                elapsed += Time.deltaTime;
                
                mat.SetColor(ColorID,Color.Lerp(init, target, elapsed / duration));
                
                if (elapsed >= duration)
                    yield break;
                
                yield return null;
            }
        }
        
        public static void MaterialFade(this Material mat, Color target, float duration, MonoBehaviour caller)
        {
            caller.StartCoroutine(ColorTransition(mat, target, duration));
        }

        public static Color Saturation(this Color color, float value)
        {
            Color initialColor = color;
            Color.RGBToHSV(initialColor, out float h, out float s, out float v);
            Color fadedColor = Color.HSVToRGB(h, value, v);
            return fadedColor;
        }
    }
}