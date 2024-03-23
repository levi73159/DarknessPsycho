using Enemy;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
    
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour , IDamage
{   
    private static MainInput _input;
    private Rigidbody2D _rb;
    
    [SerializeField] private float speed = 12f;
    [SerializeField] private float dashDistance = 10f;
    [SerializeField] private int health;
    
    [SerializeField] private GameObject bullet;
    [SerializeField] private LayerMask wallLayer;

    
    
    private Camera _cam;
    
    private void Awake()
    {
        _input ??= new MainInput();
        _input.Gameplay.Shoot.performed += Shoot;
        _input.Gameplay.Dash.performed += Dash;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
    }
    
    private void Shoot(CallbackContext _) => Instantiate(bullet, transform.position, transform.rotation);

    private void Dash(CallbackContext _)
    {
        var dashDir = GetMoveVector();
        if (dashDir == Vector2.zero)
            dashDir = transform.right;
        var hit = Physics2D.Raycast(transform.position, dashDir, dashDistance, wallLayer);
        Debug.DrawRay(transform.position, dashDir * dashDistance);
        if (hit.collider == null)
        {
            transform.position += (Vector3)(dashDir * dashDistance);
            return;
        }
        if (hit.transform.gameObject.CompareTag("Enemy"))
            return;
        transform.position = hit.point;
    }
    
    #region Enable/Disable

    private void OnEnable()
    {
        _input.Gameplay.Enable();
    }
    
    private void OnDisable()
    {
        _input.Gameplay.Disable();
    }
    
    #endregion
    
    private void Update()
    {
        var dir = GetMouseDir();
        dir = new Vector3(dir.x, dir.y).normalized;
        transform.right = dir;
    }
    
    private Vector3 GetMousePos()
    {
        return _cam.ScreenToWorldPoint(_input.Gameplay.Look.ReadValue<Vector2>());
    }

    private Vector3 GetMouseDir() => GetMousePos() - transform.position;
    
    private void FixedUpdate()
    {
        _rb.velocity = GetMoveVector() * (speed * Time.fixedDeltaTime);
    }
    
    private static Vector2 GetMoveVector() => _input.Gameplay.Move.ReadValue<Vector2>().normalized;
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
