local function addMapGens()
    mapGen.addStep("terrain_maker", function(context)
        local wallId = context.input.wall

        local floorId = context.input.floor

        local terrainGrid = context.getOutput("TerrainGrid")
    end)
end

return {
    addMapGens = addMapGens,
}
