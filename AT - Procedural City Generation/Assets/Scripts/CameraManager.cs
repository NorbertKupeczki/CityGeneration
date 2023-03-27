using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _tiltSpeed;

    private float _targetZRotation = -55.0f;
    private float _targetYPos = -4.0f;

    public void StartCameraMovement()
    {
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        Quaternion targetRotation;
        Vector3 targetYPos = new Vector3(transform.position.x, transform.position.y + _targetYPos -0.5f, transform.position.z);

        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.Rotate(0.0f, _rotationSpeed * Time.deltaTime, 0.0f, Space.World);

            if (transform.rotation.eulerAngles.z == 0 ||
                transform.rotation.eulerAngles.z > 360 + _targetZRotation + 1.0f)
            {
                targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, _targetZRotation);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _tiltSpeed * Time.deltaTime);

                transform.position = Vector3.Lerp(transform.position, targetYPos, _tiltSpeed * Time.deltaTime);
            }
        }
    }
}
