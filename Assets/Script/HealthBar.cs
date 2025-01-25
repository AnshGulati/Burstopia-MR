using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Reference to UI Slider
    public Image fillImage; // Reference to the Fill Image (for color change)
    public float smoothSpeed = 0.2f; // Speed of smooth transition

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        StartCoroutine(SmoothHealthChange(health)); // Smooth animation
    }

    private IEnumerator SmoothHealthChange(int newHealth)
    {
        float elapsedTime = 0f;
        float startValue = slider.value;

        while (elapsedTime < smoothSpeed)
        {
            slider.value = Mathf.Lerp(startValue, newHealth, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.value = newHealth; // Ensure final value is correct

        // Change color when health is low
        if (newHealth <= slider.maxValue * 0.3f)
        { // Below 30% health
            fillImage.color = Color.red; // Low health turns red
        }
        else
        {
            fillImage.color = Color.green;
        }
    }
}
