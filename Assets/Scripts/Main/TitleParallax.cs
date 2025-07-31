using UnityEngine;
using Cinemachine;

/// <summary>
/// 마우스 반대 방향으로 카메라가 살랑이며 이동
/// </summary>
public class TitleParallax : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float moveAmount = 0.15f;  // 이동 강도
    [SerializeField] private float smoothSpeed = 2f;   // 보간 속도

    private Vector3 targetOffset;
    private Vector3 currentOffset;

    void Start()
    {
        // 카메라의 초기 회전값을 저장하고 고정
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.Dutch = 0f; // 카메라 기울기 고정
        }
    }

    void Update()
    {
        // 마우스 위치를 화면 비율 (-1 ~ 1)로 정규화
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // 반대 방향 이동
        targetOffset = new Vector3(-mouseX, -mouseY, 0) * moveAmount;

        // 부드럽게 보간
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        // 카메라 위치 적용
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = new Vector3(currentOffset.x, currentOffset.y, transposer.m_FollowOffset.z);
        }

        // 카메라 회전 고정 (매 프레임마다 확인)
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.Dutch = 0f; // 카메라 기울기 고정
        }
    }
}
