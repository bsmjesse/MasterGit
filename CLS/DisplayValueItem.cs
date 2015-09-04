using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.CLS
{

    public abstract class DisplayValueItem
    {

        protected string _Display;
        public string Display { get { return this._Display; } set { this._Display = value; } }

        public DisplayValueItem() : this(string.Empty) { }
        public DisplayValueItem(string display)
        {
            this._Display = display;
        }

        public override string ToString()
        {
            return this._Display;
        }
    }

    public class DisplayShortValueItem : DisplayValueItem
    {

        short _Value;
        public short Value { get { return this._Value; } set { this._Value = value; } }

        public DisplayShortValueItem() : this(0, string.Empty) { }
        public DisplayShortValueItem(short value, string display)
        {
            this._Display = display;
            this._Value = value;
        }

        public override string ToString()
        {
            return this._Display;
        }
    }

    public class DisplayIntValueItem : DisplayValueItem
    {

        int _Value;
        public int Value { get { return this._Value; } set { this._Value = value; } }

        public DisplayIntValueItem() : this(0, string.Empty) { }
        public DisplayIntValueItem(int value, string display)
        {
            this._Display = display;
            this._Value = value;
        }

        public override string ToString()
        {
            return this._Display;
        }
    }

    public class DisplayLongValueItem : DisplayValueItem
    {

        long _Value;
        public long Value { get { return this._Value; } set { this._Value = value; } }

        public DisplayLongValueItem() : this(0, string.Empty) { }
        public DisplayLongValueItem(long value, string display)
        {
            this._Display = display;
            this._Value = value;
        }

        public override string ToString()
        {
            return this._Display;
        }
    }
}
