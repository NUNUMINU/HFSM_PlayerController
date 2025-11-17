using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField] 
    private Vector3 _delta = new Vector3(0.0f, 8.0f, -7.0f);

    [SerializeField] 
    private GameObject _player = null;
    
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            RaycastHit hit;
            if(Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
            {
                float dist = (hit.point - _player.transform.position).magnitude * 0.8f;

                transform.position = (_player.transform.position + Vector3.up) + _delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                // transform.LookAt(_player.transform);
            }
        }
    }
}
