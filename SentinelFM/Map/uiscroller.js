//****************************************************************************
//**!!!!!!!!!!!!!!!!!!!!!!!!!DO NOT EDIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//**
//** Author  :  Bill Fowler
//** Email   :  bill@siriusweblabs.com
//** Company :  Sirius Web Labs
//** File    :  siriusweblabs/UI/uiscroller.js
//** Date    :  April 11, 2003
//**
//****************************************************************************

var _isIE = (document.all != null);
var _isDOM = (document.getElementById != null);
var _isNS = (document.layers != null);
var _defaultCorrectionWidth = 8;
var _Items;  
if(_Items == null || _Items.length == null)
  _Items = new Array();   
var _PauseOnHover; 
if(_PauseOnHover == null)
  _PauseOnHover = true;
var _Speed;
if(_Speed == null)
  _Speed = 3;
var _BackColor;
if(_BackColor == null)
  _BackColor = "#FFFFFF";
var _ItemSpacing;
if(_ItemSpacing == null)
  _ItemSpacing = 10;
var _scrollobj;
var _Width;
if(_Width == null)
  _Width = 0;
var _Height;
if(_Height == null)
  _Height = 0;
var _internalwidth = 0;
var _defaultspeed = _Speed;
var _stopspeed = (new Boolean(_PauseOnHover).valueOf() ? 0 : _Speed);
var _scrollcontent = "<nobr>";
for(index in _Items)
{
  _scrollcontent += _Items[index];
  for(j=_ItemSpacing; j > 0; j--)
    _scrollcontent += "&nbsp;";
}
_scrollcontent += "</nobr>";
if(_isNS)
  _Speed = _defaultspeed = Math.max(1, _Speed - 2);
if(_isIE || _isDOM)
  document.write("<span id=\"container\" style=\"visibility:hidden;" +
                 "position:absolute;top:-100px;left:-9000px\">" + _scrollcontent + "</span>");
function onScrollerLoad()
{
  if(_isDOM)
    _scrollobj = document.getElementById("scroller");
  else if(_isIE)
    _scrollobj = document.all.scroller;
  else if(_isNS)
    _scrollobj = document.container.document.scroller;
  if(_isIE || _isDOM)
  {
    _scrollobj.style.left = parseInt(_Width) + _defaultCorrectionWidth + "px";
    _scrollobj.innerHTML = _scrollcontent;
    _internalwidth = (_isDOM ? document.getElementById("container").offsetWidth :
                      document.container.offsetWidth);
  }
  else if(_isNS)
  {
    _scrollobj.left = parseInt(_Width) + _defaultCorrectionWidth;
    _scrollobj.document.write(_scrollcontent)
    _scrollobj.document.close()
    _internalwidth = _scrollobj.document.width;
  }
  setInterval("doScrolling()",20);
}
function doScrolling()
{
  if(_isIE || _isDOM)
  {
    if(parseInt(_scrollobj.style.left) > 
      ((_internalwidth * (-1)) + _defaultCorrectionWidth))
      _scrollobj.style.left = parseInt(_scrollobj.style.left) - 
                              _Speed + "px";
    else
      _scrollobj.style.left = parseInt(_Width) + _defaultCorrectionWidth + "px";  
  }
  else if(_isNS)
  {
    if(_scrollobj.left > 
      ((_internalwidth * (-1)) + _defaultCorrectionWidth))
      _scrollobj.left -= _Speed;
    else
      _scrollobj.left = parseInt(_Width) + _defaultCorrectionWidth;
  }
}
window.onload = onScrollerLoad;
if (_isIE || _isDOM || _isNS)
{
  document.write("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><td>")
  if(_isIE || _isDOM)
  {
    document.write("<div style=\"position:relative;width:" + _Width + 
                   ";height:" + _Height + ";overflow:hidden\">" +
                   "<div style=\"position:absolute;width:" + _Width + 
                   ";height:" + _Height + ";background-color:" + _BackColor +
                   "\" onMouseOver=\"_Speed=_stopspeed;\" " +
                   "onMouseOut=\"_Speed=_defaultspeed;\">" +
                   "<div id=\"scroller\" style=\"position:absolute;" +
                   "left:0px;top:0px;\"></div></div></div>");
  }
  else if(_isNS)
  {
    document.write("<ilayer width=\"" + _Width + "\" height=" +
                   _Height + "\" name=\"container\" bgColor=\"" + 
                   _BackColor + "\"><layer name=\"scroller\" left=\"0\" " + 
                   "top=\"0\" onMouseover=\"_Speed=_stopspeed;\" " +
                   "onMouseOut=\"_Speed=_defaultspeed;\"></layer></ilayer>");
  }
  document.write("</td></table>");
}