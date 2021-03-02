using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] InputActionReference _moveP1Action;
    [SerializeField] InputActionReference _moveP2Action;
    [SerializeField] InputActionReference _startGameAction;
    [SerializeField] InputActionReference _reloadGameAction;
    [SerializeField] InputActionReference _escapeGameAction;

    [SerializeField] Transform _paddleP1Trans;
    [SerializeField] Transform _paddleP2Trans;

    [SerializeField] Transform _goalP1Trans;
    [SerializeField] Transform _goalP2Trans;

    [SerializeField] Transform _scoreTextP1Trans;
    [SerializeField] Transform _scoreTextP2Trans;

    [SerializeField] Transform _wallTopTrans;
    [SerializeField] Transform _wallBotTrans;

    [SerializeField] Transform _ballTrans;

    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _turnSpeed = 5f;
    [SerializeField] float _ballSpeed = 5f;
    [SerializeField] float _ballReflectSpeedMod = 5f;

    Rigidbody _paddleP1Rb;
    Rigidbody _paddleP2Rb;
    Rigidbody _ballRb;

    TextMesh _scoreTextP1;
    TextMesh _scoreTextP2;

    int _numOfBallReflects = 0;
    int _scoreP1 = 0;
    int _scoreP2 = 0;

    Vector2 _inputP1Vec2 = Vector2.zero;
    Vector2 _inputP2Vec2 = Vector2.zero;
    Vector3 _ballVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _goalP1Trans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => { _scoreP2 += 1; ResetGame(); };
        _goalP2Trans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => { _scoreP1 += 1; ResetGame(); };
        _paddleP1Trans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => BallHitPaddle(c.GetContact(0).normal);
        _paddleP2Trans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => BallHitPaddle(c.GetContact(0).normal);
        _wallBotTrans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => FlipBallY();
        _wallTopTrans.GetComponent<CollisionEventBehaviour>().OnCollisionEnterEvent += (c, t) => FlipBallY();

        _scoreTextP1 = _scoreTextP1Trans.GetComponent<TextMesh>();
        _scoreTextP2 = _scoreTextP2Trans.GetComponent<TextMesh>();

        _paddleP1Rb = _paddleP1Trans.GetComponent<Rigidbody>();
        _paddleP2Rb = _paddleP2Trans.GetComponent<Rigidbody>();
        _ballRb     = _ballTrans.GetComponent<Rigidbody>();

        _moveP1Action.action.performed  += cbt => SetInputVector(cbt, ref _inputP1Vec2);
        _moveP1Action.action.canceled   += cbt => SetInputVector(cbt, ref _inputP1Vec2);
        _moveP2Action.action.performed  += cbt => SetInputVector(cbt, ref _inputP2Vec2);
        _moveP2Action.action.canceled   += cbt => SetInputVector(cbt, ref _inputP2Vec2);

        _startGameAction.action.performed += StartGame;
        _reloadGameAction.action.performed += cbt => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        _escapeGameAction.action.performed += cbt => Application.Quit();

        _moveP1Action.action.Enable();
        _moveP2Action.action.Enable();
        _startGameAction.action.Enable();
        _reloadGameAction.action.Enable();
        _escapeGameAction.action.Enable();
    }

    private void FixedUpdate()
    {
        _paddleP1Rb.MovePosition(_paddleP1Rb.position + new Vector3(0f, _inputP1Vec2.y * _moveSpeed));
        _paddleP2Rb.MovePosition(_paddleP2Rb.position + new Vector3(0f, _inputP2Vec2.y * _moveSpeed));

        _paddleP1Rb.MoveRotation(_paddleP1Rb.rotation * Quaternion.Euler(0f, 0f, _inputP1Vec2.x * _turnSpeed));
        _paddleP2Rb.MoveRotation(_paddleP2Rb.rotation * Quaternion.Euler(0f, 0f, _inputP2Vec2.x * _turnSpeed));

        _ballRb.velocity = _ballVelocity;
    }

    void FlipBallY()
    {
        _ballVelocity = new Vector3(_ballVelocity.x, -_ballVelocity.y);
    }

    void BallHitPaddle(Vector3 normal)
    {
        _numOfBallReflects += 1;
        FlipBallByNormal(-normal, _ballSpeed + _numOfBallReflects * _ballReflectSpeedMod * _ballSpeed);
    }

    void FlipBallByNormal(Vector3 normal, float ballSpeed)
    {
        _ballVelocity = normal * ballSpeed;
    }

    void SetInputVector(InputAction.CallbackContext cbt, ref Vector2 inputVec2)
    {
        inputVec2 = cbt.ReadValue<Vector2>();
    }

    void StartGame(InputAction.CallbackContext _)
    {
        _startGameAction.action.performed -= StartGame;
        _ballVelocity += new Vector3(_ballSpeed, 0, 0);
    }

    void ResetGame()
    {
        _scoreTextP1.text = _scoreP1.ToString();
        _scoreTextP2.text = _scoreP2.ToString();
        _numOfBallReflects = 0;
        _ballVelocity = Vector3.zero;
        _ballRb.position = Vector3.zero;
        _startGameAction.action.performed += StartGame;
    }
}
