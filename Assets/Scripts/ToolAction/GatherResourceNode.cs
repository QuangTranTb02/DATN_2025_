using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceNodeType
{
    Underfine,
    tree,
    ore

}
[CreateAssetMenu(menuName = "Data/Tool Action/Gather Resource Node")]
public class GatherResourceNode : ToolAction
{
    [SerializeField] float SizeOfInteracableArea;

    [SerializeField] List<ResourceNodeType> CanHitNodeOfType;


    public override bool OnApply(Vector2 worldPoint)
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPoint, SizeOfInteracableArea);

        foreach (Collider2D item in colliders)
        {
            ToolHit hit = item.GetComponent<ToolHit>();
            if (hit != null)
            {
                if (hit.CanBeHit(CanHitNodeOfType) == true)
                {
                    hit.Hit();
                    return true;
                }
            }

        }
        return false;
    }
}
