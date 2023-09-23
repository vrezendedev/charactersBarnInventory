using UnityEngine;

public class Script : MonoBehaviour
{
    public InventoryManager InventoryA;
    public InventoryManager InventoryB;

    public Item Item;

    void Start()
    {
        InventoryA.Add(Item, 10);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InventoryA.ChangeActiveState(true);
            InventoryB.ChangeActiveState(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            InventoryA.ChangeActiveState(false);
            InventoryB.ChangeActiveState(true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            InventoryA.Transfer(InventoryB, 5);
        }
    }
}
