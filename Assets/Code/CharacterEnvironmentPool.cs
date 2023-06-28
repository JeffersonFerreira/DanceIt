using System.Collections.Generic;
using UnityEngine;

public class CharacterEnvironmentPool : MonoBehaviour
{
    [SerializeField] private CharacterEnvironment _prefab;
    [SerializeField] private float _spaceBetween = 5f;
    [SerializeField] private Transform _spawnPoint;

    private readonly Queue<CharacterEnvironment> _poolQueue = new();
    private int _creationCount;

    public CharacterEnvironment TakeOne()
    {
        if (!_poolQueue.TryDequeue(out var charEnv))
            charEnv = MakeOne();

        charEnv.OnTaken();
        return charEnv;
    }

    public void Store(CharacterEnvironment charEnv)
    {
        charEnv.OnStored();
        _poolQueue.Enqueue(charEnv);
    }

    private CharacterEnvironment MakeOne()
    {
        // Using "creationCount" to put each item side by side
        var pos = Vector3.right * _creationCount * _spaceBetween;
        var newInstance = Instantiate(_prefab, pos, Quaternion.identity, _spawnPoint);

        _creationCount++;
        return newInstance;
    }
}