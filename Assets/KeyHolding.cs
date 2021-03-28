using UnityEngine;
using UnityEngine.UI;

public class KeyHolding : MonoBehaviour
{
    [SerializeField]
    private KeyId keyId;

    [SerializeField]
    private Image _mainTickObj;

    [SerializeField]
    private Image _movingTickImg;

    [SerializeField]
    private Image _fill;

    [SerializeField, Range(0f, 1f)]
    private float _alpha = 1f;

    private Color _imageColor = new Color(1f, 1f, 1f, 1f);

    private const float FULL_SIZE = 255f;

    private int _rotationDirection = 1;

    public void SetDirection(int sign) => _rotationDirection = sign;

    private void OnValidate()
    {
        _imageColor = new Color(1f, 1f, 1f, _alpha);
        _mainTickObj.color = _imageColor;
        _movingTickImg.color = _imageColor;
    }
}
