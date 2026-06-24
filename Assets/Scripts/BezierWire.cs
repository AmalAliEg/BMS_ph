using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierWire : MonoBehaviour
{
    [Header("Wire Connection Points")]
    public Transform startPoint;       // نقطة بداية السلك (الـ Empty Child الأول)
    public Transform endPoint;         // نقطة نهاية السلك (الـ Empty Child الثاني)
    
    [Header("Wire Customization")]
    public Color wireColor = Color.red; // لون السلك مباشرة من الـ Inspector
    [Range(0.001f, 0.05f)] 
    public float wireWidth = 0.005f;   // سمك السلك بالمتر

    [Header("Bezier Settings")]
    [Range(10, 50)] 
    public int segmentCount = 30;      // تنعيم المنحنى
    public float curveHeight = -0.02f; // مقدار الانحناء (سالب يقترب من الطاولة)

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ConfigureLineRenderer();
    }

    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            DrawBezierWire();
        }
    }

    void ConfigureLineRenderer()
    {
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = wireWidth;
        lineRenderer.endWidth = wireWidth;
        
        // استخدام شيدر افتراضي متوافق داخلياً مع Unity 6 و URP بدون الحاجة لإنشاء ملف خامة خارجي
        Shader defaultShader = Shader.Find("Sprites/Default");
        if (defaultShader != null)
        {
            lineRenderer.material = new Material(defaultShader);
        }

        // تفعيل خيار قراءة الإحداثيات العالمية لضمان انطباق السلك على النقطة المتحركة
        lineRenderer.useWorldSpace = true;
        
        lineRenderer.numCornerVertices = 8;
        lineRenderer.numCapVertices = 8;
    }

    void DrawBezierWire()
    {
        // تحديث السمك واللون لحظياً من الـ Inspector في أي وقت
        lineRenderer.startWidth = wireWidth;
        lineRenderer.endWidth = wireWidth;
        lineRenderer.startColor = wireColor;
        lineRenderer.endColor = wireColor;

        Vector3 p0 = startPoint.position;
        Vector3 p2 = endPoint.position;
        
        Vector3 p1 = (p0 + p2) / 2.0f;
        p1.y += curveHeight; 

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 pixelPosition = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            lineRenderer.SetPosition(i, pixelPosition);
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        
        return p;
    }
}