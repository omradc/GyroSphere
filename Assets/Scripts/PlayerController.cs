using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform cam;
    [SerializeField] Transform sphere;
    [SerializeField] Transform core;
    [SerializeField] Transform weaponSystem;
    [SerializeField] Rigidbody sphereRb;
    [SerializeField] Rigidbody coreRb;
    [SerializeField] KeyCode @break;

    [Header("Sphere")]
    [SerializeField] float sphereMoveTorque = 10f;
    [SerializeField] float sphereBreaksmoothness = 1f;
    [Header("Core")]
    [SerializeField] float coreMoveTorque = 5f;
    [SerializeField] float coreBreakSpeed = 10f;
    [SerializeField] float releaseCoreEnergy;
    [SerializeField] float coreBreaksmoothness = 5f;

    [Header("Magnus Effect")]
    [SerializeField] float magnusFactor = 5f; // Magnus etkisi gücü, ayarlanabilir
    [SerializeField] float liftFactor = 1f; // Yükselme (aşağı/yukarı hareket) etkisi
    [SerializeField] float dragFactor = 0.1f; // Havanın sürükleme etkisi (dönmeye göre)


    bool releaseCoreInput;
    bool anyCoreInput;
    float sphereX, sphereZ, coreX, coreZ;
    Vector3 magnusForce;
    Vector3 coreDirection;
    Vector3 sphereDirection;
    void Update()
    {
        SetSphere();
        SetInputs();
        SetDirections();
        ReleaseCoreEnergy();
        BreakSphere();
    }

    private void FixedUpdate()
    {
        ApplyForce();
        //ApplyMagnusEffect();
    }
    void SetInputs()
    {
        sphereX = Input.GetAxis("Horizontal");
        sphereZ = Input.GetAxis("Vertical");

        coreX = Input.GetAxis("Horizontal2");
        coreZ = Input.GetAxis("Vertical2");

        releaseCoreInput = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow);
        anyCoreInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

    }
    void SetDirections()
    {
        sphereDirection = new Vector3(sphereX, 0, sphereZ);
        coreDirection = new Vector3(coreX, 0, coreZ);
    }
    void ApplyForce()
    {
        sphereRb.AddTorque(sphereDirection.normalized * sphereMoveTorque);
        coreRb.AddTorque(coreDirection.normalized * coreMoveTorque);
    }
    void SetSphere()
    {
        core.position = sphere.position;
        cam.position = sphere.position + new Vector3(0, 1, -5);
        weaponSystem.position = sphere.position;
    }
    void ReleaseCoreEnergy()
    {
        if (releaseCoreInput)
        {
            print("ReleaseCoreEnergy");
            sphereRb.AddTorque(coreRb.angularVelocity * releaseCoreEnergy, ForceMode.Impulse); // apply torque to sphere
        }
        if (!anyCoreInput)
        {
            coreRb.angularVelocity = Vector3.Lerp(coreRb.angularVelocity, Vector3.zero, Time.deltaTime * coreBreaksmoothness);// stop rotation// stop rotation
        }
    }
    void BreakSphere()
    {
        if (Input.GetKey(@break))
        {
            Vector3 reDirection = new Vector3(sphereRb.linearVelocity.z, 0, sphereRb.linearVelocity.x);  // swap x and z axis
            coreRb.angularVelocity = reDirection * coreBreakSpeed; // transfer sphere rotation to core
            sphereRb.angularVelocity = Vector3.Lerp(sphereRb.angularVelocity, Vector3.zero, Time.deltaTime * sphereBreaksmoothness);// stop rotation// stop rotation
        }
    }
    void ApplyMagnusEffect()
    {
        // Ana kürenin dönüş hızını alıyoruz
        Vector3 angularVelocity = sphereRb.angularVelocity;

        // Ana kürenin doğrusal hızını alıyoruz
        Vector3 linearVelocity = sphereRb.linearVelocity;

        // Magnus etkisini hesaplıyoruz. Bu, dönüş hızıyla doğrusal hızı birleştirir.
        // Cross product ile iki vektör arasında bir dönme kuvveti oluşturuyoruz.
        magnusForce = Vector3.Cross(angularVelocity, linearVelocity.normalized) * magnusFactor;

        // Ekstra yükselme etkisi (aşağı/yukarı hareketi) ekliyoruz.
        magnusForce += Vector3.up * (angularVelocity.magnitude * liftFactor);

        // Ana küreye Magnus kuvvetini uyguluyoruz
        sphereRb.AddForce(magnusForce, ForceMode.Force);

        // Havanın sürükleme etkisini ekliyoruz, bu da dönme hareketini ve doğrusal hızını engeller.
        sphereRb.AddForce(-linearVelocity * dragFactor, ForceMode.Force);
    }
}
