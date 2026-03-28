using UnityEngine;
using UnityEngine.UI;

namespace InventoryFramework
{
    public class ReadableItemUI : MonoBehaviour
    {
        public static ReadableItemUI Instance;

        public GameObject panel;
        public Image readableImage;

        private void Awake()
        {
            Instance = this;
            panel.SetActive(false);
        }

        public void Open(Item item)
        {
            panel.SetActive(true);
            readableImage.sprite = item.readableImage;
        }

        public void Close()
        {
            panel.SetActive(false);
        }

        private void Update()
        {
            if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }
    }
}