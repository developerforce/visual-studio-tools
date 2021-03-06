﻿using Salesforce.VisualStudio.Services.ConnectedService.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Salesforce.VisualStudio.Services.ConnectedService.Models
{
    /// <summary>
    /// The information for how to authenticate with Salesforce at design time.  This authentication information is used
    /// during the Connected Service experience to retrieve the available objects as well as create/configure a Connected
    /// App within Salesforce.
    /// </summary>
    [DataContract]
    internal class DesignTimeAuthentication : IEquatable<DesignTimeAuthentication>, INotifyPropertyChanged
    {
        public const int CurrentVersion = 2;

        [DataMember]
        private string userName;

        [DataMember]
        private EnvironmentType environmentType;

        [DataMember]
        private Uri myDomain;

        [DataMember]
        private string metadataServiceUrl;

        [DataMember]
        private int version;

        private string accessToken;

        public DesignTimeAuthentication()
        {
            this.version = DesignTimeAuthentication.CurrentVersion;
        }

        public string UserName
        {
            get { return this.userName; }
            set
            {
                this.userName = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(DesignTimeAuthentication.Summary));
                this.OnPropertyChanged(nameof(DesignTimeAuthentication.IsNewIdentity));
            }
        }

        public EnvironmentType EnvironmentType
        {
            get { return this.environmentType; }
            set
            {
                this.environmentType = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(DesignTimeAuthentication.Summary));
            }
        }

        public Uri MyDomain
        {
            get { return this.myDomain; }
            set
            {
                this.myDomain = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(DesignTimeAuthentication.Summary));
            }
        }

        public string AccessToken
        {
            get { return this.accessToken; }
            set
            {
                this.accessToken = value;
                this.OnPropertyChanged();
            }
        }

        public string RefreshToken { get; set; }

        public string MetadataServiceUrl
        {
            get
            {
                // The backing field's value contains a {version} placeholder. The placeholder is only replaced upon read rather than set
                // because this state is cached in the identity MRU.  If this VSIX is rev'd the version it depends on may change.
                return this.metadataServiceUrl == null ? null : this.metadataServiceUrl.Replace("{version}", Constants.SalesforceApiVersion);
            }
            set { this.metadataServiceUrl = value; }
        }

        [DataMember]
        public string InstanceUrl { get; set; }

        public bool IsNewIdentity
        {
            get { return this.UserName == null; }
        }

        public Uri Domain
        {
            get
            {
                switch (this.EnvironmentType)
                {
                    case EnvironmentType.Production:
                        return new Uri(Constants.ProductionDomainUrl);
                    case EnvironmentType.Sandbox:
                        return new Uri(Constants.SandboxDomainUrl);
                    case EnvironmentType.Custom:
                        return this.MyDomain;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string Summary
        {
            get
            {
                string result;

                if (this.IsNewIdentity)
                {
                    result = Resources.DesignTimeAuthentication_NewIdentityName;
                }
                else
                {
                    switch (this.EnvironmentType)
                    {
                        case EnvironmentType.Production:
                            result = Resources.DesignTimeAuthentication_Summary_Production.FormatCurrentCulture(this.UserName);
                            break;
                        case EnvironmentType.Sandbox:
                            result = Resources.DesignTimeAuthentication_Summary_Sandbox.FormatCurrentCulture(this.UserName);
                            break;
                        case EnvironmentType.Custom:
                            result = Resources.DesignTimeAuthentication_Summary_Custom.FormatCurrentCulture(this.UserName, this.MyDomain);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                return result;
            }
        }

        public int Version
        {
            get { return this.version; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool Equals(DesignTimeAuthentication other)
        {
            return other != null
                && this.UserName == other.UserName
                && this.EnvironmentType == other.EnvironmentType
                && this.MyDomain == other.MyDomain;
        }

        public override string ToString()
        {
            return this.Summary;
        }
    }
}
