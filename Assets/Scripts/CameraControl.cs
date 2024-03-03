using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public IEnumerator Zoom(Vector3 position, float duration, float zoomMagnitude)
    {
        float originalSize = GetComponent<Camera>().orthographicSize;
        float targetSize = originalSize / zoomMagnitude;

        float timer = 0;
        while(timer < duration)
        {
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(originalSize, targetSize, timer);
            timer += Time.deltaTime * 2;
            yield return null;
        }

        GetComponent<Camera>().orthographicSize = targetSize;

        yield return new WaitForSeconds(duration);

        GetComponent<Camera>().orthographicSize = originalSize;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = new Vector3(0,0,-10);

        float elapsedTime = 0;

        while(elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, originalPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
