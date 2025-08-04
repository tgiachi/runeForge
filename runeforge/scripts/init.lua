local brains = require("ai")
local mapGens = require("mapgens")

mapGens.addMapGens()

brains.register()

function bootstrap()
    logger.info("Test 123 ")

    generateName = names.generateName("")

    logger.info("Generated name: " .. generateName)
end
