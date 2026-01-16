using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : Interacable
{
    public ItemContainer storeContent;
    //chênh lệch bán ra - mua vào
    public float buyFromPlayerMultip = 0.5f;
    public float sellFromPlayerMultip = 1.5f;
    public override void Interact(Player Player)
    {
        Trading trading = Player.GetComponent<Trading>();
        if (trading == null ) { return; }
        trading.BeginTrading(this);
    }
}
