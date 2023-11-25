namespace Collections;

public class DataOverrides
{
    public static readonly List<uint> IgnoreMogStationId = new()
    {
       12995, // Silver chocobo feathers
    };

    public static readonly List<uint> IgnoreSpecialShopId = new()
    {
        // 6.2 Tomestones
        1770437, // Allagan tomestones of Astronomy (DoW)
        1770438, // Allagan tomestones of Astronomy (DoM)
        1770446, // Allagan Tomestones of Astronomy Exchange (Weapons)

        // 6.0 Tomestones
        1770434, // Allagan Tomestones of Aphorism (DoW)
        1770435, // Allagan Tomestones of Aphorism (DoM)

        // 4.4 Tomestones
        1770306, // Scaevan Gear Augmentation (ILazyRow 400 DoW)
        1770307, // Scaevan Gear Augmentation (ILazyRow 400 DoM)

        // 3.4 Redundant Shop
        1770304, // Shire Gear Augmentation (IL 270 DoW)
        1770305, // Shire Gear Augmentation (IL 270 DoM)

        // Crafting Shops (doesn't exist)
        1770263, // Crafter Gear (IL 220-230)
        1770311, // White Gatherers' Scrip Exchange (Lv. 80 Gear) III
        1770271, // Gatherer Gear (IL 220-230)
        1770310, // White Crafters' Scrip Exchange (Lv. 80 Gear) III

        // Hellbound weapons (PvP) - duplicate Wolf Collar
        1769812,

        // Events
        1769486, // 2014/2018 Valentine NPC
        1769487, // 2014/2018 Valentine NPC
        1769485, // Starlight Event NPC
        1769483, // Moonfire Fair
        1769663, // Hatching-tide (2016)
    };

    public static readonly List<uint> IgnoreGilShopId = new()
    {
        // Events TODO mark these as event items
        262423, // red uma shonin
        262468, // Moonfire Faire
        262469, // Moonfire Faire
        262489, // Heavensturn
        262587, // Valentione's Day (2016)
        262585, // Starlight Celebration (2015)

        // Redundant / non existant
        262701,
        262510,
        262623,
        262154,
        262161,
        262167,

    };

    public static readonly Dictionary<uint, uint> SpecialShopToNpcBase = new()
    {
        // Disreputable Priest (PvP Makai)
        { 1769743, 1018655 },
        { 1769744, 1018655 },
        { 1770537, 1018655 },

        // Eureka - Gerolt
        {1769820, 1025047 },
        {1769821, 1025047 },
        {1769822, 1025047 },
        {1769823, 1025047 },
        {1769824, 1025047 },
        {1769825, 1025047 },
        {1769826, 1025047 },
        {1769827, 1025047 },
        {1769828, 1025047 },
        {1769829, 1025047 },
        {1769830, 1025047 },
        {1769831, 1025047 },
        {1769832, 1025047 },
        {1769833, 1025047 },
        {1769834, 1025047 },

        // Eureka - Expedition Artisan (Hydatos)
        {1769928, 1026496 },
        {1769934, 1026496 },
        {1769935, 1026496 },

        // Faux
        {1770282, 1033921 },

        // Sajareen - Bicolor gems
        {1770471, 1037304 },

        // Gadfrid - Bicolor gems
        {1770470 ,1037055 },

        // Island - Horrendous Hoarder
        {1770659, 1043463 },

        // Commendation Quartermaster (PvP)
        {1770684 ,1043099 },
    };

    public static readonly Dictionary<uint, List<uint>> GilShopToNpcBase = new()
    {
        // Interchangeable Gil shops
        // Geraint Gridania
        {1000217, new List<uint>(){ 262204, 262716, 262717, 262718, 262719, 262404, 262194, 262184, 262720, 262721, 262722, 262723, 262724 } },
        // Faezghim Limsa Lominsa
        {1001205, new List<uint>(){ 262715, 262406 } },
        // Jelous Juggernaut Uldah
        {1001970, new List<uint>(){ 262407 } },

        // Admiranda Gridania (interchangeable with limsa/uldah)
        {1000218, new List<uint>(){ 262725, 262726, 262727, 262728, 262729, 262730, 262731, 262732, 262733, 262734} },

        // Domitien Gridania (interchangeable with limsa/uldah)
        {1000215, new List<uint>(){ 262700, 262726, 262727, 262728, 262729, 262730, 262731, 262732, 262733, 262734} },
        
        // Calamity Salvager (1 out of 3)
        {1006004, new List<uint>(){ 262443, 262444, 262445, 262449, 262450 } },

        // Norlaise
        {1011204, new List<uint>(){ 262741, 262742, 262743, 262744, 262745, 262746, 262747, 262749 } },

        // Drake relic replicas
        {1008945, new List<uint>(){ 262497, 262498, 262499, 262500, 262501, 262502, 262503, 262504, 262505, 262506, 262507 } },

    };

    public static readonly Dictionary<uint, (uint territoryId, double X, double Y)> NpcBaseIdToLocation = new()
    {
        // Cihanti
        {1037301, (963, 10.85, 10.43) }, //radz at han

        // Nesvaaz
        {1037302, (963, 10.5, 10.12) }, //radz at han

        // Rashti
        {1037305, (963, 10.9, 9.89) }, // radz at han

        // Wilmetta
        {1037312, (963, 10.5, 7.5) },// radz at han

        // Khaldeen
        {1037300, (963, 10.9, 10.5) }, // radz at han

        // junkmonger
        {1037636, (957, 10.3, 23.3) }, // thavnir

        // junkmonger
        {1037725, (958, 31.3, 17.5) }, // garlemald

        // disreputable priest
        {1018655, (250, 5, 5) }, // wolves den

        // Limbeth
        {1027566, (820, 11.7, 11) }, // Eulmore - the buttress

        // Y'sohnjin
        {1037049, (962, 12.1, 9.7) }, // Old sharlayan

        // Cwengyth
        {1037050, (962, 12.3, 9.7) }, // Old sharlayan

        // Mylenie
        {1039555, (956, 8, 28) }, // Labyrinthos
    };
}

