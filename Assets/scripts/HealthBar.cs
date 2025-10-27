using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider;               // assign this inside prefab
    public Transform target;            // set by Base or Troop
    public Vector3 offset = new Vector3(0, 3f, 0);

    [Header("Visuals")]
    public Image fillImage;             // assign in prefab (Slider -> Fill -> Image)

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;

        // Auto-assign slider if missing
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        // Auto-assign fill image if not set
        if (fillImage == null && slider != null)
            fillImage = slider.fillRect.GetComponent<Image>();
    }

    void Start()
    {
        // ✅ Make sure this health bar is parented to the main Canvas
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null && transform.parent != mainCanvas.transform)
        {
            transform.SetParent(mainCanvas.transform, false);
        }

        if (slider == null)
            Debug.LogWarning($"{name}: No Slider assigned in HealthBar!");
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;

            // ✅ Face toward camera
            if (mainCam != null)
                transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);
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

    // ✅ NEW: color setup for player vs enemy base
    public void SetColorByTag(string tag)
    {
        if (fillImage == null) return;

        if (tag == "PlayerBase")
            fillImage.color = Color.green;
        else if (tag == "EnemyBase")
            fillImage.color = Color.red;
        else
            fillImage.color = Color.yellow; // fallback for troops or others
    }
}
