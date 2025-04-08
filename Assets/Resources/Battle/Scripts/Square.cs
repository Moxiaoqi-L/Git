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
 
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnSquareClicked);
    }

    // 默认输出
    public override string ToString()
    {
        return $"棋盘方格坐标:{location},地形类型:{type},阵营:{camp}";
    }

    // 点击方格触发的事件
    public void OnSquareClicked()
    {
        var selection = SelectCore.Selection;
        //如果当前没有选中任何棋子，则点击棋盘方格后无事发生
        if(!selection)
        {
            return;
        }
        //如果当前有选中棋子，则点击棋盘方格后，被选中的棋子将移动到被点击的方格处
    }

    // 移除棋盘方格上的棋子(如果有的话)
    public void RemoveChessman()
    {
        Chessman?.ExitFromBoard();
    }
}