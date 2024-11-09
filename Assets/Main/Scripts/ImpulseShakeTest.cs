using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ImpulseShakeTest : MonoBehaviour
{
 

    private Vector3 originalRotation;    // �������� �������� ������

    void Start()
    {
        // ��������� �������� �������� ������
        originalRotation = transform.eulerAngles;

       
    }

    IEnumerator ShakeCamera(float shakeIntensity, float shakeDuration)
    {
        float shakeTimer = shakeDuration;  // ������ ��� ������

        while (shakeTimer > 0)
        {
            // ��������� ��������� �������� ��� ������
            float shakeAmount = shakeIntensity * Mathf.Sin(Time.time * 10f); // ������� �� ���������

            // ������� ������
            transform.Rotate(shakeAmount, shakeAmount, shakeAmount);

            // ��������� ������
            shakeTimer -= Time.deltaTime;

            // ���� ��������� ����
            yield return null;
        }

        // ����� ��������� ������ ��������������� �������� ��������� ������
        transform.eulerAngles = originalRotation;
    }
}
