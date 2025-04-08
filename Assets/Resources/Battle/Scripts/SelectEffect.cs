using UnityEngine;
 
public class SelectEffect : MonoBehaviour
{
    public Material GLRectMat;
 
    public Color GLRectColor(Camp camp)
    {
        if (camp == Camp.Player)
        {
            return Color.green;
        }
        else if (camp == Camp.Enemy)
        {
            return new Color(1f, 0.5f, 0.75f);//Color.red太丑了，需要自己换个颜色
        }
        else
        {
            return Color.white;
        }
    }
 
    void OnPostRender()
    {
        var selection = SelectCore.Selection;
        if (!selection)
        {
            return;
        }
 
        selection.TryGetComponent(out RectTransform rectTransform);
        var center = Camera.main.WorldToScreenPoint(rectTransform.position);
 
        GL.PushMatrix();//GL入栈
        GLRectMat.SetPass(0);//启用线框材质rectMat
        GL.LoadPixelMatrix();//设置用屏幕坐标绘图
 
        for (int radius = 46; radius <= 50; radius++)
        {
            float Xmin = center.x - radius;
            float Xmax = center.x + radius;
            float Ymin = center.y - radius;
            float Ymax = center.y + radius;
 
            GL.Begin(GL.LINES);//开始绘制线，用来描出矩形的边框
 
            GL.Color(GLRectColor(selection.camp));//设置方框的边框颜色,由选中棋子的阵营决定
 
            //描第一条边
            GL.Vertex3(Xmin, Ymin, 0);//起始于点1
            GL.Vertex3(Xmin, Ymax, 0);//终止于点2
 
            //描第二条边
            GL.Vertex3(Xmin, Ymax, 0);//起始于点2
            GL.Vertex3(Xmax, Ymax, 0);//终止于点3
 
            //描第三条边
            GL.Vertex3(Xmax, Ymax, 0);//起始于点3
            GL.Vertex3(Xmax, Ymin, 0);//终止于点4
 
            //描第四条边
            GL.Vertex3(Xmax, Ymin, 0);//起始于点4
            GL.Vertex3(Xmin, Ymin, 0);//返回到点1
 
            GL.End();//画好啦！
        }
        GL.PopMatrix();//GL出栈
    }
}