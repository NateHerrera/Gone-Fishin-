using UnityEngine;

public class BaitSplashTrigger : MonoBehaviour
{
    public GameObject splashEffectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
