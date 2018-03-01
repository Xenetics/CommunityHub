using System;
using System.Collections.Generic;
using UnityEngine;

public class PointData
{
    // Overloaded Constructors
    public PointData(PinType type, string label, Position position, bool init = true)
    {
        _Type = type;
        _Label = label;
        _Position = new Position(position.Latitude, position.Longitude);
        _Icon = SetIcon();
        _ActiveIcon = SetActiveIcon();
        if (init)
        {
            Init();
        }
    }
    public PointData(PinType type, string label, Position position, string address, bool init = true)
    {
        _Type = type;
        _Label = label;
        _Position = new Position(position.Latitude, position.Longitude);
        _Address = address;
        _Icon = SetIcon();
        _ActiveIcon = SetActiveIcon();
        if (init)
        {
            Init();
        }
    }
    public PointData(PinType type, string label, Position position, string address, int value, string startDate, string endDate, bool init = true)
    {
        _Type = type;
        _Label = label;
        _Position = new Position(position.Latitude, position.Longitude);
        _Address = address;
        _Value = value;
        _StartDate = ConvertStringToDateTime(startDate);
        _EndDate = ConvertStringToDateTime(endDate);
        _Icon = SetIcon();
        _ActiveIcon = SetActiveIcon();
        if (init)
        {
            Init();
        }
    }
    public PointData(PinType type, string label, Position position, string address, int value, DateTime startDate, DateTime endDate, bool init = true)
    {
        _Type = type;
        _Label = label;
        _Position = new Position(position.Latitude, position.Longitude);
        _Address = address;
        _Value = value;
        _StartDate = startDate;
        _EndDate = endDate;
        _Icon = SetIcon();
        _ActiveIcon = SetActiveIcon();
        if (init)
        {
            Init();
        }
    }

    /// <summary> The different type of pins </summary>
    public enum PinType { Generic, Game, Heritage, Conservation, MPL, TOM, Tokens, COUNT }
    /// <summary> This points pin type </summary>
    public PinType _Type = PinType.Generic;
    /// <summary> Refference to the pin this point data represents </summary>
    public OnlineMapsMarker _Pin;
    /// <summary> Position in lat/long </summary>
    public Position _Position;
    /// <summary> Label for this point </summary>
    public string _Label;
    /// <summary> Address of this point </summary>
    public string _Address;
    /// <summary> Date after which pin can be added to map</summary>
    public DateTime _StartDate;
    /// <summary> Date after which pin is no longer added to map </summary>
    public DateTime _EndDate;
    /// <summary> The Icon for the current pin based on type </summary>
    public Texture2D _Icon;
    /// <summary> Pin Icon when active </summary>
    public Texture2D _ActiveIcon;
    /// <summary> The token value of thsi pin </summary>
    public int _Value = 0;
    /// <summary> Refference to the circle around this point if in range </summary>
    public OnlineMapsDrawingPoly _Circle;
    /// <summary> Acts as a mutex and is set true if actioning on this point and false if its open for action </summary>
    public bool _Acting;
    /// <summary> Indicates wheather this is a jackpot point or not (only on token pins) </summary>
    public bool _Jackpot = false;

    // Initialization of the point data
    public void Init()
    {
        _Pin = new OnlineMapsMarker();
        _Pin.SetPosition(_Position.Latitude, _Position.Longitude);
        _Pin.label = _Label;
        _Pin.texture = _Icon;
        _Pin.tags.Add(_Type.ToString());
    }

    // Sets the icon to the appropriate image on load
    public Texture2D SetIcon()
    {
        _Jackpot = false;
        return UIManager.Instance.PinTextures[(int)_Type];
    }

    // Sets the Active icon to the appropriate image on load
    public Texture2D SetActiveIcon()
    {
        _Jackpot = false;
        return UIManager.Instance.AltPinTextures[(int)_Type];
    }

    // sets this pin to a jackpot pin
    public void SetJackpot()
    {
        //_Pin.Icon = BitmapDescriptorFactory.FromBundle("JackpotT.png");
        _Jackpot = true;
    }

    // changes the position of the point
    public void UpdatePosition(double lat, double lon)
    {
        _Position = new Position(lat, lon);
        _Pin.SetPosition(_Position.Latitude, _Position.Longitude);
        _Acting = false;
    }

    private List<Vector2> points;
    public int segments = 16;

    // Adds a circle to the map and holds a refference on this point
    public void AddCircle(double radius, Color color)
    {
        float r = (float)radius / OnlineMapsUtils.tileSize;
        float step = 360f / segments;
        double x, y;
        _Pin.GetPosition(out x, out y);

        OnlineMapsProjection projection = OnlineMaps.instance.projection;
        projection.CoordinatesToTile(x, y, OnlineMaps.instance.zoom, out x, out y);

        points = new List<Vector2>();
        for (int i = 0; i < segments; i++)
        {
            points.Add(new Vector2());

            double px = x + Mathf.Cos(step * i * Mathf.Deg2Rad) * r;
            double py = y + Mathf.Sin(step * i * Mathf.Deg2Rad) * r;

            projection.TileToCoordinates(px, py, OnlineMaps.instance.zoom, out px, out py);

            points[i] = new Vector2((float)px, (float)py);
        }

        _Circle = new OnlineMapsDrawingPoly(points, color, 3);

        OnlineMaps.instance.AddDrawingElement(_Circle);

        _Acting = false;
    }

    // Removes the pin from the map and nulls refference
    public void RemovePin()
    {
        // Removing the texture does not work
        if (_Pin != null)
        {
            _Pin.enabled = false;
        }
        //RemoveCircle(map);
        _Acting = false;
    }

    // Enables pin on the map
    public void AddPin()
    {
        if(_Pin != null)
        {
            _Pin.enabled = true;
        }
        _Acting = false;
    }

    // Removes circle from the map and the refference in this point
    public void RemoveCircle()
    {
        if (_Circle != null)
        {
            OnlineMaps.instance.drawingElements.Remove(_Circle);
            _Circle = null;
        }
        _Acting = false;
    }

    // Converts a pre determined date format to a datetime
    private DateTime ConvertStringToDateTime(string stringDate)
    {
        string convertedString = "";
        for (int i = 0; i < stringDate.Length; ++i)
        {
            if (stringDate[i] == ',')
            {
                convertedString += '/';
            }
            else
            {
                convertedString += stringDate[i];
            }
        }
        return Convert.ToDateTime(convertedString);
    }

    // Returns true if the point is meant to be active based on start and end date, false if not
    public bool ActiveNow()
    {
        if (DateTime.Now > _StartDate && DateTime.Now < _EndDate)
        {
            return true;
        }
        return false;
    }

    // Returns true if the pin is supposed to be active based on the delta of when it was clicked, false if not
    public bool DeltasActiveNow(long now, long then)
    {
        if (now > then + PinUtilities.pinCooldown)
        {
            return true;
        }
        return false;
    }

    // Returns true if the token pin is supposed to be active based on the delta of when it was clicked, false if not
    public bool TokenDeltasActiveNow(long now, long then)
    {
        if (now > then + PinUtilities.tokenPinCooldown)
        {
            return true;
        }
        return false;
    }
}