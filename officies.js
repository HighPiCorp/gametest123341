const thisMenu = 'FamilyOffice';
const thisHTML = 'Oscar2';
const thisMenuCall = global.oscar2Menu;

const NATIVES = {
    SET_TEXT_RENDER_ID : "0x5F15302936E07111",
    SET_SCRIPT_GFX_DRAW_ORDER : "0x61BB1D9B3A95D802",
    SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU : "0xC6372ECD45D73BCD",
    GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID : "0x52F0982D7FD156B6",
};

global.movieId = null;
global.handle = null;

mp.game.streaming.requestAnimDict("amb@prop_human_seat_chair_mp@male@generic@base");

var officeCamera = null;

var officeHashNow = null;

var officeHashes = [];
var officePrices = [];

var officePed = null;

const AllOfficeHashes = [
    "ex_dt1_02_office_01a",
    "ex_dt1_02_office_01b",
    "ex_dt1_02_office_01c",
    "ex_dt1_02_office_02a",
    "ex_dt1_02_office_02b",
    "ex_dt1_02_office_02c",
    "ex_dt1_02_office_03a",
    "ex_dt1_02_office_03b",
    "ex_dt1_02_office_03c",
    "ex_dt1_11_office_01a",
    "ex_dt1_11_office_01b",
    "ex_dt1_11_office_01c",
    "ex_dt1_11_office_02a",
    "ex_dt1_11_office_02b",
    "ex_dt1_11_office_02c",
    "ex_dt1_11_office_03a",
    "ex_dt1_11_office_03b",
    "ex_dt1_11_office_03c",
    "ex_sm_13_office_01a",
    "ex_sm_13_office_01b",
    "ex_sm_13_office_01c",
    "ex_sm_13_office_02a",
    "ex_sm_13_office_02b",
    "ex_sm_13_office_02c",
    "ex_sm_13_office_03a",
    "ex_sm_13_office_03b",
    "ex_sm_13_office_03c",
    "ex_sm_15_office_01a",
    "ex_sm_15_office_01b",
    "ex_sm_15_office_01c",
    "ex_sm_15_office_02a",
    "ex_sm_15_office_02b",
    "ex_sm_15_office_02c",
    "ex_sm_15_office_03a",
    "ex_sm_15_office_03b",
    "ex_sm_15_office_03c",
];

const callbackOnClose = () => {
    thisMenuCall.call("CEF::familybuy:update", JSON.stringify({show: false}));
    mp.players.local.freezePosition(false);
    mp.events.callRemote("SERVER::FAMILY:EXIT_BUY");
    cameraDelete();
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
};

mp.events.add('CLIENT::FAMILY:OFFICE_BUY', (info, hashes) => {
    global.anyMenuHTML.openAnyMenu(thisHTML, thisMenu, callbackOnClose());
    thisMenuCall.call("CEF::familybuy:update", info);

    officeHashes = JSON.parse(hashes);
});

async function cameraInit(){
    officeCamera = mp.cameras.new('default');
    officeCamera.setCoord(-124.05551, -643.72723, 169.62054);
    officeCamera.setRot(0.0, 0.0, 65.00, 2);
    officeCamera.setFov(55.0);

    if (!officeCamera.doesExist() || officeCamera == null || officeCamera == undefined) await mp.game.waitAsync(20);

    mp.players.local.position = new mp.Vector3(-140.42688, -641.25806, 168.35632);
    mp.players.local.freezePosition(true);

    mp.players.local.setRotation(0.0, 0.0, -129.76306, 2, true);
    mp.players.local.taskPlayAnim("amb@prop_human_seat_chair_mp@male@generic@base", "base", 8.0, 8.0, -1, 39, 0, false, false, false);

    officeCamera.setActive(true);
    mp.game.cam.renderScriptCams(true, false, 0, true, false);
}

function cameraDelete(){
    if (officeCamera != null) {
        officeCamera.destroy();
        officeCamera = null;

        officeHashNow = null;

        mp.game.cam.renderScriptCams(false, false, 500, true, false);
    }
}

mp.events.add('CLIENT::familybuy:exit', () => {
    mp.players.local.freezePosition(false);
    mp.events.callRemote("SERVER::FAMILY:EXIT_BUY");
    cameraDelete();
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::familybuy:closeServer', () => {
    mp.players.local.freezePosition(false);
    thisMenuCall.call("CEF::familybuy:update", JSON.stringify({show: false}));
     mp.events.callRemote("SERVER::FAMILY:EXIT_BUY");
    cameraDelete();
    global.anyMenuHTML.closeAnyMenu(thisHTML, thisMenu);
});

mp.events.add('CLIENT::familybuy:getAllOfice', () => {
    mp.events.callRemote("SERVER::FAMILY:SELECT_OFFICE");
    thisMenuCall.call("CEF::familybuy:update", JSON.stringify({activeTab: 1}));

    officeHashes.forEach(el => {
        mp.game.streaming.removeIpl(el);
    });

    mp.game.streaming.requestIpl(officeHashes[0]);
    officeHashNow = officeHashes[0];

    cameraInit();
});


mp.events.add('CLIENT::familybuy:buyOfice', (id) => {
    global.anyEvents.SendServer(() => mp.events.callRemote("SERVER::FAMILY:BUY_OFFICE", id));
});

mp.events.add('CLIENT::familybuy:choiceOfice', (id) => {
    mp.game.cam.doScreenFadeOut(100);

    setTimeout(() => {
     if(officeHashNow !== null) mp.game.streaming.removeIpl(officeHashNow);
     mp.game.streaming.requestIpl(officeHashes[id]);

     officeHashNow = officeHashes[id];
    }, 110);

    setTimeout(() => {
     mp.game.cam.doScreenFadeIn(150);

     thisMenuCall.call("CEF::familybuy:update", JSON.stringify({oficePrice: 1000000}));
    },220);
});

mp.events.add('CLIENT::FAMILY:ENTER_OFFICE', (ipl, name) => {

    AllOfficeHashes.forEach(el => {
        mp.game.streaming.removeIpl(el);
    });

    mp.players.local.freezePosition(true);
    mp.game.cam.doScreenFadeOut(250);

    mp.events.call('enableName', name);

    officeHashNow = ipl;

    mp.game.streaming.requestIpl(officeHashNow);

    setTimeout(() => {
        mp.players.local.freezePosition(false);
        mp.game.cam.doScreenFadeIn(250);
        officePed = mp.peds.new(0xF0D4BE2E,new mp.Vector3(-138.9904, -633.931, 168.82053), 6.30798, mp.players.local.dimension);
    }, 500);
});

mp.events.add('CLIENT::FAMILY:EXIT_OFFICE', () => {
    if(officePed != null){
        officePed.destroy();
        officePed = null;
    }

    mp.players.local.freezePosition(true);
    mp.game.cam.doScreenFadeOut(250);
    global.movieId = null;
    setTimeout(() => {
        if(officeHashNow !== null)
            mp.game.streaming.removeIpl(officeHashNow);

        mp.players.local.freezePosition(false);
        mp.game.cam.doScreenFadeIn(250);
    }, 500);
});

mp.events.add('render', () => {
    if(global.movieId != null){
        mp.game1.ui.setTextRenderId(global.handle);

        mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_ORDER, 4);

        mp.game1.invoke(NATIVES.SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU, true);

        mp.game1.graphics.drawScaleformMovie(global.movieId,  0.196*1.75, 0.345*1.5, 0.46*2.0, 0.66*2.2, 255, 255, 255, 255, 1);

        let nm = mp.game1.invoke(NATIVES.GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID);

        mp.game1.ui.setTextRenderId(nm);

        mp.game1.invoke('0xE3A3DB414A373DAB');
    }
});

mp.events.add('enableName', async (name) => {
    mp.game1.ui.registerNamedRendertarget("prop_ex_office_text", false);

    await mp.game.waitAsync(500);

    mp.game1.ui.linkNamedRendertarget(mp.game1.joaat('ex_prop_ex_office_text'));

    await mp.game.waitAsync(500);

    global.handle = mp.game1.ui.getNamedRendertargetRenderId("prop_ex_office_text");

    await mp.game.waitAsync(500);

    global.movieId = mp.game1.graphics.requestScaleformMovie("ORGANISATION_NAME");

    await mp.game.waitAsync(500);

    mp.game1.graphics.pushScaleformMovieFunction(global.movieId, 'SET_ORGANISATION_NAME');

    mp.game.graphics.pushScaleformMovieFunctionParameterString(`${name}`);
    mp.game.graphics.pushScaleformMovieFunctionParameterInt(-1); // style
    mp.game.graphics.pushScaleformMovieFunctionParameterInt(5); // color
    mp.game.graphics.pushScaleformMovieFunctionParameterInt(3); // font

    mp.game1.graphics.popScaleformMovieFunctionVoid();

});

