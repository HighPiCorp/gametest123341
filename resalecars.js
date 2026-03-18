var resale;
var caninteraction=null;
var bycar;

mp.events.add('openResalecar', function (resaledata) {
    if (resale != null) {
        resale.destroy();
        resale = null;
    }

    resale = mp.browsers.new('package://cef/resalecars/resalecars.html');
    mp.gui.cursor.visible = true;
    resale.execute(`resale.active=true;`);
    //global.inventoryk.call('inventory_SalleCarMenuShow');
    // mp.events.call('toBlur', 200);
   resale.execute(`resale.data('${resaledata}');`);
   //global.inventoryk.call('inventory_SalleCarMenuData', resaledata);
});
mp.events.add('closeResalecar', function () {
    if (resale != null) {
        resale.destroy();
        resale = null;
        //global.inventoryk.call('inventory_SalleCarMenuShow');
        mp.events.call('fromBlur', 200)
        mp.gui.cursor.visible = false;
    }
});
mp.events.add('openbycar', function (resaledata) {
    if (bycar != null) {
        bycar.destroy();
        bycar = null;
    }
    bycar = mp.browsers.new('package://cef/resalecars/index.html');
    //global.inventoryk.call('inventory_ResaleCars_MenuShow');
    // mp.events.call('toBlur', 200);
    mp.gui.cursor.visible = true;
    let speed = (mp.game.vehicle.getVehicleModelMaxSpeed(mp.players.local.vehicle.model) / 1.2).toFixed();
    let breaks = (mp.game.vehicle.getVehicleModelMaxBraking(mp.players.local.vehicle.model) * 50).toFixed(2);
    let boost = (mp.game.vehicle.getVehicleModelAcceleration(mp.players.local.vehicle.model) * 100).toFixed(2);
    let clutch = (mp.game.vehicle.getVehicleModelMaxTraction(mp.players.local.vehicle.model) * 10).toFixed(2);

    let str ="["+speed+","+breaks+","+boost+","+clutch+"]";
    bycar.execute(`BuyCar.setinfo(${resaledata},${str});`);
    //global.inventoryk.call('inventory_ResaleCars_Setinfo',resaledata,str);
});
mp.events.add('closebycar', function () {
    if (bycar != null) {
        bycar.destroy();
        bycar = null;
        // global.inventoryk.call('inventory_ResaleCars_MenuShow');
        mp.events.call('fromBlur', 200)
        mp.gui.cursor.visible = false;
    }
});
mp.events.add('resalebuycar', function (type) {
    mp.events.call('closebycar');
    global.anyEvents.SendServer(() => mp.events.callRemote('resalebuycar_server',false, type));
});
mp.events.add('resalefamilybuycar', function () {
    mp.events.call('closebycar');
    global.anyEvents.SendServer(() => mp.events.callRemote('resalebuycar_server',true, 0));
});
mp.events.add('recvsalecar', function () {
    mp.events.call('closeResalecar');
    global.anyEvents.SendServer(() => mp.events.callRemote('setsalecar_server'));
});
let resaleShape=null;
let slotcar=null;
let resalemarker=null;
let resaleblip=null;
mp.events.add('RouteResalecar', function (x,y,z,slot) {
    if(resaleShape!=null || resalemarker!=null || resaleblip!=null ) {
        resaleShape.destroy();
        resalemarker.destroy();
        resaleblip.destroy();
         resaleShape=null;
         slotcar=null;
         resalemarker=null;
         resaleblip=null;
    }
    resaleShape = mp.colshapes.newSphere(x, y, z, 1);
    resaleShape.slot=slot;
    caninteraction=true;
    resalemarker = mp.markers.new(36, new mp.Vector3(x,y, z), 1,
        {
            visible: true,
            dimension: 0,
            color: [249, 203, 52, 255]
        });
    resaleblip = mp.blips.new(1, new mp.Vector3(x,y, z), {
        //name: name,
        color: 5,
        scale: 0.7,
        shortRange: false,
    });
    resaleblip.setRoute(true);
});
mp.events.add('cancelSale', function (number) {
    mp.events.call('closeResalecar');
    global.anyEvents.SendServer(() => mp.events.callRemote('cancelSale_server',parseInt(number)));
});
mp.events.add('SaletoGov', function (number) {
    mp.events.call('closeResalecar');
    global.anyEvents.SendServer(() => mp.events.callRemote('SaletoGov_server',parseInt(number)));
});
mp.events.add('playerEnterColshape', (shape) => {

    if(shape.slot !== undefined )
    {

        slotcar = shape.slot;
        mp.game.graphics.notify(`~g~[E]~s~ Place Vehicle`);
    }
});

mp.events.add('playerExitColshape', (shape) => {
    if(shape.slot !== undefined)
    {
        slotcar = null;
    }
});
mp.events.add('cancelresale', () => {
    if(resaleShape!=null || resalemarker!=null || resaleblip!=null ){
        resaleShape.destroy();
        resalemarker.destroy();
        resaleblip.destroy();
        resaleShape=null;
        slotcar=null;
        resalemarker=null;
        resaleblip=null;
    }
});
mp.events.add('caninteraction', (state) => {
    if(!state)
    {
        caninteraction = null;
    }
});
mp.keys.bind(0x45, true, () =>  // E
{
    if(localplayer.isDead() || mp.gui.cursor.visible || slotcar == null || caninteraction==null) return false;

        global.anyEvents.SendServer(() => mp.events.callRemote("setResalecar",slotcar));
        caninteraction=null;
});
