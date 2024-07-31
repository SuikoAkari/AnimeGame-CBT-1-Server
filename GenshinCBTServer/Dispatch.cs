﻿using Google.Protobuf.WellKnownTypes;
using HttpServerLite;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshinCBTServer
{
    public class Dispatch
    {
        public class Account()
        {
            public object _id;
            public string account;
            public string md5password;
            public string token;
        }
        public Webserver server;
        public static void Print(string text)
        {
            Console.WriteLine("[Dispatch] " + text);
        }
        public void Start()
        {
            server = new Webserver("127.0.0.1", 8099, false, null, null, DefaultRoute);
            server.Settings.Headers.Host = "https://localhost:8099";
            server.Events.ResponseSent += Events_ResponseSent;
            server.Events.RequestReceived += Events_RequestReceived;
            server.Events.RequestDenied += Events_RequestDenied;
            server.Start();
            Print("Dispatch started on 8099");
        }

        private void Events_RequestDenied(object? sender, RequestEventArgs e)
        {
            Print("Denied " + e.Url);
        }

        private void Events_RequestReceived(object? sender, RequestEventArgs e)
        {
            Print("Requested " + e.Url );
        }

        private void Events_ResponseSent(object? sender, ResponseEventArgs e)
        {
            Print("Sent " + e.Url + " status: " + e.StatusCode);
        }

        static async Task DefaultRoute(HttpContext ctx)
        {
            byte[] resp;
            string curVer = "138541";
            resp = System.Text.Encoding.UTF8.GetBytes(curVer);
            if (ctx.Request.Url.Full.Contains("cur_version.txt"))
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength = resp.Length;

                ctx.Response.SendAsync(resp);
            }
            else
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentLength = 0;

                ctx.Response.SendAsync("");
            }
           
           
           // await ctx.Response.SendAsync(resp);
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/query_region_list")]
        public static async Task query_region_list(HttpContext ctx)
        {
            string resp = "{}";

            QueryRegionListHttpRsp queryRegionListHttpRsp = new QueryRegionListHttpRsp()
            {
                Retcode = 0,
                ClientCustomConfig = "{ \"sdkenv\": \"2\", \"visitor\": False, \"devicelist\": None }",
                RegionList = { 
                    new RegionSimpleInfo() { 
                        DispatchUrl="http://localhost:8099/query_cur_region",
                        Name="cbtServer",
                        Title="Closed Beta 1 Server",
                        Type="DEV_PUBLIC"
                    } 
                }
            };
            resp = queryRegionListHttpRsp.ToString();
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);
            
        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/query_cur_region")]
        public static async Task query_cur_region(HttpContext ctx)
        {
            string resp = "{}";

            QueryCurrRegionHttpRsp rsp = new QueryCurrRegionHttpRsp()
            {
                Retcode = 0,
                RegionInfo = new()
                {
                    ResourceUrl = "http://localhost:8099/game_res",
                    DataUrl = "http://localhost:8099/design_data",
                    GateserverIp = "127.0.0.1",
                    GateserverPort = 22102,
                    ResVersion = 138541,
                    DataVersion = 138541,
                    RegionCustomConfig = "{}",
                    ClientCustomConfig="{}",
                    HandbookUrl= "https://cdn.discordapp.com/attachments/441109559004889098/1043690740397641748/unknown.png",
                    BulletinUrl= "https://cdn.discordapp.com/attachments/441109559004889098/1043690740397641748/unknown.png",
                    FeedbackUrl= "https://cdn.discordapp.com/attachments/441109559004889098/1043690740397641748/unknown.png",
                }
            };
            resp = rsp.ToString();
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);

        }
        

       

        [StaticRoute(HttpServerLite.HttpMethod.GET, "/game_res/cur_version.txt")]
        public static async Task cur_version(HttpContext ctx)
        {
            byte[] resp;



            string curVer = "138541";
            resp = System.Text.Encoding.UTF8.GetBytes(curVer);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            await ctx.Response.SendAsync(resp);

        }

        [StaticRoute(HttpServerLite.HttpMethod.GET, "/design_data/cur_version.txt")]
        public static async Task design_data_cur_version(HttpContext ctx)
        {
            byte[] resp;



            string curVer = "138541";
            resp = System.Text.Encoding.UTF8.GetBytes(curVer);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            await ctx.Response.SendAsync(resp);

        }
        [StaticRoute(HttpServerLite.HttpMethod.GET, "/sdk/login")]
        public static async Task sdk_login(HttpContext ctx)
        {
            string resp = "{\"retcode\": 2003}";
            try
            {
                IMongoCollection<Account> accounts = Server.GetDatabase().GetCollection<Account>("accounts");
                foreach (Account account in accounts.Find(new BsonDocument()).ToList())
                {
                    if (account.account == ctx.Request.Query.Elements[0] && account.md5password == ctx.Request.Query.Elements[1])
                    {
                        resp = "{\"retcode\": 0,\"data\": { \"uid\": \"1\", \"token\": \"" + account.token + "\",\"email\": \"" + account.account + "\"}}";
                    }
                }
            }
            catch(Exception e)
            {
                Print(e.Message);
            }
            
           
            
            
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentLength = resp.Length;
            ctx.Response.ContentType = "application/json";

            await ctx.Response.SendAsync(resp);

        }
    }
}