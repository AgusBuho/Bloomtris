using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Settings")]
    public LevelSettings levelSettings;

    Rigidbody rb;
    float turnInput;
    float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        turnInput = Input.GetAxis("Horizontal");
        #endif
    }

    void FixedUpdate()
    {
        // 1) Aceleración forward
        currentSpeed = Mathf.Lerp(currentSpeed, levelSettings.speed, Time.fixedDeltaTime * 2f);
        rb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        // 2) Giro básico
        float steer = turnInput * levelSettings.maxTurnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steer, 0f));

        // 3) Drift avanzado
        Vector3 velocity = rb.linearVelocity;
        if (velocity.magnitude > 0.1f)
        {
            // a) Separar forward y lateral
            Vector3 forwardVel = transform.forward * Vector3.Dot(velocity, transform.forward);
            Vector3 lateralVel = velocity - forwardVel;

            // b) Calcular ángulo de slip
            float slipAngle = Vector3.Angle(transform.forward, velocity);

            // c) Facto de derrape basado en el ángulo
            float slipFactor = Mathf.Lerp(1f, levelSettings.driftFactor, slipAngle / 90f);

            // d) Reconstruir velocidad con drift
            Vector3 adjustedVel = forwardVel + lateralVel * slipFactor;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, adjustedVel, Time.fixedDeltaTime * levelSettings.gripRecovery);

            // e) Sobreviraje extra si superás el umbral
            if (slipAngle > levelSettings.driftTriggerAngle)
            {
                rb.AddTorque(Vector3.up * turnInput * levelSettings.driftOversteer, ForceMode.Acceleration);
            }
        }
    }

    // Este método lo usa tu UI touch
    public void SetTurnInput(float input) => turnInput = input;
}
