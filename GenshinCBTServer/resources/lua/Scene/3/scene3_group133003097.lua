--================================================================
-- 
-- 配置
-- 
--================================================================

-- 怪物
monsters = {
	{ config_id = 496, monster_id = 21020101, pos = { x = 2276.6, y = 266.5, z = -1644.3 }, rot = { x = 0.0, y = 218.1, z = 0.0 }, level = 7, drop_id = 303103, disableWander = true, affix = { 1002 }, pose_id = 401 },
	{ config_id = 497, monster_id = 21010501, pos = { x = 2248.5, y = 270.9, z = -1709.5 }, rot = { x = 0.0, y = 284.6, z = 0.0 }, level = 7, drop_id = 303003, disableWander = true },
	{ config_id = 498, monster_id = 21010501, pos = { x = 2223.3, y = 263.9, z = -1692.7 }, rot = { x = 0.0, y = 193.4, z = 0.0 }, level = 7, drop_id = 303003, disableWander = true }
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
	suite = 1,
	rand_suite = true,
	npcs = { }
}

--================================================================
-- 
-- 小组配置
-- 
--================================================================

suites = {
	{
		-- suite_id = 0,
		-- description = ,
		monsters = { 496, 497, 498 },
		gadgets = { },
		regions = { },
		triggers = { },
		rand_weight = 100
	}
}

--================================================================
-- 
-- 触发器
-- 
--================================================================