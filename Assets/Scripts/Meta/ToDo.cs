
public class ToDo
{
    /*
     * xStart a fresh scene, make a first sample level with probuilder
     * 
     * xMake a singleton tag manager for things to pull from
     * 
     * xBring in the existing player (Dude) objects, drop the FoV/FoW system
     * 
     * xBring in baddieSpawners and baddies
     * xFinish making both of them Damageable
     * xFinish splitting Baddies into Pathfinder/Attacker/Damageable
     * xAdd some basic baddie attack animation
     * 
     * Baddie pathfinding is a little janky (surprise surprise) when they get close to the target
     * -Need to maybe increase repath-rate when close, or add in a check for "if in LoS, then just go straight at them"
     * -Simmilarly, still want to add in some local avoidance of other baddies
     * -Also, running into a bug where the baddie pathing target is updating, but not the networkbaddie target (ie. for attacking)
     * 
     * xMake baddies do damage to player-tagged damageables
     * 
     * Add health bars? Or at least a player HP UI HUD element
     * 
     * Add in ADSing
     * -ADS slows movement speed, but decreases the spread angle of shots
     * -Perhaps certain (heavy) weapons can only be used ADS (ie. no rocket shooting from the hip)
     * -I tentatively like the idea of aim aiming laser style visual for this (and will help on controller)
     * 
     * Make a basic Cylinder+Sphere topper + cube barrel tower model
     * Make player towers
     * Add a building system - don't forget to adjust pathfinding/target aquisition
     * Need to look into/think about buildable vs non-buildable terrain
     * 
     * Start the player resource system
     * 
     * Add map patches for resources/appropriate buildings
     * 
     * 
     * 
     * 
     * Think about some swarm-like AI behavior, such as grouping up at a "rally" before attacking (similar to factorio bugs)
     * 
     * Way down the list - look into scene templates for making more levels
     * 
     */
}
