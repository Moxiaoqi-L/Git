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
            return new Color(1f, 0.5f, 0.75f);
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

        // 计算相对的缩放因子，这里以 1920x1080 为基准
        float referenceWidth = 1920f;
        float scaleFactor = Screen.width / referenceWidth;

        // 动态调整 radius
        int minRadius = Mathf.FloorToInt(46 * scaleFactor);
        int maxRadius = Mathf.FloorToInt(50 * scaleFactor);

        // 确保相机的视口坐标与屏幕坐标的转换正确
        Camera cam = Camera.main;
        if (cam.orthographic)
        {
            // 调整中心点位置以适应不同分辨率
            Vector3 viewportCenter = cam.WorldToViewportPoint(rectTransform.position);
            center.x = viewportCenter.x * Screen.width;
            center.y = viewportCenter.y * Screen.height;
        }

        GL.PushMatrix();
        GLRectMat.SetPass(0);
        GL.LoadPixelMatrix();

        for (int radius = minRadius; radius <= maxRadius; radius++)
        {
            float Xmin = center.x - radius;
            float Xmax = center.x + radius;
            float Ymin = center.y - radius;
            float Ymax = center.y + radius;

            GL.Begin(GL.LINES);
            GL.Color(GLRectColor(selection.camp));

            // 描第一条边
            GL.Vertex3(Xmin, Ymin, 0);
            GL.Vertex3(Xmin, Ymax, 0);

            // 描第二条边
            GL.Vertex3(Xmin, Ymax, 0);
            GL.Vertex3(Xmax, Ymax, 0);

            // 描第三条边
            GL.Vertex3(Xmax, Ymax, 0);
            GL.Vertex3(Xmax, Ymin, 0);

            // 描第四条边
            GL.Vertex3(Xmax, Ymin, 0);
            GL.Vertex3(Xmin, Ymin, 0);

            GL.End();
        }
        GL.PopMatrix();
    }
}