global.openOutType = -1;

var reds = 0;
var donateOpened = false;

// // //
// 0 - Open
// 1 - Close
// 2 - Statistics data
// 3 - Inventory data
// 4 - Outside data
// 5 - Outside on/off
// // //

var last
mp.events.add('board', (act, data, index) => {
 /*   if (board === null)
        global.board = mp.browsers.new('package://cef/board.html');
    //mp.gui.chat.push(`act: ${act} | data: ${data}`);
*/
	switch(act) {
			case 2:
				let date = JSON.parse(data);
				let lvl = date[0];

				let lvlPercent = 0;
				let hours = date[1].split("/");

				let hoursCurrent = parseInt(hours[0]);
				let hoursMax = parseInt(hours[1]);
				lvlPercent = hoursCurrent / hoursMax * 100;

				global.inventoryk.call('inventory_UpdateLVL', lvl);
				global.inventoryk.call('inventory_UpdateLvlPercent', lvlPercent);
			break;
	}
});
