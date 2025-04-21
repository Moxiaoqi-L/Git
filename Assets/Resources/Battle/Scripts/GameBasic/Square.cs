using UnityEngine;
using UnityEngine.UI;
 
public class Square : MonoBehaviour
{
    // 坐标位置
    public Location location;
    // 方格类型
    public SquareType type;
    // 方格阵营
    public Camp camp;
    // 通过此属性，可以访问位于此方格上的棋子
    public Chessman Chessman => Chessman.GetChessman(location);

    // 原始图片
    private Sprite originalImage;
    // 攻击范围图片
    public Sprite attackImage;
    // 组件
    private Image image;
 
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnSquareClicked);
        image = GetComponent<Image>();
        originalImage = image.sprite;
    }

    // 默认输出
    public override string ToString()
    {
        return $"棋盘方格坐标:{location},地形类型:{type},阵营:{camp}";
    }

    // 点击方格触发的事件
    public void OnSquareClicked()
    {
        // TODO
    }

    // 移除棋盘方格上的棋子(如果有的话)
    public void RemoveChessman()
    {
        Chessman?.ExitFromBoard();
    }

    // 新增：设置高亮状态
    public void SetAttackRangeHighlight(bool isHighlighted)
    {
        image.sprite = isHighlighted ? attackImage : originalImage;
    }
}