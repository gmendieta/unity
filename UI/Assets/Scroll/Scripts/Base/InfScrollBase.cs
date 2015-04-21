using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


internal static class InfScrollUtils
{
    /// <summary>
    /// Get MAX and MIN coordinates in each axis from array of vectors _coords
    /// </summary>
    /// <param name="_coords"></param>
    /// <param name="_maxCoords"></param>
    /// <param name="_minCoords"></param>
    public static void GetMinMaxCoordinates (Vector3[] _coords, ref Vector3 _maxCoords, ref Vector3 _minCoords)
    {
        _maxCoords.x = _maxCoords.y = _maxCoords.z = float.NegativeInfinity;
        _minCoords.x = _minCoords.y = _minCoords.z = float.PositiveInfinity;
        
        for (int i = 0; i < _coords.Length; ++i)
        {            
            if (_coords[i].x < _minCoords.x) _minCoords.x = _coords[i].x;
            if (_coords[i].y < _minCoords.y) _minCoords.y = _coords[i].y;
            if (_coords[i].z < _minCoords.z) _minCoords.z = _coords[i].z;
        }
 
        for (int i = 0; i < _coords.Length; ++i)
        {
            if (_coords[i].x > _maxCoords.x) _maxCoords.x = _coords[i].x;
            if (_coords[i].y > _maxCoords.y) _maxCoords.y = _coords[i].y;
            if (_coords[i].z > _maxCoords.z) _maxCoords.z = _coords[i].z;
        }        
    }

    /// <summary>
    /// Convert _coords from World to Screen coordinates using Camera _camera
    /// </summary>    
    public static void WorldToScreenCoordinates (Camera _camera, Vector3[] _coords)
    {
        if (_camera != null)
        {
            for (int i = 0; i < _coords.Length; ++i)
            {
                _coords[i] = _camera.WorldToScreenPoint (_coords[i]);
            }
        }
    }
}

public class InfScrollBase : ScrollRect 
{    
    public bool repositionOnInitialize = false;   
    public int padding = 10;

	// GMM Esto nos va a servir para controlar que el Scroll este inicializado
	// antes de hacer ciertas funciones
	protected bool _initialized = false;

    protected Vector2 _lastContentAnchoredPosition;

    // GMM Canvas that contains me and Camera if any
    protected Transform _transform = null;
    protected Canvas _canvas = null;
    protected Camera _camera = null;

    // GMM child Rect Transforms
    protected List<RectTransform> _rectTransformList = new List<RectTransform>();    

    protected int _minPivot;
    protected int _maxPivot;
    protected RectTransform _scrollRectTransform = null;    
    protected float _scrollMinScreenCo, _scrollMaxScreenCo;
    protected ScreenSizeManager _screenSizeManager = null;

    // GMM Temporary Variables
    private RectTransform _min = null;
    private RectTransform _max = null;
    private Vector3[] _coords = new Vector3[4];
    private Vector3 _minCoords = new Vector3();
    private Vector3 _maxCoords = new Vector3();

    protected override void OnEnable ()
    {
        base.OnEnable ();
    }   

    // GMM En ScrollRect no se define OnStart
    protected override void Start()
    {
        if (horizontal == true && vertical == true)
        {
            Debug.LogWarning ("[InfScrollBase] does not work with horizontal==true and vertical==true");
            this.enabled = false;
            return;
        }
        else if (horizontal == false && vertical == false)
        {
            Debug.LogWarning ("[InfScrollBase] does not work with horizontal==false and vertical==false");
            this.enabled = false;
            return;            
        }

        _scrollRectTransform = this.GetComponent<RectTransform> ();
        // GMM Get the canvas that contains InfiniteScroll        
        _transform = this.transform;
        _canvas = null;
        Transform _parent = _transform.parent;
        while (_canvas == null && _parent != null)
        {
            _canvas = _parent.GetComponent<Canvas> ();
            _parent = _parent.parent;
        }        

        if (_canvas == null)
        {
            Debug.LogWarning ("[InfScrollBase] There is NO Canvas!!");
            this.enabled = false;
            return;
        }
        // GMM Get the Canvas camera if any        
        _camera = _canvas.worldCamera;
        Debug.Log (_camera);
        _screenSizeManager = GetComponent<ScreenSizeManager> ();
        if (_screenSizeManager != null)
        {
            _screenSizeManager.OnScreenSizeChange += this.OnScreenSizeChange;
        }

        Initialize ();
        // GMM Reposicionamos elementos si repositionOnInitialize
        if (repositionOnInitialize) RepositionScrollElements ();
        // GMM set the comparison position - Its gonna be the anchoredPosition of the LeftPivot
        _lastContentAnchoredPosition = content.anchoredPosition;
    }    

    public void OnScreenSizeChange (Vector2 screenSize)
    {
        Debug.Log (string.Format ("[InfScrollBase] OnScreenSizeChange {0}x{1}", screenSize.x, screenSize.y));
        SetScrollRectScreenCo ();        
    }

    protected virtual void Initialize ()
    {        
		if (_initialized == false)
		{
            Debug.Log ("[InfScrollBase] Initialize");
            //SetScrollRectWorldCo ();
            SetScrollRectScreenCo ();

	        RectTransform _child = null;
	        for (int i = 0; i < content.childCount; ++i)
	        {
	            _child = content.GetChild(i) as RectTransform;
	            if (_child != null)
	            {                  
	               _rectTransformList.Add (_child);                    
	            }
	        }

	        // GMM supossed that the childrens are ordered
	        if (horizontal == true)
	        {
	            _minPivot = 0;
	            _maxPivot = _rectTransformList.Count - 1;
	        }
	        else // vertical
	        {
	            // GMM because bigger coords refer to top elements
	            _minPivot = 0;
	            _maxPivot = _rectTransformList.Count - 1;
	        }

			_initialized = true;
		}
    }    

    private void SetScrollRectScreenCo ()
    {
        _scrollRectTransform.GetWorldCorners (_coords);

        InfScrollUtils.WorldToScreenCoordinates (_camera, _coords);
        InfScrollUtils.GetMinMaxCoordinates (_coords, ref _maxCoords, ref _minCoords);

        if (horizontal == true)
        {
            _scrollMinScreenCo = _minCoords.x;
            _scrollMaxScreenCo = _maxCoords.x;
        }
        else // GMM vertical
        {
            _scrollMinScreenCo = _minCoords.y;
            _scrollMaxScreenCo = _maxCoords.y;
        }
        Debug.Log (string.Format ("[InfScrollBase] Scroll Bound {0} - {1}", _scrollMinScreenCo, _scrollMaxScreenCo));
    }

    /// <summary>
    /// Funcion que va a reposicionar los elementos del scroll
    /// IMPORTANT: Seguimos pensando que el orden es el mismo que cuando se inicializo la primera vez
    /// por lo que el elemento que estaba el primero en la jerarquía en Initialize es el elemento mas a la Izq o Abajo
    /// </summary>
    protected virtual void RepositionScrollElements ()
    {
        float _size = 0.0f;
        Vector2 _initPos = new Vector2 (_scrollRectTransform.anchoredPosition.x, _scrollRectTransform.anchoredPosition.y);        
        Vector2 _nextPos; // Calculos intermedios

        if (horizontal == true)
        {
            for (int i = 0; i < _rectTransformList.Count; ++i)
            {
                _size += _rectTransformList[i].rect.width;
            }
            // GMM Suponemos padding constante definido
            _size += padding * (_rectTransformList.Count - 1);
            _initPos.x -= _size / 2;

            _nextPos = new Vector2 (_initPos.x + _rectTransformList[0].rect.width / 2, _initPos.y);
            // GMM Posicionamos los elementos menos el ultimo para que no haya overflow en calculo _nextPos
            for (int i = 0; i < _rectTransformList.Count - 1; ++i)
            {
                _rectTransformList[i].anchoredPosition = _nextPos;
                _nextPos = new Vector2 (_nextPos.x + _rectTransformList[i].rect.width / 2 + _rectTransformList[i+1].rect.width / 2 + padding, _nextPos.y);
            }
            // GMM Posicionamos el ultimo
            _rectTransformList[_rectTransformList.Count - 1].anchoredPosition = _nextPos;
        }

        else // GMM Vertical
        {
            for (int i = 0; i < _rectTransformList.Count; ++i)
            {
                _size += _rectTransformList[i].rect.height;
            }
            // GMM Suponemos padding constante definido
            _size += padding * (_rectTransformList.Count - 1);
            _initPos.y -= _size / 2;

            _nextPos = new Vector2 (_initPos.x, _initPos.y + _rectTransformList[0].rect.height / 2);
            // GMM Reposicionamos elementos verticalmente
            for (int i = 0; i < _rectTransformList.Count - 1; ++i)
            {
                _rectTransformList[i].anchoredPosition = _nextPos;
                _nextPos = new Vector2 (_nextPos.x, _nextPos.y + _rectTransformList[i].rect.height / 2 + _rectTransformList[i + 1].rect.height / 2 + padding);
            }
            _rectTransformList[_rectTransformList.Count - 1].anchoredPosition = _nextPos;
        }
        _minPivot = 0;
        _maxPivot = _rectTransformList.Count - 1;        
    }

    /// <summary>
    /// Funcion que hace el intercambio de RectTransform de un lado a otro.
    /// En este caso lo hace de Izquierda a Derecha o Abajo Arriba
    /// </summary>
    protected virtual void SwapMinMax ()
    {
        // GMM update the list of rectransforms
        _maxPivot++;
        if (_maxPivot >= _rectTransformList.Count) _maxPivot = 0;
        _minPivot++;
        if (_minPivot >= _rectTransformList.Count) _minPivot = 0;
        
        Vector2 _pos = new Vector2 (_max.anchoredPosition.x, _max.anchoredPosition.y);

        // GMM Horizontal
        if (horizontal == true)
        {           
            _pos.x = _pos.x + _max.rect.width / 2 + _min.rect.width / 2 + padding;            
        }
        else // GMM Vertical
        {            
            _pos.y = _pos.y + _max.rect.height / 2 + _min.rect.height / 2 + padding;
            
        }

        _min.anchoredPosition = _pos;
    }

    /// <summary>
    /// Funcion que hace el intercambio de RectTransform de un lado a otro.
    /// En este caso lo hace de Derecha a Izquierda o Arrriba Abajo
    /// </summary>
    protected virtual void SwapMaxMin ()
    {
        // GMM Actualizamos pivotes de la lista de RectTransforms
        _maxPivot--;
        if (_maxPivot < 0) _maxPivot = _rectTransformList.Count - 1;
        _minPivot--;
        if (_minPivot < 0) _minPivot = _rectTransformList.Count - 1;

        Vector2 _pos = new Vector2 (_min.anchoredPosition.x, _min.anchoredPosition.y);

        // GMM Horizontal
        if (horizontal == true)
        {           
            _pos.x = _pos.x - _min.rect.width / 2 - _max.rect.width / 2 - padding;            
        }
        else // GMM Vertical
        {            
            _pos.y = _pos.y - _min.rect.height / 2 - _max.rect.height / 2 - padding;            
        }

        _max.anchoredPosition = _pos;
    }

    protected void SwapInfScroll()
    {
        if (horizontal == true)
        {
            _min = _rectTransformList[_minPivot];
            _max = _rectTransformList[_maxPivot];    
            // GMM we are moving left
            if (_lastContentAnchoredPosition.x > content.anchoredPosition.x)
            {
                // GMM Calculamos las coordenadas en World del xMax de left                
                _min.GetWorldCorners (_coords);
                InfScrollUtils.WorldToScreenCoordinates (_camera, _coords);
                InfScrollUtils.GetMinMaxCoordinates (_coords, ref _maxCoords, ref _minCoords);

                if (_maxCoords.x < _scrollMinScreenCo)
                {
                    // GMM Movemos el RectTransform de la parte izquierda a la parte derecha
                   SwapMinMax();
                }
            }
            else if (_lastContentAnchoredPosition.x < content.anchoredPosition.x)
            {
                // GMM Calculamos las coordenadas en World del xMax de left
                _max.GetWorldCorners (_coords);
                InfScrollUtils.WorldToScreenCoordinates (_camera, _coords);
                InfScrollUtils.GetMinMaxCoordinates (_coords, ref _maxCoords, ref _minCoords);                

                if (_minCoords.x > _scrollMaxScreenCo)
                {
                    // GMM Movemos el RectTransform de la parte derecha a la parte izquierda
                    SwapMaxMin ();
                }
            }
        }
        else // GMM Vertical
        {
            _min = _rectTransformList[_minPivot];
            _max = _rectTransformList[_maxPivot];

            // GMM we are moving down
            if (_lastContentAnchoredPosition.y > content.anchoredPosition.y)
            {
                // GMM Calculamos las coordenadas en World del xMax de left                            
                _min.GetWorldCorners (_coords);
                InfScrollUtils.WorldToScreenCoordinates (_camera, _coords);
                InfScrollUtils.GetMinMaxCoordinates (_coords, ref _maxCoords, ref _minCoords);                

                if (_maxCoords.y < _scrollMinScreenCo)
                {
                    // GMM Movemos el RectTransform inferior a la parte superior
                    SwapMinMax ();
                }
            }
            // GMM moving up
            else if (_lastContentAnchoredPosition.y < content.anchoredPosition.y)
            {                
                // GMM Calculamos las coordenadas en World del xMax de left
                _max.GetWorldCorners (_coords);
                InfScrollUtils.WorldToScreenCoordinates (_camera, _coords);
                InfScrollUtils.GetMinMaxCoordinates (_coords, ref _maxCoords, ref _minCoords);                
                
                if (_minCoords.y > _scrollMaxScreenCo)
                {
                    // GMM Movemos el RecTransform superior a la parte inferior
                    SwapMaxMin ();
                }
            }
        }
    }

    protected virtual void Update ()
    {
        if (_lastContentAnchoredPosition.x != content.anchoredPosition.x || _lastContentAnchoredPosition.y != content.anchoredPosition.y)
        {
            SwapInfScroll ();
            _lastContentAnchoredPosition = content.anchoredPosition;
        }
    }
}
