using UnityEngine;
using System.Collections;

/// <summary>
/// gimmick(from sky)
/// </summary>
public class SkySpike : MonoBehaviour {

	private Transform m_Transform;
	private Transform sub_Transform;

	private Vector3 startPos;
	private Vector3 endPos;

	void Start () {
		m_Transform = gameObject.GetComponent<Transform>();
		sub_Transform = m_Transform.FindChild("smashing_spikes_b").GetComponent<Transform>();

		startPos = sub_Transform.position;
		endPos = sub_Transform.position + new Vector3(0, 0.6f ,0);

		StartCoroutine("GoUpDown");
	}

	private IEnumerator GoUpDown(){
		while(true){
			StopCoroutine("GoDown");
			StartCoroutine("GoUp");
			yield return new WaitForSeconds(2.0f);
			StopCoroutine("GoUp");
			StartCoroutine("GoDown");
			yield return new WaitForSeconds(2.0f);
		}
	}

	private IEnumerator GoUp(){
		while(true){
			sub_Transform.position = Vector3.Lerp(sub_Transform.position, endPos, Time.deltaTime * 20);
			yield return null;
		}
	}

	private IEnumerator GoDown(){
		while(true){
			sub_Transform.position = Vector3.Lerp(sub_Transform.position, startPos, Time.deltaTime * 20);
			yield return null;
		}
	}


}
