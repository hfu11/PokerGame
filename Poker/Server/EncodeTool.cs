using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class EncodeTool
    {
        /// <summary>
        /// 构造包 包头+包尾
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
        {
            //使用内存流对象，使用using可以免去使用ms.Close()
            using (MemoryStream ms = new MemoryStream())
            {
                using(BinaryWriter bw = new BinaryWriter(ms))
                {
                    //写入包头（数据长度）+包尾
                    bw.Write(data.Length);
                    bw.Write(data);
                    byte[] packet = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, packet, 0, (int)ms.Length);
                    return packet;
                }
            }
        }

        /// <summary>
        /// 解包，从缓冲区里取出一个完整的包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] DecodePacket(ref List<byte> cache)
        {
            if(cache.Count < 4)
            {
                return null;
            }

            using(MemoryStream ms = new MemoryStream(cache.ToArray()))
            {
                using(BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();
                    int remainLength = (int)(ms.Length - ms.Position);

                    if(length > remainLength)
                    {
                        //not a complete packet
                        return null;
                    }
                    byte[] data = br.ReadBytes(length);

                    //更新缓存
                    cache.Clear();
                    remainLength = (int)(ms.Length - ms.Position);
                    cache.AddRange(br.ReadBytes(remainLength));

                    return data;
                }
            }
        }

        /// <summary>
        /// 将NetMsg转换成字节数组，发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(NetMsg msg)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.opCode);
                    bw.Write(msg.subCode);
                    if(msg.value != null)
                    {
                        bw.Write(EncodeObj(msg.value));
                    }

                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                    return data;
                }
            }
        }

        /// <summary>
        /// 将字节数组转换成NetMsg
        /// </summary>
        /// <returns></returns>
        public static NetMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using(BinaryReader br = new BinaryReader(ms))
                {
                    NetMsg msg = new NetMsg();
                    msg.opCode = br.ReadInt32();
                    msg.subCode = br.ReadInt32();
                    if(ms.Length - ms.Position > 0)
                    {
                        //value有值
                        msg.value = DecodeObj(br.ReadBytes((int)(ms.Length-ms.Position)));
                    }

                    return msg;
                }
            }
        }

        private static byte[] EncodeObj(object obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                byte[] data = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0,(int)ms.Length);

                return data;
            }
        }

        private static object DecodeObj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(ms);
                return obj;
            }
        }
    }
}
