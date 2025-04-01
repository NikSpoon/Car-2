using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera mainCamera;         // Камера для стандартного вида (например, для третьего лица)
    [SerializeField] private CinemachineCamera freeLookCamera;          // Камера для свободного вращения
    [SerializeField] private float switchSpeed = 2f;                      // Скорость переключения между камерами
    [SerializeField] private float deadZone = 0.1f;                       // Порог для отслеживания ввода мыши

    private bool isInteracting = false;                                    // Флаг для отслеживания ввода

    private CinemachineBrain cinemachineBrain;                            // Компонент для управления камерами

 

    void Update()
    {
        // Проверяем, есть ли движение мыши
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Если пользователь взаимодействует с камерой, ставим флаг isInteracting в true
        if (Mathf.Abs(mouseX) > deadZone || Mathf.Abs(mouseY) > deadZone)
        {
            isInteracting = true;
        }
        else
        {
            isInteracting = false;
        }

        // Переключение между камерами
        if (isInteracting)
        {
            SwitchToCamera(freeLookCamera);
        }
        else
        {
            SwitchToCamera(mainCamera);
        }
    }

    void SwitchToCamera(CinemachineCamera newCamera)
    {
        // Включаем новую камеру, а старую выключаем
        newCamera.gameObject.SetActive(true);

        // Отключаем старые камеры, если они не являются текущей
        if (newCamera != mainCamera && mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }

        if (newCamera != freeLookCamera && freeLookCamera != null)
        {
            freeLookCamera.gameObject.SetActive(false);
        }

        // Плавное переключение камер через CinemachineBrain
       
    }
}
