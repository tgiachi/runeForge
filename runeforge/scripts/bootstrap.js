ai.addBrain("orion", (context) => {
    context.moveRandomly();
});

mapGen.addStep("terrain_maker", (context) => {
    const wall = context.asInputs().wall;
    const floor = context.asInputs().floor;

    logger.info("Generating terrain...");
    const terrain = context.getOutput("terrain");

    for (let x = 0; x < context.width; x++) {
        for (let y = 0; y < context.height; y++) {
            if (terrain[(x, y)] == true) {
                var wallTile = tiles.create(wall);

                var wallGameObject = entities.createTerrain(x, y, wallTile, wall, false, false);
                context.applyTerrain(x, y, wallGameObject);
            } else {
                var floorTile = tiles.create(floor);
                var floorGameObject = entities.createTerrain(x, y, floorTile, floor, true, true);
                context.applyTerrain(x, y, floorGameObject);
            }
        }
    }

    return context;
});
