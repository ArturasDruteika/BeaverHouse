using UnityEngine;

public class BeaverWaterSensor : MonoBehaviour
{
    public bool IsInWater { get; private set; }

    private void OnEnable()
    {
        IsInWater = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            IsInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            IsInWater = false;
        }
    }
}
