using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
	[SerializeField] protected int reflections;
    [SerializeField] protected float maxLength;

    private LineRenderer m_LineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 direction;

	private void Awake()
	{
        m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		ray = new Ray(transform.position, transform.forward);

		m_LineRenderer.positionCount = 1;
		m_LineRenderer.SetPosition(0, transform.position);
		float remainingLength = maxLength;

		for (int i = 0; i < reflections; i++)
		{
			if(Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength))
			{
				m_LineRenderer.positionCount += 1;
				m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, hit.point);
				remainingLength -= Vector3.Distance(ray.origin, hit.point);
				ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
				if (hit.collider.tag != "Wall")
					break;
			}
			else
			{
				m_LineRenderer.positionCount += 1;
				m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
			}
		}
	}
}
