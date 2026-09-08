using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 200f;

    private float rotationX = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.parent.Rotate(Vector3.up * mouseX);
    }
}