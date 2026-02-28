using UnityEngine;

namespace InventoryFramework
{
    public class InventoryTester : MonoBehaviour
    {
        [Header("Assign the inventory window GameObject (Inv)")]
        public GameObject invPanel;

        private bool isOpen;

        void Start()
        {
            if (invPanel != null)
                invPanel.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnInventory()
        {
            isOpen = !isOpen;

            if (invPanel != null)
                invPanel.SetActive(isOpen);

            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;
        }
    }
}
