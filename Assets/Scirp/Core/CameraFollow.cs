using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smooth = 8f;
    [SerializeField] private Vector2 offset;

    private void LateUpdate()
    {
        if (!target) return;
        Vector3 pos = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * smooth);
    }

    public void SetTarget(Transform t) => target = t;
}
