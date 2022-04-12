using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAxies : Axies
{
    public override void AxiesAction()
    {
        base.AxiesAction();
        Utils utils = new Utils();
        Vector3Int position = MapHandler.instance.tileMap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0));
        Coordinates hexCoordinates = new Coordinates(position.x,position.y);
        if (!MapHandler.instance.axiesList.ContainsKey(hexCoordinates)) return;
        PathToEnemy pathToClosetEnemy = FindClosestEnemy(hexCoordinates, 8, "DEFEND_MEM", "ATTACK_MEM");
        if (pathToClosetEnemy.pathLength != -1)
        {
            if (pathToClosetEnemy.pathLength == 1)
            {
                if (MapHandler.instance.axiesList.ContainsKey(pathToClosetEnemy.targetCoordinates))
                {
                    GameObject targetGameObject = MapHandler.instance.axiesList[pathToClosetEnemy.targetCoordinates];
                    DefendAxies defendAxies = targetGameObject.GetComponent<DefendAxies>();
                    defendAxies.TakeDamage(serectRandomNum);
                }
            }
            if (pathToClosetEnemy.pathLength > 1)
            {
                HandleAxiesMovement(hexCoordinates, pathToClosetEnemy.pathCoordinates[1]);
            }
        }
    }
    public void HandleAxiesMovement(Coordinates currCoordinates, Coordinates targetCoordinates)
    {
        if (!MapHandler.instance.axiesList.ContainsKey(targetCoordinates)&& MapHandler.instance.axiesList.ContainsKey(currCoordinates)) {
            transform.position = MapHandler.instance.tileMap.CellToWorld(new Vector3Int(targetCoordinates.x, targetCoordinates.y, 0));
            MapHandler.instance.axiesList.Add(targetCoordinates,MapHandler.instance.axiesList[currCoordinates]);
            MapHandler.instance.axiesList.Remove(currCoordinates);
        }
    }
}
