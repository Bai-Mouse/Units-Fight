using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> GreenTeam = new List<GameObject>();
    public List<GameObject> RedTeam = new List<GameObject>();
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
    public GameMode gameMode;
    public GameObject SelectedUnit;
    public GameObject[] SelectableUnits;
    public bool Pause;
    void Start()
    {
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
            Wave++;
            newWave();
        }
    }
    private void Update()
    {
        if (SelectedUnit != null)
        {
            Cursor.SetActive(true);
            Vector3 mouseScreenPosition = Input.mousePosition;

            // 将屏幕坐标转换为世界坐标
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane + 10f));


            Cursor.transform.position += new Vector3((worldPosition.x - Cursor.transform.position.x) / 20, (worldPosition.y - Cursor.transform.position.y) / 20);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {

                // 生成单位
                Instantiate(SelectedUnit, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
            }
        }
        else
        {
            Cursor.SetActive(false);
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

        // 累加所有单位的坐标
        if (GreenTeam.Count > 0)
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
        if(RedTeam.Count>0)
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


        Vector3 averagePosition = sum / (GreenTeam.Count + RedTeam.Count);
        return averagePosition;
    }
    public void setUnit(int i)
    {
        SelectedUnit = SelectableUnits[i];
    }
    public void newWave()
    {
        Vector2 spawnpoint = new Vector2(Random.Range(-15, 15), Random.Range(-15, 15));
        for (int i = 0; i < SelectableUnits.Length; i++)
        {
            GameObject units = Instantiate(SelectableUnits[i]);
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
