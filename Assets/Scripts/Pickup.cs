using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public SFXInfo collectSFX;
    public PooledObjectData collectVFX;

    public abstract void AddPickupToPlayer();

    [Header("Initial Fling Settings")]
    [SerializeField] private float minFlingSpeed = 10f;
    [SerializeField] private float maxFlingSpeed = 30f;

    [Header("PD Controller Settings")]
    [SerializeField] private float Kp = 2f;  // Max proportional gain
    [SerializeField] private float Kd = 1f;  // Max derivative (damping) gain

    [Header("Magnet Timing")]
    [SerializeField] private float magnetDelay = 0.5f;      // Delay before magnet pull ramps in
    [SerializeField] private float magnetRampDuration = 1.0f; // Time to go from 0 -> Kp/Kd

    private Rigidbody2D rb;
    private Actor targetActor;
    private float elapsedTime = 0f;

    public virtual void InitializePickup(Vector2 initialPosition, Actor targetActor)
    {
        transform.position = initialPosition;

        rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            Debug.LogWarning("No Rigidbody2D found on Pickup!");
            return;
        }

        // Random fling
        Vector2 flingDirection = Random.insideUnitCircle.normalized;
        float flingSpeed = Random.Range(minFlingSpeed, maxFlingSpeed);
        rb.linearVelocity = flingDirection * flingSpeed;

        this.targetActor = targetActor;

        elapsedTime = 0f;
    }

    private void Update()
    {
        if (targetActor == null || rb == null)
            return;

        elapsedTime += Time.deltaTime;

        // 1) Compute how much of the "magnet" force should be active right now.
        //    - Before magnetDelay: no pull
        //    - Between magnetDelay & (magnetDelay + magnetRampDuration): partial pull
        //    - After that: full pull
        float currentKp = 0f;
        float currentKd = 0f;

        // We see where 'elapsedTime' lies relative to magnetDelay and magnetRampDuration.
        if (elapsedTime >= magnetDelay)
        {
            // fraction (0..1) of how far we are in the magnet ramp
            float rampFraction = Mathf.InverseLerp(
                magnetDelay,
                magnetDelay + magnetRampDuration,
                elapsedTime
            );
            // Lerp from 0..Kp
            currentKp = Mathf.Lerp(0f, Kp, rampFraction);
            currentKd = Mathf.Lerp(0f, Kd, rampFraction);
        }
        // else it remains 0..0 (no magnet)

        // 2) Calculate PD force with these ramped gains
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = targetActor.transform.position;

        Vector2 error = targetPosition - currentPosition; // "distance" to player
        Vector2 dError = rb.linearVelocity;                      // derivative = current velocity

        Vector2 force = (currentKp * error) - (currentKd * dError);

        // 3) Apply the force
        //    Typically you'd do AddForce in FixedUpdate with *Time.deltaTime, 
        //    but we'll keep your style for simplicity.
        rb.AddForce(force);

        // 4) Check if we should collect (your existing logic)
        if (error.magnitude < 1f || elapsedTime > 2f)
        {
            AddPickupToPlayer();
            ObjectPoolManager.Instance.Despawn(gameObject);

            // Play SFX and VFX
            collectSFX.Play();
            collectVFX.Spawn(transform.position);
        }
    }
}
