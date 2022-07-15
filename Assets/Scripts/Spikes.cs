using UnityEngine;
using System.Collections;

/// <summary>
/// gimmick(ground)
/// </summary>
public class Spikes : MonoBehaviour {

	private Transform m_Transform;
	private Transform sub_Transform;

	private Vector3 startPos;
	private Vector3 endPos;

	void Start () {
		m_Transform = gameObject.GetComponent<Transform>();
		sub_Transform = m_Transform.FindChild("moving_spikes_b").GetComponent<Transform>();

		startPos = sub_Transform.position;
		endPos = sub_Transform.position + new Vector3(0, 0.15f, 0);

		StartCoroutine("GoUpDown");
	}

	private IEnumerator GoUpDown(){
		while(true){
			StopCoroutine("GoDown");
			StartCoroutine("GoUp");
			yield return new WaitForSeconds(2.0f);
			StopCoroutine("GoDown");
			StopCoroutine("GoUp");
			StartCoroutine("GoDown");
			yield return new WaitForSeconds(2.0f);
			//StopCoroutine("GoDown");
		}
	}

	private IEnumerator GoUp(){
		while(true){
			sub_Transform.position = Vector3.Lerp(sub_Transform.position, endPos, Time.deltaTime * 10);
			yield return null;
		}
	}

	private IEnumerator GoDown(){
		while(true){
			sub_Transform.position = Vector3.Lerp(sub_Transform.position, startPos, Time.deltaTime * 10);
			yield return null;
		}
	}
}
