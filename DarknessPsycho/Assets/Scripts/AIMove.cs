using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AIMove : MonoBehaviour
{
    
    [Header("AI")]
    public Transform target;
    
    [SerializeField] private float speed = 100f;
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float nextWaypointDistance = 3f;

    private Path _path;
    private int _currentWaypoint;
    private bool _reachEndOfPath = false;

    private Seeker _seeker;
    protected Rigidbody2D Rb;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        Rb = GetComponent<Rigidbody2D>();

        StartMoving();
    }

    private void GetPath()
    {
        _seeker.StartPath(Rb.position, target.position, OnPathComplete);
    }

    public void StartMoving() => InvokeRepeating(nameof(GetPath), 0, 0.5f);
    public void StopMoving() => CancelInvoke(nameof(GetPath));

    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        _path = p;
        _currentWaypoint = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_path == null) return;

        _reachEndOfPath = _currentWaypoint >= _path.vectorPath.Count;
        if (_reachEndOfPath)
            return;

        var direction = ((Vector2)_path.vectorPath[_currentWaypoint] - Rb.position).normalized;
        var force = direction * (acceleration * Time.deltaTime);

        Rb.AddForce(force);
        Rb.velocity = Vector2.ClampMagnitude(Rb.velocity, speed);
            
        var distance = Vector2.Distance(Rb.position, _path.vectorPath[_currentWaypoint]);

        if (distance < nextWaypointDistance)
            _currentWaypoint++;
    }
}
