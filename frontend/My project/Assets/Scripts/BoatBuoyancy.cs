using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatBuoyancy : MonoBehaviour
{
    [Header("Configuración de Flotabilidad")]
    public Transform[] floaters;
    public float floatingPower = 15f; // Ajusta este valor si el bote se hunde o salta mucho
    public float waterHeight = 0f;    // Nivel del agua en el eje Y

    [Header("Fricción (Damping)")]
    public float underwaterLinearDamping = 1f;
    public float underwaterAngularDamping = 2f;
    public float airLinearDamping = 0f;
    public float airAngularDamping = 0.05f;

    private Rigidbody rb;
    private int floatersUnderwater;
    private bool isUnderwater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate se usa para todos los cálculos de físicas
    void FixedUpdate()
    {
        floatersUnderwater = 0;

        // Iterar por todos los puntos de flotación
        for (int i = 0; i < floaters.Length; i++)
        {
            // Calcular la diferencia entre la altura del punto y el nivel del agua
            float difference = floaters[i].position.y - waterHeight;

            // Si el punto está por debajo del agua
            if (difference < 0)
            {
                // Aplicar una fuerza hacia arriba proporcional a la profundidad (Principio de Arquímedes simplificado)
                Vector3 buoyantForce = Vector3.up * floatingPower * Mathf.Abs(difference);
                rb.AddForceAtPosition(buoyantForce, floaters[i].position, ForceMode.Force);
                
                floatersUnderwater++;

                if (!isUnderwater)
                {
                    isUnderwater = true;
                    SwitchState(true);
                }
            }
        }

        // Si ningún punto está bajo el agua
        if (isUnderwater && floatersUnderwater == 0)
        {
            isUnderwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool underwater)
    {
        if (underwater)
        {
            rb.linearDamping = underwaterLinearDamping;
            rb.angularDamping = underwaterAngularDamping;
        }
        else
        {
            rb.linearDamping = airLinearDamping;
            rb.angularDamping = airAngularDamping;
        }
    }
}