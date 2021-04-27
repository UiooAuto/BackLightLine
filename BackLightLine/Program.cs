using System;
using System.Net.Sockets;
using System.Threading;

namespace BackLightLine
{
    class Program
    {
        static Work work1 = new Work();
        static Work work2 = new Work();
        static Work work3 = new Work();
        bool startSan = true;

        static void Main(string[] args)
        {
            //var plcIp1 = "192.168.0.3";
            //var plcIp2 = "192.168.0.4";
            //var plcIp3 = "192.168.0.5";
            var plcIp1 = "127.0.0.1";
            var plcIp2 = "127.0.0.1";
            var plcIp3 = "127.0.0.1";
            var plcPort = 12289;
            var localIp = "127.0.0.1";
            var localPort = 5024;

            string cam1CmdAds = "D6030 01";
            string cam1ResAds = "D6032 01 ";
            string cam2CmdAds = "D8030 01";
            string cam2ResAds = "D8032 01 ";
            string cam3CmdAds = "D10030 01";
            string cam3ResAds = "D10032 01 ";

            Socket plc1Socket = InspectUtils.connectToTarget(plcIp1, plcPort);
            Socket plc2Socket = InspectUtils.connectToTarget(plcIp2, plcPort);
            Socket plc3Socket = InspectUtils.connectToTarget(plcIp3, plcPort);
            Socket localSocket = InspectUtils.connectToTarget(localIp, localPort);

            work1.plcSocket = plc1Socket;
            work1.localSocket = localSocket;
            work1.camCmdAds = cam1CmdAds;
            work1.camResAds = cam1ResAds;
            work1.san = true;

            work2.plcSocket = plc2Socket;
            work2.localSocket = localSocket;
            work2.camCmdAds = cam2CmdAds;
            work2.camResAds = cam2ResAds;
            work2.san = true;

            work3.plcSocket = plc3Socket;
            work3.localSocket = localSocket;
            work3.camCmdAds = cam3CmdAds;
            work3.camResAds = cam3ResAds;
            work3.san = true;

            Thread thread1 = new Thread(new ThreadStart(work1.go));
            Thread thread2 = new Thread(new ThreadStart(work2.go));
            Thread thread3 = new Thread(new ThreadStart(work3.go));

            thread1.Name = "cam1";
            thread2.Name = "cam2";
            thread3.Name = "cam3";

            thread1.Start();
            thread2.Start();
            thread3.Start();
        }
    }
}