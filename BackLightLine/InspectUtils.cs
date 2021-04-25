using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackLightLine
{
    class InspectUtils
    {
        /// <summary>
        /// 用于连接INspectExpress
        /// </summary>
        /// <param name="serverIp">Inspect的IP地址</param>
        /// <param name="serverPort">Inspect开放的端口</param>
        /// <returns></returns>
        public static Socket connectToTarget(string serverIp, int serverPort)
        {
            IPAddress ipAddress;
            Socket socket;
            if (serverIp != null && serverPort != 0)
            {
                ipAddress = IPAddress.Parse(serverIp);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(new IPEndPoint(ipAddress, serverPort));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }

                return socket;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 用于关闭连接
        /// </summary>
        /// <param name="socket"></param>
        public static void shutDownConnect(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
        }

        /// <summary>
        /// 用于向Inspect发送指令
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverPORT"></param>
        /// <param name="cmd">命令内容</param>
        /// <returns></returns>
        public static string sendCmdToTarget(string serverIP, int serverPORT, string cmd)
        {
            Socket socket;
            var cmdBytes = Encoding.UTF8.GetBytes(cmd);
            if (serverIP != null && serverPORT != 0 & cmd != null)
            {
                socket = connectToTarget(serverIP, serverPORT);
                if (socket == null)
                {
                    return "Connect_Fail";
                }

                try
                {
                    socket.Send(cmdBytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "send_Fail";
                }

                return "Send_Successful";
            }

            return "Parameter_Null";
        }

        public static string sendCmdToTarget(Socket socket, string cmd)
        {
            var cmdBytes = Encoding.UTF8.GetBytes(cmd);
            if (socket != null & cmd != "")
            {
                try
                {
                    socket.Send(cmdBytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "send_Fail";
                }

                return "Send_Successful";
            }

            return "Parameter_Null";
        }


        /*
         * 用于从Inspect接收数据
         */
        /// <summary>
        /// 用于从Inspect接收数据
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="recBytes">需要接受多少字节的返回值</param>
        /// <returns>recStr</returns>
        public static string receiveDataFromTarget(string serverIp, int serverPort, byte[] recBytes)
        {
            Socket socket;
            string recStr = "";
            if (serverIp != null && serverPort != 0 & recBytes != null)
            {
                socket = connectToTarget(serverIp, serverPort);
                if (socket == null)
                {
                    return "Connect_Fail";
                }

                try
                {
                    var receiveCode = socket.Receive(recBytes, recBytes.Length, 0);
                    recStr += Encoding.UTF8.GetString(recBytes, 0, receiveCode);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "Receive_Fail";
                }

                return recStr;
            }

            return "Parameter_Null";
        }

        public static string receiveDataFromTarget(Socket socket, byte[] recBytes)
        {
            string recStr = "";
            if (socket != null & recBytes != null)
            {
                try
                {
                    var receiveCode = socket.Receive(recBytes, recBytes.Length, 0);
                    recStr += Encoding.UTF8.GetString(recBytes, 0, receiveCode);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return "Receive_Fail";
                }

                return recStr;
            }

            return "Parameter_Null";
        }
    }
}