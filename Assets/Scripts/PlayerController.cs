using UnityEngine;
using System.Collections;

///プレイヤー制御
public class PlayerController : MonoBehaviour {

	private Transform m_Transform;
	private MapManager m_MapManager;

	public int z = 3, x = 4;

	private Color colorOne = new Color(122/255f, 85/255f, 179/255f);
	private Color colorTwo = new Color(126/255, 93/255f, 183/255f);

	private CameraFollow m_CameraFollow;
	private UIManager m_UIManager;

	private bool life = true;
	private int gemCount = 0;
	private int scoreCount = 0;

	void Start () {
		gemCount = PlayerPrefs.GetInt("gem", 0);
		m_Transform = gameObject.GetComponent<Transform>();
		m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
		m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
	    m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();
	}
	
	public void StartGame()
    {
        SetPlayerPos();
        m_CameraFollow.isFollow = true;
        m_MapManager.StartTileFall();
    }

	void Update () {
		// if(Input.GetKeyDown(KeyCode.M)){
        //     StartGame();
        // }

		if(life){
			PlayerControl();	
		}	
	}

	/// <summary>
    /// player control
    /// </summary>
	private void PlayerControl(){
		// if(Input.GetKeyDown(KeyCode.M)){
		// 	SetPlayerPos();
		// 	m_CameraFollow.isFollow = true;	
		// 	m_MapManager.StartTileFall();
		// }

		//left.
        if (Input.GetKeyDown(KeyCode.A)){
            Left();
        }

        //right.
        if (Input.GetKeyDown(KeyCode.D)){
            Right();
        }
	
	}

	public void Left(){
        if (x != 0){
            z++;
            AddScoreCount();
        }

        if (z % 2 == 1 && x != 0){
            x--;
        }
        Debug.Log("Left:z:" + z + "--x:" + x);
        SetPlayerPos();
        CalcPosition();
    }

    public void Right()
    {
        if (x != 5 || z % 2 == 0)
        {
            z++;
            AddScoreCount();
        }

        if (z % 2 == 0 && x != 5)
        {
            x++;
        }
        Debug.Log("Right:z:" + z + "--x:" + x);
        SetPlayerPos();
        CalcPosition();
    }

	/// <summary>
    /// generate trailing effect
    /// </summary>
	void SetPlayerPos(){
		Transform playerPos =  m_MapManager.mapList[z][x].GetComponent<Transform>();
		m_Transform.position = playerPos.position + new Vector3(0, 0.254f/2, 0);
		m_Transform.rotation = playerPos.rotation;

		
		MeshRenderer gimmick = null;
		if(playerPos.tag == "Tile"){
			gimmick = playerPos.FindChild("normal_a2").GetComponent<MeshRenderer>();
		}else if(playerPos.tag == "Spike"){
			gimmick = playerPos.FindChild("moving_spikes_a2").GetComponent<MeshRenderer>();
		}else if(playerPos.tag == "SkySpike"){
 		    gimmick = playerPos.FindChild("smashing_spikes_a2").GetComponent<MeshRenderer>();
		}

		
		if(z % 2 == 0 && gimmick != null){
			gimmick.material.color = colorOne;
		}else if(z % 2 == 1 && gimmick != null){
			gimmick.material.color = colorTwo;
		}else if(gimmick == null){
			gameObject.AddComponent<Rigidbody>();
			StartCoroutine("GameOver", true);
		}
		
	}

	/// <summary>
    /// calculate player position
    /// </summary>
	private void CalcPosition(){
		if(m_MapManager.mapList.Count - (z +1) <= 12){
			Debug.Log("dynamic load");

			m_MapManager.AddProp();
			float offsetZ = m_MapManager.mapList[m_MapManager.mapList.Count - 1][0].GetComponent<Transform>().position.z + m_MapManager.bottomLength / 2;
			m_MapManager.CreateMapItem(offsetZ);
			
		}
	}

	private void OnTriggerEnter(Collider coll){
		Debug.Log(coll.gameObject);
		if(coll.tag == "SpikeThorns"){
			StartCoroutine("GameOver", false);
		}

		if(coll.tag == "Gem"){
			GameObject.Destroy(coll.gameObject.GetComponent<Transform>().parent.gameObject);
			AddGemCount();
		}
	}

	public IEnumerator GameOver(bool b){
		if(b){
			yield return new WaitForSeconds(0.5f);
		}

		if(life){
			m_CameraFollow.isFollow = false;
			Debug.Log("gameover");
			life = false;
			SaveData();
			StartCoroutine("ResetGame");
		}
		
		
	}

	private void AddGemCount(){
		gemCount++;
		Debug.Log("gem count:" + gemCount);
		m_UIManager.UpdateData(scoreCount, gemCount);
	}

	private void AddScoreCount(){
		scoreCount++;
		Debug.Log("score:" + scoreCount); 
		m_UIManager.UpdateData(scoreCount, gemCount);
	}

	private void SaveData(){
		PlayerPrefs.SetInt("gem", gemCount);
		if(scoreCount > PlayerPrefs.GetInt("score", 0)){
			PlayerPrefs.SetInt("score", scoreCount);
		}

	}

    private void ResetPlayer(){
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>());
        
        z = 3;
        x = 2;

        life = true;

        scoreCount = 0;
    }

	private IEnumerator ResetGame(){
        yield return new WaitForSeconds(2);
        ResetPlayer();
        m_MapManager.ResetGameMap();
        m_CameraFollow.ResetCamera();

        m_UIManager.ResetUI();
    }
}
