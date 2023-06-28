using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 _dir;

    private void Update()
    {
        transform.Rotate(_dir, Space.World);
    }
}
