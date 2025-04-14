using UnityEngine;
using UnityEngine.UI;
 
public class ButtonClick : MonoBehaviour
{
    public Button button;
 
    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);  //代码控制监听，无参
    }
 
    //无参的方法（注意：假如需要拖到组件中进行监听，那么函数需要定义为公共的）
    public void OnButtonClick()
    {
        Debug.Log("Button Clicked!");
        // 在这里编写点击事件的逻辑
        Color[] colors = {Constants.REDPOINT,Constants.BLUEPOINT};
        ColorPointCtrl.Get.RemoveColorPointsByColors(colors);
    }   
}