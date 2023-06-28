using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _listItemTemplate;
    [SerializeField] private Repository _repository;

    private UIDocument _doc;
    private ListView _listView;
    private Scroller _scroller;

    private float _lastScrollDelta = 0;
    private VisualElement _pointerDownElement;
    private MyViewController _viewController;

    private CharacterEnvironmentPool _charPool;

    private readonly List<UserDanceModel> _userDances = new();

    private void Awake()
    {
        _charPool = FindObjectOfType<CharacterEnvironmentPool>();
    }

    private async void Start()
    {
        var dances = await _repository.GetUserDances();

        _userDances.AddRange(dances);
        _listView.RefreshItems();
    }

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();

        _listView = _doc.rootVisualElement.Q<ListView>("clip-view-list");
        Assert.IsNotNull(_listView);

        _scroller = _listView.Q<Scroller>(classes: Scroller.ussClassName);
        // Dynamically set "list element items" height to match the whole screen
        _listView.Q<VisualElement>("unity-content-viewport")
            .RegisterCallback<GeometryChangedEvent>(evt =>
            {
                var visualElement = (VisualElement)evt.target;
                _listView.fixedItemHeight = visualElement.resolvedStyle.height;
            });

        _viewController = new MyViewController(_listItemTemplate, _charPool, _userDances);
        _listView.SetViewController(_viewController);
        _listView.RegisterCallback<PointerMoveEvent>(OnPointerMoveCallback);
        _listView.RegisterCallback<PointerUpEvent>(OnPointerUpCallback);

        // Source must be set after controller
        _listView.itemsSource = _userDances;
    }

    private void OnPointerMoveCallback(PointerMoveEvent evt)
    {
        if (!Input.GetMouseButton(0))
            return;

        _lastScrollDelta = -evt.deltaPosition.y;
        _scroller.value -= evt.deltaPosition.y;
    }

    private void OnPointerUpCallback(PointerUpEvent evt)
    {
        if (Mathf.Abs(_lastScrollDelta) < 0.1f)
            return;

        if (_viewController.LastPointerDownElement is { userData: int elIndex })
        {
            var dir = _lastScrollDelta > 0 ? 1 : -1;
            var targetDir = (elIndex + dir) * _listView.fixedItemHeight;

            StopCoroutine("ScrollTo");
            StartCoroutine("ScrollTo", targetDir);
        }
        else
        {
            Debug.LogError("Fail to process last pointer down");
        }
    }

    IEnumerator ScrollTo(float positionY)
    {
        while (Mathf.Abs(_scroller.value - positionY) > 0.1f)
        {
            _scroller.value = Mathf.MoveTowards(_scroller.value, positionY, 0.5f / Time.deltaTime);
            yield return null;
        }

        _scroller.value = positionY;
    }
}


// This piece of code will be useful at some point, give me some time before removing it.
// private void Update()
// {
//     if (Input.GetKeyDown(KeyCode.T))
//     {
//         var container = _listView.Q<VisualElement>("unity-content-container");
//         var scroller = _listView.Q<Scroller>(classes: Scroller.ussClassName);
//
//         // VisualElement visualElement = _listView.contentContainer[0];
//         // Debug.Log(visualElement);
//
//         foreach (var element in container.Children())
//         {
//             var pos = element.layout.position;
//             var hei = _listView.fixedItemHeight;
//
//             float screenVisibilityFactor;
//             if (scroller.value >= pos.y)
//             {
//                 var diff = scroller.value - pos.y;
//                 screenVisibilityFactor = (hei - diff) / hei;
//             }
//             else
//             {
//                 var diff = (scroller.value + hei) - pos.y;
//                 screenVisibilityFactor = diff / hei;
//             }
//
//             // if (f > 0.5f)
//             // {
//             //     _listView.ScrollTo(element);
//             //     return;
//             // }
//
//         }
//     }
// }