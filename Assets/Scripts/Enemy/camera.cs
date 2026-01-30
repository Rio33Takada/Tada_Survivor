using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;   // プレイヤー

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;

    // 初期回転を保持
    private Quaternion fixedRotation;

    private void Start()
    {
        // ★ カメラの回転を固定
        fixedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 位置だけ追尾
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        // ★ 回転は固定
        transform.rotation = fixedRotation;
    }

    /// <summary>
    /// 外部から追尾対象を変更
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
