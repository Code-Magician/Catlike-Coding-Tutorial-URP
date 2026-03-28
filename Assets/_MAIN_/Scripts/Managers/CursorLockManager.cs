using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CursorLockManager : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.Escape;

    bool isCursorLocked = true;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isCursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }

        if (!isCursorLocked && Input.GetMouseButtonDown(0))
        {
            // Lock only if we are NOT clicking a raycastable UI
            if (!IsPointerOverRaycastUI())
            {
                LockCursor();
            }
        }
    }

    bool IsPointerOverRaycastUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
    }
}