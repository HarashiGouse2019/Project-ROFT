using UnityEngine;
using UnityEngine.UI;
public class KeyControls : MonoBehaviour
{
    [SerializeField]
    private ObjectPooler _objectPooler;

    [SerializeField]
    private KeyId _keyId;

    [SerializeField]
    private PulseEffect _pulseEffect;

    [SerializeField]
    private KeyHolding _keyHolding;

    [SerializeField]
    private Image _graphics;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ObjectPooler GetObjectPooler() => _objectPooler;
    public KeyId GetKeyId() => _keyId;
    public PulseEffect GetPulseEffect() => _pulseEffect;
    public KeyHolding GetKeyHolding() => _keyHolding;
    public Image GetGraphics() => _graphics;
}
