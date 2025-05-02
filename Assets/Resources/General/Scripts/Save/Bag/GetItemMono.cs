using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItemMono : MonoBehaviour
{
    
    public int choose;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            switch (choose)
            {
                case 1:
                    BagDataSaveLoadManager.Get.GetItem(10001, 1);
                    return;
                case 2:
                    BagDataSaveLoadManager.Get.UseItems(10001, 1);
                    return;
                case 3:
                    return;
                default:
                    return;
            }
        });
    }
}
