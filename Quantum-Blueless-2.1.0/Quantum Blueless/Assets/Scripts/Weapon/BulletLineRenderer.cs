using UnityEngine;

public class BulletLineRenderer : MonoBehaviour
{
	public float lenght = 1;
	public LineRenderer lr;

	private Vector3 lastPos;
	
	private void Start()
	{
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position);
		lastPos = transform.position;
	}

	private void Update()
	{
		var direction = Vector3.Normalize(transform.position - lastPos);

		lr.SetPosition(0, transform.position + direction / lenght);
		lr.SetPosition(1, transform.position + direction / lenght - direction * lenght);

		lastPos = transform.position;
	}
}