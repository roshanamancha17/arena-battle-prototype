using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public Transform target;
    public Vector3 offset = new Vector3(0, 3f, 0);
    private Image fillImage;

    void Start()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (slider != null)
            fillImage = slider.fillRect.GetComponent<Image>();
        else
            Debug.LogWarning($"{name}: No Slider found in HealthBar!");
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
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
        slider.gameObject.SetActive(current > 0);
    }

    public void SetColorByTag(string tag)
    {
        if (fillImage == null) return;

        if (tag == "PlayerBase" || tag == "PlayerTroop")
            fillImage.color = Color.green;
        else if (tag == "EnemyBase" || tag == "EnemyTroop")
            fillImage.color = Color.red;
        else
            fillImage.color = Color.yellow;
    }
}
