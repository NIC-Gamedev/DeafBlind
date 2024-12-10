using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ImpulseShakeTest : MonoBehaviour
{
 

    private Vector3 originalRotation;    // Исходное вращение камеры

    void Start()
    {
        // Сохраняем исходное вращение камеры
        originalRotation = transform.eulerAngles;

       
    }

    IEnumerator ShakeCamera(float shakeIntensity, float shakeDuration)
    {
        float shakeTimer = shakeDuration;  // Таймер для тряски

        while (shakeTimer > 0)
        {
            // Вычисляем случайное смещение для тряски
            float shakeAmount = shakeIntensity * Mathf.Sin(Time.time * 10f); // Ротация по синусоиде

            // Вращаем камеру
            transform.Rotate(shakeAmount, shakeAmount, shakeAmount);

            // Уменьшаем таймер
            shakeTimer -= Time.deltaTime;

            // Ждем следующий кадр
            yield return null;
        }

        // После окончания тряски восстанавливаем исходное положение камеры
        transform.eulerAngles = originalRotation;
    }
}
