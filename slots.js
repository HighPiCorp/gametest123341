var reels = [];
var data = {};
var d = null;

var a = Math.floor;

global.slotStarted = false;

var slotId = 0;

mp.game.audio.requestScriptAudioBank("DLC_VINEWOOD/CASINO_GENERAL", false);
mp.game.audio.requestScriptAudioBank("DLC_VINEWOOD/CASINO_SLOT_MACHINES_01", false);
mp.game.audio.requestScriptAudioBank("DLC_VINEWOOD/CASINO_SLOT_MACHINES_02", false);
mp.game.audio.requestScriptAudioBank("DLC_VINEWOOD/CASINO_SLOT_MACHINES_03", false);

const SLOTS_MACHINE_SOUNDS = [
    "",
    "dlc_vw_casino_slot_machine_ak_npc_sounds",
    "dlc_vw_casino_slot_machine_ir_npc_sounds",
    "dlc_vw_casino_slot_machine_rsr_npc_sounds",
    "dlc_vw_casino_slot_machine_fs_npc_sounds",
    "dlc_vw_casino_slot_machine_ds_npc_sounds",
    "dlc_vw_casino_slot_machine_kd_npc_sounds",
    "dlc_vw_casino_slot_machine_td_npc_sounds",
    "dlc_vw_casino_slot_machine_hz_npc_sounds",
];

const SLOTS_MACHINE = [
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1100.483, 230.4082, -50.8409),
        rot: 45.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1100.939, 231.0017, -50.8409),
        rot: 60.0,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3(1101.221, 231.6943, -50.8409),
        rot: 75.0,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3(1101.323, 232.4321, -50.8409),
        rot: 90.0,
    },
    {
        model: "vw_prop_casino_slot_08a",
        position: new mp.Vector3(1101.229, 233.1719, -50.8409),
        rot: 105.0,
    },
    {
        model: "vw_prop_casino_slot_01a",
        position: new mp.Vector3(1108.938, 239.4797, -50.8409),
        rot: -45.0,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3(1109.536, 239.0278, -50.8409),
        rot: -30.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3(1110.229, 238.7428, -50.8409),
        rot: -15.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1110.974, 238.642, -50.8409),
        rot: 0.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1111.716, 238.7384, -50.8409),
        rot: 15.0,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3(1112.407, 239.0216, -50.8409 ),
        rot: 30.0,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3( 1112.999, 239.4742, -50.8409 ),
        rot: 45.0,
    },
    {
        model: "vw_prop_casino_slot_01a",
        position: new mp.Vector3( 1120.853, 233.1621, -50.8409 ),
        rot: -105.0,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3( 1120.753, 232.4272, -50.8409 ),
        rot: -90.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3( 1120.853, 231.6886, -50.8409 ),
        rot: -75.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3( 1121.135, 230.9999, -50.8409 ),
        rot: -60.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3( 1121.592, 230.4106, -50.8409 ),
        rot: -45.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3( 1104.572, 229.4451, -50.8409 ),
        rot: -36.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3( 1104.302, 230.3183, -50.8409 ),
        rot: -108.0,
    },
    {
        model: "vw_prop_casino_slot_01a",
        position: new mp.Vector3(  1105.049, 230.845, -50.8409 ),
        rot: 180.0,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3(  1105.781, 230.2973, -50.8409 ),
        rot: 108.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3( 1105.486, 229.4322, -50.8409),
        rot: 36.0,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3( 1108.005, 233.9177, -50.8409),
        rot: -36.0,
    },
    {
        model: "vw_prop_casino_slot_08a",
        position: new mp.Vector3(  1107.735, 234.7909, -50.8409),
        rot: -108.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1108.482, 235.3176, -50.8409),
        rot: -80.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1109.214, 234.7699, -50.8409),
        rot: 108.0,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3( 1108.919, 233.9048, -50.8409),
        rot: 36.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3( 1113.64, 233.6755, -50.8409),
        rot: -36.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1113.37, 234.5486, -50.8409),
        rot: -108.0,
    },
    {
        model: "vw_prop_casino_slot_01a",
        position: new mp.Vector3(1114.117, 235.0753, -50.8409),
        rot: 180.0,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3(1114.848, 234.5277, -50.8409),
        rot: 108.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3(1114.554, 233.6625, -50.8409),
        rot: 36.0,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3(1116.662, 228.8896, -50.8409),
        rot: -36.0,
    },
    {
        model: "vw_prop_casino_slot_08a",
        position: new mp.Vector3(1116.392, 229.7628, -50.8409),
        rot: -108.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1117.139, 230.2895, -50.8409),
        rot: 180.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1117.871, 229.7419, -50.8409),
        rot: 108.0,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3(1117.576, 228.8767, -50.8409),
        rot: 36.0,
    },
    {
        model: "vw_prop_casino_slot_08a",
        position: new mp.Vector3( 1129.64, 250.451, -52.0409),
        rot: 180.0,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3( 1130.376, 250.3577, -52.0409),
        rot: 165.0,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3(1131.062, 250.0776, -52.0409),
        rot: 150.0,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1131.655, 249.6264, -52.0409),
        rot: 135.0,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1132.109, 249.0355, -52.0409),
        rot: 120.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3(1132.396, 248.3382, -52.0409),
        rot: 105.0,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3(1132.492, 247.5984, -52.0409),
        rot: 90.0,
    },
    {
        model: "vw_prop_casino_slot_03a",
        position: new mp.Vector3(1133.952, 256.1037, -52.0409),
        rot: -45,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1133.827, 256.9098, -52.0409),
        rot: -117,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1134.556, 257.2778, -52.0409),
        rot: 171,
    },
    {
        model: "vw_prop_casino_slot_01a",
        position: new mp.Vector3(1135.132, 256.699, -52.0409),
        rot: 99,
    },
    {
        model: "vw_prop_casino_slot_02a",
        position: new mp.Vector3(1134.759, 255.9734, -52.0409),
        rot: 27,
    },
    {
        model: "vw_prop_casino_slot_06a",
        position: new mp.Vector3(1138.195, 251.8611, -52.0409),
        rot: -45,
    },
    {
        model: "vw_prop_casino_slot_07a",
        position: new mp.Vector3(1138.07, 252.6677, -52.0409),
        rot: -117,
    },
    {
        model: "vw_prop_casino_slot_08a",
        position: new mp.Vector3(1138.799, 253.0363, -52.0409),
        rot: 171,
    },
    {
        model: "vw_prop_casino_slot_04a",
        position: new mp.Vector3(1139.372, 252.4563, -52.0409),
        rot: 99,
    },
    {
        model: "vw_prop_casino_slot_05a",
        position: new mp.Vector3(1139, 251.7306, -52.0409),
        rot: 27,
    },
]

mp.events.add('show_slots_text', () => {
    try {
        let i = 0;
        SLOTS_MACHINE.forEach((t) => {

                mp.labels.new(t.toString(), t.position,
                    {
                        los: false,
                        font: 1,
                        drawDistance: 100,

                    });

                mp.checkpoints.new(1, t.position, 1,
                    {
                        visible: true,
                        dimension: 0,
                        color: [255, 255, 255, 50]
                    });
                i = i + 1;
                mp.gui.chat.push(`${i}`)
        });
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('casino_start_slot', (i) => {
    try {
        var a = SLOTS_MACHINE[i];

        data = SLOTS_MACHINE[i];

        slotId = i;

        var seatPos = mp.game1.object.getObjectOffsetFromCoords(a.position.x, a.position.y, a.position.z, a.rot, 0, -.8, .7);
        var reelsCenterPos = mp.game1.object.getObjectOffsetFromCoords(a.position.x, a.position.y, a.position.z, a.rot, 0, .035, 1.1);
        mp.players.local.setVisible(!1, !0);
        mp.players.local.freezePosition(true);

        CreateCamera(new mp.Vector3(seatPos.x, seatPos.y, seatPos.z + .5), new mp.Vector3(0, 0, 0), 50, reelsCenterPos, 800);
    } catch (e) {
        logger.error(e);
    }
});

mp.events.add('casino_exit_slot', () => {
    try {
        data = {};

        ResetCamera();

        mp.players.local.setVisible(!0, !0)
        mp.players.local.freezePosition(false);

        d = null;
    } catch (e) {
        logger.error(e);
    }
});


mp.events.add('casino_spin_slot', (e) => {
    global.slotStarted = true;

    SpitRes(e);
});

mp.events.add('start_slot', (val) => {
    if(!global.slotStarted)
        global.anyEvents.SendServer(() => mp.events.callRemote('casino_start_slot', val));
});



function CreateReels()
{
    try {
        reels.forEach(e => {
            e.destroy()
        });

        reels =
            [
                mp.objects.new(mp.game1.joaat(data.model + `_reels`), mp.game1.object.getObjectOffsetFromCoords(data.position.x, data.position.y, data.position.z, data.rot, -.115, .035, 1.1), {
                    dimension: 0,
                    rotation: new mp.Vector3(0, 0, data.rot)
                }),
                mp.objects.new(mp.game1.joaat(data.model + `_reels`), mp.game1.object.getObjectOffsetFromCoords(data.position.x, data.position.y, data.position.z, data.rot, .01, .035, 1.1), {
                    dimension: 0,
                    rotation: new mp.Vector3(0, 0, data.rot)
                }),
                mp.objects.new(mp.game1.joaat(data.model + `_reels`), mp.game1.object.getObjectOffsetFromCoords(data.position.x, data.position.y, data.position.z, data.rot, .125, .035, 1.1), {
                    dimension: 0,
                    rotation: new mp.Vector3(0, 0, data.rot)
                }),
            ];
    } catch (e) {
        logger.error(e);
    }
}

function CreateCamera(a, b, c, e, f = 0) {
    try {
        null != d && mp.cameras.exists(d) && d.destroy(), d = mp.cameras.new("default", a, b, c), d.pointAtCoord(e.x, e.y, e.z), d.setActive(!0), mp.game1.cam.renderScriptCams(!0, 0 < f, f, !0, !1)
    } catch (e) {
        logger.error(e);
    }
}

function ResetCamera (a = 0) {
    try {
        null != d && mp.cameras.exists(d) && d.destroy(), mp.game1.cam.renderScriptCams(!1, 0 < a, a, !0, !1)
    } catch (e) {
        logger.error(e);
    }
}

function SpitRes(e)
{
    try {
        CreateReels();

        const n = a(Math.random() * 3) + 0 + (a(Math.random() * 2) + 1);
        let l = 0;
        const c = [280, 320, 360],
            d = [!0, !0, !0],
            p = -1 === e ? [n, n + (.5 < Math.random() ? 1 : 0), n + 2] : [e, e, e];


        const i = setInterval(() => {
            try {
                l += 10;
                for (let e = 0; 3 > e; e++) d[e] && (c[e]--, 0 > c[e] ? (reels[e].setRotation(22.5 * p[e], 0, data.rot, 2, !0), d[e] = !1) : reels[e].setRotation(l, 0, data.rot, 2, !0));
                if (!d[2]) return WinSlot(e), void clearInterval(i), global.slotStarted = false;
            } catch (e) {
                logger.error(e);
            }
        }, 10);

        var slotHandle = mp.game.object.getClosestObjectOfType(data.position.x, data.position.y, data.position.z, 1.0, mp.game.joaat(data.model), false, false, false);

        data.slotHandle = slotHandle;

        var soundId = parseInt(data.model.split('_')[4][1]);

        mp.game.audio.playSoundFromEntity(-1, "start_spin", slotHandle, SLOTS_MACHINE_SOUNDS[soundId], false, 0);
        mp.game.audio.playSoundFromEntity(-1, "spinning", slotHandle, SLOTS_MACHINE_SOUNDS[soundId], false, 0);

    } catch (e) {
        logger.error(e);
    }
}


function WinSlot(e){
    global.anyEvents.SendServer(() => mp.events.callRemote('casino_stop_slot', e));
}
function NoWin(){
    var soundId = parseInt(data.model.split('_')[4][1]);
    mp.game.audio.playSoundFromEntity(-1, "no_win", data.slotHandle, SLOTS_MACHINE_SOUNDS[soundId - 1], false, 0);
    global.anyEvents.SendServer(() => mp.events.callRemote('casino_stop_slot', -1));
}

mp.events.add('slots_win', (win) => {
    var soundId = parseInt(data.model.split('_')[4][1]);
    if(win) mp.game.audio.playSoundFromEntity(-1, "big_win", data.slotHandle, SLOTS_MACHINE_SOUNDS[soundId], false, 0);
    else mp.game.audio.playSoundFromEntity(-1, "no_win", data.slotHandle, SLOTS_MACHINE_SOUNDS[soundId - 1], false, 0);
});
