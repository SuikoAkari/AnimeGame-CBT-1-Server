--================================================================
-- 
-- 配置
-- 
--================================================================

-- 怪物
monsters = {
	{ config_id = 17, monster_id = 21020101, pos = { x = 2320.8, y = 218.6, z = -71.7 }, rot = { x = 0.0, y = 107.7, z = 0.0 }, level = 20, disableWander = true },
	{ config_id = 18, monster_id = 21030301, pos = { x = 2316.3, y = 218.2, z = -73.2 }, rot = { x = 0.0, y = 101.0, z = 0.0 }, level = 20 },
	{ config_id = 19, monster_id = 21010601, pos = { x = 2317.3, y = 218.2, z = -68.1 }, rot = { x = 0.0, y = 106.9, z = 0.0 }, level = 20 }
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
		monsters = { },
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