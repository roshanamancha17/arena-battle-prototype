using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float lifetime = 1f;

    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetText(float damage)
    {
        textMesh.text = "-" + Mathf.RoundToInt(damage);
    }
}
