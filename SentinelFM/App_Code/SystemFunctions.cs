using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SystemFunctions
/// </summary>
public class SystemFunctions
{
    Double result;

	public SystemFunctions()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
    {
        return Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
    }
    public bool isNumeric(char val)
    {
        return char.IsNumber(val);
    }
}