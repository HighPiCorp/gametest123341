var localplayer = mp.players.local;
var showDamage = false;
var showPos = mp.players.local;
var showDamageCount = 0;

mp.events.add("AGMS", (toggle) => {
    localplayer.setInvincible(toggle);
});

const weaponList = [
  { hash:-1834847097, damage: {HEAD: 60, NECK: 20, CHEST: 20, HANDS: 20, LEGS: 20, DEFAULT: 20}}, // Antique Cavalry Dagger
  { hash:-1786099057, damage: {HEAD: 60, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // Baseball Bat
  { hash:-102323637, damage: {HEAD: 45, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 5}}, // Broken Bottle
  { hash:-2067956739, damage: {HEAD: 35, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // Crowbar
  { hash:-1951375401, damage: {HEAD: 10, NECK: 5, CHEST: 5, HANDS: 5, LEGS: 5, DEFAULT: 5}}, // Flashlight
  { hash:1141786504, damage: {HEAD: 20, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // Golf Club
  { hash:1317494643, damage: {HEAD: 39, NECK: 22, CHEST: 22, HANDS: 22, LEGS: 22, DEFAULT: 22}}, // Hammer
  { hash:-102973651, damage: {HEAD: 75, NECK: 55, CHEST: 55, HANDS: 55, LEGS: 55, DEFAULT: 55}}, // Hatchet
  { hash:-656458692, damage: {HEAD: 25, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 5}}, // Brass Knuckles
  { hash:-1716189206, damage: {HEAD: 60, NECK: 40, CHEST: 40, HANDS: 40, LEGS: 40, DEFAULT: 40}}, // Knife
  { hash:-581044007, damage: {HEAD: 75, NECK: 40, CHEST: 40, HANDS: 40, LEGS: 40, DEFAULT: 40}}, // Machete
  { hash:-538741184, damage: {HEAD: 60, NECK: 40, CHEST: 40, HANDS: 40, LEGS: 40, DEFAULT: 40}}, // Switchblade
  { hash:1737195953, damage: {HEAD: 50, NECK: 30, CHEST: 30, HANDS: 30, LEGS: 30, DEFAULT: 30}}, // Nightstick
  { hash:419712736, damage: {HEAD: 50, NECK: 20, CHEST: 20, HANDS: 20, LEGS: 20, DEFAULT: 20}}, // Pipe Wrench
  { hash:-853065399, damage: {HEAD: 75, NECK: 55, CHEST: 55, HANDS: 55, LEGS: 55, DEFAULT: 55}}, // Battle Axe
  { hash:-1810795771, damage: {HEAD: 20, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // Pool Cue
  { hash:940833800, damage: {HEAD: 50, NECK: 30, CHEST: 30, HANDS: 30, LEGS: 30, DEFAULT: 30}}, // Stone Hatchet
  { hash:453432689, damage: {HEAD: 23, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Pistol
  { hash:584646201, damage: {HEAD: 16, NECK: 12, CHEST: 12, HANDS: 12, LEGS: 12, DEFAULT: 12}}, // AP Pistol
  { hash:911657153, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Stun Gun
  { hash:-1716589765, damage: {HEAD: 45, NECK: 26, CHEST: 26, HANDS: 26, LEGS: 26, DEFAULT: 26}}, // Pistol .50
  { hash:-1076751822, damage: {HEAD: 23, NECK: 13, CHEST: 13, HANDS: 13, LEGS: 13, DEFAULT: 13}}, // SNS Pistol
  { hash:-2009644972, damage: {HEAD: 23, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // SNS Pistol Mk II
  { hash:-771403250, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Heavy Pistol
  { hash:137902532, damage: {HEAD: 24, NECK: 16, CHEST: 16, HANDS: 16, LEGS: 16, DEFAULT: 16}}, // Vintage Pistol
  { hash:1198879012, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Flare Gun
  { hash:-598887786, damage: {HEAD: 67, NECK: 36, CHEST: 36, HANDS: 36, LEGS: 36, DEFAULT: 36}}, // Marksman Pistol
  { hash:-1045183535, damage: {HEAD: 60, NECK: 36, CHEST: 36, HANDS: 36, LEGS: 36, DEFAULT: 36}}, // Revolver = -1045183535,
  { hash:-879347409, damage: {HEAD: 67, NECK: 43, CHEST: 43, HANDS: 43, LEGS: 43, DEFAULT: 43}}, // Heavy Revolver Mk II
  { hash:-1746263880, damage: {HEAD: 28, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Double Action Revolver
  { hash:-1355376991, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Up-n-Atomizer
  { hash:727643628, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Ceramic Pistol
  { hash:-1853920116, damage: {HEAD: 60, NECK: 36, CHEST: 36, HANDS: 36, LEGS: 36, DEFAULT: 36}}, // Navy Revolver
  { hash:324215364, damage: {HEAD: 16, NECK: 12, CHEST: 12, HANDS: 12, LEGS: 12, DEFAULT: 12}}, // Micro SMG
  { hash:736523883, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // SMG
  { hash:2024373456, damage: {HEAD: 26, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // SMG Mk II
  { hash:-270015777, damage: {HEAD: 28, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Assault SMG
  { hash:171789620, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Combat PDW
  { hash:-619010992, damage: {HEAD: 23, NECK: 15, CHEST:15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // Machine Pistol
  { hash:-1121678507, damage: {HEAD: 17, NECK: 13, CHEST: 13, HANDS: 13, LEGS: 13, DEFAULT: 13}}, // Mini SMG
  { hash:1198256469, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Unholy Hellbringer
  { hash:487013001, damage: {HEAD: 35, NECK: 27, CHEST: 27, HANDS: 27, LEGS: 27, DEFAULT: 27}}, // Pump Shotgun
  { hash:1432025498, damage: {HEAD: 35, NECK: 29, CHEST: 29, HANDS: 29, LEGS: 29, DEFAULT: 29}}, // Pump Shotgun Mk II
  { hash:2017895192, damage: {HEAD: 33, NECK: 25, CHEST: 25, HANDS: 25, LEGS: 25, DEFAULT: 25}}, // Sawed-Off Shotgun
  { hash:-494615257, damage: {HEAD: 30, NECK: 21, CHEST: 21, HANDS: 21, LEGS: 21, DEFAULT: 21}}, // Assault Shotgun
  { hash:-1654528753, damage: {HEAD: 27, NECK: 24, CHEST: 24, HANDS: 24, LEGS: 24, DEFAULT: 24}}, // Bullpup Shotgun
  { hash:-1466123874, damage: {HEAD: 60, NECK: 45, CHEST: 45, HANDS: 45, LEGS: 45, DEFAULT: 45}}, // Musket
  { hash:984333226, damage: {HEAD: 50, NECK: 35, CHEST: 35, HANDS: 35, LEGS: 35, DEFAULT: 35}}, // Heavy Shotgun
  { hash:-275439685, damage: {HEAD: 37, NECK: 25, CHEST: 25, HANDS: 25, LEGS: 25, DEFAULT: 25}}, // Double Barrel Shotgun
  { hash:317205821, damage: {HEAD: 29, NECK: 20, CHEST: 20, HANDS: 20, LEGS: 20, DEFAULT: 20}}, // Sweeper Shotgun
  { hash:-1074790547, damage: {HEAD: 22, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Assault Rifle
  { hash:961495388, damage: {HEAD: 24, NECK: 24, CHEST: 24, HANDS: 24, LEGS: 24, DEFAULT: 19}}, // Assault Rifle Mk II
  { hash:-2084633992, damage: {HEAD: 23, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Carbine Rifle
  { hash:4208062921, damage: {HEAD: 27, NECK: 22, CHEST: 22, HANDS: 22, LEGS: 22, DEFAULT: 22}}, // Carbine Rifle Mk II
  { hash:2937143193, damage: {HEAD: 25, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Advanced Rifle
  { hash:3231910285, damage: {HEAD: 27, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Special Carbine
  { hash:-1768145561, damage: {HEAD: 29, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // Special Carbine Mk II
  { hash:2132975508, damage: {HEAD: 25, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Bullpup Rifle
  { hash:-2066285827, damage: {HEAD: 27, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // Bullpup Rifle Mk II
  { hash:1649403952, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // Compact Rifle
  { hash:-1660422300, damage: {HEAD: 20, NECK: 14, CHEST: 14, HANDS: 14, LEGS: 14, DEFAULT: 14}}, // MG
  { hash:2144741730, damage: {HEAD: 23, NECK: 14, CHEST: 14, HANDS: 14, LEGS: 14, DEFAULT: 14}}, // Combat MG
  { hash:3686625920, damage: {HEAD: 23, NECK: 16, CHEST: 16, HANDS: 16, LEGS: 16, DEFAULT: 16}}, // Combat MG Mk II
  { hash:1627465347, damage: {HEAD: 23, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Gusenberg
  { hash:100416529, damage: {HEAD: 40, NECK: 27, CHEST: 27, HANDS: 27, LEGS: 27, DEFAULT: 27}}, // Sniper Rifle
  { hash:205991906, damage: {HEAD: 67, NECK: 35, CHEST: 35, HANDS: 35, LEGS: 35, DEFAULT: 35}}, // Heavy Sniper
  { hash:177293209, damage: {HEAD: 78, NECK: 45, CHEST: 45, HANDS: 45, LEGS: 45, DEFAULT: 45}}, // Heavy Sniper Mk II
  { hash:-952879014, damage: {HEAD: 34, NECK: 21, CHEST: 21, HANDS: 21, LEGS: 21, DEFAULT: 21}}, // Marksman Rifle
  { hash:1785463520, damage: {HEAD: 34, NECK: 25, CHEST: 25, HANDS: 25, LEGS: 25, DEFAULT: 25}}, // Marksman Rifle Mk II
  { hash:-1312131151, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // RPG
  { hash:-1568386805, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Grenade Launcher
  { hash:1305664598, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Grenade Launcher Smoke
  { hash:1119849093, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Minigun
  { hash:2138347493, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Firework Launcher
  { hash:1834241177, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Railgun
  { hash:1672152130, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Homing Launcher
  { hash:125959754, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Compact Grenade Launcher
  { hash:-1238556825, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Widowmaker
  { hash:-1813897027, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Grenade
  { hash:-1600701090, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // BZ Gas
  { hash:615608432, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Molotov Cocktail
  { hash:-1420407917, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Proximity Mines
  { hash:126349499, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Snowballs
  { hash:-1169823560, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Pipe Bombs
  { hash:600439132, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Baseball
  { hash:-37975472, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Tear Gas
  { hash:1233104067, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Flare
  { hash:741814745, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Sticky Bomb
  { hash:883325847, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Jerry Can
  { hash:-72657034, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Parachute
  { hash:101631238, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Fire Extinguisher
  { hash:-1168940174, damage: {HEAD: 0, NECK: 0, CHEST: 0, HANDS: 0, LEGS: 0, DEFAULT: 0}}, // Hazardous Jerry Can
  { hash:419712736, damage: {HEAD: 41, NECK: 21, CHEST: 21, HANDS: 21, LEGS: 21, DEFAULT: 21}}, // Wrench = 419712736
  { hash:3415619887, damage: {HEAD: 67, NECK: 43, CHEST: 43, HANDS: 43, LEGS: 43, DEFAULT: 43}}, // Revolver_mk2 = 3415619887
  { hash:-1075685676, damage: {HEAD: 25, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // Pistol Mk II
  { hash:1593441988, damage: {HEAD: 23, NECK: 15, CHEST: 15, HANDS: 15, LEGS: 15, DEFAULT: 15}}, // Combat Pistol
  { hash:2725352035, damage: {HEAD: 15, NECK: 9, CHEST: 9, HANDS: 9, LEGS: 9, DEFAULT: 9}}, // fists
  { hash:3249783761, damage: {HEAD: 60, NECK: 36, CHEST: 36, HANDS: 36, LEGS: 36, DEFAULT: 36}}, // revolver
  { hash:2210333304, damage: {HEAD: 27, NECK: 22, CHEST: 22, HANDS: 22, LEGS: 22, DEFAULT: 22}}, // karab
  { hash:3523564046, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // heavy pistol
  { hash:3675956304, damage: {HEAD: 26, NECK: 17, CHEST: 17, HANDS: 17, LEGS: 17, DEFAULT: 17}}, // machine pistol
  { hash:3220176749, damage: {HEAD: 23, NECK: 18, CHEST: 18, HANDS: 18, LEGS: 18, DEFAULT: 18}}, // Assault Rifle
  { hash:2640438543, damage: {HEAD: 27, NECK: 24, CHEST: 24, HANDS: 24, LEGS: 24, DEFAULT: 24}}, // Bullpup Shotgun
  { hash:3800352039, damage: {HEAD: 30, NECK: 26, CHEST: 26, HANDS: 26, LEGS: 26, DEFAULT: 26}}, // Assault Shotgun
  { hash:2343591895, damage: {HEAD: 10, NECK: 5, CHEST: 5, HANDS: 5, LEGS: 5, DEFAULT: 5}}, // Flashlight
  { hash:2508868239, damage: {HEAD: 60, NECK: 35, CHEST: 35, HANDS: 35, LEGS: 35, DEFAULT: 35}}, // Baseball bat
  { hash:2227010557, damage: {HEAD: 35, NECK: 19, CHEST: 19, HANDS: 19, LEGS: 19, DEFAULT: 19}}, // Crowbar
];

mp.events.add('outgoingDamage', (sourceEntity, targetEntity, sourcePlayer, weapon, boneIndex, damage) => {
  if(targetEntity.type != 'player')
    return;

  //stungun
  if(weapon == 911657153)
  {
    return;
  }
  showDamage = false;
  const hp = targetEntity.getHealth();

  const weaponItem = weaponList.find(w => w.hash === weapon);
  if(weaponItem == null)
  {
    mp.events.callRemote('InconmingDamage', targetEntity, 0);
  }

  var FixedDamage = 0;

  switch(boneIndex)
  {
    //legs
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
    case 5:
    case 6:
      FixedDamage = weaponItem.damage.LEGS;
      break;


    //chest
    case 7:
    case 8:
    case 9:
    case 10:
      FixedDamage = weaponItem.damage.CHEST;
      break;
    //hands


    case 11:
    case 12:
    case 13:
    case 14:
    case 15:
    case 16:
    case 17:
    case 18:
      FixedDamage = weaponItem.damage.HANDS;
      break;


    //neck
    case 19:
      FixedDamage = weaponItem.damage.NECK;
      break;


    //head
    case 20:
      FixedDamage = weaponItem.damage.HEAD;
      break;


    //???
    default:
      FixedDamage = weaponItem.damage.DEFAULT;
    break;
  }
  mp.events.callRemote('InconmingDamage', targetEntity, FixedDamage);

  //Отображение урона над игроком
  showDamage = true;
  showDamageCount = FixedDamage;
  showPos = targetEntity.position;
});

mp.events.add('incomingDamage', (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex, damage) => {

  if(targetEntity.type != 'player')
  {
    return;
  }

  if(sourcePlayer == undefined)
    mp.events.call('DeathTimerKillerSet','');
  else
    killer = 'гражданин#' + sourcePlayer.getVariable('PERSON_ID') + '(' + sourcePlayer.getVariable('REMOTE_ID') +')';

  mp.events.call('DeathTimerKillerSet', killer);
  if(weapon == 911657153) return;//stungun
  if(weapon != null)
  {
    localplayer.setInvincible(true);
  }
});

mp.events.add('DrawDamage', () => {
  mp.game.graphics.drawText(showDamageCount, [showPos.x, showPos.y, showPos.z + 1.5], {
    scale: [0.3, 0.3],
    outline: true,
    color: [230, 17, 17, 255],
    font: 0,
  });
  setTimeout(() => {
    showDamage = false;
  },2000);
});
mp.events.add('render', () => {
  if(showDamage == true)
  {
    mp.events.call('DrawDamage');
  }
});
