M = {}

function M.register()
    ai.addBrain("orion", function(context)
        -- This is where the AI logic for the Orion brain would go.
        -- For now, it's just a placeholder.
        --

        context.moveRandomly()
    end)
end

return M
