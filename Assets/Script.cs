using UnityEngine;

public class Script : MonoBehaviour
{
    [Header("Test Script to change between inventories! Press A, B or C to activate each inventory and change between them.")]
    public InventoryManager InventoryA;
    public InventoryManager InventoryB;
    public InventoryManager InventoryC;

    void Start()
    {

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            InventoryA.ChangeActiveState(true);
            InventoryB.ChangeActiveState(false);
            InventoryC.ChangeActiveState(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            InventoryA.ChangeActiveState(false);
            InventoryB.ChangeActiveState(true);
            InventoryC.ChangeActiveState(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            InventoryA.ChangeActiveState(false);
            InventoryB.ChangeActiveState(false);
            InventoryC.ChangeActiveState(true);
        }

    }
}
