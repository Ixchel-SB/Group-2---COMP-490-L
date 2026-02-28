using UnityEngine;

namespace InventoryFramework
{
    public class Player : MonoBehaviour
    {
        public MonoBehaviour[] disableWhileInventoryOpen;
        public GameObject inventory;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                
                bool open = !inventory.activeSelf;
                inventory.SetActive(open);

                if (disableWhileInventoryOpen != null)
                {
                    foreach (var mb in disableWhileInventoryOpen)
                        if (mb != null) mb.enabled = !open;
                }

                Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = open;
            }
        }
    }
}

