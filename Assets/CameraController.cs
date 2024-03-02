using UnityEngine;

public class OrthoCameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float zoomSpeed = 5f;

    void Update()
    {
        // Camera movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, vertical, 0) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Camera zoom
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        float newSize = Camera.main.orthographicSize - scrollWheel * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Max(newSize, 0.1f); // Ensure orthographic size is not negative
    }
}
