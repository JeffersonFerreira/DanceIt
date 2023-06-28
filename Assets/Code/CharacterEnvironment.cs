using UnityEngine;

public class CharacterEnvironment : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _charAnchorPoint;

    public Camera Camera => _cam;
    public Transform CharacterSpawnPoint => _charAnchorPoint;
    public RenderTexture RenderTexture { get; private set; }

    private void Awake()
    {
        // Create render tex
        RenderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.Default);
        _cam.targetTexture = RenderTexture;

        // Disable components until required
        OnReleased();
    }

    public void OnRequired()
    {
        _cam.enabled = true;
        _cam.gameObject.SetActive(true);
    }

    public void OnReleased()
    {
        _cam.enabled = false;
        _cam.gameObject.SetActive(false);
    }
}