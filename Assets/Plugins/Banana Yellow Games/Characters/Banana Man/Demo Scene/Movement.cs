using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float damp = 0.08f;

    private Rigidbody rb;
    private Vector3 moveDir;
    private float yLock;
    private Animator animator;

    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        yLock = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 worldInput = new Vector3(x, 0f, z);
        if (worldInput.sqrMagnitude > 1f)
        {
            worldInput.Normalize();
        }

        Vector3 local = transform.InverseTransformDirection(worldInput);
        float h = Mathf.Clamp(local.x, -1f, 1f);
        float v = Mathf.Clamp(local.z, -1f, 1f);

        animator.SetFloat("Horizontal", h, damp, Time.deltaTime);
        animator.SetFloat("Vertical", v, damp, Time.deltaTime);
        animator.SetBool("isMoving", worldInput.sqrMagnitude > 0.0001f);

        moveDir = worldInput;
    }

    private void FixedUpdate()
    {
        Vector3 current = rb.position;
        Vector3 next = current + moveDir * moveSpeed * Time.fixedDeltaTime;

        next.y = yLock;

        rb.MovePosition(next);
    }
}
