using UnityEngine;

public class CharacterEnvironment : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _charAnchorPoint;

    public Transform CharacterSpawnPoint => _charAnchorPoint;
    public RenderTexture RenderTexture { get; private set; }

    private void Awake()
    {
        // Create render tex
        RenderTexture = new RenderTexture(512, 1024, 1, RenderTextureFormat.Default);
        _cam.targetTexture = RenderTexture;

        // Disable components until required
        OnStored();
    }

    public void OnTaken()
    {
        _cam.enabled = true;
        _cam.gameObject.SetActive(true);
    }

    public void OnStored()
    {
        _cam.enabled = false;
        _cam.gameObject.SetActive(false);
    }
}