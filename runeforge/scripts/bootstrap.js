ai.addBrain("orion", (context) => {
    context.moveRandomly();
});

mapGen.addStep("terrain_maker", (context) => {
    const wall = context.inputs["wall"];
    const floor = context.inputs["floor"];

    const terrain = context.getOutput("terrain");

    for (let x = 0; x < context.width; x++) {
        for (let y = 0; y < context.height; y++) {
            if (terrain[(x, y)] == true) {
            } else {
            }
        }
    }

    return context;
});
