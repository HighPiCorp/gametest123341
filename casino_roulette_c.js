

let lpCasinoTable = null;
let casinoTableToJoin = null;
let casinoSeatToJoin = null;
let goToSeatInterval = null;
let interactingWithTable = null;
let rouletteCamera = null;
let canDoBets = false;
let betObject = null;
let closestChipSpot = null;
let interactingWithTableTimeout = null;






let tablesPos =
[
	[ "vw_prop_casino_roulette_01", 1144.4254150390625, 269.3034973144531, -52.840850830078125 ],
	[ "vw_prop_casino_roulette_01", 1151.2305908203125, 263.14093017578125, -52.840850830078125 ],
	[ "vw_prop_casino_roulette_01b", 1148.9163818359375, 248.62892150878906, -52.03075408935547 ],
	[ "vw_prop_casino_roulette_01b", 1143.677978515625, 251.36131286621094, -52.0307502746582 ],
	[ "vw_prop_casino_roulette_01b", 1133.1802978515625, 262.3916320800781, -52.03075408935547 ],
	[ "vw_prop_casino_roulette_01b", 1129.9976806640625, 266.93695068359375, -52.0307502746582 ],
];

let tablesBets =
[
	[ 500, 2500 ],
	[ 1000, 5000 ],
	[ 3000, 15000 ],
	[ 7000, 35000 ],
	[ 10000, 50000 ],
	[ 20000, 100000 ]
];

let pedModels =
[
	"S_M_Y_Casino_01", "S_F_Y_Casino_01", "S_M_Y_Casino_01", "S_F_Y_Casino_01", "S_M_Y_Casino_01", "S_F_Y_Casino_01"
]





mp.game.streaming.requestIpl('vw_casino_main');
mp.blips.new(617, new mp.Vector3(935.8140869140625, 46.942176818847656, 81.09580993652344), { name: "Diamond Casino", color: 3, shortRange: true, scale: 1.0 });


//обмен фишек
mp.peds.new(mp.game.joaat("S_F_Y_Casino_01"), new mp.Vector3(1117.7528076171875, 220.12098693847656, -49.43511962890625), 90.0, 0);
//mp.labels.new("Beverly", new mp.Vector3(1117.7528076171875, 220.12098693847656, -49.43511962890625+1.1), { los: true, font: 0, drawDistance: 5.0 } );



// [ //barman m
	// [ 0, 3, 0, 0],
	// [ 1, 1, 0, 0],
	// [ 2, 3, 0, 0],
	// [ 3, 1, 0, 0],
	// [ 4, 0, 0, 0],
	// [ 6, 1, 0, 0],
	// [ 7, 2, 0, 0],
	// [ 8, 3, 0, 0],
	// [ 10, 1, 0, 0],
	// [ 11, 1, 0, 0],
// ],
// [ //barman m
		// [ 0, 2, 0, 0],
		// [ 1, 1, 0, 0],
		// [ 2, 3, 0, 0],
		// [ 3, 1, 3, 0],
		// [ 4, 0, 0, 0],
		// [ 6, 1, 0, 0],
		// [ 7, 2, 0, 0],
		// [ 8, 3, 0, 0],
		// [ 10, 1, 0, 0],
		// [ 11, 1, 0, 0],
	// ]
	// [//barman m
		// [ 0, 4, 1, 0],
		// [ 1, 1, 0, 0],
		// [ 2, 4, 0, 0],
		// [ 3, 1, 0, 0],
		// [ 4, 0, 0, 0],
		// [ 6, 1, 0, 0],
		// [ 7, 2, 0, 0],
		// [ 8, 3, 0, 0],
		// [ 10, 1, 0, 0],
		// [ 11, 1, 0, 0],
	// ]
	// [//barman f
		// [ 0, 1, 1, 0],
		// [ 1, 0, 0, 0],
		// [ 2, 1, 0, 0],
		// [ 3, 0, 3, 0],
		// [ 4, 0, 0, 0],
		// [ 6, 0, 0, 0],
		// [ 7, 0, 0, 0],
		// [ 8, 0, 0, 0],
		// [ 10, 0, 0, 0],
		// [ 11, 0, 0, 0],
	// ]
	// [//barman f
		// [ 0, 1, 1, 0],
		// [ 1, 0, 0, 0],
		// [ 2, 1, 1, 0],
		// [ 3, 1, 3, 0],
		// [ 4, 0, 0, 0],
		// [ 6, 0, 0, 0],
		// [ 7, 2, 0, 0],
		// [ 8, 1, 0, 0],
		// [ 10, 0, 0, 0],
		// [ 11, 0, 0, 0],
	// ]
	// [//barman f
		// [ 0, 3, 0, 0],
		// [ 1, 0, 0, 0],
		// [ 2, 3, 0, 0],
		// [ 3, 0, 1, 0],
		// [ 4, 1, 0, 0],
		// [ 6, 1, 0, 0],
		// [ 7, 1, 0, 0],
		// [ 8, 0, 0, 0],
		// [ 10, 0, 0, 0],
		// [ 11, 0, 0, 0],
	// ]
	// [//barman f
		// [ 0, 3, 1, 0],
		// [ 1, 0, 0, 0],
		// [ 2, 3, 1, 0],
		// [ 3, 1, 1, 0],
		// [ 4, 1, 0, 0],
		// [ 6, 1, 0, 0],
		// [ 7, 2, 0, 0],
		// [ 8, 1, 0, 0],
		// [ 10, 0, 0, 0],
		// [ 11, 0, 0, 0],
	// ]

let pedModelVariations =
[
	[ //S_M_Y_Casino_01
		[ 0, 2, 2, 0],
		[ 1, 1, 0, 0],
		[ 2, 4, 0, 0],
		[ 3, 0, 3, 0],
		[ 4, 0, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 2, 0, 0],
		[ 8, 1, 0, 0],
		[ 10, 1, 0, 0],
		[ 11, 1, 0, 0]
	],
	[//S_F_Y_Casino_01
		[ 0, 2, 0, 0],
		[ 1, 0, 0, 0],
		[ 2, 2, 0, 0],
		[ 3, 2, 3, 0],
		[ 4, 0, 0, 0],
		[ 6, 0, 0, 0],
		[ 7, 0, 0, 0],
		[ 8, 2, 0, 0],
		[ 10, 0, 0, 0],
		[ 11, 0, 0, 0]
	],
	[ //S_M_Y_Casino_01
		[ 0, 2, 1, 0],
		[ 1, 1, 0, 0],
		[ 2, 2, 0, 0],
		[ 3, 0, 3, 0],
		[ 4, 0, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 2, 0, 0],
		[ 8, 1, 0, 0],
		[ 10, 1, 0, 0],
		[ 11, 1, 0, 0]
	],
	[//S_F_Y_Casino_01
		[ 0, 2, 1, 0],
		[ 1, 0, 0, 0],
		[ 2, 2, 1, 0],
		[ 3, 3, 3, 0],
		[ 4, 1, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 2, 0, 0],
		[ 8, 3, 0, 0],
		[ 10, 0, 0, 0],
		[ 11, 0, 0, 0]
	],
	[ //S_M_Y_Casino_01
		[ 0, 4, 2, 0],
		[ 1, 1, 0, 0],
		[ 2, 3, 0, 0],
		[ 3, 0, 0, 0],
		[ 4, 0, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 2, 0, 0],
		[ 8, 1, 0, 0],
		[ 10, 1, 0, 0],
		[ 11, 1, 0, 0]
	],
	[//S_F_Y_Casino_01
		[ 0, 4, 0, 0],
		[ 1, 0, 0, 0],
		[ 2, 4, 0, 0],
		[ 3, 2, 1, 0],
		[ 4, 1, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 1, 0, 0],
		[ 8, 2, 0, 0],
		[ 10, 0, 0, 0],
		[ 11, 0, 0, 0]
	],
	[ //S_M_Y_Casino_01 (not used)
		[ 0, 4, 0, 0],
		[ 1, 1, 0, 0],
		[ 2, 0, 0, 0],
		[ 3, 0, 0, 0],
		[ 4, 0, 0, 0],
		[ 6, 1, 0, 0],
		[ 7, 2, 0, 0],
		[ 8, 1, 0, 0],
		[ 10, 1, 0, 0],
		[ 11, 1, 0, 0]
	]
]
/*let tableSeatsPos =
    [
        [-0.7, -1.28, 1, 0],
        [0.775, -1.68, 1, 0],
        [1.8, -0.63, 1, 90],
        [1.27, 1.05, 1, 180]
    ]*/
let tableSeatsPos =
[
	[-0.7, -1.12, 1, 0],
	[0.775, -1.6, 1, 0],
	[1.6, -0.63, 1, 90],
	[1.27, 0.85, 1, 180]
]



let rouletteData = [];

/*for(var i=0; i < tablesPos.length; i++)
{
	rouletteData[i] = {};
	rouletteData[i].table = mp.objects.new(mp.game.joaat(tablesPos[i][0]), new mp.Vector3(tablesPos[i][1], tablesPos[i][2], tablesPos[i][3]));
	rouletteData[i].ball = mp.objects.new(87196104, new mp.Vector3(tablesPos[i][1]-0.734742, tablesPos[i][2]-0.16617, tablesPos[i][3]));
	rouletteData[i].ped = mp.peds.new(mp.game.joaat(pedModels[i]), new mp.Vector3(tablesPos[i][1], tablesPos[i][2]+0.7, tablesPos[i][3]+1), 180, 0); //-0.001587
	rouletteData[i].ped.croupier = i;

	for(var c=0; c < tableSeatsPos.length; c++)
	{
		var newShape = mp.colshapes.newSphere(tablesPos[i][1]+tableSeatsPos[c][0], tablesPos[i][2]+tableSeatsPos[c][1], tablesPos[i][3]+tableSeatsPos[c][2], 0.5);
		newShape.casinoTable = i;
		newShape.seatID = c;
	}

	for(var c=0; c < pedModelVariations[i].length; c++)
	{
		rouletteData[i].ped.setComponentVariation(pedModelVariations[i][c][0], pedModelVariations[i][c][1], pedModelVariations[i][c][2], pedModelVariations[i][c][3]);
	}
}

mp.events.add('playerEnterColshape', (shape) => {

	if(shape.casinoTable !== undefined && lpCasinoTable == null && interactingWithTable == null)
	{
		casinoTableToJoin = shape.casinoTable;
		casinoSeatToJoin = shape.seatID;
//		mp.game.ui.setTextComponentFormat('STRING');
//		mp.game.ui.addTextComponentSubstringPlayerName('~INPUT_CONTEXT~ сесть за стол~n~Min. ставка: '+tablesBets[casinoTableToJoin][0]+'~n~Max. ставка: '+tablesBets[casinoTableToJoin][1]);
//		mp.game.ui.displayHelpTextFromStringLabel(0, false, true, 5000);

		mp.game.audio.playSound(-1, "BACK", "HUD_AMMO_SHOP_SOUNDSET", true, 0, true);
		mp.game.graphics.notify(`~g~[E]~s~ сесть за стол~n~Ставка от ~b~${tablesBets[casinoTableToJoin][0]}~s~ до ~b~${tablesBets[casinoTableToJoin][1]}~s~ фишек`);
	}
});

mp.events.add('playerExitColshape', (shape) => {
	if(shape.casinoTable !== undefined)
	{
		casinoTableToJoin = null;
		casinoSeatToJoin = null;
	}
});
*/






let animInfo = null;
mp.events.add("initRoulette", (jsonString) =>
{
	animInfo = JSON.parse(jsonString);

	loadAnim(animInfo.tableLib);
	loadAnim(animInfo.dealerLib);
	loadAnim(animInfo.dealerLib+"_female");

	mp.events.add("render", rouletteRender);

	mp.events.add('entityStreamIn', (entity) => {
		if(entity.type == "ped" && entity.croupier != null)
		{
			if(entity.model == mp.game.joaat('S_M_Y_Casino_01')) entity.taskPlayAnim(animInfo.dealerLib, "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
			else entity.taskPlayAnim(animInfo.dealerLib+"_female", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);



			var id = entity.croupier;

			rouletteData[id].ball.position = new mp.Vector3(tablesPos[id][1]-0.734742, tablesPos[id][2]-0.16617, tablesPos[id][3]);

			for(var c=0; c < pedModelVariations[id].length; c++)
			{
				entity.setComponentVariation(pedModelVariations[id][c][0], pedModelVariations[id][c][1], pedModelVariations[id][c][2], pedModelVariations[id][c][3]);
			}
		}
	});
});

function rouletteRender()
{

	for(var i=0; i < rouletteData.length; i++)
	{
		if(rouletteData[i].table.isPlayingAnim(animInfo.tableLib, animInfo.tableStart, 3))
		{
			if(rouletteData[i].table.getAnimCurrentTime(animInfo.tableLib, animInfo.tableStart) > 0.9425)
			{
				rouletteData[i].table.playAnim(animInfo.tableMain, animInfo.tableLib, 1000.0, true, true, true, 0, animInfo.speed);
			}
		}

		if(rouletteData[i].ball.isPlayingAnim(animInfo.tableLib, animInfo.ballStart, 3))
		{
			if(rouletteData[i].ball.getAnimCurrentTime(animInfo.tableLib, animInfo.ballStart) > 0.99)
			{

				//rouletteData[i].ball.position = new mp.Vector3(tablesPos[i][1]-0.734742, tablesPos[i][2]-0.16617, tablesPos[i][3]+1.0715);
                rouletteData[i].ball.position = new mp.Vector3(tablesPos[i][1]-0.554742, tablesPos[i][2]+0.08917, tablesPos[i][3]+1.0715);
				rouletteData[i].ball.rotation = new mp.Vector3(0.0, 0.0, animInfo.ballRot);

				rouletteData[i].ball.playAnim(animInfo.ballMain, animInfo.tableLib, 1000.0, true, true, false, 0, animInfo.speed);
			}
		}

		if(rouletteData[i].table.isPlayingAnim(animInfo.tableLib, animInfo.tableMain, 3))
		{

			if(rouletteData[i].table.getAnimCurrentTime(animInfo.tableLib, animInfo.tableMain) >= 0.9 && Date.now()-rouletteData[i].lastSpinTime > 1000)
			{
				rouletteData[i].spins++;
				rouletteData[i].lastSpinTime = Date.now();
			}
			if(rouletteData[i].spins == rouletteData[i].needSpins-1)
			{
				rouletteData[i].ball.setAnimSpeed(animInfo.tableLib, animInfo.ballMain, 0.71);
			}
			if(rouletteData[i].spins == rouletteData[i].needSpins && rouletteData[i].table.getAnimCurrentTime(animInfo.tableLib, animInfo.tableMain) > 0.99)
			{
				rouletteData[i].table.playAnim(rouletteData[i].endTable, animInfo.tableLib, 1000.0, false, true, true, 0, animInfo.speed);

				//rouletteData[i].ball.position = new mp.Vector3(tablesPos[i][1]-0.734742, tablesPos[i][2]-0.16617, tablesPos[i][3]+1.0715);
                rouletteData[i].ball.position = new mp.Vector3(tablesPos[i][1]-0.554742, tablesPos[i][2]+0.08917, tablesPos[i][3]+1.0715);
				rouletteData[i].ball.rotation = new mp.Vector3(0.0, 0.0, animInfo.ballRot);
				rouletteData[i].ball.playAnim(rouletteData[i].endBall, animInfo.tableLib, 1000.0, false, true, true, 0, animInfo.speed);

			}
		}
        //mp.gui.chat.push("pos X:"+rouletteData[i].ball.position.x+" Y:"+rouletteData[i].ball.position.y +" z:"+rouletteData[i].ball.position.z+" rot x:"+rouletteData[i].ball.rotation.x+" rot y:"+rouletteData[i].ball.rotation.y+" rot z:"+rouletteData[i].ball.rotation.z);
	}
}

mp.events.add('CLIENT::roulette:leave_table', function(){
	if(lpCasinoTable != null)
	{//mp.gui.chat.push("seat1  leave");
		//rouletteData[lpCasinoTable].table.setCollision(true, false);
		interactingWithTable = lpCasinoTable;
		lpCasinoTable = null;
		global.BLOCK_CONTROLS_DURING_ANIMATION = false;
		if(rouletteCamera != null) destroyRouletteCamera();
		if(canDoBets) canDoBets = false;
		mp.players.local.freezePosition(false);
		interactingWithTableTimeout = setTimeout(
			function()
			{
				interactingWithTable = null;
				interactingWithTableTimeout = null;
			},1500
		);
        for (let chip of ChipBets) {
            if(chip)
            {
                chip.destroy();
            }
        }
	}
});

let ChipBets = [];


mp.events.add("cancelInteractingWithTable", () =>
{
	rouletteData[interactingWithTable].table.setCollision(true, false);
	interactingWithTable = null;
	if(interactingWithTableTimeout != null)
	{
		clearTimeout(interactingWithTableTimeout);
		interactingWithTableTimeout = null;
	}
});

// function checkPlayerCanSit(table, seat)
// {
	// var heading = localplayer.getHeading();
	// if(mp.Vector3.getDistanceBetweenPoints3D(localplayer.position, new mp.Vector3(tablesPos[table][1]+tableSeatsPos[seat][0], tablesPos[table][2]+tableSeatsPos[seat][1], tablesPos[table][3]+tableSeatsPos[seat][2])) < 0.01 )
	// {
		// localplayer.setHeading(tableSeatsPos[seat][3]);
		// global.anyEvents.SendServer(() => mp.events.callRemote("occupyCasinoSeat", table, seat));
		// clearInterval(goToSeatInterval);
		// goToSeatInterval = null;

		// interactingWithTableTimeout = setTimeout(
			// function()
			// {
				// interactingWithTable = null;
				// interactingWithTableTimeout = null;
			// },5500
		// );
	// }
// }

mp.events.add('playerDeath', (player) =>
{
	if(player == localplayer)
	{
		if(interactingWithTable != null) interactingWithTable = null;
		if(global.BLOCK_CONTROLS_DURING_ANIMATION) global.BLOCK_CONTROLS_DURING_ANIMATION = false;
		if(rouletteCamera != null) destroyRouletteCamera();
		if(canDoBets) canDoBets = false;
	}
});


mp.events.add("playerSitAtCasinoTable", (player, tableID) => {

	if(player == localplayer)
	{
		lpCasinoTable = casinoTableToJoin;
    global.BLOCK_CONTROLS_DURING_ANIMATION = true;

		//showAlert('alert-blue', 'ЛКМ - сделать/повысить ставку</br>ПКМ - убрать/понизить ставку</br>F - вид на стол');
	}
	else
	{
		//rouletteData[tableID].table.setNoCollision(player.handle, false);
	}
});

// mp.events.add("playerLeaveCasinoTable", (player, tableID) => {

	// rouletteData[tableID].table.setCollision(true, false);
// });

mp.events.add("rouletteAllowBets", (toggle) => {
	canDoBets = toggle;
	if(toggle) mp.game.graphics.notify("Сделайте ставки");
	else mp.game.graphics.notify("Ставки сделаны");
});










mp.events.add("spinRouletteWheel", (table, needSpins, endTable, endBall) => {

	rouletteData[table].table.playAnim(animInfo.tableStart, animInfo.tableLib, 1000.0, false, true, true, 0, animInfo.speed); // loop, freezeLastFrame, ?

	rouletteData[table].ball.position = new mp.Vector3(tablesPos[table][1]-0.534742, tablesPos[table][2]+0.12217, tablesPos[table][3]+1.0715);
	rouletteData[table].ball.rotation = new mp.Vector3(0.0, 0.0, animInfo.ballRot);

	rouletteData[table].ball.playAnim(animInfo.ballStart, animInfo.tableLib, 1000.0, false, true, false, 0, animInfo.speed); // loop, freezeLastFrame, ?
	rouletteData[table].spins = 0;
	rouletteData[table].lastSpinTime = 0;
	rouletteData[table].needSpins = needSpins;
	rouletteData[table].endTable = endTable;
	rouletteData[table].endBall = endBall;

	if(rouletteData[table].ped.model == mp.game.joaat('S_M_Y_Casino_01')) rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib, "spin_wheel", 8.0, 1, -1, 2, 0.0, false, false, false);
	else rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib+"_female", "spin_wheel", 8.0, 1, -1, 2, 0.0, false, false, false);

	setTimeout(
		function()
		{
			if(rouletteData[table].ped.model == mp.game.joaat('S_M_Y_Casino_01')) rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib, "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
			else rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib+"_female", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
		}, 8000
	);
});

mp.events.add("setChipObject", (chip) =>
{
	var temp =mp.objects.new(mp.game.joaat("vw_prop_chip_100dollar_x1"), new mp.Vector3(tablesPos[lpCasinoTable][1]+tableChipsOffsets[chip][0], tablesPos[lpCasinoTable][2]+tableChipsOffsets[chip][1], tablesPos[lpCasinoTable][3]+tableChipsOffsets[chip][2]));
    setTimeout(
        function() {
            temp._chip = chip;
            ChipBets.push(temp);
        }, 200
	);

});
mp.events.add("RmChipObject", (chip) =>
{
    var object = ChipBets.find(obj => {
        return obj._chip===chip
    });
	if (object!==undefined)
    {
        if (mp.objects.exists(object)) {

            object.destroy();
        }
    }
});
mp.events.add("clearRouletteTable", (table) =>
{
	if(rouletteData[table].ped.model == mp.game.joaat('S_M_Y_Casino_01')) rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib, "clear_chips_zone2", 8.0, 1, -1, 2, 0.0, false, false, false);
	else rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib+"_female", "clear_chips_zone2", 8.0, 1, -1, 2, 0.0, false, false, false);
    for (let chip of ChipBets) {
    	if(mp.objects.exists(chip))
		{
			chip.destroy();
		}
	}

	setTimeout(
		function()
		{
			if(rouletteData[table].ped.model == mp.game.joaat('S_M_Y_Casino_01')) rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib, "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
			else rouletteData[table].ped.taskPlayAnim(animInfo.dealerLib+"_female", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
		}, 2000
	);
});

mp.keys.bind(Keys.VK_F, true, () =>  // F
{

	if(localplayer.isDead() || mp.gui.cursor.visible || interactingWithTable != null || lpCasinoTable == null) return false;

	if(rouletteCamera != null)
	{
		destroyRouletteCamera();
	}
	else
	{
		rouletteCamera = mp.cameras.new('default', new mp.Vector3(tablesPos[lpCasinoTable][1], tablesPos[lpCasinoTable][2]-1, tablesPos[lpCasinoTable][3]+3), new mp.Vector3(0,0,0), 45);
		rouletteCamera.setRot(-75.0, 0.0, 0.0, 2);
		rouletteCamera.setActive(true);
		mp.game.cam.renderScriptCams(true, false, 0, true, false);
	}
});

function destroyRouletteCamera()
{
	rouletteCamera.destroy(true);
	rouletteCamera = null;
    mp.game.cam.renderScriptCams(false, false, 0, true, false);
}

mp.events.add('render', () =>
{
	if(canDoBets && rouletteCamera && betObject == null)
	{
		betObject = mp.objects.new(mp.game.joaat("vw_prop_chip_100dollar_x1"), new mp.Vector3(tablesPos[lpCasinoTable][1], tablesPos[lpCasinoTable][2], tablesPos[lpCasinoTable][3]));
		betObject.setCollision(false, false);
	}

	if(betObject != null)
	{
		if(!canDoBets || rouletteCamera == null)
		{
			betObject.destroy();
			betObject = null;
			clearTableMarkers();
		}
	}

	if(rouletteCamera != null && lpCasinoTable != null)
	{
		if(betObject != null)
		{
			if(mp.game.controls.isDisabledControlJustReleased(0, 25) && !mp.gui.cursor.visible) // ПКМ
			{
				if(closestChipSpot != null) global.anyEvents.SendServer(() => mp.events.callRemote("removeRouletteBet", closestChipSpot));
			}

			if(mp.game.controls.isDisabledControlJustReleased(0, 24) && !mp.gui.cursor.visible) // ЛКМ
			{
				if(closestChipSpot != null) global.anyEvents.SendServer(() => mp.events.callRemote("makeRouletteBet", closestChipSpot));
			}

			let drawObj = getCameraHitCoord();
			if(drawObj != null)
			{
				// let height = betObject.getHeight(editorFocusObject.position.x, editorFocusObject.position.y, editorFocusObject.position.z, false, true);
				//drawObj.position.z += height / 2;
				drawObj.position.z = tablesPos[lpCasinoTable][3]+0.95;
				betObject.setCoordsNoOffset(drawObj.position.x, drawObj.position.y, drawObj.position.z, false, false, false);

				getClosestChipSpot(new mp.Vector3(drawObj.position.x, drawObj.position.y, drawObj.position.z));
			}
		}

		let rightAxisX = mp.game.controls.getDisabledControlNormal(0, 220);
		let rightAxisY = mp.game.controls.getDisabledControlNormal(0, 221);

		let leftAxisX = 0;
		let leftAxisY = 0;

		let pos = rouletteCamera.getCoord();
		let rr = rouletteCamera.getDirection();
		let vector = new mp.Vector3(0, 0, 0);
		vector.x = rr.x * leftAxisY;
		vector.y = rr.y * leftAxisY;
		vector.z = rr.z * leftAxisY;

		let upVector = new mp.Vector3(0, 0, 1);
		let rightVector = getCrossProduct(getNormalizedVector(rr), getNormalizedVector(upVector));
		rightVector.x *= leftAxisX * 0.5;
		rightVector.y *= leftAxisX * 0.5;
		rightVector.z *= leftAxisX * 0.5;

		let rot = rouletteCamera.getRot(2);

		let rotx = rot.x + rightAxisY * -5.0;
		if(rotx > 89) rotx = 89;
		if(rotx < -89) rotx = -89;

		rouletteCamera.setRot(rotx, 0.0, rot.z + rightAxisX * -5.0, 2);
	}
});





function getCameraHitCoord()
{
	let position = rouletteCamera.getCoord();
	let direction = rouletteCamera.getDirection();
	let farAway = new mp.Vector3((direction.x * 150) + position.x, (direction.y * 150) + position.y, (direction.z * 150) + position.z);

	let hitData = mp.raycasting.testPointToPoint(position, farAway, mp.players.local);

	if(hitData != undefined)
	{
		return hitData;
	}
	return null;
}

function getNormalizedVector(vector)
{
	let mag = Math.sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
	vector.x = vector.x / mag;
	vector.y = vector.y / mag;
	vector.z = vector.z / mag;
	return vector;
}

function getCrossProduct(v1, v2)
{
	let vector = new mp.Vector3(0, 0, 0);
	vector.x = v1.y * v2.z - v1.z * v2.y;
	vector.y = v1.z * v2.x - v1.x * v2.z;
	vector.z = v1.x * v2.y - v1.y * v2.x;
	return vector;
}







let tableMarkers = [];
const tableMarkersOffsets =
{
	"0": [-0.137451171875, -0.146942138671875, 0.9449996948242188],
	"00": [-0.1387939453125, 0.10546875, 0.9449996948242188],
	"1": [-0.0560302734375, -0.1898193359375, 0.9449996948242188],
	"2": [-0.0567626953125, -0.024017333984375, 0.9449996948242188],
	"3": [-0.056884765625, 0.141632080078125, 0.9449996948242188],
	"4": [0.02392578125, -0.187347412109375, 0.9449996948242188],
	"5": [0.0240478515625, -0.02471923828125, 0.9449996948242188],
	"6": [0.02392578125, 0.1422119140625, 0.9449996948242188],
	"7": [0.1038818359375, -0.18902587890625, 0.9449996948242188],
	"8": [0.1044921875, -0.023834228515625, 0.9449996948242188],
	"9": [0.10546875, 0.1419677734375, 0.9449996948242188],
	"10": [0.18701171875, -0.188385009765625, 0.9449996948242188],
	"11": [0.18603515625, -0.0238037109375, 0.9449996948242188],
	"12": [0.1851806640625, 0.143157958984375, 0.9449996948242188],
	"13": [0.2677001953125, -0.18780517578125, 0.9449996948242188],
	"14": [0.26806640625, -0.02301025390625, 0.9449996948242188],
	"15": [0.26611328125, 0.143310546875, 0.9449996948242188],
	"16": [0.3497314453125, -0.18829345703125, 0.9449996948242188],
	"17": [0.349609375, -0.023101806640625, 0.9449996948242188],
	"18": [0.3497314453125, 0.142242431640625, 0.9449996948242188],
	"19": [0.4307861328125, -0.18829345703125, 0.9449996948242188],
	"20": [0.4312744140625, -0.02392578125, 0.9449996948242188],
	"21": [0.431884765625, 0.1416015625, 0.9449996948242188],
	"22": [0.51220703125, -0.188873291015625, 0.9449996948242188],
	"23": [0.5123291015625, -0.023773193359375, 0.9449996948242188],
	"24": [0.511962890625, 0.14215087890625, 0.9449996948242188],
	"25": [0.5931396484375, -0.18890380859375, 0.9449996948242188],
	"26": [0.59375, -0.023651123046875, 0.9449996948242188],
	"27": [0.59375, 0.14080810546875, 0.9449996948242188],
	"28": [0.67529296875, -0.189849853515625, 0.9449996948242188],
	"29": [0.6751708984375, -0.02337646484375, 0.9449996948242188],
	"30": [0.674560546875, 0.141845703125, 0.9449996948242188],
	"31": [0.756591796875, -0.18798828125, 0.9449996948242188],
	"32": [0.7547607421875, -0.0234375, 0.9449996948242188],
	"33": [0.7554931640625, 0.14263916015625, 0.9449996948242188],
	"34": [0.836669921875, -0.188323974609375, 0.9449996948242188],
	"35": [0.8365478515625, -0.0244140625, 0.9449996948242188],
	"36": [0.8359375, 0.14276123046875, 0.9449996948242188]
};

const tableChipsOffsets =
[
	[-0.154541015625, -0.150604248046875, 0.9449996948242188, ["0"]],
	[-0.1561279296875, 0.11505126953125, 0.9449996948242188, ["00"]],
	[-0.059326171875, -0.18701171875, 0.9449996948242188, ["1"]],
	[-0.058349609375, -0.019378662109375, 0.9449996948242188, ["2"]],
	[-0.0587158203125, 0.142059326171875, 0.9449996948242188, ["3"]],
	[0.02294921875, -0.1920166015625, 0.9449996948242188, ["4"]],
	[0.023193359375, -0.01947021484375, 0.9449996948242188, ["5"]],
	[0.024658203125, 0.147369384765625, 0.9449996948242188, ["6"]],
	[0.105224609375, -0.1876220703125, 0.9449996948242188, ["7"]],
	[0.1055908203125, -0.028472900390625, 0.9449996948242188, ["8"]],
	[0.10400390625, 0.147430419921875, 0.9449996948242188, ["9"]],
	[0.187744140625, -0.191802978515625, 0.9449996948242188, ["10"]],
	[0.1866455078125, -0.02667236328125, 0.9449996948242188, ["11"]],
	[0.1842041015625, 0.145965576171875, 0.9449996948242188, ["12"]],
	[0.2696533203125, -0.182464599609375, 0.9449996948242188, ["13"]],
	[0.265869140625, -0.027862548828125, 0.9449996948242188, ["14"]],
	[0.2667236328125, 0.138946533203125, 0.9449996948242188, ["15"]],
	[0.35009765625, -0.186126708984375, 0.9449996948242188, ["16"]],
	[0.348876953125, -0.027740478515625, 0.9449996948242188, ["17"]],
	[0.3497314453125, 0.14715576171875, 0.9449996948242188, ["18"]],
	[0.43212890625, -0.17864990234375, 0.9449996948242188, ["19"]],
	[0.4337158203125, -0.02508544921875, 0.9449996948242188, ["20"]],
	[0.430419921875, 0.138336181640625, 0.9449996948242188, ["21"]],
	[0.51416015625, -0.18603515625, 0.9449996948242188, ["22"]],
	[0.5135498046875, -0.02301025390625, 0.9449996948242188, ["23"]],
	[0.5146484375, 0.14239501953125, 0.9449996948242188, ["24"]],
	[0.59130859375, -0.192413330078125, 0.9449996948242188, ["25"]],
	[0.596923828125, -0.022216796875, 0.9449996948242188, ["26"]],
	[0.5924072265625, 0.14385986328125, 0.9449996948242188, ["27"]],
	[0.6749267578125, -0.187286376953125, 0.9449996948242188, ["28"]],
	[0.67431640625, -0.0262451171875, 0.9449996948242188, ["29"]],
	[0.6756591796875, 0.140594482421875, 0.9449996948242188, ["30"]],
	[0.7542724609375, -0.19415283203125, 0.9449996948242188, ["31"]],
	[0.7542724609375, -0.01898193359375, 0.9449996948242188, ["32"]],
	[0.75439453125, 0.1448974609375, 0.9449996948242188, ["33"]],
	[0.8392333984375, -0.18951416015625, 0.9449996948242188, ["34"]],
	[0.837646484375, -0.023468017578125, 0.9449996948242188, ["35"]],
	[0.8380126953125, 0.14227294921875, 0.9449996948242188, ["36"]],
	[-0.1368408203125, -0.02099609375, 0.9449996948242188, ["0","00"]],
	[-0.055419921875, -0.105804443359375, 0.9449996948242188, ["1","2"]],
	[-0.0567626953125, 0.058624267578125, 0.9449996948242188, ["2","3"]],
	[0.02587890625, -0.10498046875, 0.9449996948242188, ["4","5"]],
	[0.0244140625, 0.058837890625, 0.9449996948242188, ["5","6"]],
	[0.100341796875, -0.10382080078125, 0.9449996948242188, ["7","8"]],
	[0.1064453125, 0.06011962890625, 0.9449996948242188, ["8","9"]],
	[0.19189453125, -0.1060791015625, 0.9449996948242188, ["10","11"]],
	[0.1856689453125, 0.05438232421875, 0.9449996948242188, ["11","12"]],
	[0.27099609375, -0.10870361328125, 0.9449996948242188, ["13","14"]],
	[0.2667236328125, 0.058502197265625, 0.9449996948242188, ["14","15"]],
	[0.3463134765625, -0.107696533203125, 0.9449996948242188, ["16","17"]],
	[0.34814453125, 0.0556640625, 0.9449996948242188, ["17","18"]],
	[0.42822265625, -0.109130859375, 0.9449996948242188, ["19","20"]],
	[0.4302978515625, 0.0550537109375, 0.9449996948242188, ["20","21"]],
	[0.511474609375, -0.107421875, 0.9449996948242188, ["22","23"]],
	[0.512451171875, 0.0614013671875, 0.9449996948242188, ["23","24"]],
	[0.5980224609375, -0.107147216796875, 0.9449996948242188, ["25","26"]],
	[0.596435546875, 0.0574951171875, 0.9449996948242188, ["26","27"]],
	[0.673828125, -0.106903076171875, 0.9449996948242188, ["28","29"]],
	[0.6751708984375, 0.058685302734375, 0.9449996948242188, ["29","30"]],
	[0.7532958984375, -0.1102294921875, 0.9449996948242188, ["31","32"]],
	[0.750244140625, 0.06103515625, 0.9449996948242188, ["32","33"]],
	[0.834716796875, -0.108978271484375, 0.9449996948242188, ["34","35"]],
	[0.836181640625, 0.05828857421875, 0.9449996948242188, ["35","36"]],
	[-0.0167236328125, -0.187042236328125, 0.9449996948242188, ["1","4"]],
	[-0.0167236328125, -0.02154541015625, 0.9449996948242188, ["2","5"]],
	[-0.0164794921875, 0.140350341796875, 0.9449996948242188, ["3","6"]],
	[0.064453125, -0.1865234375, 0.9449996948242188, ["4","7"]],
	[0.06494140625, -0.01727294921875, 0.9449996948242188, ["5","8"]],
	[0.068603515625, 0.13873291015625, 0.9449996948242188, ["6","9"]],
	[0.144287109375, -0.184173583984375, 0.9449996948242188, ["7","10"]],
	[0.14501953125, -0.024139404296875, 0.9449996948242188, ["8","11"]],
	[0.14501953125, 0.136993408203125, 0.9449996948242188, ["9","12"]],
	[0.2291259765625, -0.18670654296875, 0.9449996948242188, ["10","13"]],
	[0.227783203125, -0.0242919921875, 0.9449996948242188, ["11","14"]],
	[0.2286376953125, 0.14398193359375, 0.9449996948242188, ["12","15"]],
	[0.308349609375, -0.18792724609375, 0.9449996948242188, ["13","16"]],
	[0.308837890625, -0.02374267578125, 0.9449996948242188, ["14","17"]],
	[0.3099365234375, 0.14410400390625, 0.9449996948242188, ["15","18"]],
	[0.39111328125, -0.192230224609375, 0.9449996948242188, ["16","19"]],
	[0.390869140625, -0.0189208984375, 0.9449996948242188, ["17","20"]],
	[0.39111328125, 0.146514892578125, 0.9449996948242188, ["18","21"]],
	[0.470947265625, -0.188690185546875, 0.9449996948242188, ["19","22"]],
	[0.4705810546875, -0.0205078125, 0.9449996948242188, ["20","23"]],
	[0.4725341796875, 0.140167236328125, 0.9449996948242188, ["21","24"]],
	[0.5491943359375, -0.189666748046875, 0.9449996948242188, ["22","25"]],
	[0.548095703125, -0.022552490234375, 0.9449996948242188, ["23","26"]],
	[0.553955078125, 0.1446533203125, 0.9449996948242188, ["24","27"]],
	[0.6324462890625, -0.191131591796875, 0.9449996948242188, ["25","28"]],
	[0.635498046875, -0.0224609375, 0.9449996948242188, ["26","29"]],
	[0.6392822265625, 0.139190673828125, 0.9449996948242188, ["27","30"]],
	[0.71533203125, -0.187042236328125, 0.9449996948242188, ["28","31"]],
	[0.7181396484375, -0.02447509765625, 0.9449996948242188, ["29","32"]],
	[0.7152099609375, 0.138153076171875, 0.9449996948242188, ["30","33"]],
	[0.7969970703125, -0.1904296875, 0.9449996948242188, ["31","34"]],
	[0.7955322265625, -0.024871826171875, 0.9449996948242188, ["32","35"]],
	[0.7960205078125, 0.137664794921875, 0.9449996948242188, ["33","36"]],
	[-0.0560302734375, -0.271240234375, 0.9449996948242188, ["1","2","3"]],
	[0.024658203125, -0.271392822265625, 0.9449996948242188, ["4","5","6"]],
	[0.1051025390625, -0.272125244140625, 0.9449996948242188, ["7","8","9"]],
	[0.1898193359375, -0.27001953125, 0.9449996948242188, ["10","11","12"]],
	[0.2696533203125, -0.271697998046875, 0.9449996948242188, ["13","14","15"]],
	[0.351318359375, -0.268096923828125, 0.9449996948242188, ["16","17","18"]],
	[0.4287109375, -0.269561767578125, 0.9449996948242188, ["19","20","21"]],
	[0.5098876953125, -0.2716064453125, 0.9449996948242188, ["22","23","24"]],
	[0.5960693359375, -0.271148681640625, 0.9449996948242188, ["25","26","27"]],
	[0.67724609375, -0.268524169921875, 0.9449996948242188, ["28","29","30"]],
	[0.7523193359375, -0.27227783203125, 0.9449996948242188, ["31","32","33"]],
	[0.8382568359375, -0.272125244140625, 0.9449996948242188, ["34","35","36"]],
	[-0.017333984375, -0.106170654296875, 0.9449996948242188, ["1","2","4","5"]],
	[-0.0162353515625, 0.060882568359375, 0.9449996948242188, ["2","3","5","6"]],
	[0.06591796875, -0.110107421875, 0.9449996948242188, ["4","5","7","8"]],
	[0.0653076171875, 0.060028076171875, 0.9449996948242188, ["5","6","8","9"]],
	[0.146484375, -0.10888671875, 0.9449996948242188, ["7","8","10","11"]],
	[0.1451416015625, 0.057159423828125, 0.9449996948242188, ["8","9","11","12"]],
	[0.22705078125, -0.1092529296875, 0.9449996948242188, ["10","11","13","14"]],
	[0.22802734375, 0.059356689453125, 0.9449996948242188, ["11","12","14","15"]],
	[0.307373046875, -0.1043701171875, 0.9449996948242188, ["13","14","16","17"]],
	[0.309814453125, 0.05584716796875, 0.9449996948242188, ["14","15","17","18"]],
	[0.3919677734375, -0.111083984375, 0.9449996948242188, ["16","17","19","20"]],
	[0.3924560546875, 0.0596923828125, 0.9449996948242188, ["17","18","20","21"]],
	[0.471923828125, -0.1044921875, 0.9449996948242188, ["19","20","22","23"]],
	[0.4698486328125, 0.060028076171875, 0.9449996948242188, ["20","21","23","24"]],
	[0.5531005859375, -0.106170654296875, 0.9449996948242188, ["22","23","25","26"]],
	[0.5546875, 0.059417724609375, 0.9449996948242188, ["23","24","26","27"]],
	[0.633544921875, -0.101531982421875, 0.9449996948242188, ["25","26","28","29"]],
	[0.6337890625, 0.0579833984375, 0.9449996948242188, ["26","27","29","30"]],
	[0.7156982421875, -0.106292724609375, 0.9449996948242188, ["28","29","31","32"]],
	[0.7158203125, 0.0604248046875, 0.9449996948242188, ["29","30","32","33"]],
	[0.7947998046875, -0.108642578125, 0.9449996948242188, ["31","32","34","35"]],
	[0.7952880859375, 0.059051513671875, 0.9449996948242188, ["32","33","35","36"]],
	[-0.099609375, -0.2711181640625, 0.9449996948242188, ["0","00","1","2","3"]],
	[-0.0147705078125, -0.27154541015625, 0.9449996948242188, ["1","2","3","4","5","6"]],
	[0.064697265625, -0.270263671875, 0.9449996948242188, ["4","5","6","7","8","9"]],
	[0.144775390625, -0.271209716796875, 0.9449996948242188, ["7","8","9","10","11","12"]],
	[0.226806640625, -0.27142333984375, 0.9449996948242188, ["10","11","12","13","14","15"]],
	[0.306396484375, -0.27142333984375, 0.9449996948242188, ["13","14","15","16","17","18"]],
	[0.3895263671875, -0.27099609375, 0.9449996948242188, ["16","17","18","19","20","21"]],
	[0.468017578125, -0.275238037109375, 0.9449996948242188, ["19","20","21","22","23","24"]],
	[0.5509033203125, -0.2738037109375, 0.9449996948242188, ["22","23","24","25","26","27"]],
	[0.6336669921875, -0.27386474609375, 0.9449996948242188, ["25","26","27","28","29","30"]],
	[0.7144775390625, -0.272186279296875, 0.9449996948242188, ["28","29","30","31","32","33"]],
	[0.7935791015625, -0.272918701171875, 0.9449996948242188, ["31","32","33","34","35","36"]],
	[0.0643310546875, -0.304718017578125, 0.9449996948242188, ["1","2","3","4","5","6","7","8","9","10","11","12"]], //1st12
	[0.392822265625, -0.304779052734375, 0.9449996948242188, ["13","14","15","16","17","18","19","20","21","22","23","24"]],//2nd12
	[0.712158203125, -0.30303955078125, 0.9449996948242188, ["25","26","27","28","29","30","31","32","33","34","35","36"]],//3rd12
	[0.9222412109375, -0.185882568359375, 0.9449996948242188, ["1","4","7","10","13","16","19","22","25","28","31","34"]],//2to1
	[0.9229736328125, -0.0181884765625, 0.9449996948242188, ["2","5","8","11","14","17","20","23","26","29","32","35"]],//2to1
	[0.9248046875, 0.14849853515625, 0.9449996948242188, ["3","6","9","12","15","18","21","24","27","30","33","36"]],//2to1
	[-0.011474609375, -0.378875732421875, 0.9449996948242188, ["1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18"]],//1-18
	[0.142822265625, -0.375732421875, 0.9449996948242188, ["2","4","6","8","10","12","14","16","18","20","22","24","26","28","30","32","34","36"]], //even
	[0.308349609375, -0.37542724609375, 0.9449996948242188, ["1","3","5","7","9","12","14","16","18","19","21","23","25","27","30","32","34","36"]],//red
	[0.4713134765625, -0.376861572265625, 0.9449996948242188, ["2","4","6","8","10","11","13","15","17","20","22","24","26","28","29","31","33","35"]],//black
	[0.6341552734375, -0.376495361328125, 0.9449996948242188, ["1","3","5","7","9","11","13","15","17","19","21","23","25","27","29","31","33","35"]],//odd
	[0.7926025390625, -0.382232666015625, 0.9449996948242188, ["19","20","21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36"]]//19-36
];



function clearTableMarkers()
{
	for(var i=0; i < tableMarkers.length; i++)
	{
		tableMarkers[i].destroy();
	}
	tableMarkers = [];
}
mp.Vector3.getDistanceBetweenPoints3D = function (v1, v2){
    return Math.abs(Math.sqrt(Math.pow((v2.x - v1.x),2) +
        Math.pow((v2.y - v1.y),2)+
        Math.pow((v2.z - v1.z),2)));
}
function getClosestChipSpot(vector)
{
	var spot = null;
	var prevDistance = 0.05;
	var dist = null;

	for(var i=0; i < tableChipsOffsets.length; i++)
	{
		dist = mp.Vector3.getDistanceBetweenPoints3D(vector, new mp.Vector3(tablesPos[lpCasinoTable][1]+tableChipsOffsets[i][0], tablesPos[lpCasinoTable][2]+tableChipsOffsets[i][1], tablesPos[lpCasinoTable][3]+tableChipsOffsets[i][2]));
		if(dist <= prevDistance)
		{
			spot = i;
			prevDistance = dist;
		}
	}

	if(spot != closestChipSpot)
	{
		closestChipSpot = spot;
		clearTableMarkers();

		if(spot != null)
		{
			var key = null;
			var obj = null;
			for(var i=0; i < tableChipsOffsets[spot][3].length; i++)
			{
				key = tableChipsOffsets[spot][3][i];
				if(key == "00" || key == "0")
				{
					obj = mp.objects.new(269022546, new mp.Vector3(tablesPos[lpCasinoTable][1]+tableMarkersOffsets[key][0], tablesPos[lpCasinoTable][2]+tableMarkersOffsets[key][1], tablesPos[lpCasinoTable][3]+tableMarkersOffsets[key][2]));
					obj.setCollision(false, false);
					tableMarkers.push(obj);
				}
				else
				{
					tableMarkers.push(mp.objects.new(3267450776, new mp.Vector3(tablesPos[lpCasinoTable][1]+tableMarkersOffsets[key][0], tablesPos[lpCasinoTable][2]+tableMarkersOffsets[key][1], tablesPos[lpCasinoTable][3]+tableMarkersOffsets[key][2])));
				}
			}
		}
	}

}



