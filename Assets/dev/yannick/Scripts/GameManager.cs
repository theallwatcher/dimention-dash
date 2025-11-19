using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool _playerOneReady;
    private bool _playerTwoReady;

    [SerializeField] Transform _playerOnePos, _playerTwoPos;
    [SerializeField] private PlayerInventory playerOneInventory;
    [SerializeField] private PlayerInventory playerTwoInventory;
    public string leader = " ";

    public float roadSpeed = 2f;


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


            if (_playerOnePos.position.z > _playerTwoPos.position.z)
            {
                //player one is in front
                leader = "Player 1";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
            }
            else if (_playerOnePos.position.z < _playerTwoPos.position.z)
            {
                //player 2 is in front
                leader = "Player 2";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.LastPlace);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.FirstPlace);
        }        
        else if(_playerOnePos.position.z == _playerTwoPos.position.z)
        {
            //tie
            leader = "tie";
            playerOneInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
            playerTwoInventory.SetPosition(PlayerInventory.PlayerPosition.Tie);
        }
    }

    public void StartGame()
    {
        
    }

    public void PlayerOneReady()
    {
        _playerOneReady = true; 
    }

    public void PlayerTwoReady()
    {
        _playerTwoReady = true;
    }

}
