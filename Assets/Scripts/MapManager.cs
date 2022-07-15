using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// map manager
/// </summary>
public class MapManager : MonoBehaviour {

	//マップデータ
	public List<GameObject[]> mapList = new List<GameObject[]>();

	private GameObject m_prefab_tile;
	private GameObject m_prefab_wall;
	private GameObject m_prefab_spike;
	private GameObject m_prefab_skySpike;
	private GameObject m_prefab_gem;

	private Transform m_Transform;
	private PlayerController m_PlayerController;

	public float bottomLength = Mathf.Sqrt(2) * 0.254f;

	private Color colorOne = new Color(124/255f, 155/255f, 230/255f);
	private Color colorTwo = new Color(125/255f, 169/255f, 233/255f);
	private Color colorWall = new Color(87/255f, 93/255f, 169/255f);

	private int index = 0;

	//probability
	private int prob_hole = 0;
	private int prob_spike = 0;
	private int prob_skySpike = 0;
	private int prob_gem = 2;

	void Start () {
		m_prefab_tile = Resources.Load("tile_white") as GameObject;
		m_prefab_wall = Resources.Load("wall2") as GameObject;
		m_prefab_spike = Resources.Load("moving_spikes") as GameObject;
		m_prefab_skySpike = Resources.Load("smashing_spikes") as GameObject;
		m_prefab_gem = Resources.Load("gem 2") as GameObject;

		m_PlayerController = GameObject.Find("cube_books").GetComponent<PlayerController>();
		m_Transform = gameObject.GetComponent<Transform>();
		CreateMapItem(0);
		
	}
	
	/// <summary>
    /// Create map elements
    /// </summary>
	public void CreateMapItem(float offsetZ){
		for(int i = 0;i < 10; i++){
			GameObject[] item = new GameObject[7];
			for(int j = 0; j < 7; j++){
				Vector3 pos = new Vector3(j * bottomLength, 0, offsetZ + i * bottomLength);
				Vector3 rot = new Vector3(-90, 45 ,0);
				GameObject tile = null;
				if(j == 0||j ==6){
					tile = GameObject.Instantiate(m_prefab_wall, pos, Quaternion.Euler(rot)) as GameObject;
					tile.GetComponent<Transform>().GetComponent<MeshRenderer>().material.color = colorWall;				
				}else{
					int type = CalcType();
					switch(type){
						case 0:
							tile = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;
							tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;
							tile.GetComponent<Transform>().GetComponent<MeshRenderer>().material.color = colorOne;

							break;
						case 1://hole
							tile = new GameObject();
							tile.GetComponent<Transform>().position = pos;
							tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
							break;
						case 2://gimmick(ground)
						    tile = GameObject.Instantiate(m_prefab_spike, pos, Quaternion.Euler(rot)) as GameObject;
							break;
						case 3://gimmick(from sky)
						    tile = GameObject.Instantiate(m_prefab_skySpike, pos, Quaternion.Euler(rot)) as GameObject;
							break;
					}

					
				}
				tile.GetComponent<Transform>().SetParent(m_Transform);
				item[j] = tile;
			}
			mapList.Add(item);

			GameObject[] item2 = new GameObject[6];
			for(int j = 0; j < 6; j++){
				Vector3 pos = new Vector3(j * bottomLength + bottomLength / 2, 0, offsetZ + i * bottomLength + bottomLength / 2);
				Vector3 rot = new Vector3(-90, 45 ,0);
				GameObject tile = null;

				int type = CalcType();
				switch(type){
						case 0:
							tile = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;
							tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
							tile.GetComponent<Transform>().GetComponent<MeshRenderer>().material.color = colorTwo;

							if(CalcType_Gem() == 1){
								GameObject gem = GameObject.Instantiate(m_prefab_gem, tile.GetComponent<Transform>().position + new Vector3(0.0f, 0.07f, 0.0f), 
								Quaternion.identity) as GameObject;
								//generate gems
								gem.GetComponent<Transform>().SetParent(tile.GetComponent<Transform>());
							}
							break;
						case 1://hole
							tile = new GameObject();
							tile.GetComponent<Transform>().position = pos;
							tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
							break;
						case 2://gimmick(ground)
						    tile = GameObject.Instantiate(m_prefab_spike, pos, Quaternion.Euler(rot)) as GameObject;
							break;
						case 3://gimmick(from sky)
						    tile = GameObject.Instantiate(m_prefab_skySpike, pos, Quaternion.Euler(rot)) as GameObject;
							break;
					}			

				
				tile.GetComponent<Transform>().SetParent(m_Transform);
				item2[j] = tile;
			}
			mapList.Add(item2);
		}
	}

	
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Space)){
			for(int i = 0; i < mapList.Count; i++){
				string str = "";
				for(int j = 0; j < mapList[i].Length; j++){
					str += mapList[i][j].name;
					mapList[i][j].name = i + "--" + j;
					
				}
				Debug.Log(str);
			}
		}
	}

 	/// <summary>
    /// turn on the ground fall
    /// </summary>
	public void StartTileFall(){
		StartCoroutine("TileFall");
	}

 	/// <summary>
    /// turn off the ground fall
    /// </summary>
	public void StopTileFall(){
		StopCoroutine("TileFall");
	}

	/// <summary>
    /// ground(tile) fall
    /// </summary>
	private IEnumerator TileFall(){
		while (true)
		{
			yield return new WaitForSeconds(0.65f);
			for(int j = 0;j < mapList[index].Length; j++){
				Rigidbody rigidbody = mapList[index][j].AddComponent<Rigidbody>();
				rigidbody.angularVelocity = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) * Random.Range(1.0f, 10.0f);
				GameObject.Destroy(mapList[index][j], 1.0f);
			}
			
			if(m_PlayerController.z == index){
				StopTileFall();
				m_PlayerController.gameObject.AddComponent<Rigidbody>();
				m_PlayerController.StartCoroutine("GameOver", true);
			}
			index++;
		}
	}

	/// <summary>
    /// 0:tile
    /// 1:hole
    /// 2:gimmick(ground)
    /// 3:gimmick(from sky)
    /// </summary>
	private int CalcType(){

		int random = Random.Range(1,100);
		if(random < prob_hole){
			return 1;
		}else if(31 < random && random < prob_spike + 30){
			return 2;
		}else if(61 < random && random < prob_skySpike + 60){
			return 3;
		}

		return 0;
	}
	
	/// <summary>
    /// calculate gem generation probability
    /// </summary>
    /// <returns>0:donnot generate, 1:generate</returns>
	private int CalcType_Gem(){
		int random = Random.Range(1, 100);
	
		if(random <= prob_gem){
			return 1;
		}

		return 0;
	}

	public void AddProp(){
		prob_hole += 2;
		prob_spike += 1;
		prob_skySpike += 2;
	}

	public void ResetGameMap()
    {
        Transform[] sonTransform = m_Transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < sonTransform.Length; i++)
        {
            GameObject.Destroy(sonTransform[i].gameObject);
        }

        prob_hole = 0;
        prob_spike = 0;
        prob_skySpike = 0;
        prob_gem = 2;

        index = 0;

        mapList.Clear();

        CreateMapItem(0);
    }
}
