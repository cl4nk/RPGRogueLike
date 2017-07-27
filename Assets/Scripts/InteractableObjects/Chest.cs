using UnityEngine;

public class Chest : InteractableObject
{
    private float angle;
    [SerializeField] private float animationSpeed;

    private bool animChest;
    [SerializeField] private LootInventory lootInventory;
    private readonly float maxAngle = 50;
    private bool opened;

    private Vector3 pivot;

    private UIGame uiGame;

    private Transform upPart;

    private void Start()
    {
        uiGame = UIGame.Instance;

        ResetChest();
    }

    public void ResetChest()
    {
        upPart = transform.Find("UpPart");
        pivot = upPart.position;
        pivot.y -= upPart.localScale.y / 2;
        pivot.z += upPart.localScale.z / 2;
    }

    private void Update()
    {
        FixedUpdate();
        if (animChest)
            AnimChest();
    }

    public override void OnUse()
    {
        opened = !opened;
        animChest = true;

        if (opened)
        {
            UIGame.Instance.ToggleInventoryWindow(lootInventory);
            inputMgr.LockInventory();
            uiGame.playerCam.ToggleCursorLock();
        }
    }

    private void AnimChest()
    {
        if (opened)
            OpenChest();
        else
            CloseChest();
    }

    private void OpenChest()
    {
        angle += animationSpeed * Time.fixedDeltaTime;
        upPart.RotateAround(pivot, transform.right, animationSpeed * Time.fixedDeltaTime);

        if (angle >= maxAngle)
            animChest = false;
    }

    private void CloseChest()
    {
        angle -= animationSpeed * Time.deltaTime;
        upPart.RotateAround(pivot, transform.right, -animationSpeed * Time.deltaTime);

        if (angle <= 0)
            animChest = false;
    }
}