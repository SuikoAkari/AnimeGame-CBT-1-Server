--================================================================
-- 
-- 配置
-- 
--================================================================

-- 怪物
monsters = {
	{ config_id = 29, monster_id = 20103026, pos = { x = 1146.7, y = 68.4, z = 276.0 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 20, drop_id = 200020 },
	{ config_id = 31, monster_id = 20103026, pos = { x = 1148.0, y = 69.7, z = 267.2 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 20, drop_id = 200020 },
	{ config_id = 390, monster_id = 21010401, pos = { x = 1151.3, y = 69.0, z = 270.7 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 20, drop_id = 200020 },
	{ config_id = 391, monster_id = 21010601, pos = { x = 1151.3, y = 68.4, z = 275.2 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 20, drop_id = 200020 },
	{ config_id = 392, monster_id = 21010401, pos = { x = 1147.6, y = 69.0, z = 272.2 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 20, drop_id = 200020 },
	{ config_id = 235, monster_id = 28020101, pos = { x = 1165.1, y = 68.4, z = 268.2 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 1 },
	{ config_id = 236, monster_id = 28020101, pos = { x = 1132.8, y = 68.2, z = 279.1 }, rot = { x = 0.0, y = 0.0, z = 0.0 }, level = 1 }
}

-- NPC
npcs = {
}

-- 装置
gadgets = {
}

-- 区域
regions = {
}

-- 触发器
triggers = {
}

-- 变量
variables = {
}

--================================================================
-- 
-- 初始化配置
-- 
--================================================================

-- 初始化时创建
init_config = {
	monsters = { 29, 31, 235, 236, 390, 391, 392 },
	npcs = { },
	gadgets = { },
	regions = { },
	triggers = { }
}

--================================================================
-- 
-- 触发器
-- 
--================================================================