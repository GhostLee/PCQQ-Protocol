﻿using QQ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ.Framework.Packets.Send.Message
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public class Send_0x0002 : SendPacket
    {
        public Send_0x0002(QQUser User, string Message, FriendMessageType messageType, long Group)
            : base(User)
        {
            Sequence = GetNextSeq();
            _secretKey = user.QQ_SessionKey;
            Command = QQCommand.Message0x0002;
            _message = Encoding.UTF8.GetBytes(Message);
            _messageType = messageType;
            _group = Group;
        }
        protected override void PutHeader(ByteBuffer buf)
        {
            base.PutHeader(buf);
            buf.Put(user.QQ_PACKET_FIXVER);
        }

        private byte _packetCount = 1;
        private byte _packetIndex = 0;
        long _group;
        /// <summary>
        /// 消息类型
        /// </summary>
        public FriendMessageType _messageType { get; set; }
        private byte[] _message { get; set; }
        /// <summary>
        /// 初始化包体
        /// </summary>
        /// <param name="buf">The buf.</param>
        protected override void PutBody(ByteBuffer buf)
        {
            var _DateTime = Util.GetTimeSeconds(DateTime.Now);
            var group = GroupToGid(_group);
            if (_messageType == FriendMessageType.Xml)
            {

            }
            else if (_messageType == FriendMessageType.GroupMessage)
            {
                var Length = _message.Length + 56;
                
                buf.Put(new byte[] { 0x2A });
                buf.PutLong(group);
                buf.PutUShort((ushort)Length);

                buf.Put(new byte[] { 0x00, 0x01, _packetCount, _packetIndex, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4D, 0x53, 0x47, 0x00, 0x00, 0x00, 0x00, 0x00 });
                buf.PutLong(_DateTime);
                buf.Put(Util.RandomKey(4));
                buf.Put(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x86, 0x00 });
                buf.Put(new byte[] { 0x00, 0x0C });
                buf.Put(new byte[] { 0xE5, 0xBE, 0xAE, 0xE8, 0xBD, 0xAF, 0xE9, 0x9B, 0x85, 0xE9, 0xBB, 0x91 });
                buf.Put(new byte[] { 0x00, 0x00 });

                buf.Put(new byte[] { 0x01 });
                buf.PutUShort((ushort)(_message.Length + 3));
                buf.Put(new byte[] { 0x01 });
                buf.PutUShort((ushort)_message.Length);
                buf.Put(_message);

            }
            else if (_messageType == FriendMessageType.ExitGroup)
            {
                buf.Put(new byte[] { 0x09 });
                buf.PutLong(group);
            }
        }
        public long GroupToGid(long groupid)
        {
            var group = groupid.ToString();
            var left = Convert.ToInt64(group.Substring(0, group.Length - 6));
            string right = "", gid = "";
            if (left >= 1 && left <= 10)
            {
                right = group.Substring(group.Length - 6, 6);
                gid = (left + 202).ToString() + right;
            }
            else if (left >= 11 && left <= 19)
            {
                right = group.Substring(group.Length - 6, 6);
                gid = (left + 469).ToString() + right;
            }
            else if (left >= 20 && left <= 66)
            {
                left = Convert.ToInt64(left.ToString().Substring(0, 1));
                right = group.Substring(group.Length - 7, 7);
                gid = (left + 208).ToString() + right;
            }
            else if (left >= 67 && left <= 156)
            {
                right = group.Substring(group.Length - 6, 6);
                gid = (left + 1943).ToString() + right;
            }
            else if (left >= 157 && left <= 209)
            {
                left = Convert.ToInt64(left.ToString().Substring(0, 2));
                right = group.Substring(group.Length - 7, 7);
                gid = (left + 199).ToString() + right;
            }
            else if (left >= 210 && left <= 309)
            {
                left = Convert.ToInt64(left.ToString().Substring(0, 2));
                right = group.Substring(group.Length - 7, 7);
                gid = (left + 389).ToString() + right;
            }
            else if (left >= 310 && left <= 499)
            {
                left = Convert.ToInt64(left.ToString().Substring(0, 2));
                right = group.Substring(group.Length - 7, 7);
                gid = (left + 349).ToString() + right;
            }
            else
            {
                return groupid;
            }
            return Convert.ToInt64(gid);
        }

        public static List<Send_0x0002> SendLongMessage(QQUser User, string Message, FriendMessageType messageType, long Group)
        {
            var buffer = new ByteBuffer();
            var list = new List<byte[]>();
            foreach (var chr in Message)
            {
                var bytes = Encoding.UTF8.GetBytes(chr.ToString());
                if (buffer.Length + bytes.Length > 699)
                {
                    list.Add(buffer.ToByteArray());
                    buffer.Initialize();
                }

                buffer.Put(bytes);
            }
            list.Add(buffer.ToByteArray());
            buffer.Initialize();

            byte index = 0;
            var ret = new List<Send_0x0002>();
            foreach (var byteBuffer in list)
            {
                ret.Add(new Send_0x0002(User, "", messageType, Group)
                {
                    _packetCount = (byte)list.Count,
                    _packetIndex = index++,
                    _message = byteBuffer
                });
            }

            return ret;
        }
    }
}
