using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public partial class HOS_frmBar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ArrayList _years=new ArrayList();
            _years.Add("2000");
            _years.Add("2001");
            _years.Add("2002");
            _years.Add("2003");
            _years.Add("2004");

            ArrayList _sales=new ArrayList();
            _sales.Add(270500);
            _sales.Add(211930);
            _sales.Add(223300);
            _sales.Add(343000);
            _sales.Add(424750);

            GenerateBarGraph("ABC Ltd. Sales (2000-2004)",_years, _sales, 50, 25, 400);

    }


        protected void GenerateBarGraph(
            string graphTitle,
            ArrayList xValues,
            ArrayList yValues,
            int barWidth,
            int barSpaceWidth,
            int graphHeight)
            {
            int graphTitleHeight=20; // Height in pixels utilized by the title in the graph
            int itemsHeight=35; // Height in pixels utilized by the items in the x-axis

            /*
            The Graph’s width is calculated by adding the width of a bar and the space between
            two bars multiplied by the total values in the x-axis plus the space between two bars
            */
            int graphWidth= (barWidth + barSpaceWidth) * xValues.Count + barSpaceWidth;

            /*
            The maximum height that a bar can attain needs to be found from the y-values passed
            as parameter
            */
            int maxBarHeight=0;

            //Total height of the image is calculated
            int totalGraphHeight = graphHeight + graphTitleHeight + itemsHeight;

            //Create an instance of Bitmap class with the given width and height
            Bitmap barBitmap=new Bitmap(graphWidth, totalGraphHeight);

            /*
            Graphics class does not have a constructor and hence we call its static method
            FromImage and pass the Bitmap object to it
            */
            Graphics barGraphics= Graphics.FromImage(barBitmap);

            /*
            Using the Graphics object we fill the image of given dimensions with light gray color
            */

            barGraphics.FillRectangle(
            new SolidBrush(Color.WhiteSmoke),
            0,
            0,
            graphWidth,
            totalGraphHeight);

            /*
            We create an instance of Font class available in System.Drawing. We will be using this
            to display the title of the graph.
            */
            Font titleFont=new Font("Verdana",14, FontStyle.Bold);

            /*
            Use the Graphics object’s DrawString method to draw the title at the specified location
            */
            barGraphics.DrawString(
            graphTitle,
            titleFont,
            new SolidBrush(Color.Red),
            (graphWidth / 2) - graphTitle.Length * 5,
            totalGraphHeight - itemsHeight);

            /*
            Find the highest value in the yValues ArrayList and set it as the maximum height of the bar
            */
            foreach(int _value in yValues)
            if(_value > maxBarHeight) maxBarHeight=_value;

            //barXPos will store the x position of a bar
            int barXPos = barSpaceWidth;
            int barHeight;

            Font itemsFont=new Font("Verdana",9, FontStyle.Bold);
            Font valuesFont=new Font("Verdana", 7, FontStyle.Italic);

            Random rnd=new Random();

            for(int i=0;i < xValues.Count;i++)
            {
            //Generate a random color so that each bar will have a unique color
            Color color = Color.FromArgb(rnd.Next(255),rnd.Next(255),rnd.Next(255));
            SolidBrush barBrush=new SolidBrush(color);

            /*
            (int)yValues[i]* 100 / maxBarHeight -> Gives the bar height in percentage with respect to
            the maximum bar height set by us

            ((((int)yValues[i]* 100 / maxBarHeight) )* graphHeight)/100 will give the bar height in
            pixels
            */
            barHeight=((((int)yValues[i]* 100 / maxBarHeight) )* graphHeight)/100;

            //Draw the bar with the set brush, x and y positions, width and height
            barGraphics.FillRectangle(
            barBrush,
            barXPos,
            graphHeight-barHeight,
            barWidth,
            barHeight);

            //Draw the x-value along the x-axis
            barGraphics.DrawString(
            xValues[i].ToString(),
            itemsFont,
            barBrush,
            barXPos,
            graphHeight);

            //Draw the respective y value on top of the bar
            barGraphics.DrawString(
            yValues[i].ToString(),
            valuesFont,
            barBrush,
            barXPos,
            (graphHeight-barHeight)-itemsHeight);

            //Change the x position of the next bar
            barXPos += (barWidth + barSpaceWidth);
            } 
 

            /*
            Save the image to the web server’s D: drive. We use the PNG format to make it look
            crisp.
            */
            barBitmap.Save("C:\\bargraph.png",ImageFormat.Png);

            //Dispose off the Graphics and Bitmap objects
            barGraphics.Dispose();
            barBitmap.Dispose();
} 



}
