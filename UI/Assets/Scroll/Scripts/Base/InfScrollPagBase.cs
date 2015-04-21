using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfScrollPagBase : ScrollRect
{

    [Tooltip ("Minimum Drag in % to consider it Drag")]
    [Range (1, 100)]
    public int dragThreshold = 25;

    [Range (0.1f, 5.0f)]
    public float movementTime = 0.5f;
    public AnimationCurve movementCurve;

    protected bool _initialized = false;
    protected Vector2 _size;

    protected RectTransform _rect;
    // GMM child Rect Transforms
    protected List<RectTransform> _childRectTransformList = new List<RectTransform> ();

    // GMM pivots
    protected int _minPivot;
    protected int _maxPivot;

    // GMM _offset define how many pages are we going to move
    protected int _offset;

    protected Vector2 _lastContentAnchoredPosition;
    protected Vector2 _targetPosition;
    // GMM _touchPosition nos sirve para saber cual fue el primer toque
    protected Vector2 _touchPosition;
    protected Vector2 _startPosition;
    protected bool isMovementActive = false;

    private float _time = 0.0f;
    private float _rate = 0.0f;

    // Use this for initialization
    protected override void Start ()
    {
        if (horizontal == false || vertical == true)
        {
            Debug.LogWarning ("[InfiniteScrollPaging] InfiniteScrollPaging only works in horizontal");
            this.enabled = false;
            return;
        }
        this.movementType = MovementType.Unrestricted;
        this.inertia = false;

        // GMM Get the size of the RectTransform of the scroll
        _rect = GetComponent<RectTransform> ();
        _size = new Vector2 (_rect.rect.width, _rect.rect.height);

        //_screenSizeManager = GetComponent<ScreenSizeManager> ();
        //if (_screenSizeManager != null)
        //{
        //    _screenSizeManager.OnScreenSizeChange += this.OnScreenSizeChange;
        //}

        Initialize ();
        // GMM set the comparison position - Its gonna be the anchoredPosition of the LeftPivot
        _lastContentAnchoredPosition = content.anchoredPosition;
    }

    protected virtual void Initialize ()
    {
        if (_initialized == false)
        {
            RectTransform _child = null;
            for (int i = 0; i < content.childCount; ++i)
            {
                _child = content.GetChild (i) as RectTransform;
                if (_child != null)
                {
                    _childRectTransformList.Add (_child);
                }
            }
            // GMM supossed that the childrens are ordered
            //if (horizontal == true)
            //{
            _minPivot = 0;
            _maxPivot = _childRectTransformList.Count - 1;
            //}
            _initialized = true;
        }
    }

    protected virtual void SwapMinMax ()
    {
        RectTransform _min = _childRectTransformList[_minPivot];
        RectTransform _max = _childRectTransformList[_maxPivot];

        //if (horizontal == true && vertical == false)
        //{
        // GMM update the list of rectransforms
        _maxPivot++;
        if (_maxPivot >= _childRectTransformList.Count)
        {
            _maxPivot = 0;
        }
        _minPivot++;
        if (_minPivot >= _childRectTransformList.Count)
        {
            _minPivot = 0;
        }
        Vector2 _pos = new Vector2 (_max.anchoredPosition.x, _max.anchoredPosition.y);
        _pos.x = _pos.x + _min.rect.width;
        _min.anchoredPosition = _pos;
        //}
        /*else if (vertical == true && horizontal == false)
        {
            // GMM update the list of rectransforms
            _maxPivot++;
            if (_maxPivot >= _childRectTransformList.Count)
            {
                _maxPivot = 0;
            }
            _minPivot++;
            if (_minPivot >= _childRectTransformList.Count)
            {
                _minPivot = 0;
            }
            Vector2 _pos = new Vector2 (_max.anchoredPosition.x, _max.anchoredPosition.y);
            _pos.y = _pos.y + _min.rect.height + padding;
            _min.anchoredPosition = _pos;
        }*/
    }

    /// <summary>
    /// Funcion que hace el intercambio de RectTransform de un lado a otro.
    /// En este caso lo hace de Derecha a Izquierda o Arrriba Abajo
    /// </summary>
    protected virtual void SwapMaxMin ()
    {
        RectTransform _min = _childRectTransformList[_minPivot];
        RectTransform _max = _childRectTransformList[_maxPivot];
        //if (horizontal == true && vertical == false)
        //{
        // GMM Actualizamos pivotes de la lista de RectTransforms
        _maxPivot--;
        if (_maxPivot < 0)
        {
            _maxPivot = _childRectTransformList.Count - 1;
        }
        _minPivot--;
        if (_minPivot < 0)
        {
            _minPivot = _childRectTransformList.Count - 1;
        }
        Vector2 _pos = new Vector2 (_min.anchoredPosition.x, _min.anchoredPosition.y);
        _pos.x = _pos.x - _max.rect.width;
        _max.anchoredPosition = _pos;
        //}
        /*else if (vertical == true && horizontal == false)
        {
            // GMM Actualizamos pivotes de la lista de RectTransforms
            _maxPivot--;
            if (_maxPivot < 0)
            {
                _maxPivot = _childRectTransformList.Count - 1;
            }
            _minPivot--;
            if (_minPivot < 0)
            {
                _minPivot = _childRectTransformList.Count - 1;
            }
			
            Vector2 _pos = new Vector2 (_min.anchoredPosition.x, _min.anchoredPosition.y);
            _pos.y = _pos.y - _max.rect.height - padding;
            _max.anchoredPosition = _pos;
        }*/
    }

    protected override void OnEnable ()
    {
        base.OnEnable ();
    }

    /// <summary>
    /// CalculateTarget
    /// Numero de paginas que te quieres mover. Negativo sería hacia la derecha, positivo sería hacia la izquierda
    /// </summary>
    /// <param name="offset">Offset.</param>
    protected void CalculateTarget (int offset)
    {
        _targetPosition = _lastContentAnchoredPosition + new Vector2 (offset * content.rect.width, 0.0f);
    }

    public override void OnBeginDrag (PointerEventData eventData)
    {
        base.OnBeginDrag (eventData);
        _touchPosition = content.anchoredPosition;
    }

    public override void OnEndDrag (PointerEventData eventData)
    {
        base.OnEndDrag (eventData);

        _startPosition = content.anchoredPosition;

        float distance = Vector2.Distance (_startPosition, _touchPosition);
        _time = 0.0f;

        // Si no hemos superado el threshold
        if (distance < _size.x * dragThreshold / 100)
        {
            _targetPosition = _lastContentAnchoredPosition;
            isMovementActive = true;
        }
        // GMM Drag hacia la izquierda, queremos movernos hacia la izquierda
        // GMM Por lo tanto nuestro target es negativo
        else
        {
            if (content.anchoredPosition.x < _lastContentAnchoredPosition.x)
            {
                _offset = -1;
                CalculateTarget (_offset);
                isMovementActive = true;
            }
            // GMM Drag hacia la derecha, queremos movernos hacia la derecha
            // GMM Por lo tanto nuestro target es positivo
            else if (content.anchoredPosition.x > _lastContentAnchoredPosition.x)
            {
                _offset = 1;
                CalculateTarget (_offset);
                isMovementActive = true;
            }
        }
    }

    protected override void LateUpdate ()
    {
        // GMM No llamamos a LateUpdate de ScrollRect
        //base.LateUpdate();

        if (isMovementActive == true)
        {
            _rate = 1.0f / movementTime;
            _time += Time.deltaTime * _rate;
            // Lerp the camera's position between it's current position and it's new position.
            content.anchoredPosition = Vector2.Lerp (_startPosition, _targetPosition, movementCurve.Evaluate (_time));

            if (_time >= 1.0f)
            {
                if (_offset > 0)
                {
                    SwapMaxMin ();
                }
                else if (_offset < 0)
                {
                    SwapMinMax ();
                }
                _lastContentAnchoredPosition = content.anchoredPosition;
                _offset = 0;
                isMovementActive = false;
            }
        }
    }



}
