using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Web.Hubs
{
    public class P2PHub : Hub
    {
        public UserContext db => Startup.db;

        /// <summary>
        /// 重写Hub连接事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            // 查询用户
            int userid = 0;
            int.TryParse(Context.GetHttpContext().Request.Cookies["userid"], out userid);
            string username = Context.GetHttpContext().Request.Cookies["username"].ToString();
            var user = db.Users.SingleOrDefault(u => u.UserID == userid);

            //判断用户是否存在,否则添加
            if (user == null)
            {
                user = new User()
                {
                    UserID = userid,
                    UserName = username
                };
                db.Users.Add(user);
            }
            var conn = user.Connections.SingleOrDefault(c => c.ConnectionID == Context.ConnectionId);
            if (conn == null)
            {
                conn = new Connection()
                {
                    ConnectionID = Context.ConnectionId,
                    Connected = true,
                    UserID = userid
                };
                user.Connections.Add(conn);

                db.Connections.Add(conn);
            }
            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 重写Hub连接断开的事件
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var conn = db.Connections.Where(u => u.ConnectionID == Context.ConnectionId).FirstOrDefault();

            //判断用户是否存在,存在则删除
            if (conn != null)
            {
                db.Connections.Remove(conn);
                //删除用户
                var user = db.Users.SingleOrDefault(u => u.UserID == conn.UserID);
                if (user != null)
                {
                    user.Connections.RemoveAll(c => c.ConnectionID == conn.ConnectionID);                   
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
