return {
    name = "BucketHelper/BucketKillBarrier",
    placements = {
        name = "BucketKillBarrier",
        data = {
            width = 8, height = 8,
        }
    },
    sprite = function(room,entity) return {
        require('structs.drawable_rectangle').fromRectangle("fill",entity.x,entity.y,entity.width,entity.height,{0.251, 0.753, 0.816, 0.8})
    } end
}