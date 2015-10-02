using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Collections.Generic;

namespace Bar
{

    public class BarValue
    {
        string _Caption;
        public string Caption { get { return _Caption; } set { _Caption = value; } }

        double _Percentage;
        public double Percentage { get { return _Percentage; } set { _Percentage = value; } }

        public BarValue() : this(0) { }
        public BarValue(double percentage) : this(string.Format("{0}%", percentage), percentage) { }
        public BarValue(string caption, double percentage)
        {
            _Caption = caption;
            _Percentage = percentage;
        }
    }

    public class BarValueCollection : List<BarValue>
    {
        string _Caption;
        public string Caption { get { return _Caption; } set { _Caption = value; } }

        public BarValueCollection(string caption)
            : base()
        {
            _Caption = caption;
        }

    }

    public class StackBarCollection
    {
        string _Title;
        public string Title { get { return _Title; } set { _Title = value; } }

        bool _ShowTitle;
        public bool ShowTitle { get { return _ShowTitle; } set { _ShowTitle = value; } }

        double _Scale;
        public double Scale { get { return _Scale; } set { _Scale = value; } }

        string[] _Colors;
        public string[] Colors { get { return _Colors; } set { _Colors = value; } }

        List<string> _LegendValues;
        public string[] LegendValues { get { return _LegendValues.ToArray(); } }

        List<BarValueCollection> _Values;
        public BarValueCollection[] Values { get { return _Values.ToArray(); } }

        public void Add(BarValueCollection collection) { _Values.Add(collection); }

        public void AddLegend(string item) { _LegendValues.Add(item); }

        public StackBarCollection() : this(string.Empty) { }


        public StackBarCollection(string title)
        {
            _Title = title;
            _Scale = 1;
            _Colors = new string[] { "#00ff00", "#ffa300", "#ff00ff", "#a3a3a3", "#ffffff", "#000000", "#ffa300" };
            _Values = new List<BarValueCollection>();
            _LegendValues = new List<string>();
        }

        public StackBarCollection(string title, double scale, string[] colors)
        {
            _Title = title;
            _Scale = scale;
            _Colors = colors;
            _Values = new List<BarValueCollection>();
            _LegendValues = new List<string>();
        }
    }
}