using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition; // Исходная позиция камеры
    public float maxRadius = 0.5f;
    void Start()
    {
        originalPosition = transform.localPosition;
        //StartShake(shakeSpeed: 10f, shakeDuration: 2f);
    }

    
    public void StartShake( float shakeSpeed, float shakeDuration)
    {
        StartCoroutine(ShakeRoutine(shakeSpeed, shakeDuration));
    }

    private IEnumerator ShakeRoutine( float shakeSpeed, float shakeDuration)
    {
        float timer = shakeDuration; // Таймер тряски

        while (timer > 0)
        {
            // Вычисляем случайное смещение в пределах заданного радиуса
            float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * 2 - 1; // Перлин-шум для плавности
            float offsetY = Mathf.PerlinNoise(0, Time.time * shakeSpeed) * 2 - 1;

            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0) * maxRadius;

            // Смещаем камеру
            transform.localPosition = originalPosition + shakeOffset;

            // Уменьшаем таймер
            timer -= Time.deltaTime;

            // Ждем следующий кадр
            yield return null;
        }

        // Возвращаем камеру в исходную позицию
        transform.localPosition = originalPosition;
    }
}
