using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower
{
    public string EntityName { get; private set; }
    public string CharacterName { get; private set; }

    public int InventorySpace { get; protected set; }

    public List<Item> Items { get; protected set; }


}
