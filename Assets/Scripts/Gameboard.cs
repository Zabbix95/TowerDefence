using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;

    private Vector2Int _size;
    private GameTile[] _tiles;
    private Queue<GameTile> _searchfrontier = new Queue<GameTile>();
    private GameTileContentFactory _contentFactory;

    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        _size = size;
        _ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);        

        _tiles = new GameTile[size.x * size.y];
        _contentFactory = contentFactory;
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = _tiles[i] = Instantiate(_tilePrefab, transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbours(tile, _tiles[i - 1]);
                }

                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbours(tile, _tiles[i - size.x]);
                }

                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }

                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            }
        }

        ToggleDestination(_tiles[_tiles.Length / 2]);
    }    

    public bool FindPaths()
    {
        foreach (var tile in _tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                _searchfrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (_searchfrontier.Count == 0)
        {
            return false;
        }

        while (_searchfrontier.Count > 0)
        {
            GameTile tile = _searchfrontier.Dequeue();

            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchfrontier.Enqueue(tile.GrowPathToNorth());
                    _searchfrontier.Enqueue(tile.GrowPathToSouth());
                    _searchfrontier.Enqueue(tile.GrowPathToEast());
                    _searchfrontier.Enqueue(tile.GrowPathToWest());
                }
                else
                {
                    _searchfrontier.Enqueue(tile.GrowPathToWest());
                    _searchfrontier.Enqueue(tile.GrowPathToEast());
                    _searchfrontier.Enqueue(tile.GrowPathToSouth());
                    _searchfrontier.Enqueue(tile.GrowPathToNorth());
                }                
            }
        }

        foreach (var tile in _tiles)
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }

        foreach (var tile in _tiles)
        {
            tile.ShowPath();
        }

        return true;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            FindPaths();
        }
    }

    public GameTile GetTile(Ray ray)
    {        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + _size.x * 0.5f);
            int y = (int)(hit.point.z + _size.y * 0.5f);
            
            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _tiles[x + y * _size.x];
            }
        }

        return null;
    }
}
