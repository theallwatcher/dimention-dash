using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private SettingsObject settings;

    Transform _playerOnePos, _playerTwoPos;

    private PlayerInventory playerOneInventory, playerTwoInventory;

    public string leader = " ";

    public string PlayerOneZPos = "";
    public string PlayerTwoZPos = "";

    public float roadSpeed = 2f;
    private bool isPaused = false;

    //singleton class
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        else 
        Instance = this;
        DontDestroyOnLoad(this);
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
    }

    private void UpdateLeader()
    {
        if (_playerOnePos.position.z > _playerTwoPos.position.z)//check if player one is in front
        {
            
            leader = "Player 1";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
        }
        else if (_playerOnePos.position.z < _playerTwoPos.position.z)//check if player two is in front
        {
            leader = "Player 2";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
        }
        else if (_playerOnePos.position.z == _playerTwoPos.position.z)//players are tied
        {
            //tie
            leader = "tie";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
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
}
