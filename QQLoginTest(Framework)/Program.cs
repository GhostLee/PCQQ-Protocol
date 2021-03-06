﻿using QQ.Framework;
using QQ.Framework.Packets.Send.Login;
using QQ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQLoginTest_Framework_
{
    class Program
    {
        static QQClient client;
        static void Main(string[] args)
        {
            long MyQQ = 0;
            string MyPassWord = "";
            QQUser user = new QQUser(MyQQ, MyPassWord);
            client = new QQClient()
            {
                QQUser = user
            };
            client.EventReceive_0x0017 += Client_EventReceive_0x0017;
            client.EventReceive_0x00CE += Client_EventReceive_0x00CE;

            client.EventReceive_0x00BA += Client_EventReceive_0x00BA;

            client.Login();

            Console.ReadKey();
        }
        private static void Client_EventReceive_0x00BA(object sender, QQEventArgs<QQ.Framework.Packets.Receive.Login.Receive_0x00BA> e)
        {
            if (e.ReceivePacket.VerifyCommand == 0x02)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "//yanzhengma";
                var img = ImageHelper.CreateImageFromBytes(path, e.QQClient.QQUser.QQ_PACKET_00BAVerifyCode);
                string VerifyCode = Console.ReadLine();
                if (!string.IsNullOrEmpty(VerifyCode))
                {
                    var buf = new ByteBuffer();
                    new Send_0x00BA(e.ReceivePacket.user, VerifyCode).Fill(buf);
                    e.QQClient.Send(buf);
                }
            }
        }

        /// <summary>
        /// 好友消息
        /// </summary>
        private static void Client_EventReceive_0x00CE(object sender, QQEventArgs<QQ.Framework.Packets.Receive.Message.Receive_0x00CE> e)
        {
        }

        /// <summary>
        /// 群消息
        /// </summary>
        private static void Client_EventReceive_0x0017(object sender, QQEventArgs<QQ.Framework.Packets.Receive.Message.Receive_0x0017> e)
        {
        }
    }
}
