using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> GreenTeam = new List<GameObject>();
    public List<GameObject> RedTeam = new List<GameObject>();
    public Camera mainCamera;
    public GameObject GreenTeamTarget;
    public GameObject RedTeamTarget;
    public enum GameMode
    {
        Free,
        Defend
    }
    public GameMode gameMode;
    public GameObject SelectedUnit;
    public GameObject[] SelectableUnits;
    void Start()
    {

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    private void FixedUpdate()
    {
        SetCameraToAveragePosition();
        
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
        foreach (GameObject unit in GreenTeam)
        {
            if (unit != null)
            {
                sum += unit.transform.position;
            }
        }

        foreach (GameObject unit in RedTeam)
        {
            if (unit != null)
            {
                sum += unit.transform.position;
            }
        }


        Vector3 averagePosition = sum / (GreenTeam.Count + RedTeam.Count);
        return averagePosition;
    }
    public void setUnit(int i)
    {
        SelectedUnit = SelectableUnits[i];
    }
    // Update is called once per frame
}
