using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization;
using Remote.Infrastructure.Interfaces;

namespace Remote.Infrastructure.DataContracts
{
    public enum OnlineStateEnum
    {
        Offline,
        Online,
        Hiding
    }

    public class ClientTokenCompare : IEqualityComparer<ClientToken>
    {
        public bool Equals(ClientToken x, ClientToken y)
        {
            return x.Address.Equals(y.Address) && x.ServicePort == y.ServicePort;
        }

        public int GetHashCode(ClientToken obj)
        {
            return obj.Address.GetHashCode();
        }
    }

    [DataContract]
    public class ClientToken : IClientToken, INotifyPropertyChanged
    {
        public ClientToken()
        {
            AddedGroupList = new List<string>();
        }

        private OnlineStateEnum _onlineState;
        private string _iPv4StringAddress;
        private IPAddress _address;
        private int _servicePort;
        private string _nickName;

        public bool IsDisposed { get; set; } = false;

        public List<string> AddedGroupList { get; set; }

        [DataMember]
        public string NickName
        {
            get { return _nickName; }
            set {
                if (value.Length>20)
                {
                    throw new IndexOutOfRangeException();
                }
                _nickName = value; OnPropertyChanged(nameof(NickName)); }
        }

        [DataMember]
        public int ServicePort
        {
            get { return _servicePort; }
            set {
                if (value < 0 || value >65535)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _servicePort = value; OnPropertyChanged(nameof(ServicePort)); }
        }

        [DataMember]
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(nameof(Address)); }
        }

        [DataMember]
        public string IPv4StringAddress
        {
            get { return _iPv4StringAddress; }
            set
            {
                var address = IPAddress.Parse(value);
                this._address = address;
                _iPv4StringAddress = value;
                OnPropertyChanged(nameof(IPv4StringAddress));
            }
        }

        [DataMember]
        public OnlineStateEnum OnlineState
        {
            get { return _onlineState; }
            set { _onlineState = value;
                OnPropertyChanged(nameof(OnlineState));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static void PropertyCopy(ClientToken src, ClientToken dst)
        {
            dst.IPv4StringAddress = src.IPv4StringAddress;
            dst.Address = src.Address ?? IPAddress.Parse(src.IPv4StringAddress);
            dst.ServicePort = src.ServicePort;
            dst.NickName = src.NickName;
            dst.OnlineState = src.OnlineState;
        }

        

        public bool Equals(IClientToken other)
        {
            return other.Address.Equals(this.Address)
                && other.ServicePort == this.ServicePort;
        }

        public bool AddNewGroup(string groupName, IErrMessageHandler errMsg)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                errMsg.ErrorMessage = "组名不允许为空";
            }

            var contain = AddedGroupList.Contains(groupName);
            if (!contain)
            {
                AddedGroupList.Add(groupName);
                return true;
            }
            errMsg.ErrorMessage = "已在此组内";
            return false;
        }

        public bool QuitGroup(string groupName, IErrMessageHandler errMsg)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                errMsg.ErrorMessage = "组名不允许为空";
            }

            var contain = AddedGroupList.Contains(groupName);
            if (contain)
            {
                AddedGroupList.Remove(groupName);
                return true;
            }
            errMsg.ErrorMessage = "不在此组内";
            return false;
        }

        public IClientToken PropertyCopy()
        {
            ClientToken clientCopy = new ClientToken();
            ClientToken.PropertyCopy(this, clientCopy);
            return clientCopy;
        }

        public string GenServiceAddress()
        {
            return $"net.tcp://{this.IPv4StringAddress}:{this.ServicePort}/";
        }
    }
}