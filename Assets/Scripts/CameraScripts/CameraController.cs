using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private CompositeCollider2D mapBounds;

    private Camera mainCam;

    private float xMin, xMax;
    private float cameraRatio;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();

        // ��������� ������� �����
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;

        // ����������� ������ ������
        cameraRatio = (xMax + mainCam.orthographicSize) / 2.0f;

    }

    void LateUpdate()
    {
        // ������� ������ �� �������
        MoveCamera();
    }

    // �������� ������ �� �������
    private void MoveCamera()
    {
        // ���������� ������� ������, ����� �� ��� � ������ ������
        float camX = Mathf.Clamp(playerTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);

        // ����������� ������� ������
        transform.position = new Vector3(camX, transform.position.y, transform.position.z);
    }
}
