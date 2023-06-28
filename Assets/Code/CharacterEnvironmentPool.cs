using System.Collections.Generic;
using UnityEngine;

public class CharacterEnvironmentPool : MonoBehaviour
{
    [SerializeField] private CharacterEnvironment _prefab;
    [SerializeField] private float _spaceBetween = 5f;
    [SerializeField] private Transform _spawnPoint;

    private readonly Queue<CharacterEnvironment> _poolQueue = new();
    private int _creationCount;

    public CharacterEnvironment Request()
    {
        if (!_poolQueue.TryDequeue(out var charEnv))
            charEnv = MakeOne();

        charEnv.OnRequired();
        return charEnv;
    }

    public void Release(CharacterEnvironment charEnv)
    {
        _poolQueue.Enqueue(charEnv);
        charEnv.OnReleased();
    }

    private CharacterEnvironment MakeOne()
    {
        // Compute world position for it
        var pos = Vector3.right * _creationCount * _spaceBetween;
        return Instantiate(_prefab, pos, Quaternion.identity, _spawnPoint);
    }
}