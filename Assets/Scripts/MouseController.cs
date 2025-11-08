using UnityEngine;
using UnityEngine.InputSystem;

public enum UnitSlotType
{
    Waiting, Front, Middle, Rear
}

public class MouseController : Singleton<MouseController>
{
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject gridRed;
    [SerializeField] private GameObject selectEffect;
    public UnitController selectedUnit;
    private Camera cam;

    private void Start()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                UnitController unit = hit.collider.GetComponent<UnitController>();
                if (unit != null && !StageManager.instance.isStage)
                {
                    selectedUnit = unit;
                    grid.SetActive(true);
                    if(DungeonManager.instance.LevelIndex >= DungeonManager.instance.Level)
                    {
                        gridRed.SetActive(true);
                    }
                    selectEffect.SetActive(true);
                    selectEffect.transform.parent = unit.transform;
                    selectEffect.transform.localPosition = Vector3.zero;
                    unit.OnSelect();
                }
            }
        }

        if (Mouse.current.leftButton.isPressed && selectedUnit != null)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPos = ray.GetPoint(enter);
                selectedUnit.FollowMouse(worldPos);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (selectedUnit != null)
            {
                grid.SetActive(false);
                gridRed.SetActive(false);
                selectEffect.SetActive(false);
                selectedUnit.OnRelease();
                selectedUnit = null;
            }
        }

    }

    public void MouseUp()
    {
        if (selectedUnit != null)
        {
            grid.SetActive(false);
            selectedUnit.OnRelease();
            selectedUnit = null;
        }
    }
}
