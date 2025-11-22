local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local wdAndWater = {}

wdAndWater.name = "BucketHelper/WdAndWater"
wdAndWater.depth = -9999
wdAndWater.nodeLimits = {1, 1}
wdAndWater.nodeLineRenderType = "line"

wdAndWater.placements = {
    name = "WdAndWater",
    data = {
        width = 16,
        height = 16,
        waterPersistent = false
    }
}

local function addNodeSprites(sprites, entity, wdTexture, nodeX, nodeY)
    local nodeWdSprite = drawableSprite.fromTexture(wdTexture, entity)

    nodeWdSprite:setPosition(nodeX, nodeY)
    nodeWdSprite:setJustification(0.5, 1)

    table.insert(sprites, nodeWdSprite)
end

local function addBlockSprites(sprites, entity, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x, y, width, height, { 0,0,1,0.5 })

    table.insert(sprites, rectangle:getDrawableSprite())
end

function wdAndWater.sprite(room, entity)
    local sprites = {}

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 16, entity.height or 16
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y

    local centerX, centerY = x + halfWidth, y + halfHeight
    local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight

    addNodeSprites(sprites, entity, "BucketHelper/water_dispenser/insert", nodeX, nodeY)
    addBlockSprites(sprites, entity, x, y, width, height)

    return sprites
end

function wdAndWater.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y
    local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight
    
    local wdSprite = drawableSprite.fromTexture("BucketHelper/water_dispenser/insert", entity)
    local wdWidth, wdHeight = wdSprite.meta.width, wdSprite.meta.height

    local mainRectangle = utils.rectangle(x, y, width, height)
    local nodeRectangle = utils.rectangle(nodeX - math.floor(wdWidth / 2), nodeY - wdHeight, wdWidth, wdHeight)

    return mainRectangle, {nodeRectangle}
end

return wdAndWater