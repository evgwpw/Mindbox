﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18444
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
// 
// Этот исходный код был создан с помощью wsdl, версия=4.0.30319.17929.
// 
using Core.Diagnostics;
using Inec.StateMachine.Loging;

namespace Inec.StateMachine.WSReferences
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ServiceSoap", Namespace = "http://tempuri.org/")]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback CancelApplicationOperationCompleted;

        private System.Threading.SendOrPostCallback GetApplicationsOperationCompleted;

        /// <remarks/>
        public Service()
        {
            this.Url = Settings1.Default.ServiceUrl;// "http://middle/gosusl/spdservice/service.asmx";
        }

        /// <remarks/>
        public event CancelApplicationCompletedEventHandler CancelApplicationCompleted;

        /// <remarks/>
        public event GetApplicationsCompletedEventHandler GetApplicationsCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CancelApplication", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public spdRet CancelApplication(string appName)
        {
            object[] results = this.Invoke("CancelApplication", new object[] {
                    appName});
            return ((spdRet)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCancelApplication(string appName, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CancelApplication", new object[] {
                    appName}, callback, asyncState);
        }

        /// <remarks/>
        public spdRet EndCancelApplication(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((spdRet)(results[0]));
        }

        /// <remarks/>
        public void CancelApplicationAsync(string appName)
        {
            this.CancelApplicationAsync(appName, null);
        }

        /// <remarks/>
        public void CancelApplicationAsync(string appName, object userState)
        {
            if ((this.CancelApplicationOperationCompleted == null))
            {
                this.CancelApplicationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelApplicationOperationCompleted);
            }
            this.InvokeAsync("CancelApplication", new object[] {
                    appName}, this.CancelApplicationOperationCompleted, userState);
        }

        private void OnCancelApplicationOperationCompleted(object arg)
        {
            if ((this.CancelApplicationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelApplicationCompleted(this, new CancelApplicationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetApplications", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [TraceExtension]
        public spdRet GetApplications(int id_rubr, string appName, System.DateTime appDate, System.Data.DataSet ds)
        {
            object[] results = this.Invoke("GetApplications", new object[] {
                    id_rubr,
                    appName,
                    appDate,
                    ds});
            return ((spdRet)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginGetApplications(int id_rubr, string appName, System.DateTime appDate, System.Data.DataSet ds, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("GetApplications", new object[] {
                    id_rubr,
                    appName,
                    appDate,
                    ds}, callback, asyncState);
        }

        /// <remarks/>
        public spdRet EndGetApplications(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((spdRet)(results[0]));
        }

        /// <remarks/>
        public void GetApplicationsAsync(int id_rubr, string appName, System.DateTime appDate, System.Data.DataSet ds)
        {
            this.GetApplicationsAsync(id_rubr, appName, appDate, ds, null);
        }

        /// <remarks/>
        public void GetApplicationsAsync(int id_rubr, string appName, System.DateTime appDate, System.Data.DataSet ds, object userState)
        {
            if ((this.GetApplicationsOperationCompleted == null))
            {
                this.GetApplicationsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetApplicationsOperationCompleted);
            }
            this.InvokeAsync("GetApplications", new object[] {
                    id_rubr,
                    appName,
                    appDate,
                    ds}, this.GetApplicationsOperationCompleted, userState);
        }

        private void OnGetApplicationsOperationCompleted(object arg)
        {
            if ((this.GetApplicationsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetApplicationsCompleted(this, new GetApplicationsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class spdRet
    {

        private int codeField;

        private string errorField;

        /// <remarks/>
        public int code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    public delegate void CancelApplicationCompletedEventHandler(object sender, CancelApplicationCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelApplicationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CancelApplicationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public spdRet Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((spdRet)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    public delegate void GetApplicationsCompletedEventHandler(object sender, GetApplicationsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetApplicationsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetApplicationsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public spdRet Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((spdRet)(this.results[0]));
            }
        }
    }
}