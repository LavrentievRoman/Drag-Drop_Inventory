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

        // Указываем границы карты
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;

        // Опеределяем размер экрана
        cameraRatio = (xMax + mainCam.orthographicSize) / 2.0f;

    }

    void LateUpdate()
    {
        // Двигаем камеру за игроком
        MoveCamera();
    }

    // Движение камеры за игроком
    private void MoveCamera()
    {
        // Определяем позицию игрока, чтобы он был в центре экрана
        float camX = Mathf.Clamp(playerTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);

        // Присваиваем позицию камере
        transform.position = new Vector3(camX, transform.position.y, transform.position.z);
    }
}
