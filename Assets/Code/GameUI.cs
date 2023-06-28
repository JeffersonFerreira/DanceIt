using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _listItemTemplate;
    [SerializeField] private Repository _repository;

    private ListView _listView;
    private Scroller _scroller;
    private readonly List<UserDanceModel> _userDanceList = new();

    private float _lastScrollDelta;
    private MainViewController _viewController;

    private CharacterEnvironmentPool _charPool;

    private void Awake()
    {
        _charPool = FindObjectOfType<CharacterEnvironmentPool>();
    }

    private async void Start()
    {
        var dances = await _repository.GetUserDances();

        _userDanceList.AddRange(dances);
        _listView.RefreshItems();
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();

        _listView = doc.rootVisualElement.Q<ListView>("clip-view-list");
        _scroller = _listView.Q<Scroller>(classes: Scroller.ussClassName);
        _viewController = new MainViewController(_listItemTemplate, _charPool, _userDanceList);

        // Dynamically set "list element items" height to match the whole screen.
        // We can't just set the property for `100%` as it only accepts pixels,
        //  computing it a runtime is the only way I could figure out
        _listView.Q<VisualElement>("unity-content-viewport")
            .RegisterCallback<GeometryChangedEvent>(evt => {
                _listView.fixedItemHeight = ((VisualElement)evt.target).resolvedStyle.height;
            });

        _listView.SetViewController(_viewController);
        _listView.RegisterCallback<PointerDownEvent>(_ => _lastScrollDelta = 0);
        _listView.RegisterCallback<PointerUpEvent>(OnPointerUpCallback);
        _listView.RegisterCallback<PointerMoveEvent>(OnPointerMoveCallback);

        // Source must be set after controller
        _listView.itemsSource = _userDanceList;
    }

    private void OnPointerMoveCallback(PointerMoveEvent evt)
    {
        // For whatever reason `evt.button` is always `-1`.
        // Because of that, I need to use the old API to get this information
        if (!Input.GetMouseButton(0))
            return;

        _lastScrollDelta = -evt.deltaPosition.y;
        _scroller.value += _lastScrollDelta;
    }

    private void OnPointerUpCallback(PointerUpEvent _)
    {
        if (Mathf.Abs(_lastScrollDelta) < 0.1f)
            return;

        // Get elements on the screen and try to scroll to for the next one
        var visualElements = GetVisibleElements().ToArray();

        // When scrolling, the current and next/prev screen will be visible at the time.
        if (visualElements.Length == 2)
        {
            var targetElement = _lastScrollDelta > 0
                ? visualElements[1]
                : visualElements[0];

            StopCoroutine("ScrollTo");
            StartCoroutine("ScrollTo", targetElement.layout.y);
        }
    }

    private IEnumerator ScrollTo(float positionY)
    {
        while (Mathf.Abs(_scroller.value - positionY) > 0.1f)
        {
            _scroller.value = Mathf.MoveTowards(_scroller.value, positionY, 0.5f / Time.smoothDeltaTime);
            yield return null;
        }

        _scroller.value = positionY;
    }

    // Unity does not has a built-in way to grab visible elements from a ListView, we must code our own solution.
    private IEnumerable<VisualElement> GetVisibleElements()
    {
        var scroller = _listView.Q<Scroller>(classes: Scroller.ussClassName);
        var container = _listView.Q<VisualElement>("unity-content-container");

        foreach (var element in container.Children())
        {
            Vector2 pos = element.layout.position;
            float hei = _listView.fixedItemHeight;

            // Position is computed from element top-left
            //  while scroller value starts from top screen position.
            //
            // This operation uses that fact in consideration to evaluate how much an element is currently
            //  visible on the screen.
            float screenVisibilityFactor;
            if (scroller.value >= pos.y)
            {
                // The scroller has passed this element, by computing a diff from it's top position,
                //  we get how much the lower part is visible
                float diff = scroller.value - pos.y;
                screenVisibilityFactor = (hei - diff) / hei;
            }
            else
            {
                // We have not passed this one yet, let's perform the same operation but from it's bottom position,
                //  this way we know how much the upper part is visible
                float diff = (scroller.value + hei) - pos.y;
                screenVisibilityFactor = diff / hei;
            }

            if (screenVisibilityFactor is >= 0.0f and <= 1.0f)
                yield return element;
        }
    }
}