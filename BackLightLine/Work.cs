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
        public string readCmd = "01WRD";
        public string writeCmd = "01WWR";
        public string camCmdAds;
        public string camResAds;
        public byte[] resBytes = new byte[1024];
        public string result;
        public bool san = true;
        private string lastCmd = "";

        public void go()
        {
            Thread currentThread = Thread.CurrentThread;

            while (san)
            {
                for (int i = 0; i < resBytes.Length; i++)
                {
                    resBytes[i] = 0;
                }

                lock (this)
                {
                    result = "";
                    /*
                     *var plcCmdStr = InspectUtils.receiveDataFromTarget(plcSocket,resBytes);
                     *var plcCmd = int.Parse(plcCmdStr);
                     */
                    /*
                     * 读取PLC命令
                     */
                    int plcCmd = getPlcCmd(plcSocket, camCmdAds);
                    if (plcCmd != 0)
                    {
                        result = readInspect(plcCmd);
                    }
                    if (result == "1")
                    {
                        setPlcCmd(plcSocket, camResAds, "0001\r\n");
                    }
                    else if (result == "2")
                    {
                        setPlcCmd(plcSocket, camResAds, "0002\r\n");
                    }
                    result = "";
                    Thread.Sleep(100);
                }
            }
        }

        private string setPlcCmd(Socket socket, string plcAddress, string setResult)
        {
            Console.WriteLine(writeCmd + plcAddress + setResult + "\r\n");
            string rtn = InspectUtils.sendCmdToTarget(socket, writeCmd + plcAddress + setResult + "\r\n");
            return rtn;
        }

        private int getPlcCmd(Socket socket, string plcAddress)
        {
            int enCamId = 0;
            InspectUtils.sendCmdToTarget(socket, readCmd + plcAddress + "\r\n");
            var cmd = InspectUtils.receiveDataFromTarget(socket, new byte[1024]);
            var indexOf = cmd.IndexOf('\r');
            if (indexOf != -1)
            {
                cmd = cmd.Substring(0, indexOf);
            }

            if (cmd == "11OK0001" && cmd != lastCmd)
            {
                Console.WriteLine(Thread.CurrentThread.Name + "触发");
                if (plcAddress == "D6030 01")
                {
                    enCamId = 1;
                }

                if (plcAddress == "D8030 01")
                {
                    enCamId = 2;
                }

                if (plcAddress == "D10030 01")
                {
                    enCamId = 3;
                }
            }

            lastCmd = (string) cmd.Clone();
            return enCamId;
        }

        private string readInspect(int camId)
        {
            string str = camId.ToString() + ';';
            InspectUtils.sendCmdToTarget(localSocket, str);
            var receiveData = InspectUtils.receiveDataFromTarget(localSocket, resBytes);
            Console.WriteLine(receiveData + "---Inspect");
            return receiveData;
        }
    }
}