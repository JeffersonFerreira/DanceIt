using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class MainViewController : ListViewController
{
    private readonly VisualTreeAsset _listItemTemplate;
    private readonly CharacterEnvironmentPool _charPool;
    private readonly List<UserDanceModel> _userDances;

    public MainViewController(VisualTreeAsset template, CharacterEnvironmentPool charPool, List<UserDanceModel> userDances)
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

        // Store relevant information on this element for cleaning later
        element.userData = new ViewUserData {
            PlayableGraph = playableGraph,
            CharacterEnvironment = charEnv,
            ModelInstance = modelInstance
        };
    }

    protected override void UnbindItem(VisualElement element, int index)
    {
        if (element.userData is not ViewUserData viewData)
            return;

        viewData.PlayableGraph.Destroy();
        Object.Destroy(viewData.ModelInstance);
        _charPool.Store(viewData.CharacterEnvironment);
    }
}

public class ViewUserData
{
    public PlayableGraph PlayableGraph;
    public GameObject ModelInstance;
    public CharacterEnvironment CharacterEnvironment;
}