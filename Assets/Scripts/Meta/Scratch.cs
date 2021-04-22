
public class Scratch 
{
    /*
     * Game gist - Co-op 2.5D tower defense/arena shooter. Players are robots, baddies are... also robots? Tbh, bugs would make sense, but look to be a pita to animate
     * Players start in one small area of a largeish map, and will be constantly attacked by a small trickle of enemies plus regular large waves
     * As more areas are claimed by the players, baddies will spawn stronger/bigger numbers
     * 
     * Players will have objectives that range from "Destroy all enemy structures"/destroy specific structures or McGuffins/escort (or timed survival)
     * Why include turrets? B/c I like turrets. Turrets end up being needed to deal with the constant trickle while players move out and the large surges.
     *  (this seems slightly (overwhelmingly) circular - let's roll with it for now, though)
     * 
     * If playerTurrets are damageable (yes) - then turrets should also be rebuildable, so players should be getting building mats over time
     *  (and maybe also caches)
     *  
     *  Mats are gained similar to an RTS by holding and "mining" areas. It'll trickle in over time (with a cap/storage).
     *  To prevent just straight turret-creeping a secondary resource is needed that's a static "food/supply". Also increased from points? Maybe have something like pylons/depots.
     * Let's call the two resources Minerals and Energy.
     *  
     * Players will be able to move/attack/build (on a grid or "free" positioning building?)
     * On the attacking front, players will have multiple weapons that they can swap between (two/threeish slots?)
     *  Not sure if these will just be chosen before the mission, findable in the map?, purchaseable from a shop/building?
     *  Weapons will (typically) use ammo, refill ammo should come from a building that (slowly) converts supplies into ammo bricks
     *  3 slot idea - "pistol"ish weapon that has unlimited ammo (maybe a recharge bar?), a "main" weapon, and a "heavy/power" weapon
     *  
     * I think I want to avoid going too heavily RTS, so we'll pass on any sort of unit-making factories for now
     * 
     * Longer term ideas - RPGish upgrades with players getting better weapons/towers?
     * For now levels will be lovingly handcrafed, since doing procedural generation for something like this sounds like a nightmare
     * 
     * Control-wise, this will be controller-playable. Basic twin-stick-style shooting controls. Building placement will bring up a cursor if needed.
     * Camera is locked to the player. There's a minimap that can be enlarged.
     * Let's ditch the FoW/FoV system - while it works I don't think it'll be needed/look good with the locked camera. Can always look into adding it back in later.
     * 
     * 
     * 
     * Both player characters and player owned structures (towers/etc.) will have the Player tag
     * 
     * Structure of baddies
     * -BaddiePathfinder - handles movement, has a positional target (obviously not used on BaddieTowers)
     * -BaddieAttacker - handles attacking, has a GameObject target, if it's mobile it calls baddiePathfinder
     * --for now just scans for a nearby Player tagged object and goes for it
     * --Should later have a "high level" persistant target (a player, or a player structure), that it heads for, but will attack (and move to kite/chase?) nearby players/towers
     * --Could either use a case switch for different "AI" types, or make some stuff in here overrideable and use extensions for different AI
     * -Damageable - Everything with health has this. Handles taking damage (and healing). Will also contain stats for armor/DR if/when added
     * --Also responsible for updating attatched health bars/floating damage text (if/when either are added)
     * --Oh hey, in a game with robots, this should probably also track some sort of shield secondary HP
     * 
     * BaddieSpawners are seperate, but will have Damageable (if we're going the gauntlet monsterspawner route)
     * 
     * Players
     * -DudeController
     * -Damageable? Likely yes
     * 
     * PlayerTowers
     * -Pla
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */

}
