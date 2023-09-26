using UnityEngine;

public class Script : MonoBehaviour
{
    public InventoryManager InventoryA;
    public InventoryManager InventoryB;

    public Item Item;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InventoryA.Add(Item, 10);
            InventoryA.ChangeActiveState(true);
            InventoryB.ChangeActiveState(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            InventoryA.ChangeActiveState(false);
            InventoryB.ChangeActiveState(true);
        }

    }
}
