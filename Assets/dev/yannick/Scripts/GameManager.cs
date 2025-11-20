using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool _playerOneReady;
    private bool _playerTwoReady;

    Transform _playerOnePos, _playerTwoPos;
    private PlayerInventory playerOneInventory, playerTwoInventory;
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

}
