﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSharpGame
{
    class Logic
    {
        MyClientSoc myClientSoc;
        string myClientName;

        public const int MAX_PIC = 64;
        static System.Timers.Timer timeElapsed;                 //计时器
        int curTime = 0;                                        //当前游戏剩余时间
        //Button[] butArry = new Button[MAX_PIC];
        int[] butArry = new int[MAX_PIC];

        int[] picArry = new int[MAX_PIC];
        int last_click = -1;
        int cur_click = -1;
        //Button last_click = null;                             //记录上一次点击的按钮
        //Button curr_click = null;                             //当前点击的按钮
        //Hashtable btnVal;

        int pairPicCounts = -1;//记录当前副图的总共消除次数
        public int totalTurns = 1;//总共需要完成的轮数，默认是1次
        public bool started = false;//游戏开始与否
        Random r;
        private Thread receiveThread;
        string[] receiveStr;
        public bool keepalive = true;

        public delegate void NetworkProcess(object param, int type);    //处理网络消息
        public event NetworkProcess newtworkProcessor;                  //form传递函数，直接操作form中的控件



        public void InitLogic() {
            // 生成button对应的图像 和 统计总共消除的次数
            MyFormat.genPic(ref butArry);
            int[] tmp = (int[])butArry.Clone();
            pairPicCounts = MyFormat.countPairPic(tmp);

            keepalive = false;
            myClientSoc = new MyClientSoc();
        }

        public int GetPicType(int pos) {
            if (pos < 0 || pos >= butArry.Length)
                return -1;
            return butArry[pos];
        }

        public int[] PushButton(int pos) {
            if (last_click == -1) {
                last_click = pos;
                return null;
            } else {
                if (last_click != pos && butArry[last_click] == butArry[pos]) {
                    int ret = last_click;
                    last_click = -1;
                    int[] r = new int[2] {ret, pos};
                    return r;
                } else {
                    last_click = pos;
                    return null;
                }
            }
        }

        public int ClearAnPair() {
            pairPicCounts--;
            if (pairPicCounts == 0) {
                if (--totalTurns == 0) {
                    return 2;
                }
                return 1;
            }
            return 0;
        }

        //
        // 网络通信的功能
        //
        public void ConnectNet(string msg) {
            if (keepalive == false)
            {
	            //... someting to do
	            // 初始化网络
                keepalive = true;

	            myClientSoc.InitialSoc();


                // 启动单独的线程用于接收服务器端发送来的消息
                //receiveThread = new Thread(new ThreadStart(NetRuning));
                if (receiveThread == null)
                    receiveThread = new Thread(new ThreadStart(NetRuning));
                receiveThread.Start();

                myClientSoc.SendStr("login", msg);
            }
        }

        public void ConnectNet()
        {
            Random r = new Random();
            myClientName = "user" + r.Next(0, 1000);
            ConnectNet(myClientName);
        }

        public void CloseConn(string msg)
        {
            if (keepalive)
            {
	            myClientSoc.SendStr("exit", myClientName);
	            //... someting to do
	            //myClientSoc.CloseConn();

                //这里选择等待receive线程的结束，否则可能在断时间内，多次
                //receiveThread.Join();
                //myClientSoc.CloseConn();
                //receiveThread.Abort();

                //receiveThread.Join();
                
	            //keepalive = false;
                receiveThread = null;
            }
            
        }
        public void NetRuning() {
            while (keepalive) {
                // 处理网络连接
                if (myClientSoc.connected)
                {
                    receiveStr = myClientSoc.RecieveStr();
                    if (receiveStr != null && receiveStr[0] != null)
                    {
                        //pthread = new Thread(new ThreadStart(processStr));
                        //pthread.Start();
                        if (processStr())
                        {
                            break;
                        }
                    }
                }
            }

            keepalive = false;
            myClientSoc.CloseConn();
            //receiveThread.Abort();
        }

        //
        // 判断接收的数据，如果是退出消息且是本客户端发送的退出消息，
        // 则返回true，表示应该结束这个接收线程
        //
        public bool processStr()
        {
            if (newtworkProcessor == null) return true;

            switch (receiveStr[0])
            {
                case "list":
                    {
                        string[] param = new string[receiveStr.Length - 2];
                        for (int i = 1; i < receiveStr.Length - 1; i++)
                        {
                            param[i - 1] = receiveStr[i];
                        }
                        newtworkProcessor(param, 1);
                        break;
                    }
                case "xxxexit":
                    {
                        newtworkProcessor(receiveStr[1], 2);
                        if (receiveStr[1] == myClientName)
                            return true;
                        break;
                    }
                case "xxxjion":
                    {
                        newtworkProcessor(receiveStr[1], 3);
                        break;
                    }
            }
            return false;
        }
       // public delegate void PairBingoHandle(object sender, EventArgs e);//消除两张图代理
        //public event PairBingoHandle pairBingoEvent;//消除事件
       // public delegate void ListviewDeleg(string userName);


    }
}
