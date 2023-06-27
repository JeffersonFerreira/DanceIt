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
    private UIDocument _doc;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _listView.ScrollToItem(++_i);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            _listView.ScrollToItem(--_i);

        if (Input.GetKeyDown(KeyCode.T))
        {
            var container = _listView.Q<VisualElement>("unity-content-container");
            var scroller = _listView.Q<Scroller>(classes: Scroller.ussClassName);

            // VisualElement visualElement = _listView.contentContainer[0];
            // Debug.Log(visualElement);

            foreach (var element in container.Children())
            {
                var pos = element.layout.position;
                var hei = _listView.fixedItemHeight;

                float screenVisibilityFactor;
                if (scroller.value >= pos.y)
                {
                    var diff = scroller.value - pos.y;
                    screenVisibilityFactor = (hei - diff) / hei;
                }
                else
                {
                    var diff = (scroller.value + hei) - pos.y;
                    screenVisibilityFactor = diff / hei;
                }

                // if (f > 0.5f)
                // {
                //     _listView.ScrollTo(element);
                //     return;
                // }
            }
        }
    }

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        // Dynamically set "list element items" height to match the whole screen
        _doc
            .rootVisualElement.Q<VisualElement>("unity-content-viewport")
            .RegisterCallback<GeometryChangedEvent>(evt =>
            {
                var visualElement = (VisualElement)evt.target;
                _listView.fixedItemHeight = visualElement.resolvedStyle.height;
            });

        _listView = _doc.rootVisualElement.Q<ListView>("main-list");

        _listView.SetViewController(new MyViewController(_renderTextures, _listItemTemplate));

        // Source must be set after controller
        _listView.itemsSource = _renderTextures;
    }
}

public class MyViewController : ListViewController
{
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
        RenderTexture tex = _renderTextures[index];

        VisualElement visualElement = element.Q<VisualElement>("texture");
        visualElement.style.backgroundImage = Background.FromRenderTexture(tex);

        // Debug.Log($"Element '{index}' was bound");
    }
}