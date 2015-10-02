using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

/// <summary>
/// This code is totally free and open. You may reuse it in any way you wish.
/// </summary>

public partial class ColorPicker : System.Web.UI.UserControl, IPostBackEventHandler
{

    protected override void Render(HtmlTextWriter writer)
    {
        drawTable();
        base.Render(writer);
    }

    private void drawTable()
    {
        tblShow.Width = (int)_boardSize;
        tblPicker.Width = (int)_boardSize;
        int increment = ((int)_boardSize / (int)_fineness);

        cellPal1.Attributes.Add("onClick", Page.ClientScript.GetPostBackEventReference(this, "pal1"));
        cellPal2.Attributes.Add("onClick", Page.ClientScript.GetPostBackEventReference(this, "pal2"));
        cellPal3.Attributes.Add("onClick", Page.ClientScript.GetPostBackEventReference(this, "pal3"));
        cellPal4.Attributes.Add("onClick", Page.ClientScript.GetPostBackEventReference(this, "pal4"));
        cellPal5.Attributes.Add("onClick", Page.ClientScript.GetPostBackEventReference(this, "pal5"));

        string tmpCol;

        for (int x = 0; x <= (int)_boardSize; x += increment)
        {
            TableRow tr = new TableRow();
            for (int y = 0; y <= (int)_boardSize; y += increment)
            {
                tmpCol = getColor(x, y);
                TableCell tc = new TableCell();
                tc.Attributes.Add("bgcolor", tmpCol);
                tc.Attributes.Add("width", increment.ToString());
                tc.Attributes.Add("height", increment.ToString());
                tc.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(this, tmpCol));
                tr.Cells.Add(tc);

            }
            tblPicker.Rows.Add(tr);
            
        }
    }

    public enum Size
    {
        Large = 256,
        Medium = 128,
        Small = 64
    }

    public enum Resolution
    {
        Fine = 32,
        Medium = 16,
        Coarse = 4
    }

    private Resolution _fineness = Resolution.Medium;

    public Resolution Fineness
    {
        get { return _fineness; }
        set { _fineness = value; }
    }

    private Size _boardSize = Size.Medium;

    public Size BoardSize
    {
        get { return _boardSize; }
        set { _boardSize = value; }
    }

    private string Palette
    {
        get { return (string)ViewState["Palette"]; }
        set { ViewState["Palette"] = value; }
    }

    public Color SelectedColor
    {
        get
        {
            try
            {
                return (Color)ViewState["selectedColor"];
            }
            catch (NullReferenceException)
            {
                return Color.White;
            }
        }
        set { ViewState["selectedColor"] = value; }
    }

    private string getColor(int x, int y)
    {
        int xmod = (int)((255F / (float)_boardSize) * (float)x);
        int ymod = (int)((255F / (float)_boardSize) * (float)y);

        int r = 0, g = 0, b = 0;

        switch (Palette)
        {
            case "pal1":
                r = xmod;
                g = ymod;
                break;
            case "pal2":
                g = xmod;
                b = ymod;
                break;
            case "pal3":
                r = xmod;
                b = ymod;
                break;
            case "pal4":
                r = ymod;
                g = xmod;
                b = ymod;
                break;
            case "pal5":
                r = ymod;
                g = ymod;
                b = ymod;
                break;
            default:
                r = xmod;
                g = ymod;
                break;
        }

        return ColorTranslator.ToHtml(Color.FromArgb(r, g, b));
    }

    public void RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case "pal1":
            case "pal2":
            case "pal3":
            case "pal4":
            case "pal5":
                Palette = eventArgument;
                break;
            default:
                try
                {
                    cellPreview.BackColor = ColorTranslator.FromHtml(eventArgument);
                    cellPreview.ForeColor = Color.FromArgb(~cellPreview.BackColor.ToArgb());
                    this.SelectedColor = cellPreview.BackColor;
                    cellPreview.Text = eventArgument;
                }
                catch (Exception)
                {

                }
                break;
        }

    }
}
