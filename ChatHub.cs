using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace Chat.Core
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 接收客户端推送过来的消息，通过函数名称来判断回调客户端的什么函数
        /// </summary>
        /// <param name="json">json格式的消息数据</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("SendMessage")]
        public void SendMessage(string json, string methodName, string groupName)
        {
            switch (methodName.ToLower())
            {
                case "sendmessage":
                    //把从A客户端接收到的消息推送给同一组的其他客户端
                    Clients.Group(groupName).ReceiveMessage(json);
                    break;
                case "danmu":
                    //接收客户端的弹幕数据，并推到同一组的客户端
                    Clients.Group(groupName).DanMu(json);
                    break;
                case "privatechat":
                    //私聊，分组字符串必须在发起私聊前加入到分组中
                    Clients.Group(groupName).PrivateChat(json);
                    break;
            }

        }
        /// <summary>
        /// 私聊分组添加
        /// </summary>
        /// <param name="connectionId">Signalr连接标识</param>
        /// <param name="groupName">分组字符串</param>
        public void AddPrivateGroupName(string connectionId, string groupName)
        {
            Groups.Add(connectionId, groupName);
        }
        /// <summary>
        /// 推送的消息只有自己可以看
        /// </summary>
        /// <param name="json">json数据</param>
        public void SendToYourself(string json)
        {
            Clients.Client(Context.ConnectionId).ReceiveYourself(json);
        }
        /// <summary>
        /// 接收客户端审核推送的消息
        /// </summary>
        /// <param name="html">html格式的数据</param>
        /// <param name="msgId">被审核消息的编号</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("CheckMessage")]
        public void CheckMessage(string html, string msgId, string groupName)
        {
            //调用客户端审核通过的js函数
            Clients.Group(groupName).CheckedMessage(html, msgId);
        }
        /// <summary>
        /// 清除客户端的消息
        /// </summary>
        /// <param name="msgId">被清除的消息的消息编号</param>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("DeleteMessage")]
        public void DeleteMessage(string msgId, string groupName)
        {
            //调用客户端清除的js函数
            Clients.Group(groupName).DeletedMessage(msgId);
        }
        /// <summary>
        /// 客户端页面加载成功后，主动链接Signalr，分组字符串如果为空字符串，分组无效
        /// </summary>
        /// <param name="groupName">分组字符串</param>
        [HubMethodName("Connect")]
        public void Connect(string groupName)
        {
            
            //将分组字符串加入Signalr分组
            Groups.Add(Context.ConnectionId, groupName);
            //把connectionId推到客户端，以便后面使用
            Clients.Client(Context.ConnectionId).Connected(Context.ConnectionId);
        }
        /// <summary>
        /// 关闭浏览器或关闭页面，断开Signalr
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //调用客户端的断开函数
            Clients.Client(Context.ConnectionId).Disconnected();
            return base.OnDisconnected(stopCalled);
        }

    }
}
