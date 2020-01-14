using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class Demo_ClickToMove : MonoBehaviour
{
    public NavMeshAgent m_Agent;
	public Camera _mainCamera;

    RaycastHit m_HitInfo = new RaycastHit();

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)/* && !Input.GetKey(KeyCode.LeftShift)*/)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
			{
                m_Agent.destination = m_HitInfo.point;
				Debug.Log(m_HitInfo);
			}
        }
    }
}
