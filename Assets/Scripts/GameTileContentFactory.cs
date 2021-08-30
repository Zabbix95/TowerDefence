using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : ScriptableObject
{
    [SerializeField] private GameTileContent _destinationPrefab;
    [SerializeField] private GameTileContent _emptyPrefab;
    [SerializeField] private GameTileContent _wallPrefab;
    [SerializeField] private GameTileContent _spawnPrefab;

    private Scene _contentScene;

    private GameTileContent Get(GameTileContent content)
    {
        GameTileContent instance = Instantiate(content);
        instance.OriginFactory = this;
        MoveToFactoryScene(instance.gameObject);
        return instance;
    }

    public GameTileContent Get(GameTileContentType type)
    {
        switch(type)
        {
            case GameTileContentType.Destination:
                return Get(_destinationPrefab);
            case GameTileContentType.Empty:
                return Get(_emptyPrefab);
            case GameTileContentType.Wall:
                return Get(_wallPrefab);
            case GameTileContentType.SpawnPoint:
                return Get(_spawnPrefab);
        }

        return null;
    }
    
    public void Reclaim(GameTileContent content)
    {
        Destroy(content.gameObject);        
    }

    private void MoveToFactoryScene(GameObject o)
    {
        if (!_contentScene.isLoaded)
        {
            if (Application.isEditor)
            {
                _contentScene = SceneManager.GetSceneByName(name);

                if (!_contentScene.isLoaded)
                {
                    _contentScene = SceneManager.CreateScene(name);
                }
            }
            else
            {
                _contentScene = SceneManager.CreateScene(name);
            }
        }

        SceneManager.MoveGameObjectToScene(o, _contentScene);
    }
}
