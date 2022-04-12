using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendAxies : Axies
{
    public override void AxiesAction()
    {
        base.AxiesAction();
        Vector3Int position = MapHandler.instance.tileMap.WorldToCell(new Vector3(transform.position.x, transform.position.y, 0));
        Coordinates hexCoordinates = new Coordinates(position.x, position.y);
        PathToEnemy pathToClosetEnemy = FindClosestEnemy(hexCoordinates, 1, "ATTACK_MEM", "DEFEND_MEM");
        if (pathToClosetEnemy.pathLength != -1)
        {
            if (pathToClosetEnemy.pathLength == 1)
            {
                if (MapHandler.instance.axiesList.ContainsKey(pathToClosetEnemy.targetCoordinates))
                {
                    GameObject targetGameObject = MapHandler.instance.axiesList[pathToClosetEnemy.targetCoordinates];
                    AttackAxies attackAxies = targetGameObject.GetComponent<AttackAxies>();
                    attackAxies.TakeDamage(serectRandomNum);
                }
            }
        }
    }
}
