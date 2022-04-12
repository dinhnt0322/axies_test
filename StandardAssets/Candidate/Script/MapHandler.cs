using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class MapHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject attackAxies;
    public GameObject defendAxies;
    public Dictionary<Coordinates, GameObject> axiesList = new Dictionary<Coordinates, GameObject>();
    public Tilemap tileMap;
    public static MapHandler instance;
    public delegate void HandleAxies(string action);
    public HandleAxies handleAxiesCallback;
    public delegate void ChangeAxiesNum(int attNum, int defNum);
    public ChangeAxiesNum changeAxiesNumCallback;
    private int attackMemNum = 0;
    private int defendMemNum = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        Utils utils = new Utils();
        InstantiateAxiesInMap(defendAxies,new Coordinates(0,0), false, 2,0);
        InstantiateAxiesInMap(attackAxies, new Coordinates(0, 0), true, 5, 2);
    }

    public void StartGame()
    {
        if (handleAxiesCallback != null)
            handleAxiesCallback.Invoke("PLAY");
    }

    public void PauseGame()
    {
        if (handleAxiesCallback != null)
            handleAxiesCallback.Invoke("PAUSE");
    }

    public void HandleAxiesNum(string actionTag , string gameObjectTag )
    {
        if(actionTag == "DECREASE")
        {
            if (gameObjectTag == "DEFEND_MEM")
                defendMemNum--;
            if (gameObjectTag == "ATTACK_MEM")
                attackMemNum--;
        }
        if (actionTag == "INCREASE")
        {
            if (gameObjectTag == "DEFEND_MEM")
                defendMemNum++;
            if (gameObjectTag == "ATTACK_MEM")
                attackMemNum++;
        }
        changeAxiesNumCallback.Invoke(attackMemNum,defendMemNum);
    }

    void InstantiateAxiesInMap(GameObject axiesObject, Coordinates defaultCoordinates, bool isHollow, int radius,int thickness )
    {
        Utils utils = new Utils();
        List<Coordinates> fullCoordinates = utils.FindAllCoordinatesByRadius(defaultCoordinates, radius);
        if (!isHollow)
        {
            foreach(Coordinates coordinates in fullCoordinates)
            {
                InstantiateAxiesWithHexPosition(coordinates, axiesObject);
            }
        }
        else
        {

            List<Coordinates> insideCoordinates = utils.FindAllCoordinatesByRadius(defaultCoordinates, radius - thickness);
            Dictionary<string, bool> insideCoordinatesDictionary = new Dictionary<string, bool>();
            foreach (Coordinates coordinate in insideCoordinates)
            {
                insideCoordinatesDictionary.Add($"x:{coordinate.x},y:{coordinate.y}", true);
            }
            foreach (Coordinates coordinate in fullCoordinates)
            {
                if(!insideCoordinatesDictionary.ContainsKey($"x:{coordinate.x},y:{coordinate.y}"))
                    InstantiateAxiesWithHexPosition(coordinate, axiesObject);
            }
        }
    }

    void InstantiateAxiesWithHexPosition(Coordinates hexPosition, GameObject gameObject)
    {
        Vector2 position = tileMap.CellToWorld(new Vector3Int(hexPosition.x, hexPosition.y, 0));
        GameObject axiesObj = Object.Instantiate(gameObject, new Vector2(position.x, position.y), Quaternion.identity, tileMap.transform);
        HandleAxiesNum("INCREASE",axiesObj.tag);
        axiesList.Add(hexPosition, axiesObj);
    }

}

public class Utils
{
    public List<Coordinates> FindAllCoordinatesByRadius(Coordinates defaultCoordinates, int radius)
    {
        List<Coordinates> listCoordinates = new List<Coordinates>();
        for(int i = defaultCoordinates.y - radius, currRadius = -radius; i <= defaultCoordinates.y + radius; i++, currRadius++)
        {
            for(int j = defaultCoordinates.x - radius; j <= defaultCoordinates.x + radius; j++)
            {
                int left = currRadius / 2;
                int right = currRadius - left;
                if (defaultCoordinates.y % 2 != 0) {
                    right = currRadius / 2;
                    left = currRadius - right;
                }
                if (currRadius < 0 && j <= defaultCoordinates.x + radius + right && j >= defaultCoordinates.x - radius - left)
                    listCoordinates.Add(new Coordinates(j, i));
                else if (currRadius >= 0 && j <= defaultCoordinates.x + radius - right && j >= defaultCoordinates.x - radius + left)
                    listCoordinates.Add(new Coordinates(j, i));
            }
        }
        return listCoordinates;
    }

    public Dictionary<Coordinates,int> ConvertCoordinatesToGraph(List<Coordinates> coordinates)
    {
        Dictionary<Coordinates, int> returnValue = new Dictionary<Coordinates, int>();
        for(int i = 0; i < coordinates.Count; i++)
        {
            returnValue.Add(coordinates[i],i);
        }
        return returnValue;
    }
    public int GetTileDistance(Coordinates firstCoordinates, Coordinates secondCoordinates)
    {
        int dx = secondCoordinates.x - firstCoordinates.x;     // signed deltas
        int dy = secondCoordinates.y - firstCoordinates.y;
        int x = Mathf.Abs(dx);  // absolute deltas
        int y = Mathf.Abs(dy);
        // special case if we start on an odd row or if we move into negative x direction
        if ((dx < 0) ^ ((firstCoordinates.x & 1) == 1))
            x = Mathf.Max(0, x - (y + 1) / 2);
        else
            x = Mathf.Max(0, x - (y) / 2);
        return x + y;
    }
}

public struct Coordinates
{
    public int x;
    public int y;
    public Coordinates(int x, int y) {
        this.x = x;
        this.y = y;
    }
}