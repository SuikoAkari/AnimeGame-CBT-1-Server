﻿
using GenshinCBTServer.Network;
using GenshinCBTServer.Protocol;
using Google.Protobuf;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GenshinCBTServer.ENet;
using static PlayerQuitDungeonReq.Types;

namespace GenshinCBTServer
{
    public class Server
    {
        public class HandlerAttribute : Attribute
        {

            public CmdType CmdId { get; set; }
            public HandlerAttribute(CmdType cmdID)
            {
                this.CmdId = cmdID;
            }

            public delegate void HandlerDelegate(Client client, int cmdId, Network.Packet packet);
        }
        public static List<Client> clients = new List<Client>();
        public IntPtr server;
        public static MongoClient databaseClient = null;
        public static Dispatch dispatch;
        public static ResourceManager resourceManager;
        public static IMongoDatabase GetDatabase()
        {
            return databaseClient.GetDatabase("GenshinCBT1");
        }
        public static ResourceManager getResources()
        {
            return resourceManager;
        }
        public void Start()
        {
           {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();

                foreach (var type in types)
                {

                    NotifyManager.AddReqGroupHandler(type);

                }

                NotifyManager.Init();
            }

            enet_initialize();

            ENetAddress address = new ENetAddress();

            Print("Connecting to MongoDB...");
            databaseClient = new MongoClient("mongodb://localhost:27017");
            if(databaseClient == null)
            {
                Print("An error occured while trying to connect to MongoDB server");
                return;
            }
            Print("Loading resources");
            resourceManager=new ResourceManager();  
            resourceManager.Load();
            Print("Resources loaded");
             enet_address_set_host(ref address, "127.0.0.1");
            address.port = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)22102); 
            //address.host = 0;
            Print($"{address.host}:{address.port}");
           
            server = enet_host_create(ref address, 16,0,0,0,0);
           
            if (server == IntPtr.Zero)
            {
                Print("An error occurred while trying to create an ENet server host.");
                ENet.enet_deinitialize();
                return;
            }
            enet_host_compress_with_range_coder(server);
           // enet_host_set_checksum(server, new ENetChecksumCallback(ENet.enet_crc32)); //Non esiste nel .dll
            Print($"Gameserver started");
            new Thread(new ThreadStart(PeerHandle)).Start();
            new Thread(new ThreadStart(DispatchServer)).Start();
        }
        public void DispatchServer()
        {
            dispatch = new Dispatch();
            dispatch.Start();
        }
        public void PeerHandle()
        {

            ENet.ENetEvent netEvent;
            while (true)
            {


                //server.CheckEvents(out netEvent);

                if (enet_host_service(server, out netEvent, 20) > 0)
                {
                   // Print("Net event " + netEvent.type);
                }




                switch (netEvent.type)
                {
                    case ENet.EventType.None:
                        
                        break;
                    case ENet.EventType.Connect:
                        Print("Client connected - ID: " + netEvent.peer + ", IP: " + netEvent.peer);
                        Client client = new Client(netEvent.peer);
                        clients.Add(client);

                        break;

                    case ENet.EventType.Disconnect:
                        Print("Client disconnected - ID: " + netEvent.peer + ", IP: " + netEvent.peer);
                        break;



                    case ENet.EventType.Receive:
                        
                            ENetPacket enetPacket = Marshal.PtrToStructure<ENetPacket>(netEvent.packet);
                          //  Print("Packet received from - ID: " + netEvent.peer + ", IP: " + netEvent.peer + ", Channel ID: " + netEvent.channelID + ", Data length: " + enetPacket.data);
                            Packet genshinPacket = Packet.Read(enetPacket);
                           // Print($"Received from client: {genshinPacket.cmdId} ({((CmdType)genshinPacket.cmdId).ToString()})");
                         Server.Print($"[client->server] {((CmdType)genshinPacket.cmdId).ToString()}");
                        NotifyManager.Notify(clients.Find(client => client.peer == netEvent.peer), (CmdType)genshinPacket.cmdId, genshinPacket);
                       

                        break;
                }

                //enet_host_flush(server);
            }
        }
        public static void Print(string text)
        {
            Console.WriteLine("[Server] " + text);
        }
    }
}