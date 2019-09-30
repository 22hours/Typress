﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TypressPacket;

namespace ControlBlock
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        DataPacket dp = new DataPacket();

        public static Socket socket;
        public static DataPacket packet = new DataPacket();
        public static byte[] getbyte = new byte[1024];
        public static byte[] setbyte = new byte[1024];
        public const int sPort = 5002;

        sealed class AllowAllAssemblyVersionsDeserializationBinder : System.Runtime.Serialization.SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                String currentAssembly = Assembly.GetExecutingAssembly().FullName;
                assemblyName = currentAssembly;
                typeToDeserialize = Type.GetType(string.Format("{0},{1}", typeName, assemblyName));
                return typeToDeserialize;
            }
        }
        public App()
        {

            TypressServerConnect();
            Thread.Sleep(2000);
            getDataPacketFromServer();
            Thread.Sleep(1000);
        }


        public void getDataPacketFromServer()
        {
            // server 에서 datapacket 수신하여 dp 변수에 넣으면 됩니다.
            // dp = server에서 받아온 Datapacekt;


            /*
            dp.Id = "winterlood";
            dp.Money = Int32.Parse("12000");
            dp.TotalUsage = Int32.Parse("12000");
            dp.OneWeekUsage = Int32.Parse("12000");
            dp.TwoWeekUsage = Int32.Parse("12000");
            dp.ThreeWeekUsage = Int32.Parse("12000");
            */

            socket.Receive(getbyte, 0,
                getbyte.Length, SocketFlags.None);

            dp = (DataPacket)ByteArrayToObject(getbyte);
        }

        public DataPacket getNowDataPacket()
        {
            // 각 window instance에서 해당 함수를 호출하여 DataPacket을 불러와 사용함
            return dp;
        }

        public static void TypressServerConnect()
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse("127.0.0.1");
                IPEndPoint serverEndPoint = new IPEndPoint(serverIP, sPort);

                socket = new Socket(
                    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverEndPoint);
            }
            catch (SocketException e)
            {
                MessageBox.Show("Server Stopped!");

            }
        }

        public static void ReceivePacketFromServer()
        {
            socket.Receive(getbyte, 0,
                getbyte.Length, SocketFlags.None);

            packet = (DataPacket)ByteArrayToObject(getbyte);
        }

        public static void SendPacketToServer(DataPacket tp)
        {
            setbyte = ObjectToByteArray(tp);
            socket.Send(setbyte, 0, setbyte.Length, SocketFlags.None);
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }
    }
}