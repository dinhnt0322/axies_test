using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axies : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth { get; private set; }
    public int serectRandomNum { get; private set; }
    public Transform heathBar;
    private void Awake()
    {
        currentHealth = maxHealth;
        serectRandomNum = Random.Range(0, 2);
    }
    private void Start()
    {
        MapHandler.instance.handleAxiesCallback += HandleAxiesAction;
    }
    private void OnDestroy()
    {
        MapHandler.instance.handleAxiesCallback -= HandleAxiesAction;
    }
    public virtual void HandleAxiesAction(string action) {
        if (action == "PLAY" && !IsInvoking("AxiesAction"))
        {
            OnStartSimulator();
        }
        else if (action == "PAUSE") {
            OnCancelSimulator();
        }
    }
    public virtual void OnStartSimulator()
    {
        InvokeRepeating("AxiesAction", 1, 1);
    }
    public virtual void AxiesAction()
    {

    }
    public virtual void OnCancelSimulator()
    {
        CancelInvoke();
    }
    public void Update()
    {
        if ((float)heathBar.localScale.x != (float)currentHealth / (float)maxHealth)
            heathBar.localScale = new Vector3(Mathf.Lerp(heathBar.localScale.x, (float)currentHealth / (float)maxHealth, 3 * Time.deltaTime), 1, 1);
    }
    public void TakeDamage(int enemySerectNum) {
        int calNum = (3 + enemySerectNum - serectRandomNum) % 3;
        switch (calNum) {
            case 0:{
                    currentHealth -= 4;
                    break;
                }
            case 1:
                {
                    currentHealth -= 5;
                    break;
                }
            case 2:
                {
                    currentHealth -= 3;
                    break;
                }
        }
        //heathBar.localScale.Set(0.5f, 1, 1);
        if (currentHealth <= 0)
            Die();
    }
    public void Die() {
        Vector3Int position = MapHandler.instance.tileMap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0));
        Coordinates hexCoordinates = new Coordinates(position.x, position.y);
        if (MapHandler.instance.axiesList.ContainsKey(hexCoordinates)) {
            MapHandler.instance.HandleAxiesNum("DECREASE", MapHandler.instance.axiesList[hexCoordinates].tag);
            MapHandler.instance.axiesList.Remove(hexCoordinates);
            Destroy(gameObject);
        }
    }
    public PathToEnemy FindClosestEnemy(Coordinates fistCoordinates, int radius, string enemyTag, string allianceTag)
    {
        try {
            Utils utils = new Utils();
            List<Coordinates> listCoordinates = utils.FindAllCoordinatesByRadius(fistCoordinates, radius);
            //Dictionary<Coordinates, GameObject> axiesList = MapHandler.instance.axiesList;
            int[,] graph = new int[listCoordinates.Count, listCoordinates.Count];
            PathToEnemy minPathToEnemy = new PathToEnemy(-1, new List<Coordinates>(), new Coordinates());
            for (int i = 0; i < listCoordinates.Count; i++)
            {
                List<Coordinates> coordinates = utils.FindAllCoordinatesByRadius(listCoordinates[i], 1);
                for (int j = 0; j < listCoordinates.Count; j++)
                {
                    if (coordinates.Contains(listCoordinates[j]))
                    {
                        if (MapHandler.instance.axiesList.ContainsKey(listCoordinates[i]) &&
                            MapHandler.instance.axiesList.ContainsKey(listCoordinates[j]) &&
                            MapHandler.instance.axiesList[listCoordinates[i]]?.tag == allianceTag &&
                            MapHandler.instance.axiesList[listCoordinates[j]]?.tag == allianceTag)
                            graph[i, j] = 0;
                        else
                            graph[i, j] = 1;
                    }
                }
            }
            foreach (Coordinates coordinates in listCoordinates)
            {
                if (MapHandler.instance.axiesList.ContainsKey(coordinates) && MapHandler.instance.axiesList[coordinates].tag == enemyTag)
                {
                    PathToEnemy currPath = new PathToEnemy();
                    currPath = FindWayToEnemies(fistCoordinates, coordinates, listCoordinates, graph);
                    if (minPathToEnemy.pathLength == -1 || currPath.pathLength < minPathToEnemy.pathLength)
                        minPathToEnemy = currPath;
                }

            }
            return minPathToEnemy;
        } catch {
            Debug.Log("ThereSomeError");
            return new PathToEnemy(-1, new List<Coordinates>(), new Coordinates());
        }
        
    }
    public struct PathToEnemy
    {
        public int pathLength;
        public List<Coordinates> pathCoordinates;
        public Coordinates targetCoordinates;
        public PathToEnemy(int pathLength, List<Coordinates> pathCoordinates, Coordinates targetCoordinates)
        {
            this.pathLength = pathLength;
            this.pathCoordinates = pathCoordinates;
            this.targetCoordinates = targetCoordinates;
        }
    }
    public PathToEnemy FindWayToEnemies(Coordinates fistCoordinates, Coordinates secondCoordinates, List<Coordinates> listCoordinates, int[,] graph)
    {
        //Find closet enemy to have secondCoordinates
        Dictionary<Coordinates, int> coordinatesDic = new Dictionary<Coordinates, int>();
        for (int i = 0; i < listCoordinates.Count; i++)
            coordinatesDic.Add(listCoordinates[i], i);
        List<int> intPath = DijkstraWithoutQueue.DijkstraAlgorithm(graph, coordinatesDic[fistCoordinates], coordinatesDic[secondCoordinates]);
        PathToEnemy pathToEnemy = new PathToEnemy(-1, new List<Coordinates>(), new Coordinates());
        if (intPath == null)
        {
            return pathToEnemy;
        }
        else
        {
            int pathLength = 0;
            List<Coordinates> pathCoordinates = new List<Coordinates>();
            for (int i = 0; i < intPath.Count - 1; i++)
            {
                pathLength += graph[intPath[i], intPath[i + 1]];
                pathCoordinates.Add(listCoordinates[intPath[i]]);

            }
            pathToEnemy.pathLength = pathLength;
            pathToEnemy.pathCoordinates = pathCoordinates;
            pathToEnemy.targetCoordinates = secondCoordinates;
            return pathToEnemy;
        }
    }
}
