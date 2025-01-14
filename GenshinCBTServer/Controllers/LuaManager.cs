﻿using GenshinCBTServer.Player;

using NLua;
using GenshinCBTServer.Protocol;
using GenshinCBTServer.Quests;
using GenshinCBTServer.Data;

namespace GenshinCBTServer.Controllers
{
    public static class LuaTableExtensions
    {
        public static int[] GetIntArray(this LuaTable table_, string name)
        {
            LuaTable table = table_[name] as LuaTable;

            if (table == null) return new int[0];
            int[] result = new int[table.Values.Count];
            int i = 0;

            // Iterate through LuaTable and convert values to integers
            foreach (var value in table.Values)
            {
                result[i++] = Convert.ToInt32(value);
            }

            return result;
        }
        public static Vector GetVector(this LuaTable table_, string name)
        {
            LuaTable table = table_[name] as LuaTable;

            if (table == null) return new Vector();
            float x = (float)(double)Convert.ToDouble(table["x"]);
            float y = (float)(double)Convert.ToDouble(table["y"]);
            float z = (float)(double)Convert.ToDouble(table["z"]);
            return new Vector() { X = x, Y = y, Z = z };
        }
        public static int[] ToIntArray(this LuaTable table)
        {
            if (table == null) return new int[0];
            // Create an integer array from the LuaTable
            int[] result = new int[table.Values.Count];
            int i = 0;

            // Iterate through LuaTable and convert values to integers
            foreach (var value in table.Values)
            {
                result[i++] = Convert.ToInt32(value);
            }
            return result;
        }
    }
    public class ScriptLib
    {
        public int currentGroupId;
        public YPlayer curClient;

        public void PrintLog(string msg)
        {
            Server.Print($"[LUA] {msg}");
        }
        public int GetGroupMonsterCount(YPlayer client)
        {
            return client.world.entities.FindAll(e => e.groupId == currentGroupId && e is GameEntityMonster).Count;
        }
        public int GetGadgetConfigId(int entityId)
        {
            GameEntity en = curClient.world.entities.Find(e => e.entityId == entityId);
            if (en != null)
            {
                return (int)en.configId;
            }
            return 0;
        }
        public int GetGroupMonsterCountByGroupId(YPlayer client, int groupId)
        {
            return client.world.entities.FindAll(e => e.groupId == groupId && e is GameEntityMonster).Count;
        }
        public int GetQuestState(YPlayer client, int target_eid, int questId)
        {
            GameQuest quest = client.GetQuestManager().GetQuestById((uint)questId);

            if(quest == null)
            {
                return (int)QuestState.NONE;
            }
            return (int)quest.state;
        }
        public int ShowReminder(YPlayer client, int reminderId)
        {
            DungeonShowReminderNotify ntf = new DungeonShowReminderNotify()
            {
                ReminderId = (uint)reminderId
            };
            client.SendPacket((uint)CmdType.DungeonShowReminderNotify, ntf);
            return 0;
        }
        // ScriptLib.StartPlatform(context, 28)
        public int StartPlatform(YPlayer client, int configId)
        {
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            if (entity == null) return 1;
            if (entity is not GameEntityGadget) return 1;
            GameEntityGadget platform = (GameEntityGadget)entity;
            if (platform.route_id <= 0) return 1;
            /*  PlatformStartRouteNotify ntf = new PlatformStartRouteNotify()
              {
                  EntityId = platform.entityId,
                  Platform = platform.asInfo().Gadget.Platform,
                  SceneTime = 9000
              };
              client.SendPacket((uint)CmdType.PlatformStartRouteNotify, ntf);*/
            ;
            return platform.StartPlatform() ? 0 : 2;
        }
        // ScriptLib.StopPlatform(context, 172)
        public int StopPlatform(YPlayer client, int configId)
        {
            /*   GameEntity entity = client.world.entities.Find(e => e.configId == configId);
               if (entity == null) return 1;
               if (entity is not GameEntityGadget) return 1;
               GameEntityGadget platform = (GameEntityGadget)entity;
               if (platform.route_id <= 0) return 1;
               PlatformStopRouteNotify ntf = new PlatformStopRouteNotify()
               {
                   EntityId = platform.entityId,
                   SceneTime = 9000
               };
               client.SendPacket((uint)CmdType.PlatformStopRouteNotify, ntf);*/
            //TODO
            return 0;
        }
        // ScriptLib.SetPlatformRouteId(context, 87, 20000009)
        public int SetPlatformRouteId(YPlayer client, int configId, int routeId)
        {
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            Server.Print($"[LUA] Call SetPlatformRouteId with {configId}, entity is {(entity != null ? "not null" : "null")}");
            if (entity == null) return 1;
            if (entity is not GameEntityGadget) return 1;
            GameEntityGadget platform = (GameEntityGadget)entity;
            if (platform.Route.RouteId == routeId) return 0;
            platform.Route.RouteId = routeId;
            platform.Route.StartIndex = 0;

            platform.route_id = (uint)routeId;
            //Implement tasks?

            //
            PlatformChangeRouteNotify ntf = new PlatformChangeRouteNotify()
            {
                EntityId = platform.entityId,
                Platform = platform.asInfo().Gadget.Platform,
            };
            client.SendPacket((uint)CmdType.PlatformChangeRouteNotify, ntf);
            if (platform.Route.IsStarted)
            {
                platform.Route.IsStarted = false;
                platform.StartPlatform();
            }
            else
            {
                // platform.StartPlatform();
            }


            /* PlatformStopRouteNotify stopNtf = new PlatformStopRouteNotify()
             {
                 EntityId = platform.entityId,
                 SceneTime = 9000
             };
             client.SendPacket((uint)CmdType.PlatformStopRouteNotify, stopNtf);
             PlatformStartRouteNotify startNtf = new PlatformStartRouteNotify()
             {
                 EntityId = platform.entityId,
                 Platform = platform.asInfo().Gadget.Platform,
                 SceneTime = 9000
             };
             client.SendPacket((uint)CmdType.PlatformStartRouteNotify, startNtf);*/
            return 0;
        }
        // ScriptLib.CreateGadget(context, { config_id = 1405 })
        public int CreateGadget(YPlayer client, LuaTable parameters)
        {
            int configId = (int)(long)parameters["config_id"];
            uint entityId = ((uint)ProtEntityType.ProtEntityGadget << 24) + (uint)client.random.Next();
            SceneGadget sceneGadget = client.world.currentBlock.groups.Find(g => g.id == currentGroupId).gadgets.Find(g => g.config_id == configId);
            if (sceneGadget == null) return 1;
            sceneGadget.pos.Y -= 1.0f;
            GameEntityGadget gadget = new GameEntityGadget(entityId, sceneGadget.gadget_id, new MotionInfo() { Pos = sceneGadget.pos, Rot = sceneGadget.rot });
            gadget.configId = sceneGadget.config_id;
            gadget.groupId = (uint)currentGroupId;
            gadget.owner = (uint)client.gamePeer;
            gadget.chest_drop = sceneGadget.chest_drop_id;
            gadget.state = (uint)sceneGadget.state;
            gadget.route_id = sceneGadget.route_id;
            gadget.gadgetType = sceneGadget.type;
            client.world.SpawnEntity(gadget, true, VisionType.VisionMeet);
            return 0;
        }
        // ScriptLib.PlayCutScene(context, 200201, 60)
        public int PlayCutScene(YPlayer client, int cutSceneId, int var2)
        {
            CutSceneBeginNotify ntf = new CutSceneBeginNotify()
            {
                CutsceneId = (uint)cutSceneId,
            };
            client.SendPacket((uint)CmdType.CutSceneBeginNotify, ntf);
            return 0;
        }
        // ScriptLib.KillEntityByConfigId(context, { config_id = 2603 })
        public int KillEntityByConfigId(YPlayer client, LuaTable parameters)
        {
            int configId = (int)(long)parameters["config_id"];
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            if (entity == null) return 1;
            entity.Die();
            return 0;
        }
        // ScriptLib.RefreshGroup(context, { group_id = 201011003, suite = 3 })
        public int RefreshGroup(YPlayer client, LuaTable parameters)
        {
            int groupId = (int)(long)parameters["group_id"];
            int suite = (int)(long)parameters["suite"];
            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == groupId);
            if (group == null) return 1;
            // TODO: Implement group refresh
            return 0;
        }
        // ScriptLib.UnlockForce(context, 2)
        public int UnlockForce(YPlayer client, int sceneId)
        {
            SceneForceUnlockNotify ntf = new SceneForceUnlockNotify()
            {
                ForceIdList = { (uint)sceneId },
                IsAdd = true
            };
            client.SendPacket((uint)CmdType.SceneForceUnlockNotify, ntf);
            return 0;
        }
        // ScriptLib.LockForce(context, 2)
        public int LockForce(YPlayer client, int sceneId)
        {
            SceneForceLockNotify ntf = new SceneForceLockNotify()
            {
                ForceIdList = { (uint)sceneId }
            };
            client.SendPacket((uint)CmdType.SceneForceLockNotify, ntf);
            return 0;
        }
        //ScriptLib.GetMonsterDieCount(context)
        public int GetMonsterDieCount(YPlayer client)
        {
            return (int)client.world.monsterDieCount;
        }
        // ScriptLib.UnlockMonsterHp(context, 15)
        public int UnlockMonsterHp(YPlayer client, int configId)
        {
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            if (entity == null) return 1;
            if (entity is not GameEntityMonster) return 1;
            GameEntityMonster monster = (GameEntityMonster)entity;
            monster.isHpLock = false;
            return 0;
        }
        // ScriptLib.LockMonsterHp(context, 15)
        public int LockMonsterHp(YPlayer client, int configId)
        {
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            if (entity == null) return 1;
            if (entity is not GameEntityMonster) return 1;
            GameEntityMonster monster = (GameEntityMonster)entity;
            monster.isHpLock = true;
            return 0;
        }
        // ScriptLib.CheckRemainGadgetCountByGroupId(context, {group_id = 133003196})
        public int CheckRemainGadgetCountByGroupId(YPlayer client, LuaTable parameters)
        {
            int groupId = (int)(long)parameters["group_id"];
            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == groupId);
            if (group == null) return 0;
            int count = 0;
            foreach (GameEntity entity in client.world.entities)
            {
                if (entity.groupId == groupId && entity is GameEntityGadget)
                {
                    count++;
                }
            }
            return count;
        }
        // ScriptLib.UnlockScenePoint(context, 40)
        public int UnlockScenePoint(YPlayer client, int scenePointId)
        {
            ScenePointUnlockNotify ntf = new ScenePointUnlockNotify()
            {
                SceneId = client.currentSceneId,
                PointList = { (uint)scenePointId }
            };
            client.UnlockTransPoint(client.currentSceneId, (uint)scenePointId, false);
            client.SendPacket((uint)CmdType.ScenePointUnlockNotify, ntf);
            return 0;
        }

        // ScriptLib.SetMonsterAIByGroup(context, 0, 26, 220011001)
        public int SetMonsterAIByGroup(YPlayer client, int is_enable, int configId, int group_id)
        {
            GameEntity entity = client.world.entities.Find(e => e.configId == configId && e.groupId == group_id);
            if (entity == null) return 1;
            if (entity is not GameEntityMonster) return 1;
            GameEntityMonster monster = (GameEntityMonster)entity;
            monster.IsAiOpen = Convert.ToBoolean(is_enable);
            return 0;
        }
        // ScriptLib.GetContextGroupId(context)
        public int GetContextGroupId(YPlayer client)
        {
            return currentGroupId;
        }
        // ScriptLib.ScenePlaySound(context, {play_pos = pos, sound_name = "DungeonSound1001", play_type= 2, is_broadcast = false })
        public int ScenePlaySound(YPlayer client, LuaTable parameters)
        {

            ScenePlayerSoundNotify ntf = new ScenePlayerSoundNotify()
            {
                PlayPos = parameters.GetVector("play_pos"),
                SoundName = (string)parameters["sound_name"],
                PlayType = (ScenePlayerSoundNotify.Types.PlaySoundType)(uint)(long)parameters["play_type"],
            };
            client.SendPacket((uint)CmdType.ScenePlayerSoundNotify, ntf);
            return 0;
        }
        public int ChangeGroupGadget(YPlayer client, LuaTable parameters)
        {
            int configId = (int)(long)parameters["config_id"];
            GadgetState state = (GadgetState)(int)(long)parameters["state"];

            Server.Print($"[LUA] Call ChangeGroupGadget with {configId},{state}");
            GameEntity entity = client.world.entities.Find(e => e.configId == configId);
            if (entity == null)
            {

                Server.Print($"[LUA] Entity not found with configId {configId}");
                return 0;
            }
            if (entity is not GameEntityGadget) return 0;
            Server.Print($"[LUA] Entity found with configId {configId}");
            GameEntityGadget gadget = (GameEntityGadget)entity;
            gadget.ChangeState(state);
            return 1;
        }

        // ScriptLib.BeginCameraSceneLook(context, { look_pos = pos, duration = 2, is_force = true, is_broadcast = false })
        public int BeginCameraSceneLook(YPlayer client, LuaTable parameters)
        {
            LuaTable lookPos = (LuaTable)parameters["look_pos"];
            float x = (float)(double)Convert.ToDouble(lookPos["x"]);
            float y = (float)(double)Convert.ToDouble(lookPos["y"]);
            float z = (float)(double)Convert.ToDouble(lookPos["z"]);
            float duration = (float)(double)Convert.ToDouble(parameters["duration"]);
            bool isForce = (bool)parameters["is_force"];
            bool isBroadcast = (bool)parameters["is_broadcast"];
            Server.Print($"[LUA] Call BeginCameraSceneLook with {x},{y},{z},{duration},{isForce},{isBroadcast}");
            BeginCameraSceneLookNotify ntf = new BeginCameraSceneLookNotify()
            {
                LookPos = new Vector()
                {
                    X = x,
                    Y = y,
                    Z = z
                },
                Duration = duration,
                IsForce = isForce
            };
            // GOD THIS IS ANNOYING
            // client.SendPacket((uint)CmdType.BeginCameraSceneLookNotify, ntf);
            return 1;
        }
        public int SetGadgetStateByConfigId(YPlayer client, int configId, int gadgetState)
        {
            Server.Print($"[LUA] SetGadgetStateByConfigId with {configId},{gadgetState}");
            GameEntity entity = client.world.entities.Find(e => e.configId == configId && e.groupId == currentGroupId);
            if (entity == null) return 1;
            if (!(entity is GameEntityGadget))
            {
                return 1;
            }
            GameEntityGadget gadget = (GameEntityGadget)entity;
            gadget.ChangeState((GadgetState)gadgetState);
            return 0;

        }

        public int SetGroupGadgetStateByConfigId(YPlayer client, int groupId, int configId, int gadgetState)
        {
            Server.Print($"[LUA] CallSetGroupGadgetStateByConfigId with {configId},{gadgetState}");
            GameEntity entity = client.world.entities.Find(e => e.configId == configId && e.groupId == groupId);
            if (entity == null) return 1;
            if (!(entity is GameEntityGadget))
            {
                return 1;
            }
            GameEntityGadget gadget = (GameEntityGadget)entity;
            gadget.ChangeState((GadgetState)gadgetState);
            return 0;

        }
        // ScriptLib.ChangeGroupVariableValue(context, "var_MONSTER_NUM", -1)
        public int ChangeGroupVariableValue(YPlayer client, string variable, int value)
        {
            Server.Print($"[LUA] Call ChangeGroupVariableValue with {variable},{value}");
            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == currentGroupId);
            if (group == null) return 1;
            foreach (Variable var in group.variables)
            {
                if (var.name == variable)
                {
                    var.value = value;
                    return 0;
                }
            }
            return 1;
        }
        public int SetGroupVariableValue(YPlayer client, string variable, int value)
        {

            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == currentGroupId);
            if (group == null) return 0;
            foreach (Variable var in group.variables)
            {
                if (var.name == variable)
                {
                    var.value = value;
                    Server.Print($"[LUA] Call SetGroupVariableValue with {variable} set value: {var.value}");
                    return 0;
                }
            }
            group.variables.Add(new Variable() { name = variable, value = value });
            Server.Print($"[LUA] Call SetGroupVariableValue with {variable} new value: {value}");
            return 0;
        }
        // ScriptLib.GetGroupVariableValue(context, "var_MONSTER_NUM")
        public int GetGroupVariableValue(YPlayer client, string variable)
        {

            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == currentGroupId);
            if (group == null) return 0;
            foreach (Variable var in group.variables)
            {
                if (var.name == variable)
                {
                    Server.Print($"[LUA] Call GetGroupVariableValue with {variable} value: {var.value}");
                    return var.value;
                }
            }
            return 0;
        }
        // ScriptLib.CreateVariable(context, "int", 3)
        public int CreateVariable(YPlayer client, string variable, int value)
        {
            Server.Print($"[LUA] Call CreateVariable with {variable},{value}");
            Variable var = new Variable()
            {
                name = variable,
                value = value
            };
            SceneGroup group = client.world.currentBlock.groups.Find(g => g.id == currentGroupId);
            if (group == null) return 1; // idk if should be value
            group.variables.Add(var);
            return 0;
        }
        public int KillGroupEntity(YPlayer client, LuaTable table)
        {
            uint groupId = (uint)(long)table["group_id"];
            int[] gadgets = (table["gadgets"] as LuaTable).ToIntArray();
            int[] monsters = (table["monsters"] as LuaTable).ToIntArray();
            List<GameEntity> entities = client.world.entities.FindAll(e => e.groupId == groupId && gadgets.Contains((int)e.configId) || monsters.Contains((int)e.configId));
            client.world.KillEntities(entities);
            return 1;
        }
        public int GetRegionEntityCount(YPlayer client, LuaTable table)
        {
            // logger.debug("[LUA] Call GetRegionEntityCount with {}", printTable(table));
            int regionId = (int)(long)table["region_eid"];

            int entityType = 0;
            if (table["entity_type"] != null)
            {
                entityType = (int)(long)table["entity_type"];
            }
            if (entityType == (int)EntityType.Avatar)
            {
                if (client.inRegions.Contains((uint)regionId))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return client.world.entities.FindAll(e => e.inRegions.Contains((uint)regionId)).Count;
            }

        }
    }
    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Debug = 3
    }
    public class ScriptArgs
    {
        public int param1;
        public int param2;
        public int param3;
        public int source_eid; // Source entity
        public int target_eid;
        public int group_id;
        public string source; // source string, used for timers
        public int type;

        public ScriptArgs(int groupId, int eventType, int param1, int param2)
        {
            this.group_id = groupId;
            this.type = eventType;
            this.param1 = param1;
            this.param2 = param2;
        }
        public ScriptArgs(int groupId, int eventType)
        {
            this.group_id = groupId;
            this.type = eventType;
        }
        public ScriptArgs(int groupId, int eventType, int param1)
        {
            this.group_id = groupId;
            this.type = eventType;
            this.param1 = param1;
        }

        public object toTable()
        {
            return new { param1 = param1, param2 = param2, param3 = param3, source_eid = source_eid, target_eid = target_eid, type = type, group_id = group_id, source = source };
        }
    }
    public class LuaManager
    {
        public static List<GroupTrigger> errorTriggers = new();

        public static void executeTrigger(YPlayer client, GroupTrigger trigger, ScriptArgs args, SceneGroup group = null)
        {
            if (group == null)
            {
                group = client.world.currentBlock.groups.Find(g => g.id == args.group_id);
            }

            if (group != null)
            {
                using (Lua groupLua = new Lua())
                {
                    ScriptLib scriptLib = new();
                    scriptLib.curClient = client;
                    scriptLib.currentGroupId = (int)group.id;
                    groupLua["ScriptLib"] = scriptLib;
                    groupLua["context_"] = client;
                    groupLua["evt_"] = args.toTable();

                    groupLua.DoString(group.luaFile.Replace("ScriptLib.", "ScriptLib:"));

                    string luaScript = @$"
                              
                                if {trigger.conditionLua}(context_, evt_) then
                                    {trigger.actionLua}(context_, evt_)
                                end
                            
                        ";
                    try
                    {
                        if (trigger.conditionLua.Length == 0)
                        {
                            luaScript = @$"
                            {trigger.actionLua}(context_, evt_)
                            ";
                        }
                        groupLua.DoString(luaScript);
                        Server.Print("[LUA] Executed successfully LUA of type: " + ((EventType)trigger.eventType).ToString());
                    }
                    catch (Exception ex)
                    {
                        Server.Print("[LUA] Error occured in executing Trigger Lua " + ex.Message);
                        errorTriggers.Add(trigger);
                    }
                }
            }
        }
        public static void executeClientTriggerLua(YPlayer client, SceneGroup group, ScriptArgs args)
        {
            if (group == null) return;
            GameEntity en = client.world.entities.Find(e => e.entityId == args.source_eid);
            if (en == null) return;
            List<GroupTrigger> triggers = group.triggers.FindAll(t => t.eventType == (int)args.type);

            if (triggers.Count > 0)
            {
                using (Lua groupLua = new Lua())
                {
                    ScriptLib scriptLib = new();
                    scriptLib.curClient = client;
                    scriptLib.currentGroupId = (int)group.id;
                    groupLua["ScriptLib"] = scriptLib;
                    groupLua["context_"] = client;
                    groupLua["evt_"] = args;
                    groupLua.DoString(group.luaFile.Replace("ScriptLib.", "ScriptLib:"));

                    foreach (GroupTrigger trigger in triggers)
                    {
                        string luaScript = @$"
                               
                                {trigger.actionLua}(context_, evt_)
                            
                        ";
                        try
                        {
                            groupLua.DoString(luaScript);
                            Server.Print("Executed successfully LUA of type: " + ((EventType)trigger.eventType).ToString());
                        }
                        catch (Exception ex)
                        {
                            Server.Print("Error occured in LUA " + ex.Message);

                        }
                        // Execute the Lua script
                    }
                }
            }
        }
        public static void executeTriggersLua(YPlayer client, SceneGroup group, ScriptArgs args)
        {
            if (group == null) return;
            List<GroupTrigger> triggers = group.triggers.FindAll(t => t.eventType == (int)args.type && !errorTriggers.Contains(t));

            if (triggers.Count > 0)
            {
                foreach (GroupTrigger trigger in triggers)
                {

                    executeTrigger(client, trigger, args, group);
                }
            }
        }
    }
}
