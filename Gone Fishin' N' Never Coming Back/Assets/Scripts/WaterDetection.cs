using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    private bool isNearWater = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = true;
            Debug.Log("Player is near water. Ready to fish!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isNearWater = false;
            Debug.Log("Player moved away from the water.");
        }
    }

    public bool CanFish()
    {
        return isNearWater;
    }
}
