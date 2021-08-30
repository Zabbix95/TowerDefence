using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Transform _arrow;

    private GameTile _north, _east, _south, _west, _nextOnPath;
    private int _distance;
    private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 0f);
    private Quaternion _southRotaion = Quaternion.Euler(90f, 180f, 0f);
    private Quaternion _westRotation = Quaternion.Euler(90f, 270f, 0f);
    private GameTileContent _content;

    public GameTileContent Content
    {
        get => _content;
        set
        {
            if (_content != null)
            {
                _content.Recycle();
            }

            _content = value;
            _content.transform.localPosition = transform.localPosition;
        }
    }

    public bool HasPath => _distance != int.MaxValue;
    public bool IsAlternative { get; set; }

    public static void MakeEastWestNeighbours(GameTile east, GameTile west)
    {
        Debug.Assert(west._east == null && east._west == null, "Необходимо переназначить соседей т.к. оба равны null");
        west._east = east;
        east._west = west;
    }

    public static void MakeNorthSouthNeighbours(GameTile north, GameTile south)
    {
        Debug.Assert(north._south == null && south._north == null, "Необходимо переназначить соседей т.к. оба равны null");
        north._south = south;
        south._north = north;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        _nextOnPath = null;
    }

    public void BecomeDestination()
    {
        _distance = 0;
        _nextOnPath = null;
    }

    private GameTile GrowPathTo(GameTile neighbour)
    {
        if (!HasPath || neighbour == null || neighbour.HasPath)
            return null;

        neighbour._distance = _distance + 1;
        neighbour._nextOnPath = this;
        return neighbour.Content.Type != GameTileContentType.Wall ? neighbour : null;
    }

    public GameTile GrowPathToNorth() => GrowPathTo(_north);

    public GameTile GrowPathToEast() => GrowPathTo(_east);

    public GameTile GrowPathToSouth() => GrowPathTo(_south);

    public GameTile GrowPathToWest() => GrowPathTo(_west);

    public void ShowPath()
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }

        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            _nextOnPath == _north ? _northRotation :
            _nextOnPath == _east ? _eastRotation :
            _nextOnPath == _south ? _southRotaion :
            _westRotation;
    }
}
