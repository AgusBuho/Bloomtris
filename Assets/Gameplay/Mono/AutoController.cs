using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoController : MonoBehaviour
{
    [SerializeField] private AutoConfig config;
    [SerializeField] private Transform visualModel;

    private Rigidbody rb;
    private float horizontalInput;

    void Awake() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        Vector3 forward = transform.forward * config.acceleration;
        rb.AddForce(forward, ForceMode.Acceleration);

        Vector3 steer = transform.right * horizontalInput * config.turnStrength;
        rb.AddForce(steer, ForceMode.Acceleration);

        ApplyDriftEffect();
    }

    void ApplyDriftEffect()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 forward = transform.forward;

        float driftAmount = Vector3.Dot(velocity, transform.right);
        Vector3 driftForce = -transform.right * driftAmount * config.driftFactor;

        rb.AddForce(driftForce, ForceMode.Acceleration);
    }
}