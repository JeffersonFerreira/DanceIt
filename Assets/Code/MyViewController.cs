using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class MyViewController : ListViewController
{
    [CanBeNull] public VisualElement LastPointerDownElement { get; private set; }

    private readonly VisualTreeAsset _listItemTemplate;
    private readonly CharacterEnvironmentPool _charPool;
    private readonly List<UserDanceModel> _userDances;

    public MyViewController(VisualTreeAsset template, CharacterEnvironmentPool charPool, List<UserDanceModel> userDances)
    {
        _charPool = charPool;
        _userDances = userDances;
        _listItemTemplate = template;
    }

    protected override VisualElement MakeItem()
    {
        return _listItemTemplate.Instantiate();
    }

    protected override void BindItem(VisualElement element, int index)
    {
        // Query elements
        var textureEl = element.Q<VisualElement>("texture");
        var background = element.Q<VisualElement>("background");
        var usernameLabel = element.Q<Label>("user-name");
        var userDanceNameLabel = element.Q<Label>("user-dance-name");

        var songTitle = element.Q<Label>("song-title");
        var songArtist = element.Q<Label>("song-artist");

        // Populate
        var dance = _userDances[index];
        var charEnv = _charPool.TakeOne();

        textureEl.style.backgroundColor = Color.clear;
        textureEl.style.backgroundImage = Background.FromRenderTexture(charEnv.RenderTexture);
        background.style.backgroundImage = Background.FromTexture2D(dance.EnvironmentBackground);

        usernameLabel.text = dance.UserName;
        userDanceNameLabel.text = dance.UserDanceName;

        songTitle.text = dance.SongTitle;
        songArtist.text = dance.SongArtistName;

        // Create model and Play dancing animation
        var modelInstance = Object.Instantiate(dance.ModelPrefab, Vector3.zero, Quaternion.identity);
        modelInstance.transform.SetParent(charEnv.CharacterSpawnPoint, false);

        AnimationPlayableUtilities.PlayClip(
            modelInstance.GetComponent<Animator>(),
            dance.AnimationClip,
            out PlayableGraph playableGraph
        );

        // Binding info and callback to this element because they are the click target on this current setup
        textureEl.RegisterCallback<PointerDownEvent>(PointerDownCallback);

        // Store relevant information on this element for cleaning later
        textureEl.userData = new ViewUserData {
            Index = index,
            PlayableGraph = playableGraph,
            CharacterEnvironment = charEnv,
            ModelInstance = modelInstance
        };
    }

    protected override void UnbindItem(VisualElement element, int index)
    {
        var textureEl = element.Q<VisualElement>("texture");
        textureEl.UnregisterCallback<PointerDownEvent>(PointerDownCallback);

        if (textureEl.userData is ViewUserData viewData)
        {
            viewData.PlayableGraph.Destroy();
            Object.Destroy(viewData.ModelInstance);
            _charPool.Store(viewData.CharacterEnvironment);
        }
    }

    private void PointerDownCallback(PointerDownEvent evt)
    {
        LastPointerDownElement = evt.target as VisualElement;
    }
}

public class ViewUserData
{
    public int Index;
    public PlayableGraph PlayableGraph;
    public GameObject ModelInstance;
    public CharacterEnvironment CharacterEnvironment;
}