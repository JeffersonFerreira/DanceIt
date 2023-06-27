using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    [SerializeField] private List<RenderTexture> _renderTextures;
    [SerializeField] private VisualTreeAsset _listItemTemplate;

    private ListView _listView;

    private int _i;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _listView.ScrollToItem(++_i);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            _listView.ScrollToItem(--_i);
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();

        _listView = doc.rootVisualElement.Q<ListView>("main-list");

        // Dynamically set "list element items" height to match the whole screen
        doc.rootVisualElement.Q<VisualElement>("unity-content-viewport")
            .RegisterCallback<GeometryChangedEvent>(evt =>
            {
                var visualElement = (VisualElement)evt.target;
                _listView.fixedItemHeight = visualElement.resolvedStyle.height;
            });

        _listView.itemsSource = _renderTextures;
        _listView.makeItem = () => _listItemTemplate.Instantiate();

        _listView.bindItem = (element, index) =>
        {
            RenderTexture tex = _renderTextures[index];

            VisualElement visualElement = element.Q<VisualElement>("texture");
            visualElement.style.backgroundImage = Background.FromRenderTexture(tex);
        };
    }
}