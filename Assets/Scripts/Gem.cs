using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {
	private Transform m_Transform;
	private Transform m_gem;

	void Start () {
		m_Transform = gameObject.GetComponent<Transform>();
		m_gem = m_Transform.FindChild("gem 3");
	}
	
	void Update () {
		//to do:random
		m_gem.Rotate(Vector3.up);
	}
}
