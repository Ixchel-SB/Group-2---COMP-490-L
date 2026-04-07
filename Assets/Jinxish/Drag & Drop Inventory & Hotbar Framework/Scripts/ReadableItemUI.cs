using UnityEngine;
using UnityEngine.UI;

namespace InventoryFramework
{
    public class ReadableItemUI : MonoBehaviour
    {
        public static ReadableItemUI Instance;

        public GameObject panel;
        public Image readableImage;
        public Button nextButton;
        public Button prevButton;

        private Item currentItem;
        private int currentPageIndex;

        private void Awake()
        {
            Instance = this;
            panel.SetActive(false);
        }

        public void Open(Item item)
        {
            if (item.readablePages == null || item.readablePages.Length == 0)
            {
                Debug.LogWarning("No readable pages assigned for item: " + item.itemName);
                return;
            }

            currentItem = item;
            currentPageIndex = 0;

            panel.SetActive(true);
            ShowPage();
        }

        public void Close()
        {
            panel.SetActive(false);
            currentItem = null;
            currentPageIndex = 0;
        }

        public void NextPage()
        {
            if (currentItem == null) return;
            if (currentPageIndex >= currentItem.readablePages.Length - 1) return;

            currentPageIndex++;
            ShowPage();
        }

        public void PrevPage()
        {
            if (currentItem == null) return;
            if (currentPageIndex <= 0) return;

            currentPageIndex--;
            ShowPage();
        }

        private void ShowPage()
        {
            readableImage.sprite = currentItem.readablePages[currentPageIndex];

            prevButton.gameObject.SetActive(currentPageIndex > 0);
            nextButton.gameObject.SetActive(currentPageIndex < currentItem.readablePages.Length - 1);
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