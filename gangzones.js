/*const positions = [
    { 'position': { 'x': -200.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1431.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -200.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1531.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -200.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1631.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -100.8397, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1731.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': -0.8397, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1831.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 99.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 599.1603, 'y': -1931.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 199.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 499.1603, 'y': -2031.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 299.1603, 'y': -2131.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 399.1603, 'y': -2131.556, 'z': 30.18104 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2401.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1068.898, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 968.8984, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1168.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1701.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 768.8984, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 868.8984, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1168.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1601.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1268.898, 'y': -1501.556, 'z': 28.17772 }, 'color': 10 },
	{ 'position': { 'x': 1368.898, 'y': -1501.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -1801.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1368.898, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1268.898, 'y': -1901.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1268.898, 'y': -2001.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1268.898, 'y': -2101.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1268.898, 'y': -2201.556, 'z': 28.17772 }, 'color': 10 },
  { 'position': { 'x': 1268.898, 'y': -2301.556, 'z': 28.17772 }, 'color': 10 },
]*/

let natives = {}

mp.game1.graphics.clearDrawOrigin = () => mp.game1.invoke('0xFF0B610F6BE0D7AF'); // 26.07.2018 // GTA 1.44

natives.SET_BLIP_SPRITE = (blip, sprite) => mp.game1.invoke("0xDF735600A4696DAF", blip, sprite); // SET_BLIP_SPRITE
natives.SET_BLIP_ALPHA = (blip, a) => mp.game1.invoke("0x45FF974EEE1C8734", blip, a); // SET_BLIP_ALPHA
natives.SET_BLIP_COLOUR = (blip, c) => mp.game1.invoke("0x03D7FB09E75D6B7E", blip, c); // SET_BLIP_COLOUR
natives.SET_BLIP_ROTATION = (blip, r) => mp.game1.invoke("0xF87683CDF73C3F6E", blip, r); // SET_BLIP_ROTATION
natives.SET_BLIP_FLASHES = (blip, f) => mp.game1.invoke("0xB14552383D39CE3E", blip, f); // SET_BLIP_FLASHES
natives.SET_BLIP_FLASH_TIMER = (blip, t) => mp.game1.invoke("0xD3CD6FD297AE87CC", blip, t); // SET_BLIP_FLASH_TIMER
natives.SET_BLIP_COORDS = (blip, x, y, z) => mp.game1.invoke("0xAE2AF67E9D9AF65D", blip, x, y, z); // SET_BLIP_COORDS
natives.SET_THIS_SCRIPT_CAN_REMOVE_BLIPS_CREATED_BY_ANY_SCRIPT = (toggle) => mp.game1.invoke("0xB98236CAAECEF897", toggle); // SET_THIS_SCRIPT_CAN_REMOVE_BLIPS_CREATED_BY_ANY_SCRIPT
natives.GET_FIRST_BLIP_INFO_ID = (i) => mp.game1.invoke("0x1BEDE233E6CD2A1F", i); // GET_FIRST_BLIP_INFO_ID
natives.GET_NEXT_BLIP_INFO_ID = (i) => mp.game1.invoke("0x14F96AA50D6FBEA7", i); // GET_NEXT_BLIP_INFO_ID
natives.DOES_BLIP_EXIST = (blip) => mp.game1.invoke("0xA6DB27D19ECBB7DA", blip); // DOES_BLIP_EXIST
natives.GET_NUMBER_OF_ACTIVE_BLIPS = () => mp.game1.invoke("0x9A3FF3DE163034E8"); // GET_NUMBER_OF_ACTIVE_BLIPS
natives.SET_BLIP_SCALE = (blip,scale) => mp.game1.invoke("0xD38744167B2FA257",blip,scale); // SET_BLIP_SCALE
natives.SET_ENTITY_NO_COLLISION_ENTITY = (entity1, entity2, collision) => mp.game1.invoke("0xA53ED5520C07654A", entity1.handle, entity2.handle, collision); // SET_ENTITY_NO_COLLISION_ENTITY

var positions = [
	{ 'position': { 'x': -302.197, 'y': -1528.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 }, // 1
	{ 'position': { 'x': -200.196, 'y': -1415.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 }, // 2
	{ 'position': { 'x': -98.196, 'y': -1302.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 3
	{ 'position': { 'x': 14.804, 'y': -1404.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },   // 4
	{ 'position': { 'x': 127.804, 'y': -1506.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 5
	{ 'position': { 'x': 240.804, 'y': -1608.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 6
	{ 'position': { 'x': 353.804, 'y': -1710.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 7
	{ 'position': { 'x': 466.804, 'y': -1812.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 8
	{ 'position': { 'x': -87.196, 'y': -1517.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 9
	{ 'position': { 'x': -189.196, 'y': -1630.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 }, // 10
	{ 'position': { 'x': 25.804, 'y': -1619.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },   // 11
	{ 'position': { 'x': -76.196, 'y': -1732.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 12
	{ 'position': { 'x': 138.804, 'y': -1721.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 13
	{ 'position': { 'x': 36.804, 'y': -1834.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },   // 14
	{ 'position': { 'x': 251.804, 'y': -1823.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 15
	{ 'position': { 'x': 149.804, 'y': -1936.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 16
	{ 'position': { 'x': 364.804, 'y': -1925.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 17
	{ 'position': { 'x': 262.804, 'y': -2038.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 18
	{ 'position': { 'x': 375.804, 'y': -2140.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 19
	{ 'position': { 'x': 477.804, 'y': -2027.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 20
	//{ 'position': { 'x':569.0000, 'y': -1940.0000, 'r': -42.0000}, 'range': 44, 'color': 10, 'rotation': 0 },  // 21
	//{ 'position': { 'x':600.0000, 'y': -1876.0000, 'r': -42.0000}, 'range': 24, 'color': 10, 'rotation': 0 },  // 22
	{ 'position': { 'x': 568.804, 'y': -1699.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 23
	{ 'position': { 'x': 455.804, 'y': -1597.965, 'r': -42.0000 }, 'range': 76, 'color': 10, 'rotation': 48 },  // 24
	//{ 'position': { 'x':379.0000, 'y': -1564.0000, 'r': -42.0000}, 'range': 44, 'color': 10, 'rotation': 0 },  // 25
	//
	{ 'position': { 'x': 798.0000, 'y': -1572.0000, 'r': 0.0000 }, 'range': 76, 'color': 10, 'rotation': 90 },    // 26
	{ 'position': { 'x':950.0000, 'y': -1572.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 27
	{ 'position': { 'x':950.0000, 'y': -1724.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 28
	{ 'position': { 'x':798.0000, 'y': -1724.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 29
	{ 'position': { 'x':798.0000, 'y': -1876.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 30
	{ 'position': { 'x':798.0000, 'y': -2028.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 31
	{ 'position': { 'x':798.0000, 'y': -2180.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 32
	{ 'position': { 'x':798.0000, 'y': -2332.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 33
	{ 'position': { 'x':798.0000, 'y': -2484.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 34
	{ 'position': { 'x':950.0000, 'y': -2484.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 35
	{ 'position': { 'x':1102.000, 'y': -2484.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 36
	//{ 'position': { 'x':1254.0000, 'y': -2454.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 37
	//{ 'position': { 'x':1254.0000, 'y': -2302.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 38
	{ 'position': { 'x':1102.000, 'y': -2332.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 39
	{ 'position': { 'x':950.0000, 'y': -2332.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 40
	{ 'position': { 'x':950.0000, 'y': -2180.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 41
	{ 'position': { 'x':1102.000, 'y': -2180.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 42
	{ 'position': { 'x':1102.000, 'y': -2028.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 43
	{ 'position': { 'x':950.0000, 'y': -2028.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 44
	{ 'position': { 'x':950.0000, 'y': -1876.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },    // 45
	//{ 'position': { 'x':1102.0000, 'y': -1846.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 46
	//{ 'position': { 'x':1160.0000, 'y': -1694.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 47
	//{ 'position': { 'x':1160.0000, 'y': -1542.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 48
	{ 'position': { 'x':1352.0000, 'y': -1582.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 49
	{ 'position': { 'x':1504.0000, 'y': -1582.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 50
	{ 'position': { 'x':1504.0000, 'y': -1734.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 51
	{ 'position': { 'x':1352.0000, 'y': -1734.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 52
	{ 'position': { 'x':1352.0000, 'y': -1886.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 53
	{ 'position': { 'x':1352.0000, 'y': -2038.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 54
	{ 'position': { 'x':1504.0000, 'y': -2038.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 55
	{ 'position': { 'x':1504.0000, 'y': -1886.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 90 },   // 56
	//{ 'position': { 'x':1616.0000, 'y': -1846.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 57
	//{ 'position': { 'x':1616.0000, 'y': -1694.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 58
	//{ 'position': { 'x':1616.0000, 'y': -1998.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 59
	//{ 'position': { 'x':1616.0000, 'y': -2150.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 60
	//{ 'position': { 'x':1616.0000, 'y': -2302.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 61
	//{ 'position': { 'x':1464.0000, 'y': -2302.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 62
	//{ 'position': { 'x':1464.0000, 'y': -2150.0000, 'r': 0.0000}, 'range': 76, 'color': 10, 'rotation': 0 },   // 63
	//{ 'position': { 'x':1222.0000, 'y': -2182.0000, 'r': 0.0000}, 'range': 44, 'color': 10, 'rotation': 0 },   // 64
];

var gangSkins = [
    {First: 0x33A464E5, Second: 0xDB729238, Name: "Families"},
    {First: 0x231AF63F, Second: 0x23B88069, Name: "Ballas"},
    {First: 0x4D5696F7, Second: 0xC2A87702, Name: "Vagos"},
    {First: 0x68709618, Second: 0xAA82FF9B, Name: "Marabunta"},
    {First: 0x7E0961B8, Second: 0x9D0087A8, Name: "Blood Street"},
];

var GangColors = {
	52: 0,
	58: 1,
	70: 2,
	77: 3,
	59: 4,
}

var GangPeds = [];
let num = 1;

var blipss = [];

mp.events.add('loadCaptureBlips', function (json) {
	try {
		// let last_blip = natives.GET_FIRST_BLIP_INFO_ID(5)
		// while (natives.DOES_BLIP_EXIST(last_blip)) {
		// 	mp.game1.ui.removeBlip(last_blip)
		// 	//mp.events.call('stc', `dellllll`);
		// 	last_blip = natives.GET_NEXT_BLIP_INFO_ID(5)
		// }
		var colors = JSON.parse(json);
		for (var i = 0; i < colors.length; i++) {
			positions[i].color = colors[i];
		}
		positions.forEach(element => {
			try {

				const blip = mp.blips.new(5, new mp.Vector3(element.position.x, element.position.y, 4294967295),
					{
						color: element.color,
						alpha: 90,
						rotation: element.rotation,
						dimension: 0,
						radius: element.range,
					});
				blipss.push(blip);
			} catch (e) {
				logger.error(e);
			}
		});
		let gang = 0;
	
	} catch (e) {
		logger.error(e);
	}
});

var isCapture = false;
var captureAtt = 0;
var captureDef = 0;
var captureMin = 0;
var captureSec = 0;
var captureAttacker = '';
var captureDefer = '';

var bizWarBlip;

var gangCapture = mp.browsers.new('http://package/interfaces/ui/GangCapture/index.html');

mp.events.add('sendCaptureInformation', function (att, def, min, sec, attacker, defer) {
    captureAtt = att;
    captureDef = def;
    captureMin = min;
    captureSec = sec;
    captureAttacker = attacker;
    captureDefer = defer;
});

mp.events.add('addBizwarBlip', function(x, y, z){
	try {
		bizWarBlip = mp.game1.ui.addBlipForRadius(x, y, z, 100);
		mp.game1.invoke(getNative("SET_BLIP_SPRITE"), bizWarBlip, 9);
		mp.game1.invoke(getNative("SET_BLIP_ALPHA"), bizWarBlip, 200);
		mp.game1.invoke(getNative("SET_BLIP_COLOUR"), bizWarBlip, 6);
	} catch (e) {
		logger.error(e);
	}
});

mp.events.add('delBizwarBlip', function(){
	try {
		if (bizWarBlip !== undefined && bizWarBlip != null) {
			mp.game1.ui.removeBlip(bizWarBlip);
			//mp.game1.invoke(getNative("SET_BLIP_ALPHA"), bizWarBlip, 0);
			//bizWarBlip.destroy();
		}
	} catch (e) {
		logger.error(e);
	}
});

mp.events.add('captureHud', function (argument) {
	try {
		if (argument)
			gangCapture.execute('show();');
		else
			gangCapture.execute('hide();');

		isCapture = argument;
	} catch (e) {
		logger.error(e);
	}
});

mp.events.add('setZoneColor', function (id, color) {
	try {
		if (blipss.length == 0) return;
	
		blipss[id].setColour(color);
		
	} catch (e) {
		logger.error(e);
	}
});
mp.events.add('setZoneFlash', function (id, state, color) {
	try {
		if (id) {
			if (blipss.length === 1 || blipss.length === 0) {
				if (state) {
					const blip = mp.game1.ui.addBlipForRadius(positions[id].position.x, positions[id].position.y, positions[id].position.z, 50);
					mp.game1.invoke(getNative("SET_BLIP_SPRITE"), blip, 5);
					mp.game1.invoke(getNative("SET_BLIP_ALPHA"), blip, 90);
					mp.game1.invoke(getNative("SET_BLIP_COLOUR"), blip, color);
					blipss[id] = blip;
				} else {
					if (blipss.length == 0) return;
					mp.game1.invoke(getNative("SET_BLIP_ALPHA"), blipss[id], 0);
				}
			}

			blipss[id].setFlashTimer(1000)
			blipss[id].setFlashes(state);
		}
	} catch (e) {
		logger.error(e);
	}
});


mp.events.add('render', () => {
	try {
		if (isCapture) {
			gangCapture.execute(`gangCapture.init('${captureMin}', '${captureSec}', '${captureAttacker}', '${captureDefer}', ${captureAtt}, ${captureDef});`);
		}

		if (blipss.length === 0) return;
		for (let i = 0; i < blipss.length; i++) {
			mp.game1.invoke("0xF87683CDF73C3F6E", blipss[i], positions[i].position.r);
		}
	} catch (e) {
		logger.error(e);
	}
});

