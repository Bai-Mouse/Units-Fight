using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> GreenTeam = new List<GameObject>();
    public List<GameObject> RedTeam = new List<GameObject>();
    public Vector3 GreenCenter, RedCenter;
    float wavetimer;
    public Camera mainCamera;
    public GameObject GreenTeamTarget;
    public GameObject RedTeamTarget;
    public GameObject Cursor;
    public GameObject Units;
    public GameObject Turrut;
    public GameObject Canvas,RetryButton;
    public enum GameMode
    {
        Free,
        Defend,
        Pause,
    }
    public int Wave = 1;
    public int Money = 0;
    public int Unit =0;
    public GameMode gameMode;
    public CharacterData SelectedUnit;
    public CharacterData[] SelectableUnits;
    public bool Pause;
    public TextMeshProUGUI MoneyText,Counter,WaveCount,UnitsCount;
    public UnitsInfo UnitsInfo;
    public GameObject _turrut;
    public GameObject Boss;
    public bool ManualCam;
    public float sensitivity = 0.1f;

    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoom = 15f; // Minimum FOV or orthographic size
    public float maxZoom = 90f; // Maximum FOV or orthographic size
    void Start()
    {
        Unit = 0;
        for (int i = 0; i < SelectableUnits.Length; i++)
        {
            SelectableUnits[i] = Instantiate(SelectableUnits[i]);
        }
        addMoney(50);
        Pause = true;
        Units.SetActive(false);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    private void FixedUpdate()
    {

        if (RedTeam.Count==0&& !Pause&&_turrut)
        {
            if (gameMode == GameMode.Defend)
            {
                GreenCenter = transform.position;
            }
                Counter.gameObject.SetActive(true);
            wavetimer += Time.fixedDeltaTime;
            Counter.text = ((int)(10-wavetimer)).ToString();
            if (wavetimer > 10|| (Input.GetKey(KeyCode.Space)))
            {
                newWave();
                Wave++;
                WaveCount.text = "Wave: " + Wave.ToString();
                wavetimer = 0;
                Counter.gameObject.SetActive(false);
            }

        }
    }
    public void addMoney(int money)
    {
        Money += money;
        MoneyText.text = "Money: "+Money.ToString();
        if(SelectedUnit)
        UnitsInfo.setColor( Money >= SelectedUnit.cost ? Color.green : Color.red, Money);
    }
    public void addUnit(int unit)
    { 
        Unit += unit;
        UnitsCount.text="Units: "+Unit.ToString();
        if (Unit <= 0)
        {
            Unit = 0;
        }
        if (Unit >= 100)
        {
            UnitsCount.color=Color.red;
        }
        else
        {
            UnitsCount.color = Color.green;
        }
    }
    private void Update()
    {
        SetCameraToAveragePosition();
        if (SelectedUnit != null)
        {
            Cursor.SetActive(true);
            Vector3 mouseScreenPosition = Input.mousePosition;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane + 10f));


            Cursor.transform.position += new Vector3((worldPosition.x - Cursor.transform.position.x) / 5, (worldPosition.y - Cursor.transform.position.y) / 5);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if(Money >= SelectedUnit.cost&& Unit<100)
                {
                    addMoney(SelectedUnit.cost * -1);
                    MovementAI info = Instantiate(SelectedUnit.instance, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity).GetComponent<MovementAI>();
                    info.Damage = SelectedUnit.damage;
                    info.speed = SelectedUnit.speed;
                    info.Health = SelectedUnit.health;
                    info.Strength = SelectedUnit.strength;
                    info.CharacterData = SelectedUnit;
                    addUnit(SelectedUnit.occupancy);

                }
                if (Money < SelectedUnit.cost)
                {
                    SelectedUnit = null;
                    UnitsInfo.GetComponent<Animator>().SetBool("show", false);
                    Cursor.SetActive(false);
                }

            }
        }

        
    }
    public void SetCameraToAveragePosition()
    {
        Vector3 averagePosition = CalculateAveragePosition();
        if (ManualCam)
        {
            Vector3 deltamove = Vector3.zero;
            if (((Input.GetMouseButton(0)&& SelectedUnit == null )|| Input.GetMouseButton(2)) && !EventSystem.current.IsPointerOverGameObject())
            {
                 // Adjust as needed
                deltamove = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * sensitivity;

            }

            mainCamera.transform.position += deltamove;
        }
        else
        {
            
            mainCamera.transform.position += new Vector3((averagePosition.x - mainCamera.transform.position.x) / 10, (averagePosition.y - mainCamera.transform.position.y) / 10, 0);
        }
        if (mainCamera != null)
        {
            // Get the scroll input
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (mainCamera.orthographic)
            {
                // Adjust orthographic size for orthographic camera
                mainCamera.orthographicSize -= scrollInput * zoomSpeed;
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            }
            else
            {
                // Adjust Field of View (FOV) for perspective camera
                mainCamera.fieldOfView -= scrollInput * zoomSpeed;
                mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
            }
        }

    }
    public void CameraSetting()
    {
        if (ManualCam) ManualCam = false;
        else ManualCam = true;

    }

    private Vector3 CalculateAveragePosition()
    {

        if (GreenTeam.Count == 0 && RedTeam.Count == 0)
        {
            return mainCamera.transform.position;
        }

        Vector3 sum = Vector3.zero;
        
        if (GreenTeam.Count > 0)
        {
            foreach (GameObject unit in GreenTeam)
            {
                if (unit != null)
                {
                    sum += unit.transform.position;
                }
                else
                {
                    GreenTeam.Remove(unit);
                }
            }
            GreenCenter = sum / GreenTeam.Count;
        }
        sum = Vector3.zero;
        if (RedTeam.Count > 0)
        {
            foreach (GameObject unit in RedTeam)
            {
                if (unit != null)
                {
                    sum += unit.transform.position;
                }
                else
                {
                    RedTeam.Remove(unit);
                }
            }
            RedCenter = sum / RedTeam.Count;
        }
        Vector3 averagePosition;
        if (RedTeam.Count <= 1)
            averagePosition = GreenCenter;
        else if(GreenTeam.Count <= 1)
            averagePosition = RedCenter;
        else
            averagePosition = (RedCenter+GreenCenter)/2;
        return averagePosition;
    }
    public void setUnit(int i)
    {
        SelectedUnit = SelectableUnits[i];
        
        UnitsInfo.transform.GetComponent<Animator>().SetBool("show", true);
        UnitsInfo.transform.GetComponent<Animator>().SetTrigger("showanim");
        UnitsInfo.setInfo(SelectedUnit,Money);
        UnitsInfo.setColor(Money >= SelectedUnit.cost ? Color.green : Color.red,Money);
        UnitsInfo.setDescription(SelectedUnit.description);
    }

    public void newWave()
    {
        int WaveNum = Wave<24?1:2;
        _turrut.GetComponent<MovementAI>().Health += 10;
        _turrut.GetComponent<MovementAI>().Damage +=1f;
        _turrut.GetComponent<MovementAI>().GetHit(-10,Vector2.zero,0, _turrut);
        if ((Wave+1) % 10 == 0)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);

            float radius = Random.Range(15, 25);

            float x = transform.position.x + radius * Mathf.Cos(angle);
            float y = transform.position.y + radius * Mathf.Sin(angle);
            Vector2 spawnpoint = new Vector2(x, y);
            GameObject units = Instantiate(Boss);
            units.transform.position = spawnpoint + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            units.transform.SetParent(Units.transform);
            units.tag = "RedTeam";
            units.GetComponent<MovementAI>().Health += Wave*2 < 1200 ? Wave*2 : 1200;
            units.GetComponent<MovementAI>().Damage += (Wave / 2) < 30 ? Wave / 2 : 30;
            units.GetComponent<MovementAI>().TracingRange = 10000;
        }
        for (int i = 0; i < WaveNum; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);

            float radius = Random.Range(15, 25);

            float x = transform.position.x + radius * Mathf.Cos(angle);
            float y = transform.position.y + radius * Mathf.Sin(angle);

            Vector2 spawnpoint = new Vector2(x, y);
            int maxcount = (1+Mathf.Ceil(Wave / 2) < 12) ? 1+(int)Mathf.Ceil(Wave / 2) : 12;
            for (int j = 0; j < maxcount; j++)
            {
                int index = Random.Range(0, SelectableUnits.Length);
                int count=0;
                while (SelectableUnits[index].cost>(Wave+1)*10 && count<10)
                {
                    index = Random.Range(0, SelectableUnits.Length);
                    count++;
                }
                GameObject units = Instantiate(SelectableUnits[index].instance);
                units.transform.position = spawnpoint + new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                units.transform.SetParent(Units.transform);
                units.tag = "RedTeam";
                units.GetComponent<MovementAI>().Health += Wave < 400 ? Wave : 400;
                units.GetComponent<MovementAI>().Damage += (Wave / 2) < 30 ? Wave / 2: 30;
                units.GetComponent<MovementAI>().TracingRange = 10000;
            }
            
        }
    }

    public void NewGame()
    {
        if(gameMode == GameMode.Defend)
        {
            _turrut = Instantiate(Turrut);
            _turrut.transform.position = gameObject.transform.position;
        }
        Pause = false;
        Units.SetActive(true);
    }
    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Update is called once per frame
    public void UpgradeHealth()
    {
        int cost = (int)(1 + SelectedUnit.HPUpgradeCount);
        if (Money < cost)
        {
            return;
        }
        addMoney(-cost);
        SelectedUnit.HPUpgradeCount += 1;
        SelectedUnit.health += 2;
        UnitsInfo.setInfo(SelectedUnit,Money);

    }
    public void UpgradeDamage()
    {
        int cost = (int)(10 + SelectedUnit.DamUpgradeCount * 5);
        if (Money< cost)
        {
            return;
        }
        addMoney(-cost);
        SelectedUnit.DamUpgradeCount += 1;
        if (SelectedUnit.damage>0)
            SelectedUnit.damage += 1;
        else
            SelectedUnit.damage -= 1;
        UnitsInfo.setInfo(SelectedUnit,Money);
    }
}
