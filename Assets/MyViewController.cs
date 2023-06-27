using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class MyViewController : ListViewController
{
    [CanBeNull] public VisualElement LastPointerDownElement { get; private set; }

    private List<RenderTexture> _renderTextures;
    private VisualTreeAsset _listItemTemplate;

    public MyViewController(List<RenderTexture> renderTextures, VisualTreeAsset template) : base()
    {
        _renderTextures = renderTextures;
        _listItemTemplate = template;
    }

    protected override VisualElement MakeItem()
    {
        return _listItemTemplate.Instantiate();
    }

    protected override void BindItem(VisualElement element, int index)
    {
        VisualElement visualElement = element.Q<VisualElement>("texture");

        visualElement.userData = index;
        visualElement.style.backgroundImage = Background.FromRenderTexture(_renderTextures[index]);

        visualElement.RegisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    protected override void UnbindItem(VisualElement element, int index)
    {
        element.UnregisterCallback<PointerDownEvent>(PointerDownCallback);
    }

    private void PointerDownCallback(PointerDownEvent evt)
    {
        LastPointerDownElement = evt.target as VisualElement;
    }
}