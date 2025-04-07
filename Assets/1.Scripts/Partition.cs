using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
/// <summary>
/// 캔버스 하위에 있는 UI 패널 오브젝트 객체들을 원하는 구성으로 분할 해주는 기능이 있다.(휴대폰 노치에 호환되기 위해 사용)
/// </summary>
public class Partition : MonoBehaviour
{
    private bool _hasRectTransform = false;

    private RectTransform _rectTransform = null;

    protected RectTransform getRectTransform
    {
        get
        {
            if (_hasRectTransform == false)
            {
                _rectTransform = GetComponent<RectTransform>();
                _hasRectTransform = true;
            }
            return _rectTransform;
        }
    }

    [Serializable]
    private struct Frame
    {
        [SerializeField]
        private Vector2 division;
        [SerializeField]
        private Vector2 pivot;

        public RectTransform rectTransform;

        public void Resize(Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
        {
            if (division.x < 1)
            {
                division.x = 1;
            }
            if (division.y < 1)
            {
                division.y = 1;
            }
            if (rectTransform != null)
            {
                rectTransform.pivot = pivot;
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.sizeDelta = new Vector2((sizeDelta.x / division.x) - sizeDelta.x, (sizeDelta.y / division.y) - sizeDelta.y);
            }
        }
    }

    [SerializeField]
    private float _ratio = 0;

    [SerializeField]
    private Frame[] _frames = new Frame[0];

    private Vector2 _sizeDelta = new Vector2();

    private static Vector2 screenSize = new Vector2(Screen.width, Screen.height);

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < _frames.Length - 1; i++)
        {
            for (int j = i + 1; j < _frames.Length; j++)
            {
                if (_frames[j].rectTransform != null && _frames[j].rectTransform == _frames[i].rectTransform)
                {
                    _frames[j].rectTransform = null;
                }
            }
        }
        UnityEditor.EditorApplication.delayCall += () => Resize();
    }
#endif

    private void LateUpdate()
    {
        int width = Screen.width;
        int height = Screen.height;
        if (screenSize.x != width || screenSize.y != height)
        {
            screenSize.x = width;
            screenSize.y = height;
        }
        Vector2 sizeDelta = getRectTransform.sizeDelta;
        if (_sizeDelta != sizeDelta)
        {
            _sizeDelta = sizeDelta;
        }
        Resize();
    }

    private void Resize()
    {
        if (screenSize.x != 0 && screenSize.y != 0)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 ratio = safeArea.size / screenSize;
            Vector2 sizeDelta = new Vector2(_sizeDelta.x * ratio.x, _sizeDelta.y * ratio.y);
            Vector2 anchorMin = new Vector2(safeArea.position.x / screenSize.x, safeArea.position.y / screenSize.y);
            Vector2 anchorMax = new Vector2((safeArea.position.x + safeArea.size.x) / screenSize.x, (safeArea.position.y + safeArea.size.y) / screenSize.y);
            //값이 양수이면 가로폭이 좁아짐
            if (_ratio > 0)
            {
                float value = (safeArea.size.y / safeArea.size.x) * 0.5f * (1 / (_ratio + 1));
                anchorMin.x = Mathf.Clamp(0.5f - value, anchorMin.x, 0.5f);
                anchorMax.x = Mathf.Clamp(0.5f + value, 0.5f, anchorMax.x);
            }
            //값이 음수이면 세로폭이 좁아짐
            else if (_ratio < 0)
            {
                float value = (safeArea.size.x / safeArea.size.y) * 0.5f * (1 / (-_ratio + 1));
                anchorMin.y = Mathf.Clamp(0.5f - value, anchorMin.y, 0.5f);
                anchorMax.y = Mathf.Clamp(0.5f + value, 0.5f, anchorMax.y);
            }
            for (int i = 0; i < _frames.Length; i++)
            {
                _frames[i].Resize(anchorMin, anchorMax, sizeDelta);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("화면 비율 확인하기")]
    private void Log()
    {
        if (_ratio > 0)
        {
            Debug.Log("화면 비율 1:" + (_ratio + 1));
        }
        else if (_ratio < 0)
        {
            Debug.Log("화면 비율 " + (-_ratio + 1) + ":1");
        }
        else
        {
            Debug.Log("화면 비율 제한 없음");
        }
    }
#endif
}