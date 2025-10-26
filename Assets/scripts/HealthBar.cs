using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider;               // assign this inside prefab
    public Transform target;            // set by Base or Troop
    public Vector3 offset = new Vector3(0, 3f, 0);

    void Start()
    {
        // Try auto-assign if the slider was not linked manually
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (slider == null)
            Debug.LogWarning($"{name}: No Slider assigned in HealthBar!");
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHealth(float current, float max)
    {
        if (slider == null)
        {
            Debug.LogWarning($"{name}: Slider missing in SetHealth()");
            return;
        }

        slider.maxValue = max;
        slider.value = current;

        // Optional: hide bar when dead
        slider.gameObject.SetActive(current > 0);
    }
}
