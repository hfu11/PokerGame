using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NetMsg
    {
        //操作码
        public int opCode { get; set; }
        //子操作码
        public int subCode { get; set; }
        //传递的对象
        public object value { get; set; }

        public NetMsg()
        {

        }

        public NetMsg(int opCode, int subCode, object value)
        {
            this.opCode = opCode;
            this.subCode = subCode;
            this.value = value;
        }

        /// <summary>
        /// 这样的话，每次传递消息就不用总是new一个新对象了，只需要在开始new一个，之后根据需要改变里面的值
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        public void Change(int opCode, int subCode, object value)
        {
            this.opCode = opCode;
            this.subCode = subCode;
            this.value = value;
        }        
    }
}
