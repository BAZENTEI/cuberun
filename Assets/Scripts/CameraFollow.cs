using UnityEngine;
using System.Collections;

/// <summary>
/// Smooth Camera Follow
/// </summary>
public class CameraFollow : MonoBehaviour {
	private Transform m_Transform;
	private Transform m_player;

	public bool isFollow = false;
	private Vector3 normalPos;

	void Start () {
		m_Transform = gameObject.GetComponent<Transform>();
		m_player = GameObject.Find("cube_books").GetComponent<Transform>();
		normalPos = m_Transform.position;		
	}
	
	
	void Update () {
		CameraMove();
	}

	void CameraMove(){
		if(isFollow){
			
			Vector3 pos = new Vector3(m_Transform.position.x, m_player.position.y + 1.45f, m_player.position.z);
		
			m_Transform.position = Vector3.Lerp(m_Transform.position, pos, 1/60.0f);
		}
	}

	public void ResetCamera() {
        m_Transform.position = normalPos;
    }

}
