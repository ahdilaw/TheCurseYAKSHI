using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public float radius = 5f;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = (mousePos - playerTransform.position).normalized;
        float distance = Mathf.Min(Vector3.Distance(mousePos, playerTransform.position), radius);

        transform.position = playerTransform.position + direction * distance;
    }
}
