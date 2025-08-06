/**
 * Run v0.0.83.0 JavaScript API TypeScript Definitions
 * Auto-generated documentation on 2025-08-06 16:50:32
 **/

// Constants

/**
 * VERSION constant 
 * ""0.0.83.0""
 */
declare const VERSION: string;


/**
 * LoggerModule module
 */
declare const logger: {
    /**
     * Log message with info level
     * @param message string
     * @param args any[]
     */
    info(message: string, args: any[]): void;
    /**
     * Log message with warning level
     * @param message string
     * @param args any[]
     */
    warning(message: string, args: any[]): void;
    /**
     * Log message with error level
     * @param message string
     * @param args any[]
     */
    error(message: string, args: any[]): void;
    /**
     * Log message with debug level
     * @param message string
     * @param args any[]
     */
    debug(message: string, args: any[]): void;
};

/**
 * ActionsModule module
 */
declare const actions: {
    /**
     * Add new action
     * @param name string
     * @param action (arg: any) => void
     */
    addAction(name: string, action: (arg: any) => void): void;
    /**
     * Execute action
     * @param name string
     * @param parameter any
     */
    executeAction(name: string, parameter?: any): void;
};

/**
 * RandomModule module
 */
declare const random: {
    /**
     * Get random integer between min and max
     * @param min number
     * @param max number
     * @returns number
     */
    int(min: number, max: number): number;
    /**
     * Get random boolean value
     * @returns boolean
     */
    bool(): boolean;
    /**
     * Roll a dice expression
     * @param dice string
     * @returns number
     */
    roll(dice: string): number;
};

/**
 * NamesModule module
 */
declare const names: {
    /**
     * Generate new name
     * @param type string
     * @returns string
     */
    generateName(type?: string): string;
};

/**
 * AiModule module
 */
declare const ai: {
    /**
     * Add brain
     * @param name string
     * @param action (arg: IAiContext) => void
     */
    addBrain(name: string, action: (arg: IAiContext) => void): void;
};

/**
 * MapGenModule module
 */
declare const mapGen: {
    /**
     * add step to map generator
     * @param name string
     * @param func (arg: IMapGeneratorContext) => IMapGeneratorContext
     */
    addStep(name: string, func: (arg: IMapGeneratorContext) => IMapGeneratorContext): void;
};

/**
 * TilesModule module
 */
declare const tiles: {
    /**
     * create tile from tag or tileId
     * @param tileOrTag string
     * @returns IColoredGlyph
     */
    create(tileOrTag: string): IColoredGlyph;
};

/**
 * EntitiesModule module
 */
declare const entities: {
    /**
     * Create entity terrain
     * @param x number
     * @param y number
     * @param coloredGlyph IColoredGlyph
     * @param tileId string
     * @param isWalkable boolean
     * @param isTransparent boolean
     * @returns ITerrainGameObject
     */
    createTerrain(x: number, y: number, coloredGlyph: IColoredGlyph, tileId: string, isWalkable: boolean, isTransparent: boolean): ITerrainGameObject;
};


/**
 * Generated enum for SadConsole.Mirror
 */
export enum mirror {
    None = 0,
    Vertical = 1,
    Horizontal = 2,
}

/**
 * Generated enum for SadConsole.FocusBehavior
 */
export enum focusBehavior {
    Set = 0,
    Push = 1,
    None = 2,
}

/**
 * Generated enum for SadRogue.Primitives.Distance+Types
 */
export enum types {
    Manhattan = 0,
    Euclidean = 1,
    Chebyshev = 2,
}

/**
 * Generated enum for SadConsole.AnimatedScreenObject+AnimationState
 */
export enum animationState {
    Stopped = 0,
    Playing = 1,
    Restarted = 2,
    Finished = 3,
    Activated = 4,
    Deactivated = 5,
}


/**
 * Generated interface for Runeforge.Engine.Contexts.AiContext
 */
interface IAiContext {
    /**
     * player
     */
    player: IPlayerGameObject;
    /**
     * self
     */
    self: INpcGameObject;
}

/**
 * Generated interface for Runeforge.Engine.Contexts.MapGeneratorContext
 */
interface IMapGeneratorContext {
    /**
     * step
     */
    step: number;
    /**
     * name
     */
    name: string;
    /**
     * width
     */
    width: number;
    /**
     * height
     */
    height: number;
    /**
     * outputs
     */
    outputs: { [key: string]: any };
    /**
     * inputs
     */
    inputs: { [key: string]: any };
    /**
     * map
     */
    map: IGameMap;
}

/**
 * Generated interface for SadConsole.ColoredGlyph
 */
interface IColoredGlyph {
    /**
     * decorators
     */
    decorators: ICellDecorator[];
    /**
     * foreground
     */
    foreground: IColor;
    /**
     * background
     */
    background: IColor;
    /**
     * glyph
     */
    glyph: number;
    /**
     * mirror
     */
    mirror: mirror;
    /**
     * glyphCharacter
     */
    glyphCharacter: any;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isDirty
     */
    isDirty: boolean;
}

/**
 * Generated interface for Runeforge.Engine.GameObjects.TerrainGameObject
 */
interface ITerrainGameObject {
    /**
     * darkAppearance
     */
    darkAppearance: IColoredGlyph;
    /**
     * tileId
     */
    tileId: string;
    /**
     * trueAppearance
     */
    trueAppearance: ITerrainAppearance;
    /**
     * lastSeenAppearance
     */
    lastSeenAppearance: ITerrainAppearance;
    /**
     * appearance
     */
    appearance: ITerrainAppearance;
    /**
     * position
     */
    position: IPoint;
    /**
     * isWalkable
     */
    isWalkable: boolean;
    /**
     * isTransparent
     */
    isTransparent: boolean;
    /**
     * id
     */
    id: any;
    /**
     * layer
     */
    layer: number;
    /**
     * currentMap
     */
    currentMap: IMap;
    /**
     * goRogueComponents
     */
    goRogueComponents: any;
}

/**
 * Generated interface for Runeforge.Engine.GameObjects.PlayerGameObject
 */
interface IPlayerGameObject {
    /**
     * name
     */
    name: string;
    /**
     * isDead
     */
    isDead: boolean;
    /**
     * allComponents
     */
    allComponents: any;
    /**
     * id
     */
    id: any;
    /**
     * layer
     */
    layer: number;
    /**
     * currentMap
     */
    currentMap: IMap;
    /**
     * goRogueComponents
     */
    goRogueComponents: any;
    /**
     * isTransparent
     */
    isTransparent: boolean;
    /**
     * isWalkable
     */
    isWalkable: boolean;
    /**
     * zindex
     */
    zindex: number;
    /**
     * isDirty
     */
    isDirty: boolean;
    /**
     * usePixelPositioning
     */
    usePixelPositioning: boolean;
    /**
     * appearanceSingle
     */
    appearanceSingle: ISingleCell;
    /**
     * appearanceSurface
     */
    appearanceSurface: IAnimated;
    /**
     * isSingleCell
     */
    isSingleCell: boolean;
    /**
     * sortOrder
     */
    sortOrder: any;
    /**
     * children
     */
    children: IScreenObjectCollection;
    /**
     * parent
     */
    parent: any;
    /**
     * position
     */
    position: IPoint;
    /**
     * absolutePosition
     */
    absolutePosition: IPoint;
    /**
     * ignoreParentPosition
     */
    ignoreParentPosition: boolean;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isEnabled
     */
    isEnabled: boolean;
    /**
     * isFocused
     */
    isFocused: boolean;
    /**
     * focusedMode
     */
    focusedMode: focusBehavior;
    /**
     * isExclusiveMouse
     */
    isExclusiveMouse: boolean;
    /**
     * useKeyboard
     */
    useKeyboard: boolean;
    /**
     * useMouse
     */
    useMouse: boolean;
    /**
     * sadComponents
     */
    sadComponents: any;
}

/**
 * Generated interface for Runeforge.Engine.GameObjects.NpcGameObject
 */
interface INpcGameObject {
    /**
     * name
     */
    name: string;
    /**
     * isDead
     */
    isDead: boolean;
    /**
     * allComponents
     */
    allComponents: any;
    /**
     * id
     */
    id: any;
    /**
     * layer
     */
    layer: number;
    /**
     * currentMap
     */
    currentMap: IMap;
    /**
     * goRogueComponents
     */
    goRogueComponents: any;
    /**
     * isTransparent
     */
    isTransparent: boolean;
    /**
     * isWalkable
     */
    isWalkable: boolean;
    /**
     * zindex
     */
    zindex: number;
    /**
     * isDirty
     */
    isDirty: boolean;
    /**
     * usePixelPositioning
     */
    usePixelPositioning: boolean;
    /**
     * appearanceSingle
     */
    appearanceSingle: ISingleCell;
    /**
     * appearanceSurface
     */
    appearanceSurface: IAnimated;
    /**
     * isSingleCell
     */
    isSingleCell: boolean;
    /**
     * sortOrder
     */
    sortOrder: any;
    /**
     * children
     */
    children: IScreenObjectCollection;
    /**
     * parent
     */
    parent: any;
    /**
     * position
     */
    position: IPoint;
    /**
     * absolutePosition
     */
    absolutePosition: IPoint;
    /**
     * ignoreParentPosition
     */
    ignoreParentPosition: boolean;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isEnabled
     */
    isEnabled: boolean;
    /**
     * isFocused
     */
    isFocused: boolean;
    /**
     * focusedMode
     */
    focusedMode: focusBehavior;
    /**
     * isExclusiveMouse
     */
    isExclusiveMouse: boolean;
    /**
     * useKeyboard
     */
    useKeyboard: boolean;
    /**
     * useMouse
     */
    useMouse: boolean;
    /**
     * sadComponents
     */
    sadComponents: any;
}

/**
 * Generated interface for Runeforge.Engine.Data.Maps.GameMap
 */
interface IGameMap {
    /**
     * id
     */
    id: string;
    /**
     * name
     */
    name: string;
    /**
     * description
     */
    description: string;
    /**
     * level
     */
    level: number;
    /**
     * renderers
     */
    renderers: any;
    /**
     * allComponents
     */
    allComponents: any;
    /**
     * defaultRenderer
     */
    defaultRenderer: any;
    /**
     * sortOrder
     */
    sortOrder: any;
    /**
     * children
     */
    children: IScreenObjectCollection;
    /**
     * parent
     */
    parent: any;
    /**
     * position
     */
    position: IPoint;
    /**
     * absolutePosition
     */
    absolutePosition: IPoint;
    /**
     * ignoreParentPosition
     */
    ignoreParentPosition: boolean;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isEnabled
     */
    isEnabled: boolean;
    /**
     * isFocused
     */
    isFocused: boolean;
    /**
     * focusedMode
     */
    focusedMode: focusBehavior;
    /**
     * isExclusiveMouse
     */
    isExclusiveMouse: boolean;
    /**
     * useKeyboard
     */
    useKeyboard: boolean;
    /**
     * useMouse
     */
    useMouse: boolean;
    /**
     * sadComponents
     */
    sadComponents: any;
    /**
     * playerFov
     */
    playerFov: any;
    /**
     * goRogueComponents
     */
    goRogueComponents: any;
    /**
     * terrain
     */
    terrain: any;
    /**
     * entities
     */
    entities: any;
    /**
     * layerMasker
     */
    layerMasker: ILayerMasker;
    /**
     * layersBlockingWalkability
     */
    layersBlockingWalkability: any;
    /**
     * layersBlockingTransparency
     */
    layersBlockingTransparency: any;
    /**
     * transparencyView
     */
    transparencyView: any;
    /**
     * walkabilityView
     */
    walkabilityView: any;
    /**
     * astar
     */
    astar: IAStar;
    /**
     * distanceMeasurement
     */
    distanceMeasurement: IDistance;
    /**
     * height
     */
    height: number;
    /**
     * width
     */
    width: number;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
    /**
     * count
     */
    count: number;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
}

/**
 * Generated interface for SadConsole.CellDecorator
 */
interface ICellDecorator {
    /**
     * color
     */
    color: IColor;
    /**
     * glyph
     */
    glyph: number;
    /**
     * mirror
     */
    mirror: mirror;
}

/**
 * Generated interface for SadRogue.Primitives.Color
 */
interface IColor {
    /**
     * b
     */
    b: any;
    /**
     * g
     */
    g: any;
    /**
     * r
     */
    r: any;
    /**
     * a
     */
    a: any;
    /**
     * packedValue
     */
    packedValue: any;
}

/**
 * Generated interface for SadRogue.Integration.TerrainAppearance
 */
interface ITerrainAppearance {
    /**
     * decorators
     */
    decorators: ICellDecorator[];
    /**
     * foreground
     */
    foreground: IColor;
    /**
     * background
     */
    background: IColor;
    /**
     * glyph
     */
    glyph: number;
    /**
     * mirror
     */
    mirror: mirror;
    /**
     * glyphCharacter
     */
    glyphCharacter: any;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isDirty
     */
    isDirty: boolean;
}

/**
 * Generated interface for SadRogue.Primitives.Point
 */
interface IPoint {
}

/**
 * Generated interface for GoRogue.GameFramework.Map
 */
interface IMap {
    /**
     * playerFov
     */
    playerFov: any;
    /**
     * goRogueComponents
     */
    goRogueComponents: any;
    /**
     * terrain
     */
    terrain: any;
    /**
     * entities
     */
    entities: any;
    /**
     * layerMasker
     */
    layerMasker: ILayerMasker;
    /**
     * layersBlockingWalkability
     */
    layersBlockingWalkability: any;
    /**
     * layersBlockingTransparency
     */
    layersBlockingTransparency: any;
    /**
     * transparencyView
     */
    transparencyView: any;
    /**
     * walkabilityView
     */
    walkabilityView: any;
    /**
     * astar
     */
    astar: IAStar;
    /**
     * distanceMeasurement
     */
    distanceMeasurement: IDistance;
    /**
     * height
     */
    height: number;
    /**
     * width
     */
    width: number;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
    /**
     * count
     */
    count: number;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
    /**
     * item
     */
    item: IMapObjectsAtEnumerator;
}

/**
 * Generated interface for SadConsole.Entities.Entity+SingleCell
 */
interface ISingleCell {
    /**
     * isDirty
     */
    isDirty: boolean;
    /**
     * appearance
     */
    appearance: IColoredGlyphBase;
    /**
     * effect
     */
    effect: any;
}

/**
 * Generated interface for SadConsole.Entities.Entity+Animated
 */
interface IAnimated {
    /**
     * animation
     */
    animation: IAnimatedScreenObject;
    /**
     * defaultCollisionRectangle
     */
    defaultCollisionRectangle: IRectangle;
    /**
     * customCollisionRectangle
     */
    customCollisionRectangle: IRectangle;
    /**
     * isDirty
     */
    isDirty: boolean;
}

/**
 * Generated interface for SadConsole.ScreenObjectCollection
 */
interface IScreenObjectCollection {
    /**
     * count
     */
    count: number;
    /**
     * isLocked
     */
    isLocked: boolean;
    /**
     * item
     */
    item: any;
}

/**
 * Generated interface for SadRogue.Primitives.SpatialMaps.LayerMasker
 */
interface ILayerMasker {
    /**
     * numberOfLayers
     */
    numberOfLayers: number;
}

/**
 * Generated interface for GoRogue.Pathing.AStar
 */
interface IAStar {
    /**
     * distanceMeasurement
     */
    distanceMeasurement: IDistance;
    /**
     * walkabilityView
     */
    walkabilityView: any;
    /**
     * heuristic
     */
    heuristic: (arg1: IPoint, arg2: IPoint) => number;
    /**
     * weights
     */
    weights: any;
    /**
     * maxEuclideanMultiplier
     */
    maxEuclideanMultiplier: number;
}

/**
 * Generated interface for SadRogue.Primitives.Distance
 */
interface IDistance {
    /**
     * type
     */
    type: types;
}

/**
 * Generated interface for GoRogue.GameFramework.MapObjectsAtEnumerator
 */
interface IMapObjectsAtEnumerator {
    /**
     * current
     */
    current: any;
}

/**
 * Generated interface for SadConsole.ColoredGlyphBase
 */
interface IColoredGlyphBase {
    /**
     * decorators
     */
    decorators: ICellDecorator[];
    /**
     * foreground
     */
    foreground: IColor;
    /**
     * background
     */
    background: IColor;
    /**
     * glyph
     */
    glyph: number;
    /**
     * mirror
     */
    mirror: mirror;
    /**
     * glyphCharacter
     */
    glyphCharacter: any;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isDirty
     */
    isDirty: boolean;
}

/**
 * Generated interface for SadConsole.AnimatedScreenObject
 */
interface IAnimatedScreenObject {
    /**
     * center
     */
    center: IPoint;
    /**
     * repeat
     */
    repeat: boolean;
    /**
     * isPlaying
     */
    isPlaying: boolean;
    /**
     * animationDuration
     */
    animationDuration: any;
    /**
     * currentFrameIndex
     */
    currentFrameIndex: number;
    /**
     * isEmpty
     */
    isEmpty: boolean;
    /**
     * frames
     */
    frames: any[];
    /**
     * name
     */
    name: string;
    /**
     * state
     */
    state: animationState;
    /**
     * moveToFrontOnMouseClick
     */
    moveToFrontOnMouseClick: boolean;
    /**
     * focusOnMouseClick
     */
    focusOnMouseClick: boolean;
    /**
     * forceRendererRefresh
     */
    forceRendererRefresh: boolean;
    /**
     * defaultRendererName
     */
    defaultRendererName: string;
    /**
     * renderer
     */
    renderer: any;
    /**
     * isDirty
     */
    isDirty: boolean;
    /**
     * font
     */
    font: any;
    /**
     * fontSize
     */
    fontSize: IPoint;
    /**
     * tint
     */
    tint: IColor;
    /**
     * absoluteArea
     */
    absoluteArea: IRectangle;
    /**
     * usePixelPositioning
     */
    usePixelPositioning: boolean;
    /**
     * widthPixels
     */
    widthPixels: number;
    /**
     * heightPixels
     */
    heightPixels: number;
    /**
     * width
     */
    width: number;
    /**
     * height
     */
    height: number;
    /**
     * viewWidth
     */
    viewWidth: number;
    /**
     * viewHeight
     */
    viewHeight: number;
    /**
     * viewPosition
     */
    viewPosition: IPoint;
    /**
     * currentFrame
     */
    currentFrame: any;
    /**
     * sortOrder
     */
    sortOrder: any;
    /**
     * children
     */
    children: IScreenObjectCollection;
    /**
     * parent
     */
    parent: any;
    /**
     * position
     */
    position: IPoint;
    /**
     * absolutePosition
     */
    absolutePosition: IPoint;
    /**
     * ignoreParentPosition
     */
    ignoreParentPosition: boolean;
    /**
     * isVisible
     */
    isVisible: boolean;
    /**
     * isEnabled
     */
    isEnabled: boolean;
    /**
     * isFocused
     */
    isFocused: boolean;
    /**
     * focusedMode
     */
    focusedMode: focusBehavior;
    /**
     * isExclusiveMouse
     */
    isExclusiveMouse: boolean;
    /**
     * useKeyboard
     */
    useKeyboard: boolean;
    /**
     * useMouse
     */
    useMouse: boolean;
    /**
     * sadComponents
     */
    sadComponents: any;
}

/**
 * Generated interface for SadRogue.Primitives.Rectangle
 */
interface IRectangle {
    /**
     * area
     */
    area: number;
    /**
     * center
     */
    center: IPoint;
    /**
     * isEmpty
     */
    isEmpty: boolean;
    /**
     * maxExtent
     */
    maxExtent: IPoint;
    /**
     * maxExtentX
     */
    maxExtentX: number;
    /**
     * maxExtentY
     */
    maxExtentY: number;
    /**
     * minExtent
     */
    minExtent: IPoint;
    /**
     * minExtentX
     */
    minExtentX: number;
    /**
     * minExtentY
     */
    minExtentY: number;
    /**
     * position
     */
    position: IPoint;
    /**
     * size
     */
    size: IPoint;
}
