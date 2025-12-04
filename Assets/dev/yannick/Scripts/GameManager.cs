using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //settings scriptable object
    [SerializeField] private SettingsObject settings;

    //players data
    private Transform _playerOnePos, _playerTwoPos;
    private PlayerInventory playerOneInventory, playerTwoInventory;

    public string leader = " ";

    public string PlayerOneZPos = "";
    public string PlayerTwoZPos = "";

    public float roadSpeed;
    public float startSpeed;
    private float speedTimer = 0f;

    [SerializeField] private float speedUpInterval = 7;

    //singleton class
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else 
        Instance = this;
        DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

    }

    private void Update()
    {
        if(_playerOnePos != null && _playerTwoPos != null)
        {
            UpdateLeader();

            //return players positions for debugging use
            PlayerOneZPos = _playerOnePos.position.z.ToString();
            PlayerTwoZPos = _playerTwoPos.position.z.ToString();
        }
        else
        {
            PlayerOneZPos = "Player 1 not found";
            PlayerTwoZPos = "Player 2 not found";
        }

        //increase speed over time
        speedTimer += Time.deltaTime;
        if(speedTimer > speedUpInterval)
        {
            IncreaseSpeed();
            speedTimer = 0f;
        }
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "start")
        {
            roadSpeed = 10;
        }


        InputManager input = FindFirstObjectByType<InputManager>();
        if(input != null)
        {
            input.SetupPlayers();
        }
    }

    private void UpdateLeader()
    {
        if (_playerOnePos.position.z > _playerTwoPos.position.z)//check if player one is in front
        {
            
            leader = "Player 1";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
            playerOneInventory.ShowCrown();

            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
            playerTwoInventory.HideCrown();
        }
        else if (_playerOnePos.position.z < _playerTwoPos.position.z)//check if player two is in front
        {
            leader = "Player 2";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
            playerOneInventory.HideCrown();

            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
            playerTwoInventory.ShowCrown();
        }
        else if (_playerOnePos.position.z == _playerTwoPos.position.z)//players are tied
        {
            //tie
            leader = "tie";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
            playerOneInventory.HideCrown();

            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
            playerTwoInventory.HideCrown();
        }
    }

    public void SetupPlayersPositions(Transform pos1, Transform pos2)
    {
        _playerOnePos = pos1;
        _playerTwoPos = pos2;
    }

    public void SetPlayersInventory(PlayerInventory inv1, PlayerInventory inv2)
    {
        playerOneInventory = inv1;
        playerTwoInventory = inv2;
    }

    public void Pause()
    {
        settings.IsPaused = !settings.IsPaused;

        if (settings.IsPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void StartGame()
    {
        PlayerMovement movementPlayer1 = playerOneInventory.GetComponent<PlayerMovement>();
        PlayerMovement movementPlayer2 = playerTwoInventory.GetComponent<PlayerMovement>();

        movementPlayer1.gameStarted = true;
        movementPlayer2.gameStarted = true;
    }

    public void IncreaseSpeed()
    {
        roadSpeed += roadSpeed / 10;
    }

    public bool PlayersFound()
    {
        return playerOneInventory != null && playerTwoInventory != null;
    }
}
