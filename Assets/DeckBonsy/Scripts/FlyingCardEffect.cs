using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyingCardEffect : MonoBehaviour
{
    public Image cardImage; 

    public void Launch(Sprite sprite, Vector3 startPos, Vector3 endPos, float duration = 0.6f)
    {
        cardImage.sprite = sprite;
        transform.position = startPos;
        StartCoroutine(FlyToTarget(endPos, duration));
    }

    private IEnumerator FlyToTarget(Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        Destroy(gameObject); // usuñ efekt po dotarciu
    }
}
