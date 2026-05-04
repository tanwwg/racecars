using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ValueBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetValue(float v)
    {
        fillImage.fillAmount = Mathf.Clamp01(v);
    }

}