using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum PlayerMode
{
    Standing,
    Rolling,
    Landing,
}
public class Character : MonoBehaviour
{
    [SerializeField] [Range(0, 20)] float force;
    [SerializeField] [Range(0, 20)] float forceTorque;
    [SerializeField] ForceMode forcemodeJump;
    [SerializeField] ForceMode forcemodeTorque;
    PlayerMode _playerMode;

    Animator _animator;
    Rigidbody _rigidbody;
    CapsuleCollider _collider;
    bool _isCoroutineStarted = false;
    bool _isInAir = false;
    bool _hasLanded = false;

    Vector3 standingCenter = new Vector3(0f, 0.91f, 0f);
    float standingHeight = 1.8f;
    float radius = 0.2f;

    Vector3 flippingCenter = new Vector3(0f, 1.5f, -0.21f);
    float flippingHeight = 0.94f;
    float flippingRadius = 0.46f;

    Vector3 landingCenter = new Vector3(0f, 1.07f, 0f);
    float landingHeight = 1.8f;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    //private void Reset()
    //{
    //    _isInAir = false;
    //    _hasLanded = false;
    //    _playerMode = PlayerMode.Standing;
    //    ApplyCollider(false);
    //    Physics.gravity = new Vector3(0f, -9.81f, 0f);
    //    Time.timeScale = 1f;
    //}

    // Start is called before the first frame update
    void Start()
    {
        _isInAir = false;
        _hasLanded = false;
        _playerMode = PlayerMode.Standing;
        ApplyCollider();
        Physics.gravity = new Vector3(0f, -9.81f, 0f);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isInAir)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_isCoroutineStarted)
            {
                _animator.SetTrigger("squat");
                _isCoroutineStarted = true;
                //StartCoroutine(EnableRBRotation());
                EnableRBRotation();
            }
            else if (_isCoroutineStarted)
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    Time.timeScale *= 0.7f;
                    //jump start
                    //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    _animator.SetTrigger("jump");                    
                    _rigidbody.AddTorque(Vector3.right * -forceTorque, forcemodeTorque);
                    _rigidbody.AddForce(transform.up.normalized * force , forcemodeJump);
                    _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                    _isCoroutineStarted = false;
                    _isInAir = true;
                    Physics.gravity = new Vector3(0f, -4.5f, 0f);
                }
            }
        }
        else
        {
            if (_hasLanded)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.SetTrigger("roll");
                _playerMode = PlayerMode.Rolling;
                ApplyCollider();
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                //Debug.Log("Applying Torque");
                _rigidbody.AddTorque(Vector3.right * -forceTorque, ForceMode.VelocityChange);
                //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                //Debug.Log("Not Applying Torque");
                _animator.SetTrigger("land");
                _playerMode = PlayerMode.Landing;
                ApplyCollider();
                //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

    void ApplyCollider(bool isFlipping)
    {
        if (isFlipping)
        {
            _collider.center = flippingCenter;
            _collider.height = flippingHeight;
        }
        else
        {
            _collider.center = standingCenter;
            _collider.height = standingHeight;
        }
    }

    void ApplyCollider()
    {
        switch (_playerMode)
        {
            case PlayerMode.Standing:
                _collider.center = standingCenter;
                _collider.height = standingHeight;
                _collider.radius = radius;
                _collider.direction = 1;// y-axis
                break;
            case PlayerMode.Rolling:
                _collider.center = flippingCenter;
                _collider.height = flippingHeight;
                _collider.radius = flippingRadius;
                _collider.direction = 2; // z-axis
                break;
            case PlayerMode.Landing:
                _collider.center = landingCenter;
                _collider.height = landingHeight;
                _collider.radius = radius;
                _collider.direction = 1;// y-axis
                break;         
        }
    }

    async void EnableRBRotation()
    {
        float _rotX = 0f;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        while (_rotX > -0.4f)
        {
            //_rigidbody.AddRelativeTorque(Vector3.right * -0.1f, ForceMode.Acceleration);
            _rotX -= 1 / (100f);
            transform.localEulerAngles = new Vector3(_rotX, 0f, 0f);
        }
        await Task.Yield();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LandingZone"))
        {
            _hasLanded = true;
            if (_playerMode == PlayerMode.Rolling)
            {
                _playerMode = PlayerMode.Landing;
                ApplyCollider();
            }
            _rigidbody.angularDrag = 0.5f;
        }
    }
}
