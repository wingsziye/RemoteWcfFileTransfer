using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Remote.Infrastructure.Interfaces;
using Remote.Infrastructure.Settings;

namespace Remote.Infrastructure.DataContracts
{
    /// <summary>
    /// 客户端组比较器
    /// </summary>
    public class ClientGourpCompare : IEqualityComparer<ClientGroup>
    {
        public bool Equals(ClientGroup x, ClientGroup y)
        {
            return x.GroupName == y.GroupName;
        }

        public int GetHashCode(ClientGroup obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// 客户端的组
    /// </summary>
    [DataContract]
    public class ClientGroup : IDisposable
    {
        public ClientGroup(string groupName)
        {
            this.GroupName = groupName;
            _onlineClients = new ClientTokenList() ;
        }

        private string groupName;

        private ClientTokenList _onlineClients;

        private ClientToken _owner;

        /// <summary>
        /// 组在线成员
        /// </summary>
        [DataMember]
        public ClientTokenList OnlineClients
        {
            get { return _onlineClients; }
            set { _onlineClients = value; }
        }

        /// <summary>
        /// 组名
        /// </summary>
        [DataMember]
        public string GroupName
        {
            get { return groupName; }
            private set { groupName = value; }
        }

        /// <summary>
        /// 组所有者
        /// </summary>
        [DataMember]
        public ClientToken Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        #region AutoGen

        

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (groupName == MagicStrings.DefaultGroup)
            {
                throw new InvalidOperationException("不允许销毁默认组");
            }
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Owner = null;
                    Console.WriteLine($"解散组 {this.groupName} 中共有{this.OnlineClients.Count}人");
                    foreach (var onlineClient in OnlineClients)
                    {
                        onlineClient.QuitGroup(this.groupName,null);
                    }
                    OnlineClients.Clear();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                GroupName = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~ClientGroup() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// 客户端组的单例集合
    /// </summary>
    public class ClientGroupCollection : IClientGroupCollection
    {
        private static readonly List<ClientGroup> clientGroupList = new List<ClientGroup>(){new ClientGroup(MagicStrings.DefaultGroup)};

        public static readonly ClientGroupCollection Instance = new ClientGroupCollection();

        private ClientGroupCollection()
        {
        }


        public ClientGroup this[string index]
        {
            get { return clientGroupList.Single((item) => index == item.GroupName); }
        }

        public int Count
        {
            get { return clientGroupList.Count; }
        }

        public ClientGroup this[int index]
        {
            get { return clientGroupList[index]; }
        }

        public void Add(ClientGroup group)
        {
            bool b = clientGroupList.Contains(group, new ClientGourpCompare());
            if (!b)
            {
                clientGroupList.Add(group);
            }
            else
            {
                throw new ArgumentException("已添加了具有相同名字的项");
            }
        }

        public void AddRange(IEnumerable<ClientGroup> clients)
        {
            clientGroupList.AddRange(clients);
        }

        public void Remove(ClientGroup group)
        {
            clientGroupList.Remove(group);
        }

        public void Remove(string groupName)
        {
            clientGroupList.Remove(this[groupName]);
        }

        /// <summary>
        /// 判断GroupList中是否包含参数名字的Group,若发生异常，则不包含
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool Contains(string groupName)
        {
            try
            {
                var c = this[groupName];
                if (c == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
