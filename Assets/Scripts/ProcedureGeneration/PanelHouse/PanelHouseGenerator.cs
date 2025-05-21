using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PanelHouseGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] doorPrefabs;
    [SerializeField] private GameObject[] stairsPrefabs;
    [SerializeField] private GameObject[] fullWallPrefabs;
    [SerializeField] private GameObject[] bacloniesPrefabs;
    [SerializeField] private GameObject lowerWallPrefab;
    [SerializeField] private GameObject upperWallPrefab;
    
    [Header("Generate Parameters")]
    [SerializeField] private int numberOfFloors = -1;
    [SerializeField] private int maxNumFloor = 16;
    [SerializeField] private int width = -1;
    [SerializeField] private int maxWidth = 6;
    [SerializeField] private PanelGenerateType panelType;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform tr;

    private int turn = 0;
    private int wallInRow = 0;
    
    private const int MINIMUM_PLATES = 9; 
    private const int MAX_PLATES = 100;
    private const int MAX_ONE_WAY_TURNS = 3;
    


    private List<PanelPart> generatedPanels;

    private void Awake()
    {
        generatedPanels = new List<PanelPart>();
        generatedPanels.Add(PanelPart.Door);
        if (numberOfFloors < 2)
        {
            numberOfFloors = Random.Range(2, maxNumFloor);
        }

        if (panelType == null)
        {
            panelType = GetRandomPanelType();
        }

        if (width < 2)
        {
            width = Random.Range(2, maxWidth);
        }

        if (startPoint == null)
        {
            startPoint = gameObject.transform;
        }
        
        //StartGeneration();
        /*tr = new GameObject().transform;*/
    }

    private PanelGenerateType GetRandomPanelType()
    {
        // Получаем все значения enum
        PanelGenerateType[] values = (PanelGenerateType[])System.Enum.GetValues(typeof(PanelGenerateType));
        // Выбираем случайный индекс
        int randomIndex = Random.Range(0, values.Length);
        // Возвращаем случайное значение
        return values[randomIndex];
    }

    public void StartGeneration()
    {
        if (panelType == PanelGenerateType.Classic)
        {
            GenerateClassicHouse();
        }
    }

    private void GenerateClassicHouse()
    {
        bool canStop = false;
        bool mustStop = false;
        turn = 0;
        
        
        tr.position = startPoint.position;
        tr.rotation = startPoint.rotation;
        tr.rotation *= Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < width; i++)
        {
            tr.position += tr.forward * 5;
            tr.position = new Vector3(tr.position.x, startPoint.position.y, tr.position.z);
            GenerateFullWall(tr);
        }
        while (generatedPanels.Count < MAX_PLATES)
        {
            int random = Random.Range(0, 100);
            if (generatedPanels.Count > MINIMUM_PLATES)
                canStop = true;
            
            if(random > 90 && canStop)
                mustStop = true;
            
            if (mustStop && wallInRow >= width) // Stop generation, close hole
            {
                tr.rotation *= Quaternion.Euler(0, 90, 0);
                for (int i = 0; i < width; i++)
                {
                    tr.position += tr.forward * 5;
                    tr.position = new Vector3(tr.position.x, startPoint.position.y, tr.position.z);
                    GenerateFullWall(tr);
                }
                return; 
            }
            
            
            tr.position = startPoint.position;
            tr.rotation = startPoint.rotation;
            
            TurnHandler(tr);  
            
            tr.position = startPoint.position;
            tr.rotation = startPoint.rotation;

            if (generatedPanels[generatedPanels.Count - 1] != PanelPart.Door)
            {
                random = Random.Range(0, 100);
                if (random <= 50)
                {
                   
                    GenerateDoorPanel(tr);
                    generatedPanels.Add(PanelPart.Door);
                    startPoint.position += startPoint.forward * -5;
                }
                else
                {
                    GenerateBalconyPanel(tr);
                    generatedPanels.Add(PanelPart.Balcony);
                    startPoint.position += startPoint.forward * -5;

                }
            }
            else
            {
                GenerateBalconyPanel(tr);
                generatedPanels.Add(PanelPart.Balcony);
                startPoint.position += startPoint.forward * -5;
            }
            wallInRow++;
            // Generate Opposite Walls
            tr.rotation *= Quaternion.Euler(0, 180, 0);
            tr.position += tr.right * -5 * width;
            tr.position = new Vector3(tr.position.x, startPoint.position.y, tr.position.z);
            GenerateBalconyPanel(tr);
        }
    }

    private void TurnHandler(Transform tr)
    {
        int random = Random.Range(0, 100);
        if (random <= 20 && Math.Abs(turn) <= 3 && wallInRow > width)
        {
            wallInRow = 0;
            if (random % 2 == 0 || turn == -2)
            {
                turn = turn > 0 ? turn + 1 : 1;
                startPoint.rotation *= Quaternion.Euler(0, 90, 0);
                tr.rotation = startPoint.rotation;
                Debug.Log(tr.position);
                CornerGenerator(tr);
            }
            else
            {
                turn = turn > 0 ? -1 : turn - 1;
                startPoint.rotation *= Quaternion.Euler(0, -90, 0);

            }
            
        }
    }

    private void CornerGenerator(Transform tran)
    {
        tran.position += tran.forward * 5 * width;
        tran.rotation *= Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < width; i++)
        {
            tran.position += tran.forward * 5;
            GenerateFullWall(tran);
            
            tran.position = new Vector3(tran.position.x, startPoint.position.y, tran.position.z);
        }
        
        tran.position = new Vector3(tran.position.x, startPoint.position.y, tran.position.z);
        tran.rotation *= Quaternion.Euler(0, 90, 0);
        for (int i = 0; i < width; i++)
        {
            tran.position += tran.forward * 5;
            GenerateFullWall(tran);
            tran.position = new Vector3(tran.position.x, startPoint.position.y, tran.position.z);
        }
    }
    private void GenerateDoorPanel(Transform tr)
    {
        //tr.position = new Vector3(tr.position.x, tr.position.y + PlaceLowerWall(tr), tr.position.z);
        tr.position = new Vector3(tr.position.x, tr.position.y + PlaceDoorWall(tr), tr.position.z);
        for (int i = 0; i < numberOfFloors - 1; i++)
        {
            tr.position = new Vector3(tr.position.x, tr.position.y + PlaceStairsWall(tr), tr.position.z);
        }
        tr.position = new Vector3(tr.position.x, tr.position.y + PlaceUpperWall(tr), tr.position.z);
        
    }

    private void GenerateBalconyPanel(Transform tr)
    {
        tr.position = new Vector3(tr.position.x, tr.position.y + PlaceLowerWall(tr), tr.position.z);
        for (int i = 0; i < numberOfFloors; i++)
        {
            tr.position = new Vector3(tr.position.x, tr.position.y + PlaceBalconyWall(tr), tr.position.z);
        }
        tr.position = new Vector3(tr.position.x, tr.position.y + PlaceUpperWall(tr), tr.position.z);
    }
    private void GenerateFullWall(Transform tr)
    {
        tr.position = new Vector3(tr.position.x, tr.position.y + PlaceLowerWall(tr), tr.position.z);
        for (int i = 0; i <= numberOfFloors; i++)
        {
            tr.position = new Vector3(tr.position.x, tr.position.y + PlaceUpperWall(tr), tr.position.z);
        }
        
    }
    private float PlaceBalconyWall(Transform tr)
    {
        int randomIndex = Random.Range(0, bacloniesPrefabs.Length);
        GameObject gb = Instantiate(bacloniesPrefabs[randomIndex], tr.position, tr.rotation);
        return GetPrefabSize(gb).y;
    }


    private float PlaceDoorWall(Transform tr)
    {
        int randomIndex = Random.Range(0, doorPrefabs.Length);
        GameObject gb = Instantiate(doorPrefabs[randomIndex], tr.position, tr.rotation);
        return GetPrefabSize(gb).y;
    }
    private float PlaceLowerWall(Transform tr)
    {
        GameObject gb = Instantiate(lowerWallPrefab, tr.position, tr.rotation);
        return GetPrefabSize(gb).y;
    }
    private float PlaceUpperWall(Transform tr)
    {
        GameObject gb = Instantiate(upperWallPrefab, tr.position, tr.rotation);
        return GetPrefabSize(gb).y;
    }
    private float PlaceStairsWall(Transform tr)
    {
        int randomIndex = Random.Range(0, stairsPrefabs.Length);
        GameObject gb = Instantiate(stairsPrefabs[randomIndex], tr.position, tr.rotation);
        return GetPrefabSize(gb).y;
    }
    
    private Vector3 GetPrefabSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        return renderer ? renderer.bounds.size : Vector3.zero;
    }
   
    public Bounds GetHouseBounds()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }


}

public enum PanelGenerateType
{
    Classic,
    Checkmate,
    Centered,
}

public enum PanelPart
{
    FullWall,
    Door,
    Balcony,
    Window
}
