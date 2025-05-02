using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetHeroMono : MonoBehaviour
{
    
    public int choose;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(() => {
            switch (choose)
            {
                case 1:
                    GetNewHero(1);
                    return;
                case 2:
                    HeroLevelUp();
                    return;
                case 3:
                    HeroGetXP(1);
                    return;
                default:
                    return;
            }
        });
    }

    public void GetNewHero(int heroID)
    {
        HeroDataSaveLoadManager.Get.GetNewHero(1);
    }

    public void HeroLevelUp()
    {
        HeroDataSaveLoadManager.Get.HeroLevelUp(1);
    }

    public void HeroGetXP(int heroID)
    {
        HeroDataSaveLoadManager.Get.HeroGetXP(heroID, 10000);
    }
}
