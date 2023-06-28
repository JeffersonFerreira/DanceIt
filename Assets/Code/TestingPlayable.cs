using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestingPlayable : MonoBehaviour
{
    [SerializeField] private AnimationClip _clip;

    private PlayableGraph _graph;

    void Start()
    {
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), _clip, out _graph);
    }

    private void OnDestroy()
    {
        _graph.Destroy();
    }
}