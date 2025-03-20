using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    private Exp exp;
    private Slider slider;

    void Start()
    {
        exp = FindFirstObjectByType<Exp>();
        slider = GetComponent<Slider>();
        slider.value = 0f;
        slider.value = exp.GetCurrentExp() / exp.GetMaxExp();
        exp.OnExpChanged += UpdateExpBar;
    }

    public void UpdateExpBar(float expPercent)
    {
        slider.value = expPercent;
    }
}
