using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

public class MyViewController : ListViewController
{
    [CanBeNull] public VisualElement LastPointerDownElement { get; private set; }

    private readonly VisualTreeAsset _listItemTemplate;
    private readonly CharacterEnvironmentPool _charPool;
    private readonly List<CharacterEnvironment> _charEnvList = new();

    public MyViewController(VisualTreeAsset template, CharacterEnvironmentPool charPool)
    {
        _charPool = charPool;
        _listItemTemplate = template;
    }

    protected override VisualElement MakeItem()
    {
        return _listItemTemplate.Instantiate();
    }

    protected override void BindItem(VisualElement element, int index)
    {
        VisualElement visualElement = element.Q<VisualElement>("texture");

        var characterEnvironment = _charPool.TakeOne();
        _charEnvList.Insert(index, characterEnvironment);

        visualElement.userData = index;
        visualElement.style.backgroundImage = Background.FromRenderTexture(characterEnvironment.RenderTexture);

        visualElement.RegisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    protected override void UnbindItem(VisualElement element, int index)
    {
        _charPool.Store(_charEnvList[index]);
        element.UnregisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    private void PointerDownCallback(PointerDownEvent evt)
    {
        LastPointerDownElement = evt.target as VisualElement;
    }
}