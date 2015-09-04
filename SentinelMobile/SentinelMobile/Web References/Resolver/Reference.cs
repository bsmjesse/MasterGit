﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
// 
#pragma warning disable 1591

namespace SentinelMobile.Resolver {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ResolverSoap", Namespace="http://sentinelfm.com/")]
    public partial class Resolver : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback StreetAddressOperationCompleted;
        
        private System.Threading.SendOrPostCallback LocationOperationCompleted;
        
        private System.Threading.SendOrPostCallback StreetAddressSimpleOperationCompleted;
        
        private System.Threading.SendOrPostCallback LocationSimpleOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Resolver() {
            this.Url = global::SentinelMobile.Properties.Settings.Default.SentinelMobile_Resolver_Resolver;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event StreetAddressCompletedEventHandler StreetAddressCompleted;
        
        /// <remarks/>
        public event LocationCompletedEventHandler LocationCompleted;
        
        /// <remarks/>
        public event StreetAddressSimpleCompletedEventHandler StreetAddressSimpleCompleted;
        
        /// <remarks/>
        public event LocationSimpleCompletedEventHandler LocationSimpleCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sentinelfm.com/StreetAddress", RequestNamespace="http://sentinelfm.com/", ResponseNamespace="http://sentinelfm.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool StreetAddress(double latitude, double longitude, ref string location) {
            object[] results = this.Invoke("StreetAddress", new object[] {
                        latitude,
                        longitude,
                        location});
            location = ((string)(results[1]));
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void StreetAddressAsync(double latitude, double longitude, string location) {
            this.StreetAddressAsync(latitude, longitude, location, null);
        }
        
        /// <remarks/>
        public void StreetAddressAsync(double latitude, double longitude, string location, object userState) {
            if ((this.StreetAddressOperationCompleted == null)) {
                this.StreetAddressOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStreetAddressOperationCompleted);
            }
            this.InvokeAsync("StreetAddress", new object[] {
                        latitude,
                        longitude,
                        location}, this.StreetAddressOperationCompleted, userState);
        }
        
        private void OnStreetAddressOperationCompleted(object arg) {
            if ((this.StreetAddressCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StreetAddressCompleted(this, new StreetAddressCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sentinelfm.com/Location", RequestNamespace="http://sentinelfm.com/", ResponseNamespace="http://sentinelfm.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool Location([System.Xml.Serialization.XmlElementAttribute("location")] string location1, ref double latitude, ref double longitude, ref string resolvedAddress) {
            object[] results = this.Invoke("Location", new object[] {
                        location1,
                        latitude,
                        longitude,
                        resolvedAddress});
            latitude = ((double)(results[1]));
            longitude = ((double)(results[2]));
            resolvedAddress = ((string)(results[3]));
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void LocationAsync(string location1, double latitude, double longitude, string resolvedAddress) {
            this.LocationAsync(location1, latitude, longitude, resolvedAddress, null);
        }
        
        /// <remarks/>
        public void LocationAsync(string location1, double latitude, double longitude, string resolvedAddress, object userState) {
            if ((this.LocationOperationCompleted == null)) {
                this.LocationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLocationOperationCompleted);
            }
            this.InvokeAsync("Location", new object[] {
                        location1,
                        latitude,
                        longitude,
                        resolvedAddress}, this.LocationOperationCompleted, userState);
        }
        
        private void OnLocationOperationCompleted(object arg) {
            if ((this.LocationCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LocationCompleted(this, new LocationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sentinelfm.com/StreetAddressSimple", RequestNamespace="http://sentinelfm.com/", ResponseNamespace="http://sentinelfm.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string StreetAddressSimple(double latitude, double longitude) {
            object[] results = this.Invoke("StreetAddressSimple", new object[] {
                        latitude,
                        longitude});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void StreetAddressSimpleAsync(double latitude, double longitude) {
            this.StreetAddressSimpleAsync(latitude, longitude, null);
        }
        
        /// <remarks/>
        public void StreetAddressSimpleAsync(double latitude, double longitude, object userState) {
            if ((this.StreetAddressSimpleOperationCompleted == null)) {
                this.StreetAddressSimpleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnStreetAddressSimpleOperationCompleted);
            }
            this.InvokeAsync("StreetAddressSimple", new object[] {
                        latitude,
                        longitude}, this.StreetAddressSimpleOperationCompleted, userState);
        }
        
        private void OnStreetAddressSimpleOperationCompleted(object arg) {
            if ((this.StreetAddressSimpleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.StreetAddressSimpleCompleted(this, new StreetAddressSimpleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sentinelfm.com/LocationSimple", RequestNamespace="http://sentinelfm.com/", ResponseNamespace="http://sentinelfm.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string LocationSimple(string location) {
            object[] results = this.Invoke("LocationSimple", new object[] {
                        location});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void LocationSimpleAsync(string location) {
            this.LocationSimpleAsync(location, null);
        }
        
        /// <remarks/>
        public void LocationSimpleAsync(string location, object userState) {
            if ((this.LocationSimpleOperationCompleted == null)) {
                this.LocationSimpleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLocationSimpleOperationCompleted);
            }
            this.InvokeAsync("LocationSimple", new object[] {
                        location}, this.LocationSimpleOperationCompleted, userState);
        }
        
        private void OnLocationSimpleOperationCompleted(object arg) {
            if ((this.LocationSimpleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LocationSimpleCompleted(this, new LocationSimpleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void StreetAddressCompletedEventHandler(object sender, StreetAddressCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class StreetAddressCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal StreetAddressCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string location {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void LocationCompletedEventHandler(object sender, LocationCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LocationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal LocationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public double latitude {
            get {
                this.RaiseExceptionIfNecessary();
                return ((double)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public double longitude {
            get {
                this.RaiseExceptionIfNecessary();
                return ((double)(this.results[2]));
            }
        }
        
        /// <remarks/>
        public string resolvedAddress {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[3]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void StreetAddressSimpleCompletedEventHandler(object sender, StreetAddressSimpleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class StreetAddressSimpleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal StreetAddressSimpleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void LocationSimpleCompletedEventHandler(object sender, LocationSimpleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LocationSimpleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal LocationSimpleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591