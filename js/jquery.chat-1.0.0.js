(function ($) {
    var serviceHandlers = new Object();
    var defaults = new Object();
    var hub = new Object();
    var clientHandlers = new Object();
    var methods = {
        init: function (options) {
            var setting = $.extend(defaults, options);
            if (setting) {
                hub = $.connection.ChatHub;
                //判断链接Signalr成功后是否要执行回调
                if (setting.hasOwnProperty("connected") && typeof (setting.connected) == "function") {
                    serviceHandlers.Connected(setting.connected);
                } else {
                    serviceHandlers.Connected();
                }
                if (setting.hasOwnProperty("receiveMessage") && typeof (setting.receiveMessage) == "function") {
                    serviceHandlers.ReceiveMessage(setting.receiveMessage);
                }
                if (setting.hasOwnProperty("checkedMessage") && typeof (setting.checkedMessage) == "function") {
                    serviceHandlers.CheckedMessage(setting.checkedMessage);
                }
                if (setting.hasOwnProperty("deletedMessage") && typeof (setting.deletedMessage) == "function") {
                    serviceHandlers.DeletedMessage(setting.deletedMessage);
                }
                if (setting.hasOwnProperty("danMu") && typeof (setting.danMu) == "function") {
                    serviceHandlers.DanMu(setting.danMu);
                }
                if (setting.hasOwnProperty("privateChat") && typeof (setting.privateChat) == "function") {
                    serviceHandlers.PrivateChat(setting.privateChat);
                }
                if (setting.hasOwnProperty("receiveYourself") && typeof (setting.receiveYourself) == "function") {
                    serviceHandlers.ReceiveYourself(setting.receiveYourself);
                }
                return clientHandlers;
            }
            return null;
        }
    };
    defaults = {
        groupName: "",
        //是否允许跨域
        crossDomain: false,
        //跨域的"/signalr/hubs"地址
        crossUrl: "",
        //Signalr通讯的连接标识
        connectionId: ""
    }
    //服务端回调客户端的函数
    serviceHandlers = {
        //成功链接Signalr，callBack-回调函数
        Connected: function (callBack) {
            //判断是否需要设置跨域访问
            if (defaults.crossDomain && defaults.crossUrl) {
                $.connection.hub.url = defaults.crossUrl;
            }
            hub.client.Connected = function (connectionId) {
                defaults.connectionId = connectionId;
            }
            $.connection.hub.start().done(function () {
                hub.server.Connect(defaults.groupName);
                if (typeof (callBack) == "function") {
                    callBack();
                }
            });
        },
        //接收Signalr发送过来的消息，callBack-回调函数，回调函数会带上发送过来的消息的json数据
        ReceiveMessage: function (callBack) {
            hub.client.ReceiveMessage = function (msg) {
                if (typeof (callBack) == "function" && msg) {
                    callBack(msg);
                }
            }
        },
        //接收审核通过的消息，callBack-回调函数，回调函数会带上html字符串和被审核消息的消息编号
        CheckedMessage: function (callBack) {
            hub.client.CheckedMessage = function (html, msgId) {
                if (typeof (callBack) == "function" && html) {
                    callBack(html, msgId);
                }
            }
        },
        //接收推送的弹幕消息，callBack-回调函数，回调函数会带上发送过来的消息的json数据
        DanMu: function (callBack) {
            hub.client.DanMu = function (json) {
                if (typeof (callBack) == "function") {
                    callBack(json);
                }
            }
        },
        //私聊，callBack-回调函数，回调函数会带上发送过来的消息的json数据
        PrivateChat: function (callBack) {
            hub.client.PrivateChat = function (json) {
                if (typeof (callBack) == "function") {
                    callBack(json);
                }
            }
        },
        //接收自己发送的消息，只有直接看到，其他人看不到，callBack-回调函数，回调函数会带上发送过来的消息的json数据
        ReceiveYourself: function (callBack) {
            hub.client.ReceiveYourself = function (json) {
                if (typeof (callBack) == "function") {
                    callBack(json);
                }
            }
        },
        //清除客户端的消息，callBack-回调函数，回调函数会带上msgId字符串和被审核消息的消息编号
        DeletedMessage: function (callBack) {
            hub.client.DeletedMessage = function (msgId) {
                if (typeof (callBack) == "function" && msgId) {
                    callBack(msgId);
                }
            }
        },
        //关闭浏览器或关闭当前页面的时候执行，callBack-回调函数
        Disconnected: function (callBack) {
            hub.client.Disconnected = function () {
                if (typeof (callBack) == "function") {
                    callBack();
                }
            }
        }
    };
    //客户端调用服务端的函数
    clientHandlers = {
        //审核消息，html-审核通过的消息的html字符串 msgId-被审核消息的消息编号
        CheckMessage: function (html, msgId) {
            hub.server.CheckMessage(html, msgId, defaults.groupName);
        },
        //发送消息，msg-json格式的消息数据
        SendMessage: function (msg) {
            hub.server.SendMessage(msg, "SendMessage", defaults.groupName);
        },
        //推送弹幕
        DanMu: function (msg) {
            hub.server.SendMessage(msg, "DanMu", defaults.groupName);
        },
        //私聊
        PrivateChat: function (msg) {
            hub.server.SendMessage(msg, "PrivateChat", defaults.groupName);
        },
        //发送只有自己能看到的消息，一般在屏蔽IP或者设置梦游的情况下调用
        SendToYourself: function (msg) {
            hub.server.SendToYourself(msg);
        },
        DeleteMessage: function (msgId) {
            hub.server.DeleteMessage(msgId, defaults.groupName);
        }
    }
    $.Chat = function (method) {
        //初始化数据
        if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }
        return methods.init.apply(this, arguments);
    };
})($)
