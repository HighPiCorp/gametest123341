const VENDING_MACHINES = [
    mp.game.joaat("prop_vend_soda_01"),
    mp.game.joaat("prop_vend_soda_02"),
];

function getNearestVendingHandle(player)
{
    VENDING_MACHINES.forEach(machine => {

        var handle = mp.game.object.getClosestObjectOfType(player.position.x, player.position.y, player.position.z, 2, machine, false, true, true)

        if(handle)
        {
            return handle;
        }
    });
}

function tryBuySprunk()
{
    if(getNearestVendingHandle(mp.players.local))
        global.anyEvents.SendServer(() => mp.events.callRemote("buyVendingSprunk"));

}

mp.keys.bind(Keys.VK_E, false, () => {
    tryBuySprunk();
});
