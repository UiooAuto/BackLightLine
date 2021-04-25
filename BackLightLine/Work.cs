using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace BackLightLine
{
    class Work
    {
        public Socket plcSocket;
        public Socket localSocket;
        public string readCmd = "01WRD ";
        public string writeCmd = "01WWR ";
        public string camCmdAds;
        public string camResAds;
        public byte[] resBytes = new byte[1024];
        public string result;
        public bool san = true;

        public void go()
        {
            Thread currentThread = Thread.CurrentThread;
            lock (this)
            {
                while (san)
                {
                    result = "";
                    var plcCmdStr = InspectUtils.receiveDataFromTarget(plcSocket,resBytes);
                    var plcCmd = int.Parse(plcCmdStr);
                    /*
                     * 读取PLC命令
                     */
                    /*int plcCmd = getPlcCmd(plcSocket, camCmdAds);*/
                    Console.WriteLine(plcCmd + "---" + currentThread.Name);
                    if (plcCmd == 1)
                    {
                        result = readInspect(1);

                    }
                    else if (plcCmd == 2)
                    {
                        result = readInspect(2);
                    }
                    else if (plcCmd == 3)
                    {
                        result = readInspect(3);
                    }
                    if (result == "1")
                    {
                        setPlcCmd(plcSocket, camResAds, "0001");
                    }
                    else if (result == "2")
                    {
                        setPlcCmd(plcSocket, camResAds, "0002");
                    }
                    Thread.Sleep(200);
                }
            }

        }

        private string setPlcCmd(Socket socket, string plcAddress, string setResult)
        {
            string rtn = InspectUtils.sendCmdToTarget(socket, writeCmd + plcAddress + " " + setResult + "\r\n");
            return rtn;
        }

        private int getPlcCmd(Socket socket, string plcAddress)
        {
            string cmd = InspectUtils.sendCmdToTarget(socket, readCmd + plcAddress + "\r\n");
            Console.WriteLine(cmd);
            if (cmd == "11OK0001")
            {
                InspectUtils.sendCmdToTarget(plcSocket, "ok");
                if (plcAddress == "D6030 01")
                {
                    return 1;
                }
                else if (plcAddress == "D8030 01")
                {
                    return 2;
                }
                else if (plcAddress == "D10030 01")
                {
                    return 3;
                }
            }
            return 0;
        }

        private string readInspect(int camId)
        {
            string str = camId.ToString() + '\r';
            InspectUtils.sendCmdToTarget(localSocket, str);
            var receiveDataFromTarget = InspectUtils.receiveDataFromTarget(localSocket, resBytes);
            Console.WriteLine(receiveDataFromTarget+"---Inspect");
            return receiveDataFromTarget;
        }
    }
}
