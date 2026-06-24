using UnityEngine;

public class FreeCameraControl : MonoBehaviour
{
    [Header("Target Focus")]
    public Transform targetObject;       // القطعة التي تريدين الكاميرا أن تظل متسلطة عليها (اسحبيها هنا)
    public bool lockOnTarget = true;     // تفعيل أو إلغاء قفل التركيز التلقائي

    [Header("Movement Settings")]
    public float movementSpeed = 0.5f;   // السرعة الناعمة المناسبة لحجم البوردة
    public float rotationSpeed = 80f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 currentRotation = transform.localRotation.eulerAngles;
        rotationX = currentRotation.y;
        rotationY = currentRotation.x;
    }

    void Update()
    {
        // 1. الدوران والالتفاف (Side Views) بالماوس الأيمن
        if (Input.GetMouseButton(1)) 
        {
            rotationX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -85f, 85f);

            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }

        // 2. إذا كان خيار التركيز مفعلاً وهناك هدف، تجبر الكاميرا على النظر إليه دائماً
        if (lockOnTarget && targetObject != null)
        {
            // الكاميرا ستظل تنظر للهدف حتى لو تحركت يميناً أو يساراً بـ WASD
            transform.LookAt(targetObject.position);
            
            // تحديث قيم الـ Rotation لعدم حدوث قفزة فجائية عند الإمساك بالماوس
            Vector3 currentRot = transform.localRotation.eulerAngles;
            rotationX = currentRot.y;
            rotationY = currentRot.x;
        }

        // 3. الحركة الانتقالية (W, A, S, D) مع Q و E للارتفاع
        float moveForward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveSideways = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        float moveUpDown = 0f;
        if (Input.GetKey(KeyCode.E)) moveUpDown = movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) moveUpDown = -movementSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveSideways, moveUpDown, moveForward), Space.Self);
    }
}