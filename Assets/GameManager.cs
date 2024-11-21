using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public enum GameMode
    {
        Free,
        Defend,
        Pause,
    }
    public int Wave = 1;
    public int Money = 0;
    public GameMode gameMode;
    public CharacterData SelectedUnit;
    public CharacterData[] SelectableUnits;
    public bool Pause;
    public TextMeshProUGUI MoneyText;
    public UnitsInfo UnitsInfo;
    void Start()
    {
        addMoney(100);
        Pause = true;
        Units.SetActive(false);
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    private void FixedUpdate()
    {
        SetCameraToAveragePosition();
        if (RedTeam.Count==0&& !Pause)
        {
            wavetimer += Time.fixedDeltaTime;
            if (wavetimer > 3)
            {
                Wave++;
                newWave();
                wavetimer = 0;
            }

        }
    }
    public void addMoney(int money)
    {
        Money += money;
        MoneyText.text = "Money: "+Money.ToString();
        if(SelectedUnit)
        UnitsInfo.setColor( Money > SelectedUnit.cost ? Color.green : Color.red);
    }

    private void Update()
    {
        if (SelectedUnit != null)
        {
            Cursor.SetActive(true);
            Vector3 mouseScreenPosition = Input.mousePosition;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane + 10f));


            Cursor.transform.position += new Vector3((worldPosition.x - Cursor.transform.position.x) / 20, (worldPosition.y - Cursor.transform.position.y) / 20);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if(Money >= SelectedUnit.cost)
                {
                    addMoney(SelectedUnit.cost * -1);
                    Instantiate(SelectedUnit.instance, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
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
        mainCamera.transform.position += new Vector3((averagePosition.x- mainCamera.transform.position.x)/5, (averagePosition.y- mainCamera.transform.position.y)/5, 0); // 保持原有的 Z 坐标
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
        

        Vector3 averagePosition = (RedCenter+ GreenCenter)/2;
        return averagePosition;
    }
    public void setUnit(int i)
    {
        SelectedUnit = SelectableUnits[i];
        
        UnitsInfo.transform.GetComponent<Animator>().SetBool("show", true);
        UnitsInfo.transform.GetComponent<Animator>().SetTrigger("showanim");
        UnitsInfo.setInfo(SelectedUnit.icon, "REQUIRED MONEY: " + SelectedUnit.cost.ToString());
    }

    public void newWave()
    {
        Vector2 spawnpoint = new Vector2(-39.05838f + Random.Range(-10, 10), Random.Range(-10, 10));
        for (int i = 0; i < SelectableUnits.Length; i++)
        {
            GameObject units = Instantiate(SelectableUnits[i].instance);
            units.transform.position = spawnpoint+ new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            units.transform.SetParent(Units.transform);
            units.tag = "RedTeam";
        }
    }

    public void NewGame()
    {
        Pause = false;
        Units.SetActive(true);
    }
    // Update is called once per frame
}
