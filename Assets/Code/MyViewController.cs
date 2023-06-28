using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

public class MyViewController : ListViewController
{
    [CanBeNull] public VisualElement LastPointerDownElement { get; private set; }

    private readonly VisualTreeAsset _listItemTemplate;
    private readonly CharacterEnvironmentPool _charPool;
    private readonly List<CharacterEnvironment> _charEnvList = new();
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
        var usernameLabel = element.Q<Label>("user-name");
        var userDanceNameLabel = element.Q<Label>("user-dance-name");

        var songTitle = element.Q<Label>("song-title");
        var songArtist = element.Q<Label>("song-artist");

        // Populate
        var dance = _userDances[index];
        var charEnv = _charPool.TakeOne();
        _charEnvList.Insert(index, charEnv);

        textureEl.style.backgroundImage = Background.FromRenderTexture(charEnv.RenderTexture);

        usernameLabel.text = dance.UserName;
        userDanceNameLabel.text = dance.UserDanceName;

        songTitle.text = dance.SongTitle;
        songArtist.text = dance.SongArtistName;

        // Binding info and callback to this element because they are the click target on this current setup
        textureEl.userData = index;
        textureEl.RegisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    protected override void UnbindItem(VisualElement element, int index)
    {
        _charPool.Store(_charEnvList[index]);

        var textureEl = element.Q<VisualElement>("texture");
        textureEl.UnregisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    private void PointerDownCallback(PointerDownEvent evt)
    {
        LastPointerDownElement = evt.target as VisualElement;
    }
}