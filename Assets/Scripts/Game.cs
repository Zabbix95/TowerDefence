using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private Gameboard _board;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameTileContentFactory _contentFactory;

    private Ray _touchRay => _camera.ScreenPointToRay(Input.mousePosition);

    private void Start()
    {
        _board.Initialize(_boardSize, _contentFactory);
    }

    private void OnValidate()
    {
        if (_boardSize.x < 2)
            _boardSize.x = 2;

        if (_boardSize.y < 2)
            _boardSize.y = 2;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }
    }

    private void HandleTouch()
    {
        GameTile tile = _board.GetTile(_touchRay);
        if (tile != null)
        {
            _board.ToggleWall(tile);
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = _board.GetTile(_touchRay);
        if (tile != null)
        {
            _board.ToggleDestination(tile);
        }
    }
}
